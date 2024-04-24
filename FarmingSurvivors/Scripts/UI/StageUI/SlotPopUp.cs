using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotPopUp : PopUpUI
{

    public Image icon;
    public TextMeshProUGUI skillName;
    public TextMeshProUGUI skillDescrip;
    public TextMeshProUGUI skillPrice;
    public Button buyButton;
    public Button saleButton;

    private Weapon.Weapon skill;
    private int price;
    private int level;
    private StoreSlot from;
    private StoreStageUI storeStageUI;

    public void SetPopup(Weapon.Weapon skill, int price, int level, StoreSlot slot)
    {
        this.skill = skill;
        this.price = price;
        this.level = level;
        this.from = slot;

        icon.sprite = skill.stat.icon;
        skillName.text = skill.stat.weaponName;
        skillDescrip.text = skill.stat.weaponDescription;
        skillPrice.text = price.ToString();

        buyButton.gameObject.SetActive(slot.slotType == StoreSlotType.Buy);
        saleButton.gameObject.SetActive(slot.slotType == StoreSlotType.Sale);

        storeStageUI = transform.parent.GetComponent<StoreStageUI>();
    }

    public void BuyButton()
    {
        if (!GameManager.Instance.player.currentWeapons.Contains(skill))
        {
            if (GameManager.Instance.player.currentWeapons.Count >= LevelUpManager.Instance.MaxWeaponAmount)
            {
                storeStageUI.ShowErrorPopup(ErrorMSG.NoMoreSkill);
                return;
            }
        }
        else
        {
            Weapon.Weapon w = null;
            foreach (var ww in GameManager.Instance.player.currentWeapons)
            {
                if (ww.stat.id == skill.stat.id)
                    w = ww;
            }

            if (w?.stat.currentLevel >= level)
            {
                storeStageUI.ShowErrorPopup(ErrorMSG.NoStrongSkill);
                return;
            }
        }

        if (GoldManager.Instance.CurrentGold >= price)
        {
            from.isEmpty = true;
            from.gameObject.GetComponent<CanvasGroup>().alpha = 0.5f;
            from.gameObject.GetComponent<Image>().raycastTarget = false;
            storeStageUI.BuySkill(skill, level, price);
            GoldManager.Instance.SubtractGold(price);
            storeStageUI.SetGoldText(GoldManager.Instance.CurrentGold);
            this.gameObject.SetActive(false);
        }
        else
        {
            storeStageUI.ShowErrorPopup(ErrorMSG.NoGold);
        }
    }

    public void SellButton()
    {
        if (GameManager.Instance.player.currentWeapons.Count == 1)
        {
            storeStageUI.ShowErrorPopup(ErrorMSG.NoZeroSkill);
            return;
        }

        storeStageUI.SaleSkill(skill, price);
        this.gameObject.SetActive(false);
    }


}
