using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyMovement_Base : MonoBehaviour
{
    public abstract void Move(NavMeshAgent agent, Transform self, Transform player);

    protected static float Dist(Transform a, Transform b)
        => Vector3.Distance(a.position, b.position);

    protected static Vector3 DirTo(Transform from, Transform to, bool flattenY = true)
    {
        Vector3 d = (to.position - from.position);
        if (flattenY) d.y = 0f;
        return d.normalized;
    }

    protected static bool SampleOnNavMesh(Vector3 pos, float maxDistance, out Vector3 sampled)
    {
        if(NavMesh.SamplePosition(pos, out NavMeshHit hit, maxDistance, NavMesh.AllAreas))
        {
            sampled = hit.position; return true;
        }
        sampled = pos; return false;
    }
}
