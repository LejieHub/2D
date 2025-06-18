using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private bool isJumping = false;
    private Vector3 originalScale;


    void Start()
    {
        animator = GetComponent<Animator>();
        originalScale = transform.localScale; // 记录初始缩放，防止形变
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

    public void FlipSprite(bool faceRight)
    {
        Vector3 newScale = originalScale;
        newScale.x = Mathf.Abs(originalScale.x) * (faceRight ? 1 : -1);
        transform.localScale = newScale;
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