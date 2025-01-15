using UnityEngine;
using UnityEngine.AI;
using BBUnity.Actions;
using BBUnity.Conditions;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using System.Linq;

[Condition("MyConditions/IsTargetVisible")]
public class IsTargetVisible : GOCondition
{
    [InParam("detectionRange")]
    public float detectionRange = 10f;

    [OutParam("detectedPrey")]
    public GameObject detectedPrey;

    public override bool Check()
    {
        // Buscar presas en el radio de detección
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, detectionRange);
        float closestDistance = float.MaxValue;
        GameObject closestPrey = null;

        foreach (var hitCollider in hitColliders)
        {
            // Verificar si el objeto tiene el componente Prey
            if (hitCollider.GetComponent<Prey>() != null)
            {
                float distance = Vector3.Distance(gameObject.transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPrey = hitCollider.gameObject;
                }
            }
        }

        detectedPrey = closestPrey;
        return closestPrey != null;
    }
}
    [Action("MyActions/Wander")]
public class Wander : GOAction
{
    [InParam("wanderRadius")]
    public float wanderRadius = 20f;

    [InParam("minWanderTime")]
    public float minWanderTime = 3f;

    [InParam("maxWanderTime")]
    public float maxWanderTime = 6f;

    private NavMeshAgent agent;
    private float wanderTimeRemaining;
    private bool destinationSet = false;

    public override void OnStart()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found!");
            return;
        }
        wanderTimeRemaining = Random.Range(minWanderTime, maxWanderTime);
        destinationSet = false;
    }

    public override TaskStatus OnUpdate()
    {
        if (agent == null)
            return TaskStatus.FAILED;

        // Si no tenemos destino o hemos llegado al anterior, buscamos uno nuevo
        if (!destinationSet || (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance))
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += gameObject.transform.position;

            NavMeshHit hit;
            // Encontrar el punto más cercano en el NavMesh
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                destinationSet = true;
                wanderTimeRemaining = Random.Range(minWanderTime, maxWanderTime);
            }
        }

        // Reducir el tiempo restante
        wanderTimeRemaining -= Time.deltaTime;

        // Si el tiempo se acabó, buscar nuevo destino
        if (wanderTimeRemaining <= 0)
        {
            destinationSet = false;
        }

        return TaskStatus.RUNNING;
    }

    public override void OnAbort()
    {
        if (agent != null)
        {
            agent.ResetPath();
        }
    }
}
[Action("MyActions/ChaseAndDestroy")]
public class ChaseAndDestroy : GOAction
{
    [InParam("prey")]
    public GameObject prey;

    [InParam("attackRange")]
    public float attackRange = 2f;

    [InParam("attackDamage")]
    public float attackDamage = 25f;

    private NavMeshAgent agent;
    private float attackCooldown = 1f;
    private float nextAttackTime = 0f;

    public override void OnStart()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent no encontrado en el objeto!");
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (prey == null || agent == null)
            return TaskStatus.FAILED;

        // Actualizar destino del NavMeshAgent
        agent.SetDestination(prey.transform.position);

        // Verificar si está en rango de ataque
        float distanceToPrey = Vector3.Distance(gameObject.transform.position, prey.transform.position);
        if (distanceToPrey <= attackRange && Time.time >= nextAttackTime)
        {
            // Atacar a la presa
            Prey preyComponent = prey.GetComponent<Prey>();
            if (preyComponent != null)
            {
                preyComponent.TakeDamage(attackDamage);
                nextAttackTime = Time.time + attackCooldown;
            }
        }

        return TaskStatus.RUNNING;
    }
}