using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsControler : MonoBehaviour
{
    public GameObject GroundEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            GameObject _groundEffects = Instantiate(GroundEffect, new Vector3 (transform.position.x, transform.position.y-0.5f, transform.position.z), Quaternion.identity);
            Destroy(_groundEffects,1f);
        }
    }
}
