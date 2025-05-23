using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public class UIInven : MonoBehaviour
{
    public ItemSlot[] slots;

    public GameObject invenWindow;
    public Transform slotPanel;
    public Transform dropPos;

    [Header("���õ� ������")]
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDes;
    public TextMeshProUGUI selectedStatName;
    public TextMeshProUGUI selectedStatValue;

    public GameObject useBtn;
    public GameObject equipBtn;
    public GameObject unequipBtn;
    public GameObject dropBtn;

    private PlayerController controller;
    private PlayerCondition condition;

    ItemData selectedItem;
    int selectedItemIndex = 0;

    int curEquipIndex;
    void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;
        dropPos = CharacterManager.Instance.Player.dropPos;

        controller.inven += ToggleInven;
        CharacterManager.Instance.Player.addItem += Additem;

        invenWindow.SetActive(false);
        slots = new ItemSlot[slotPanel.childCount];

        for(int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inven = this;
            slots[i].Clear();
        }

        ClearSelectedItemWindow();
    }

    void ClearSelectedItemWindow()
    {
        selectedItemName.text = string.Empty;
        selectedItemDes.text = string.Empty;
        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        useBtn.SetActive(false);
        equipBtn.SetActive(false);
        unequipBtn.SetActive(false);
        dropBtn.SetActive(false);

    }

    public void ToggleInven()
    {
        if (IsInvenOpen())
        {
            invenWindow.SetActive(false);
        }
        else
        {
            invenWindow.SetActive(true);
        }
    }

    public bool IsInvenOpen()
    {
        return invenWindow.activeInHierarchy;
    }

   void Additem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData;

        //    �������� �ߺ� �����Ѱ�?
        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if(slot != null)
            {
                slot.quantity++;
                UpdateUI();
                CharacterManager.Instance.Player.itemData = null;
                return;
            }

        }

        //    ����ִ� ������ ��������
        ItemSlot emptySlot = GetEmptySlot();

        //    ���� ����ִ� ������ �ִٸ�?
        if(emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        //    ���� ����ִ� ������ ���ٸ�?
        ThrowItem(data);
        CharacterManager.Instance.Player.itemData = null;
    }

    void UpdateUI()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    ItemSlot GetItemStack(ItemData data)
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)
            //    ���� �����Ͱ� ���ų�, �̹� �����ϴµ� �ִ� ���� ���� á�ٸ�!
            {
                return slots[i];
            }
        }
        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            //    �� ����� ����ִٸ� �ٷ� ��������
            {
                return slots[i];
            }
        }
        return null;
    }

    void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPos.position, 
            Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;

        selectedItem = slots[index].item;
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.displayName;
        selectedItemDes.text = selectedItem.description;

        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        for(int i = 0; i < selectedItem.consumables.Length; i++)
        {
            selectedStatName.text += selectedItem.consumables[i].type.ToString() + "\n" ;
            selectedStatValue.text += selectedItem.consumables[i].value.ToString() + "\n";
        }

        useBtn.SetActive(selectedItem.type == ItemType.Consumable);
        equipBtn.SetActive(selectedItem.type == ItemType.Equipable 
            && !slots[index].equipped);
        unequipBtn.SetActive(selectedItem.type == ItemType.Equipable
            && slots[index].equipped);
        dropBtn.SetActive(true);
    }

    public void OnUseBtn()
    {
        if(selectedItem.type == ItemType.Consumable)
        {
            for(int i = 0; i < selectedItem.consumables.Length; i++)
            {
                switch (selectedItem.consumables[i].type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.consumables[i].value);
                        break;

                    case ConsumableType.Hunger:
                        condition.Eat(selectedItem.consumables[i].value);
                        break;
                }
            }
            RemoveSelectedItem();
        }
    }

    public void OnDropBtn()
    {
        ThrowItem(selectedItem);
        RemoveSelectedItem();
    }

    void RemoveSelectedItem()
    {
        slots[selectedItemIndex].quantity--;

        if (slots[selectedItemIndex].quantity <= 0)
        {
            selectedItem = null;
            slots[selectedItemIndex].item = null;
            selectedItemIndex = -1;
            ClearSelectedItemWindow();
        }

        UpdateUI();
    }

    public void OnEquipBtn()
    {
        if (slots[curEquipIndex].equipped)
        {
            //    ���� �����ϱ�
            UnEquip(curEquipIndex);
        }

        slots[selectedItemIndex].equipped = true;
        curEquipIndex = selectedItemIndex;
        CharacterManager.Instance.Player.equip.EquipNew(selectedItem);
        UpdateUI();
        SelectItem(selectedItemIndex);
    }

    void UnEquip(int index)
    {
        slots[index].equipped = false;
        CharacterManager.Instance.Player.equip.UnEquip();

        if(selectedItemIndex == index)
        {
            SelectItem(selectedItemIndex);
        }
    }

    public void OnUnEquipBtn()
    {
        UnEquip(selectedItemIndex);
    }
}
