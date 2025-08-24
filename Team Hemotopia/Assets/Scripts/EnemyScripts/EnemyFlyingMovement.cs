using UnityEngine;
using UnityEngine.AI;

public class EnemyFlyingMovement : EnemyMovement_Base
{
    [SerializeField] float approachRange;
    [SerializeField] float rangeDeadZone;
    [SerializeField] bool strafeInsideRange;
    [SerializeField] float strafeSpeed;
    [SerializeField] float strafeSwitchInterval;

    [SerializeField] float moveSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float turnSpeed;

    [SerializeField] bool lockToStartAltitude = true;
    [SerializeField] float altitudeOffsetFromPlayer;
    [SerializeField] float altitudeLerp;

    [SerializeField] LayerMask obstacleMask = ~0;
    [SerializeField] float avoidProbeDist;
    [SerializeField] float avoidRadius;
    [SerializeField] float avoidWeight;

    Rigidbody rb;
    float startY;
    float strafeTimer;
    int strafeSign = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        startY = transform.position.y;
    }

    public override void Move(NavMeshAgent agent, Transform self, Transform player)
    {
        if (!player) return;

        //Facing
        FacePlayerFlat(self, player, turnSpeed);

        //Distance on XZ plane
        Vector3 toPlayer = player.position - self.position;
        Vector3 flat = new Vector3(toPlayer.x, 0f, toPlayer.z);
        float d = flat.magnitude;

        //horizontal movement
        Vector3 desiredDir = Vector3.zero;

        //approach until in range
        if(d > approachRange + rangeDeadZone)
        {
            desiredDir = flat.normalized;
        }
        else if(strafeInsideRange && d > 0.001f)
        {
            Vector3 tangent = Vector3.Cross(Vector3.up, flat.normalized) * strafeSign;
            desiredDir = tangent;
            strafeTimer += Time.deltaTime;
            if(strafeTimer >= strafeSwitchInterval)
            {
                strafeTimer = 0f;
                strafeSign *= -1;
            }
        }
        
        //Altitude control
        float targetY = lockToStartAltitude ? startY : player.position.y + altitudeOffsetFromPlayer;
        float yVel = (targetY - self.position.y) * altitudeLerp;

        //Forward avoidance
        Vector3 avoid = Avoid(self.position, self.forward);

        //Wanted velocity
        Vector3 horiz = desiredDir * (desiredDir.sqrMagnitude > 0f ? (strafeInsideRange && d <= approachRange ? strafeSpeed : moveSpeed) : 0f);
        horiz += avoid * avoidWeight;

        Vector3 wantedVel = new Vector3(horiz.x, yVel, horiz.z);

        //Accelerate toward target velocity
        #if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, wantedVel, acceleration * Time.deltaTime);
        #else
            rb.velocity= Vector3.MoveTowards(rb.velocity,wantedVel, acceleration * Time.deltaTime);
        #endif

    }

    Vector3 Avoid(Vector3 origin, Vector3 forward)
    {
        if(Physics.SphereCast(origin, avoidRadius, forward, out var hit, avoidProbeDist, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            Vector3 n = new Vector3(hit.normal.x, 0f, hit.normal.z).normalized;
            if (n.sqrMagnitude < 0.001f) n = Vector3.Cross(Vector3.up, forward).normalized;
            return n * moveSpeed;
        }
        return Vector3.zero;
    }

    static void FacePlayerFlat(Transform self, Transform player, float turnSpeed)
    {
        Vector3 look = player.position - self.position; look.y = 0f;
        if (look.sqrMagnitude < 0.0001f) return;
        Quaternion desired = Quaternion.LookRotation(look);
        self.rotation = Quaternion.Slerp(self.rotation, desired, Time.deltaTime * turnSpeed);
    }
}
