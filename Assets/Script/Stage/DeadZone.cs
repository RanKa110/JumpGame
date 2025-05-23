using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    //public TextMeshProUGUI gameOverMassage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterManager.Instance.Player.controller.gameOverMassage.gameObject.SetActive(true);
            CharacterManager.Instance.Player.controller.isDead = true;
            CharacterManager.Instance.Player.controller.ToggleCursor();
        }
        else
        {
            Destroy(other.gameObject);
        }
        
    }

    
}
