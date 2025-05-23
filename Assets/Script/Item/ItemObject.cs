using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum State
{
    Item,
    Door,
}

public interface Iinteractable
{
    public string GetInteracPrompt();
    public void OnInteract();
}

public class ItemObject : MonoBehaviour, Iinteractable
//    아이템 오브젝트를 완전히 상호작용으로 재 설계하여 사용함
//    이름을 바꾸기엔 너무 늦어 바꾸지 않고 이렇게 주석으로 남김
{
    public ItemData data = null;
    public GameObject interactObject = null;

    public State state;

    private bool canInteract = true;

    public string GetInteracPrompt()
    {
        if (!canInteract) return null;

        string str = $"{data.displayName}\n'E'키를 눌러 상호작용 하기";
        return str;
    }

    public void OnInteract()
    {
        if(state == State.Item)
        {
            CharacterManager.Instance.Player.itemData = data;
            CharacterManager.Instance.Player.addItem?.Invoke();
            Destroy(gameObject);
        }
        else if(state == State.Door && canInteract)
        {
            StartCoroutine(MoveObjectDown(interactObject, 11f, 5f));
            canInteract = false;
        }
        
    }

    private IEnumerator MoveObjectDown(GameObject obj, float distance, float speed)
    {
        Vector3 startPos = obj.transform.position;
        Vector3 targetPos = startPos + Vector3.down * distance;

        while (Vector3.Distance(obj.transform.position, targetPos) > 0.01f)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }
    }
}
