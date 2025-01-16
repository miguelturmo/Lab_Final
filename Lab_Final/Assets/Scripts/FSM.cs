using UnityEngine;
public class CharacterControllerFSM : MonoBehaviour
{
    private Animator animator;
    public float stamina = 100f;
    public float staminaRecoveryRate = 10f;
    public float staminaDecayRate = 5f;
    private bool isHit = false;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (isHit)
        {
            animator.SetTrigger("IsHit");
            return;
        }
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stamina < 100 && stateInfo.IsName("Chill") && !animator.IsInTransition(0))
        {
            stamina += staminaRecoveryRate * Time.deltaTime;
            if (stamina > 100) stamina = 100;
        }
        else if (stateInfo.IsName("Run"))
        {
            stamina -= staminaDecayRate * Time.deltaTime;
            if (stamina < 0) stamina = 0;
        }
        else if (stateInfo.IsName("Walk"))
        {
            stamina -= (staminaDecayRate / 2) * Time.deltaTime;
            if (stamina < 0) stamina = 0;
        }
        animator.SetFloat("Stamina", stamina);
    }
}