using UnityEngine;
using UnityEngine.AI;
using BBUnity.Actions;
using BBUnity.Conditions;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using System.Linq;
using Unity.VisualScripting;


[Condition("MyConditions/IsPreyVisible")]
public class IsPreyVisible : GOCondition
{
    [InParam("detectionRadius")]
    public float detectionRadius = 10f;

    [InParam("preyLayer")]
    public LayerMask preyLayer;

    [InParam("requireLineOfSight", DefaultValue = true)]
    public bool requireLineOfSight = true;

    [OutParam("targetPrey")]
    public GameObject targetPrey;

    public override bool Check()
    {
        // Detectar presas en el radio
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, detectionRadius, preyLayer);

        foreach (var collider in colliders)
        {
            // Si requerimos línea de visión, hacer un raycast
            if (requireLineOfSight)
            {
                Vector3 directionToTarget = (collider.transform.position - gameObject.transform.position).normalized;
                float distanceToTarget = Vector3.Distance(gameObject.transform.position, collider.transform.position);

                // Verificar si hay obstáculos entre el cazador y la presa
                if (Physics.Raycast(gameObject.transform.position, directionToTarget, out RaycastHit hit, distanceToTarget))
                {
                    if (hit.collider != collider) continue; // Si golpeamos algo que no es la presa, ignorar esta presa
                }
            }

            targetPrey = collider.gameObject;
            return true;
        }

        targetPrey = null;
        return false;
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
[Action("MyActions/ChaseAndCapturePrey")]
public class ChaseAndCapturePrey : GOAction
{
    [InParam("targetPrey")]
    public GameObject targetPrey;

    [InParam("updateInterval")]
    public float updateInterval = 0.2f;

    [InParam("captureDistance")]
    public float captureDistance = 1f;

    private NavMeshAgent agent;
    private float nextPathUpdate;
    private float originalSpeed;

    public override void OnStart()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

        // Guardar y aumentar velocidad para la persecución
        originalSpeed = agent.speed;
        agent.speed *= 1.5f;

        if (targetPrey != null)
        {
            agent.SetDestination(targetPrey.transform.position);
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (targetPrey == null)
            return TaskStatus.FAILED;

        // Verificar si estamos lo suficientemente cerca para capturar
        float distance = Vector3.Distance(gameObject.transform.position, targetPrey.transform.position);
        if (distance <= captureDistance)
        {
            // Destruir la presa
            GameObject.Destroy(targetPrey);
            return TaskStatus.COMPLETED;
        }

        // Si no estamos lo suficientemente cerca, continuamos persiguiendo
        if (Time.time >= nextPathUpdate)
        {
            agent.SetDestination(targetPrey.transform.position);
            nextPathUpdate = Time.time + updateInterval;
        }

        return TaskStatus.RUNNING;
    }

    public override void OnEnd()
    {
        if (agent != null)
            agent.speed = originalSpeed;
    }
}