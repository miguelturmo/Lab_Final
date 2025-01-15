using UnityEngine;
using UnityEngine.AI;

public class NavMeshWander : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Wander Settings")]
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float minWanderWaitTime = 3f;
    [SerializeField] private float maxWanderWaitTime = 10f;
    [SerializeField] private float minDistanceToTarget = 1f;

    private Vector3 wanderTarget;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private float currentWaitTime;

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
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= currentWaitTime)
            {
                isWaiting = false;
                SetNewWanderTarget();
            }
            return;
        }

        // Verificar si llegamos al destino
        if (Vector3.Distance(transform.position, wanderTarget) <= minDistanceToTarget)
        {
            StartWaiting();
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

    private void StartWaiting()
    {
        isWaiting = true;
        waitTimer = 0f;
        currentWaitTime = Random.Range(minWanderWaitTime, maxWanderWaitTime);
        agent.ResetPath();
    }

    // Para visualizar el radio de wandering en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
}