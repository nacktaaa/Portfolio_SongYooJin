using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ItemInspector : MonoBehaviour
{
    public Define.InvenType type;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemWeightText;
    public TextMeshProUGUI itemDetailText;
    public TextMeshProUGUI actionButtonText;
    public TextMeshProUGUI getOrThrowButtonText;

    public bool isShow = false;
    Slot selectSlot;
    RectTransform rectTr;
    Vector3 playerInvenPos;
    Vector3 storageInvenPos;
    string getText = "줍기";
    string throwText = "버리기";
    string unmountText = "장착 해제";

    public void Init()
    {
        rectTr = GetComponent<RectTransform>();
        playerInvenPos = rectTr.position;
        storageInvenPos = new Vector3 (rectTr.position.x, rectTr.position.y+365, 0);
        gameObject.SetActive(false);
    }

    void SetItemInspector(Item _item, Define.InvenType _type)
    {

        itemNameText.text = _item.name;
        itemWeightText.text = _item.weight.ToString();
        itemDetailText.text = _item.description;
        actionButtonText.text = _item.action;

        switch (_type)
        {
            case Define.InvenType.Player :
                {
                    rectTr.position = playerInvenPos;
                    getOrThrowButtonText.text = throwText;
                    if(_item.idx != 20)
                        actionButtonText.transform.parent.gameObject.SetActive(true);

                    if(_item is Weapon)
                    {
                        Weapon temp = (Weapon)_item;
                        if(temp.isEquiped)
                            actionButtonText.text = unmountText;
                    }
                }  
                break;
            case Define.InvenType.World :
                {
                    rectTr.position = storageInvenPos;
                    getOrThrowButtonText.text = getText;
                    actionButtonText.transform.parent.gameObject.SetActive(false);
                }
                break;
        }
    }

    public void ShowItemInspector(Slot _slot, Define.InvenType _type)
    {
        isShow = true;
        selectSlot = _slot;
        type = _type;
        SetItemInspector(_slot.item, _type);
        this.gameObject.SetActive(true);
    }

    public void HideItemInspector()
    {
        if(isShow)
        {
            isShow = false;
            this.gameObject.SetActive(false);
        }
    }

    // 아이템 정보창 > 액션 버튼에 할당된 함수  
    public void OnActionButtonClick()
    {
        selectSlot.item.Action();
    }

    public void OnGetOrThrowButtonClick()
    {
        if(selectSlot != null)
        {
            switch(type)
            {
                case Define.InvenType.Player : // 버리기 버튼
                {
                    Managers.Inven.RemoveItem(selectSlot.item);
                }
                    break;
                case Define.InvenType.World : // 줍기 버튼
                {
                    Managers.Inven.ToPlayerInven(selectSlot.item);
                    Managers.Sound.PlayEffectSound(Define.EffectSound.PutInBag);
                }
                    break;
            }
        }
    }
}
