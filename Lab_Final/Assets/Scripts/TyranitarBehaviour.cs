using UnityEngine;
using UnityEngine.AI;

public class TyranitarBehaviour : MonoBehaviour
{
    public Transform treasure;
    public GameObject aggron;
    public float aggronDetectionRadius = 3.0f;
    public float wanderRadius = 10.0f;
    public float wanderInterval = 3.0f;

    private NavMeshAgent agent;
    private float wanderTimer;

    private enum State { Wandering, Stealing }
    private State currentState = State.Wandering;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        wanderTimer = wanderInterval;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Wandering:
                Wander();
                CheckForStealingOpportunity();
                break;

            case State.Stealing:
                StealTreasure();
                break;
        }
    }

    private void Wander()
    {
        wanderTimer += Time.deltaTime;

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance &&
            wanderTimer >= wanderInterval)
        {
            Vector3 newPos = RandomNavMeshLocation(wanderRadius);
            agent.SetDestination(newPos);
            wanderTimer = 0.0f;
        }
    }

    private void CheckForStealingOpportunity()
    {
        float distanceToAggron = Vector3.Distance(transform.position, aggron.transform.position);
        float distanceToTreasure = Vector3.Distance(transform.position, treasure.position);
        Debug.Log($"Distancia a Aggron: {distanceToAggron}, Distancia al tesoro: {distanceToTreasure}");

        if (distanceToAggron > aggronDetectionRadius && distanceToTreasure <= 5.5f)
        {
            Debug.Log("Oportunidad para robar detectada.");
            currentState = State.Stealing;
            agent.SetDestination(treasure.position);
        }
    }

    private void StealTreasure()
    {
        if (!agent.pathPending && agent.remainingDistance > 0.5f)
        {
            Debug.Log("Tyranitar moviéndose hacia el tesoro.");
            return;
        }

        if (Vector3.Distance(transform.position, treasure.position) <= 0.5f)
        {
            Debug.Log("Tesoro robado!");
            treasure.gameObject.SetActive(false);
            currentState = State.Wandering;
        }
    }

    private Vector3 RandomNavMeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, radius, -1);

        return navHit.position;
    }
}

