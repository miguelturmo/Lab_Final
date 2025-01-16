using UnityEngine;
using UnityEngine.AI;

public class Patrolling_Movement : MonoBehaviour
{
    public Transform[] waypoints; // Lista de puntos de patrullaje.
    private int destPoint = 0;    // Índice del punto actual en la lista.
    private NavMeshAgent agent;   // Agente de navegación.
    private int direction = 1;    // Dirección del patrullaje (1 = adelante, -1 = atrás).

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Deshabilitar auto-braking permite movimientos continuos entre waypoints.
        agent.autoBraking = true;

        // Seleccionar un punto de inicio aleatorio.
        destPoint = Random.Range(0, waypoints.Length);
        direction = Random.Range(0, 2) == 0 ? 1 : -1;

        // Ir al primer punto.
        GotoNextPoint();
    }

    void GotoNextPoint()
    {
        // Salir si no hay waypoints configurados.
        if (waypoints.Length == 0)
            return;

        // Configurar el destino del agente como el waypoint actual.
        agent.destination = waypoints[destPoint].position;

        // Seleccionar el siguiente waypoint según la dirección.
        if (direction == 1)
        {
            destPoint = (destPoint + 1) % waypoints.Length;
        }
        else if (direction == -1)
        {
            destPoint--;

            // Si retrocedemos más allá del primer waypoint, volvemos al último.
            if (destPoint < 0)
            {
                destPoint = waypoints.Length - 1;
            }
        }
    }

    void Update()
    {
        // Si el agente está cerca del destino, selecciona el siguiente punto.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GotoNextPoint();
    }
}
