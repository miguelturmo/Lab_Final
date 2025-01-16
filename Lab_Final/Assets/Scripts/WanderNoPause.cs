using UnityEngine;
using UnityEngine.AI;

public class NavMeshWandernoPause : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Wander Settings")]
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float minDistanceToTarget = 1f;

    private Vector3 wanderTarget;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found!");
            enabled = false;
            return;
        }
        SetNewWanderTarget();
    }

    private void Update()
    {
        // Verificar si llegamos al destino
        if (Vector3.Distance(transform.position, wanderTarget) <= minDistanceToTarget)
        {
            SetNewWanderTarget();
        }
    }

    private void SetNewWanderTarget()
    {
        // Encontrar un punto aleatorio en el radio especificado
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        // Encontrar el punto más cercano en el NavMesh
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            wanderTarget = hit.position;
            agent.SetDestination(wanderTarget);
        }
        else
        {
            // Si no se encuentra un punto válido, intentar de nuevo
            SetNewWanderTarget();
        }
    }

    // Para visualizar el radio de wandering en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
}
