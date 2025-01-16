using UnityEngine;
using UnityEngine.AI;

public class CharacterControllerFSM : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    public float stamina = 100f;
    public float staminaRecoveryRate = 10f;
    public float staminaDecayRate = 5f;
    private bool isHit = false;

    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found!");
            enabled = false;
            return;
        }

        agent.speed = walkSpeed;
    }
    void Update()
    {
        if (isHit)
        {
            animator.SetTrigger("IsHit");
            return;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Chill"))
        {
            agent.isStopped = true; 
        }
        else
        {
            agent.isStopped = false; 
        }

        if (stamina < 100 && stateInfo.IsName("Chill") && !animator.IsInTransition(0))
        {
            stamina += staminaRecoveryRate * Time.deltaTime;
            if (stamina > 100) stamina = 100;
        }
        else if (stateInfo.IsName("Run"))
        {
            stamina -= staminaDecayRate * Time.deltaTime;
            if (stamina < 0) stamina = 0;
            agent.speed = runSpeed; 
        }
        else if (stateInfo.IsName("Walk"))
        {
            stamina -= (staminaDecayRate / 2) * Time.deltaTime;
            if (stamina < 0) stamina = 0;
            agent.speed = walkSpeed;
        }

        animator.SetFloat("Stamina", stamina);
    }
}
