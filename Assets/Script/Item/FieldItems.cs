using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public enum Type
{
    Speed,
    Hp,
    Stamina,
    Jump,
    Save,


}

public class FieldItems : MonoBehaviour
{
    public Type type;

    private GameObject itemObject;
    private BoxCollider boxCollider;

    private void Start()
    {
        itemObject = transform.GetChild(0).gameObject;
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (type)
            {
                case Type.Speed:
                    SpeedItem();
                    break;

                case Type.Hp:
                    CharacterManager.Instance.Player.condition.Heal(30);
                    Destroy(gameObject);
                    break;

                case Type.Stamina:
                    break;

                case Type.Jump:
                    other.GetComponent<Rigidbody>().AddForce(Vector3.up * 300f, ForceMode.Impulse);
                    break;

                case Type.Save:
                    CharacterManager.Instance.Player.controller.savePoint.position = transform.position;
                    break;
            }
        }
       
    }

    void SpeedItem()
    {
        StartCoroutine(UnlimitedRun());
    }

    IEnumerator UnlimitedRun()
    {
        boxCollider.enabled = false;

        float originStamineUseValue = CharacterManager.Instance.Player.controller.runStaminaUse;
        float originStaminaPassiveValue = CharacterManager.Instance.Player.condition.stamina.passiveValue;

        CharacterManager.Instance.Player.controller.runStaminaUse = 0;
        CharacterManager.Instance.Player.condition.stamina.passiveValue = 0;
        
        itemObject.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(4f);
        CharacterManager.Instance.Player.controller.runStaminaUse = originStamineUseValue;
        CharacterManager.Instance.Player.condition.stamina.passiveValue = originStaminaPassiveValue;
        Destroy(gameObject);
    }

    void Head()
    {

    }

}
