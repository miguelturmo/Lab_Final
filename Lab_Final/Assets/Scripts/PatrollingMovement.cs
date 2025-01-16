using UnityEngine;
using UnityEngine.AI;

public class Patrolling_Movement : MonoBehaviour
{
    public Transform[] waypoints;
    private int destPoint = 0;
    private NavMeshAgent agent;
    private int direction = 1; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
                
        agent.autoBraking = true;
                
        destPoint = Random.Range(0, waypoints.Length);
        direction = Random.Range(0, 2) == 0 ? 1 : -1;
                
        GotoNextPoint();
    }

    void GotoNextPoint()
    {        
        if (waypoints.Length == 0)
            return;
                
        agent.destination = waypoints[destPoint].position;
                
        if (direction == 1)
        {
            destPoint = (destPoint + 1) % waypoints.Length;
        }
        else if (direction == -1)
        {
            destPoint--;
                        
            if (destPoint < 0)
            {
                destPoint = waypoints.Length - 1;
            }
        }
    }

    void Update()
    {        
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GotoNextPoint();
    }
}
