using UnityEngine;

public class GravityDebug : MonoBehaviour
{
    void Update()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Debug.Log($"[DEBUG] {gameObject.name} gravityScale = {rb.gravityScale}");
        }
    }
}
