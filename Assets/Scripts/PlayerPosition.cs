using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosition : MonoBehaviour
{
    void Start()
    {
        if (GameManager.Instance.shouldLoadPosition)
        {

            transform.position = GameManager.Instance.lastPlayerPosition;
            GameManager.Instance.shouldLoadPosition = false;
        }
    }
}
