using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private bool isJumping = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetMoveSpeed(float speed)
    {
        animator.SetFloat("Speed", Mathf.Abs(speed));
    }

    public void TriggerJump()
    {
        if (!isJumping)
        {
            isJumping = true;
            animator.SetBool("IsJumping", true);
        }
    }

    public void ForceLandingCheck()
    {
        if (isJumping)
        {
            isJumping = false;
            animator.SetBool("IsJumping", false);
        }
    }
}