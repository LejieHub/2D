using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class GravityReverseTrigger : MonoBehaviour
{
    [Tooltip("玩家标签")]
    public string playerTag = "Player";

    [Header("重力设置")]
    [Tooltip("反转后的重力缩放值（建议负数表示反方向）")]
    public float reverseGravityScale = -3f;

    private Dictionary<GameObject, float> originalGravities = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> originalJumpForces = new Dictionary<GameObject, float>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            ReverseGravityComponents(other.gameObject);
        }
    }

    void ReverseGravityComponents(GameObject player)
    {
        bool isReversing = !originalGravities.ContainsKey(player);

        HandleGravity(player);
        FlipSprite(player);
        ReverseJumpForce(player, isReversing);
    }

    void HandleGravity(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        float currentGravity = rb.gravityScale;

        // 如果当前是正重力，反转
        if (currentGravity > 0f)
        {
            originalGravities[player] = currentGravity;
            rb.gravityScale = reverseGravityScale;
            Debug.Log($"{player.name} → gravity reversed to {rb.gravityScale}");
        }
        // 如果当前是反重力，恢复为默认正向
        else if (currentGravity < 0f)
        {
            float restoredGravity = originalGravities.ContainsKey(player)
                ? originalGravities[player]
                : 1f; // 正确的默认值

            rb.gravityScale = restoredGravity;
            Debug.Log($"{player.name} → gravity restored to {rb.gravityScale}");

            originalGravities.Remove(player);
        }

    }



    void FlipSprite(GameObject player)
    {
        SpriteRenderer sr = player.GetComponentInChildren<SpriteRenderer>(true);
        if (sr != null)
        {
            sr.flipY = !sr.flipY;
        }
    }

    public void ForceClearGravityCache(GameObject player)
    {
        if (originalGravities.ContainsKey(player))
            originalGravities.Remove(player);

        if (originalJumpForces.ContainsKey(player))
            originalJumpForces.Remove(player);
    }


    public void ReverseJumpForce(GameObject player, bool shouldReverse)
    {
        MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            System.Type type = script.GetType();
            System.Reflection.FieldInfo field = type.GetField("jumpForce",
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (field != null && field.FieldType == typeof(float))
            {
                if (shouldReverse)
                {
                    originalJumpForces[player] = (float)field.GetValue(script);
                    field.SetValue(script, -originalJumpForces[player]);
                }
                else
                {
                    if (originalJumpForces.ContainsKey(player))
                    {
                        field.SetValue(script, originalJumpForces[player]);
                        originalJumpForces.Remove(player);
                    }
                }
            }
        }
    }
}