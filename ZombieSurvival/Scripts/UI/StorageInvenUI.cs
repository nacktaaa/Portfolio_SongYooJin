using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 스토리지인벤토리UI 객체의 컴포넌트
// 월드에서 플레이어가 접근가능한 보관함들의 아이템을 보여줌
// UI를 열고 닫을 때의 처리가 이루어짐
// InventoryManager 를 통해 접근 
public class StorageInvenUI : MonoBehaviour
{
    public bool isOpen= false; // 열려있는 상태인가?
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI weightText;
    public Slot[] slots;  // UI를 구성하는 슬롯 배열

    private void OnEnable()
    {
        Managers.Inven.StorageInvenEvent -= SetSlots;
        Managers.Inven.StorageInvenEvent += SetSlots;
    }
    private void OnDisable()
    {
        Managers.Inven.StorageInvenEvent -= SetSlots;
    }

    public void Init()
    {
        isOpen = false;
        slots = GetComponentsInChildren<Slot>();
        nameText.text = "보관함";

        foreach(Slot slot in slots)
            slot.invenType = Define.InvenType.World;

    }

    public void SetSlots(List<Item> items)
    {
        if(isOpen)
        {
            ClearSlots();
            weightText.text = Managers.Inven.selectedStorage.GetWeightText();
            Managers.UI.HideInspectorUI();
        }

        for (int i = 0; i<items.Count; i++)
        {
            slots[i].SetSlot(items[i]);
        }
    }

    public void ClearSlots()
    {
        foreach (Slot slot in slots)
        {
            slot.isSelect = false;
            slot.ClearSlot();
        }
    }

    public void Close()
    {
        isOpen = false;
        ClearSlots();
        this.gameObject.SetActive(false);
    }

}
