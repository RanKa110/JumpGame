using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalUI : MonoBehaviour
{
    public bool needDestroy;
    //BoxCollider collider;

    private void Start()
    {
       // collider = GetComponentInChildren<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && needDestroy)
        {
            Destroy(gameObject);
        }
    }
}
