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

    [SerializeField] float roamRadius;
    [SerializeField] float roamPause;
    [SerializeField] float roamSpeed;
    [SerializeField] float arriveEpsilon;

    [SerializeField] float engageEnterFactor;
    [SerializeField] float engageExitFactor;


    [SerializeField] float avoidStickTime;

    [SerializeField] float planSmoothing;
    [SerializeField] float faceSmoothing;

    [SerializeField] bool kinematicMotion = true;

    Vector3 smoothPlainDir;
    Vector3 smoothFaceDir;


    int avoidSign = 0;
    float avoidStickTimer = 0f;

    bool engaged;

    Rigidbody rb;
    float startY;
    float strafeTimer;
    int strafeSign = 1;

    Vector3 roamCenter;
    Vector3 roamTarget;
    float roamTimer;
    bool hasRoamTarget;

    void Awake()
    {
        var a = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (a && a.enabled) a.enabled = false;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.isKinematic = kinematicMotion;

        startY = transform.position.y;
        roamCenter = transform.position;
        PickNewRoamTarget();

        smoothPlainDir = transform.forward; smoothPlainDir.y = 0f;
        if(smoothPlainDir.sqrMagnitude < 0.001f) smoothPlainDir = Vector3.forward;
        smoothFaceDir = smoothPlainDir;
    }

    public override void Move(NavMeshAgent agent, Transform self, Transform player)
    {
        if (!player) return;


        //Distance on XZ plane
        Vector3 toPlayer = player.position - self.position; toPlayer.y = 0f;
       
        float d = toPlayer.magnitude;

        float enterD = approachRange * engageEnterFactor;
        float exitD = approachRange * engageExitFactor;

        if (engaged)
        {
            if (d >= exitD) engaged = false;
        }
        else
        {
            if (d <= enterD) engaged = true;
        }

        Vector3 targetPlainDir = Vector3.zero;
        float desiredSpeed = 0f;

        if (!engaged)
        {
            if (!hasRoamTarget || Vector3.Distance(self.position, roamTarget) <= arriveEpsilon)
            {
                roamTimer += Time.deltaTime;
                if (roamTimer >= roamPause)
                {
                    roamTimer = 0f;
                    PickNewRoamTarget();
                }
            }

            Vector3 toTarget = roamTarget - self.position; toTarget.y = 0f;
            if (toTarget.sqrMagnitude > 0.001f)
            {
                targetPlainDir = toTarget.normalized;
                desiredSpeed = roamSpeed;
            }



        }
        else
        {


            if (d > approachRange + rangeDeadZone)
            {
                targetPlainDir = toPlayer.normalized;
                desiredSpeed = moveSpeed;
            }
            else if (strafeInsideRange && d > 0.001f)
            {
                Vector3 tangent = Vector3.Cross(Vector3.up, toPlayer.normalized) * strafeSign;
                targetPlainDir = tangent;
                desiredSpeed = strafeSpeed;

                strafeTimer += Time.deltaTime;
                if (strafeTimer >= strafeSwitchInterval)
                {
                    strafeTimer = 0f;
                    strafeSign *= -1;
                }
            }

        }

        if (targetPlainDir.sqrMagnitude < 0.0001f) targetPlainDir = smoothPlainDir;
        smoothPlainDir = Vector3.Slerp(smoothPlainDir, targetPlainDir.normalized, Time.deltaTime * planSmoothing);
        smoothPlainDir.y = 0f;
        if (smoothPlainDir.sqrMagnitude < 0.0001f) smoothPlainDir = targetPlainDir;


        //Altitude control
        float targetY = lockToStartAltitude ? startY : player.position.y + altitudeOffsetFromPlayer;
        float yVel = (targetY - self.position.y) * altitudeLerp;

        //Forward avoidance
        Vector3 avoid = Avoid(self.position, smoothPlainDir, desiredSpeed) * avoidWeight;

        //Wanted velocity
        Vector3 horiz = smoothPlainDir * desiredSpeed + avoid;
        Vector3 wantedVel = new Vector3(horiz.x, yVel, horiz.z);

        //Accelerate toward target velocity
        if (kinematicMotion)
        {
            Vector3 newPos = rb.position + wantedVel * Time.deltaTime;
            rb.MovePosition(newPos);
        }
        else
        {

            #if UNITY_6000_0_OR_NEWER
                rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, wantedVel, acceleration * Time.deltaTime);

            #else
                rb.velocity = Vector3.MoveTowards(rb.velocity, wantedVel, acceleration * Time.deltaTime);
            
            #endif
        }

        Vector3 targetFace = engaged ? toPlayer : (roamTarget - self.position);
        targetFace.y = 0f; if (targetFace.sqrMagnitude < 0.0001f) targetFace = smoothPlainDir;
        smoothFaceDir = Vector3.Slerp(smoothFaceDir, targetFace.normalized, Time.deltaTime * faceSmoothing);
        if (smoothFaceDir.sqrMagnitude < 0.0001f) smoothFaceDir = targetFace;

        FaceDirFlat(self, smoothFaceDir, turnSpeed, rb);
    }

    void PickNewRoamTarget()
    {
        for(int i = 0; i < 6; i++)
        {
            Vector3 offset = Random.insideUnitSphere * roamRadius; offset.y = 0f;
            Vector3 p = roamCenter + offset;

            Vector3 dir = (p - transform.position); dir.y = 0f;
            if (dir.sqrMagnitude < 1f) continue;

            if(!Physics.SphereCast(transform.position, avoidRadius, dir.normalized, out _, dir.magnitude, obstacleMask, QueryTriggerInteraction.Ignore))
            {
                roamTarget = p;
                hasRoamTarget = true;
                return;
            }
        }

        roamTarget = roamCenter;
        hasRoamTarget = true;
    }

    Vector3 Avoid(Vector3 origin, Vector3 probeDir, float maxMag)
    {
        probeDir.y = 0f;
        if (probeDir.sqrMagnitude < 0.0001f)
        {
            avoidStickTimer = Mathf.Max(0f, avoidStickTimer - Time.deltaTime);
            if (avoidStickTimer <= 0f) avoidSign = 0;
            return Vector3.zero;
        }

        Vector3 dir = probeDir.normalized;

        
        Vector3 start = origin + dir * (avoidRadius + 0.05f);

        if (Physics.SphereCast(start, avoidRadius, dir, out var hit, avoidProbeDist,
                               obstacleMask, QueryTriggerInteraction.Ignore))
        {
            
            if (hit.rigidbody == rb || hit.transform == transform || hit.transform.IsChildOf(transform))
                return Vector3.zero;

            
            if (hit.collider.CompareTag("Player"))
                return Vector3.zero;

            if (avoidStickTimer <= 0f || avoidSign == 0)
            {
                float signed = Vector3.SignedAngle(dir, hit.normal, Vector3.up);
                avoidSign = (signed >= 0f) ? -1 : +1;
                if (avoidSign == 0) avoidSign = 1;
                avoidStickTimer = avoidStickTime;
            }
            else
            {
                avoidStickTimer -= Time.deltaTime;
            }

            Vector3 tangent = Vector3.Cross(Vector3.up, dir) * avoidSign;
            float mag = Mathf.Min(maxMag, moveSpeed);
            return tangent.normalized * mag;
        }
        else
        {
            avoidStickTimer = Mathf.Max(0f, avoidStickTimer - Time.deltaTime);
            if (avoidStickTimer <= 0f) avoidSign = 0;
            return Vector3.zero;
        }
    }


    static void FaceDirFlat(Transform self, Vector3 dir, float turnSpeed, Rigidbody rb = null)
    {
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;
        Quaternion desired = Quaternion.LookRotation(dir);

        if (rb != null)
        {
            if(!rb.isKinematic) rb.angularVelocity = Vector3.zero;
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, desired, Time.deltaTime * turnSpeed));
        }
        else
        {
            self.rotation = Quaternion.Slerp(self.rotation, desired, Time.deltaTime * turnSpeed);
        }
    }
}
