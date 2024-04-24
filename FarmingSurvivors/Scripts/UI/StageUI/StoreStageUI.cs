
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Util;
using StageMap;

public enum ErrorMSG
{
    NoGold, NoMoreSkill, NoStrongSkill, NoZeroSkill
}
public class StoreStageUI : MonoBehaviour
{
    public StageType stageType = StageType.Store;
    public SlotPopUp slotPopUp;
    public GameObject errorPopup;
    public TextMeshProUGUI errorText;
    public int rerollValue = 100;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI rerollValueText;
    public StoreSlot[] buySlots = new StoreSlot[6];
    public StoreSlot[] saleSlots = new StoreSlot[6];
    // 구매 슬롯 6개 배열로 들고있기
    // 판매 슬롯 6개 배열로 들고 있기

    public void Init()
    {
        slotPopUp.gameObject.SetActive(false);

        rerollValueText.text = rerollValue.ToString() + "G";
        int curGold = GoldManager.Instance.CurrentGold;
        SetGoldText(curGold);
        SetBuySkills();
        SetSaleSkills();
    }

    // 구매 슬롯 6개에 랜덤으로 스킬 아이콘, 강화도, 구매가격 채우기
    // 현재 가지고 있는 스킬이라면, 현재 강화도 이하의 스킬은 제거
    public void SetBuySkills()
    {
        List<int> allSkillIds = new List<int>();
        List<int> maxLevelIds = new List<int>();

        foreach (var slot in buySlots)
        {
            if (slot.isEmpty)
                slot.ClearSlot();
        }

        // 모든 스킬 ID 가져오기
        foreach (var skill in LevelUpManager.Instance.AllWeapons)
            allSkillIds.Add(skill.stat.id);

        // 내 스킬 중 맥스레벨스킬 ID 가져오기
        foreach (var mySkill in GameManager.Instance.player.currentWeapons)
        {
            if (mySkill.stat.IsMaxLevel())
                maxLevelIds.Add(mySkill.stat.id);
        }

        // 맥스레벨스킬 ID 제거
        foreach (var id in maxLevelIds)
        {
            if (allSkillIds.Contains(id))
                allSkillIds.Remove(id);
        }

        for (int i = 0; i < buySlots.Length; i++)
        {
            if (allSkillIds.Count == 0)
                continue;

            int rndID = GetRandom.PickRandom(allSkillIds);
            int level = Random.Range(1, 9);
            // 소지 중인 스킬이라면 레벨 비교하여 더 높은 레벨로 설정
            foreach (var mySkill in GameManager.Instance.player.currentWeapons)
            {
                if (maxLevelIds.Contains(mySkill.stat.id))
                    continue;

                if (mySkill.stat.id == rndID)
                {
                    level = Random.Range(mySkill.stat.currentLevel + 1, mySkill.stat.maxLevel);
                    break;
                }
            }
            
            buySlots[i].SetSlot(LevelUpManager.Instance.GetWeaponByID(rndID), level);
        }
    }

    public void SetSaleSkills()
    {
        foreach (var slot in saleSlots)
            slot.ClearSlot();

        for (int i = 0; i < GameManager.Instance.player.currentWeapons.Count; i++)
            saleSlots[i].SetSlot(GameManager.Instance.player.currentWeapons[i]);
    }

    public void BuySkill(Weapon.Weapon skill, int level, int price)
    {
        // 구매처리
        // 안 갖고 있는 스킬이라면? 레벨 만큼 갖고 있게 처리
        if (!GameManager.Instance.player.currentWeapons.Contains(skill))
        {
            LevelUpManager.Instance.CreateWeapon(skill, level);
        }
        else // 갖고 있는 스킬이라면? 해당 레벨 만큼 레벨 업
        {
            //Weapon.Weapon mySkill = GameManager.Instance.player.currentWeapons.Where(w => w.stat.id == skill.stat.id).Last();
            Weapon.Weapon mySkill = GameManager.Instance.player.GetCurWeapon(skill.stat.id);
            for (int i = mySkill.stat.currentLevel; i < level; i++)
            {
                mySkill.stat.LevelUP();
            }
        }
        SetSaleSkills();
    }

    public void SaleSkill(Weapon.Weapon skill, int price)
    {
        if (GameManager.Instance.player.currentWeapons.Contains(skill))
        {
            Weapon.Weapon mySkill = GameManager.Instance.player.GetCurWeapon(skill.stat.id);
            GameManager.Instance.player.currentWeapons.Remove(mySkill.GetComponent<Weapon.Weapon>());
            Destroy(mySkill.gameObject);
            
            GoldManager.Instance.AddGold(price);
            SetGoldText(GoldManager.Instance.CurrentGold);
            SetSaleSkills();
        }
    }

    public void ReRollButton()
    {

        if (GoldManager.Instance.CurrentGold >= rerollValue)
        {
            SetBuySkills();
            GoldManager.Instance.SubtractGold(rerollValue);
            SetGoldText(GoldManager.Instance.CurrentGold);
        }
    }

    public void SetGoldText(int value)
    {
        goldText.text = value.ToString();
    }

    public void BackButtonClick()
    {
        GameManager.Instance.ExitStage(stageType);
    }

    public void ShowSlotPopUp(Weapon.Weapon skill, int price, int level, StoreSlot slot)
    {
        slotPopUp.SetPopup(skill, price, level, slot);
        slotPopUp.gameObject.SetActive(true);
    }

    public void ShowErrorPopup(ErrorMSG errorMSG)
    {
        switch (errorMSG)
        {
            case ErrorMSG.NoGold:
                errorText.text = "골드가 부족합니다.";
                break;
            case ErrorMSG.NoStrongSkill:
                errorText.text = "더 높은 레벨의 스킬만 구입할 수 있습니다.";
                break;
            case ErrorMSG.NoMoreSkill:
                errorText.text = "더 이상 스킬을 보유할 수 없습니다.";
                break;
            case ErrorMSG.NoZeroSkill:
                errorText.text = "최소 1개의 스킬을 보유해야 합니다.";
                break;
        }
        errorPopup.SetActive(true);
    }
}
