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
            animator.SetBool("IsHit", true);
            return;
        }
        if (stamina < 100 && animator.GetCurrentAnimatorStateInfo(0).IsName("Chilling"))
        {
            stamina += staminaRecoveryRate * Time.deltaTime;
            if (stamina > 100) stamina = 100;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            stamina -= staminaDecayRate * Time.deltaTime;
            if (stamina < 0) stamina = 0;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            stamina -= (staminaDecayRate / 2) * Time.deltaTime;
            if (stamina < 0) stamina = 0;
        }

        animator.SetFloat("Stamina", stamina);
    }

    public void TakeHit()
    {
        isHit = true;
        Invoke("RecoverFromHit", 1f); 
    }

    private void RecoverFromHit()
    {
        isHit = false;
        animator.SetBool("IsHit", false);
    }
}
