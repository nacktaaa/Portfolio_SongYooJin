using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum StoreSlotType
{
    Buy, Sale
}
public class StoreSlot : MonoBehaviour, IPointerClickHandler
{
    public StoreSlotType slotType;
    public Weapon.Weapon skill;

    public Image skillIcon;
    public SkillLevelIcon levelIcon;
    public TextMeshProUGUI priceText;
    public bool isEmpty = true;
    int price = 0;
    int level = 0;

    public void ClearSlot()
    {
        isEmpty = true;
        this.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void SetSlot(Weapon.Weapon skill, int level = 1)
    {
        isEmpty = false;
        this.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
        this.gameObject.GetComponent<Image>().raycastTarget = true;

        this.skill = skill;
        skillIcon.sprite = skill.stat.icon;
        if(slotType == StoreSlotType.Buy)
        {
            levelIcon.SetSkillLevelIcons(level);
            this.level = level;
        }
        else
        {
            levelIcon.SetSkillLevelIcons(skill.stat.currentLevel);
        }

        price = slotType == StoreSlotType.Buy ? GetBuyPrice(level) : GetSalePrice();
        priceText.text = price.ToString();
    }

    private int GetBuyPrice(int level)
    {
        int newPrice = 150;
        int value = level == 1 ? newPrice : 300 * (level-1);
        newPrice = value;
        return newPrice;
    }

    private int GetSalePrice()
    {
        int level = skill.stat.currentLevel;
        level = level == 0 ? 1 : level;
        int newPrice = 35;
        int value = level == 1 ? newPrice : 75 * (level-1);
        newPrice = value;
        return newPrice;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!isEmpty)
            GameManager.Instance.stageUI.storeStageUI.ShowSlotPopUp(skill, price, level, this);
    }


    // 구매, 판매 슬롯 클릭 시 팝업창 생성
    // 팝업 창에 현재 클릭한 슬롯의 스킬 정보(아이콘, 강화도, 이름, 설명, 가격) 받아오기
    // 팝업창에 구매, 판매에 따른 버튼 띄우기(구매, 판매)
    // 구매 버튼의 경우, 소지 골드에 따라 버튼 비활성화 처리
}
