using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInvenUI : MonoBehaviour
{
    public bool isOpen = false;
    public Transform slotHolder;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI weightText;

    [SerializeField]
    int initSize = 9;

    GameObject slotPrefab;
    List<Slot> slots = new List<Slot>();

    private void OnEnable()
    {
        Managers.Inven.PlayerInvenEvent -= SetSlots;
        Managers.Inven.PlayerInvenEvent += SetSlots;
    }
    private void OnDisable()
    {
        Managers.Inven.PlayerInvenEvent -= SetSlots;
    }
    
    public void Init()
    {
        isOpen = false;

        //혹시 처음에 차일드를 갖고 있다면 모두 삭제
        for (int i = 0; i < slotHolder.childCount; i ++)
            Destroy(slotHolder.GetChild(i).gameObject);

        // slot 프리팹을 가져와서 처음 개수만큼 새로 생성
        slotPrefab = Resources.Load<GameObject>("Prefabs/UI/Slot");
        AddSlot(initSize);
    }

    public void SetSlots(List<Item> items)
    {
        if(isOpen)
        {
            ClearSlots();
            weightText.text = Managers.Game.player.stat.GetWeightText();
            Managers.UI.HideInspectorUI();
        }

        if(items.Count > slots.Count)
        {
            AddSlot(3);
        }

        for (int i = 0; i<items.Count; i++)
        {
            slots[i].SetSlot(items[i]);
        }
    }

    void AddSlot(int count)
    {
        if (slotPrefab!=null)
        {
            for(int i = 0; i< count; i++)
            {
                GameObject go = Instantiate(slotPrefab, slotHolder);
                Slot slot = go.GetComponent<Slot>();
                slot.invenType = Define.InvenType.Player;
                slots.Add(slot);
            }     
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

        foreach (Slot slot in slots)
        {
            slot.isSelect = false;
        }

        this.gameObject.SetActive(false);
    }

    public void Clear()
    {
        slots.Clear();
    }
    
}
