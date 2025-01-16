using UnityEngine;
using UnityEngine.AI;

public class Patrolling_Movement : MonoBehaviour
{
    public Transform[] waypoints; // Lista de puntos de patrullaje.
    private int destPoint = 0;    // �ndice del punto actual en la lista.
    private NavMeshAgent agent;   // Agente de navegaci�n.
    private int direction = 1;    // Direcci�n del patrullaje (1 = adelante, -1 = atr�s).

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

        // Seleccionar el siguiente waypoint seg�n la direcci�n.
        if (direction == 1)
        {
            destPoint = (destPoint + 1) % waypoints.Length;
        }
        else if (direction == -1)
        {
            destPoint--;

            // Si retrocedemos m�s all� del primer waypoint, volvemos al �ltimo.
            if (destPoint < 0)
            {
                destPoint = waypoints.Length - 1;
            }
        }
    }

    void Update()
    {
        // Si el agente est� cerca del destino, selecciona el siguiente punto.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GotoNextPoint();
    }
}
