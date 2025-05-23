using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Iinteractable
{
    public string GetInteracPrompt();
    public void OnInteract();
}

public class ItemObject : MonoBehaviour, Iinteractable
{
    public ItemData data;

    public string GetInteracPrompt()
    {
        string str = $"{data.displayName}\n'E'Ű�� ���� �ݱ�";
        return str;
    }

    public void OnInteract()
    {
        CharacterManager.Instance.Player.itemData = data;
        CharacterManager.Instance.Player.addItem?.Invoke();
        Destroy(gameObject);
    }
}
