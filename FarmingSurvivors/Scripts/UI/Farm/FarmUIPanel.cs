using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FarmUIPanel : UIPanel
{
    public GameObject farmInvenSlot;
    public GameObject compost;
    public RectTransform invenArea;
    public Button harvestButton;
    public RewardPopup rewardPopup;
    public Toggle autoPlantToggle;
    FarmInvenTab[] tabs; 
    List<FarmInvenSlot> curInvenSlots = new List<FarmInvenSlot>();

    private void OnEnable() {
        FarmManager.Instance.ChangeSeedsHandler -= SetInvenSlots;
        FarmManager.Instance.ChangeSeedsHandler += SetInvenSlots;
        FarmManager.Instance.ChangeRewardHandler -= SetHarvestButton;
        FarmManager.Instance.ChangeRewardHandler += SetHarvestButton;
    }
    private void OnDisable() {
        FarmManager.Instance.ChangeSeedsHandler -= SetInvenSlots;
        FarmManager.Instance.ChangeRewardHandler -= SetHarvestButton;
    }
    private void Start() 
    {
        compost.SetActive(false);
        tabs = GetComponentsInChildren<FarmInvenTab>();
        rewardPopup.gameObject.SetActive(false);
        autoPlantToggle.isOn = FarmManager.Instance.isAutoPlant;
        SetHarvestButton();
        SetInvenSlots();
    }

    public void SetHarvestButton()
    {
        if(FarmManager.Instance.rewardGolds.Count == 0 && FarmManager.Instance.rewardEquips.Count == 0)
            harvestButton.interactable = false;
        else
            harvestButton.interactable = true;
    }

    public void TESTCreateButton()
    {
        FarmManager.Instance.CreateSeed(Util.GetRandom.RandomEnum<SeedType>(), Util.GetRandom.RandomEnum<SeedGrade>());
        ClearSlots();
        SetInvenSlots();
    }
    public void ClearSlots()
    {
        foreach(var i in curInvenSlots)
            i.gameObject.SetActive(false);
    }
    public void SetInvenSlots()
    {
        ClearSlots();

        var unique = FarmManager.Instance.playerSeeds.Where(s => s.gradeSetting.grade == SeedGrade.Unique);
        var superRare = FarmManager.Instance.playerSeeds.Where(s => s.gradeSetting.grade == SeedGrade.SuperRare);
        var rare = FarmManager.Instance.playerSeeds.Where(s => s.gradeSetting.grade == SeedGrade.Rare);
        var normal = FarmManager.Instance.playerSeeds.Where(s => s.gradeSetting.grade == SeedGrade.Normal);

        List<Crop> sortedList = new List<Crop>();

        foreach(var item in unique)
            sortedList.Add(item);
        foreach(var item in superRare)
            sortedList.Add(item);
        foreach(var item in rare)
            sortedList.Add(item);
        foreach(var item in normal)
            sortedList.Add(item);

        foreach(var c in sortedList)
        {
            FarmInvenSlot slot = GetSlot();
            slot.SetSlot(c);
            slot.gameObject.SetActive(true);
            curInvenSlots.Add(slot);
        }
        if(compost.activeSelf)
        {
            FarmManager.Instance.selectCompost = null;
            compost.GetComponent<CompostSlot>().UnSelected();
            compost.SetActive(false);        
        }
    }

    public FarmInvenSlot GetSlot()
    {
        FarmInvenSlot slot = null;
        foreach(var i in curInvenSlots)
        {
            if(!i.gameObject.activeSelf)
            {
                slot = i;
                return slot;
            }
        }

        if(slot == null)
        {
            slot = Instantiate(farmInvenSlot, invenArea).GetComponent<FarmInvenSlot>();
            curInvenSlots.Add(slot);
        }
        return slot;
    }


    public void HarvestButtonClick()
    {
        rewardPopup.Init(autoPlantToggle.isOn);
        rewardPopup.gameObject.SetActive(true);
    }

    public void ShowTap(FarmInvenType type, FarmInvenTab tab)
    {
        switch(type)
        {
            case FarmInvenType.Seed :
            {
                compost.SetActive(false);
                SetInvenSlots();
            }
                break;
            case FarmInvenType.Compost :
            {
                ClearSlots();
                compost.SetActive(true);
            }
                break;
        }

        foreach (var t in tabs)
        {
            if(t == tab)
                t.isActive = true;
            else
                t.isActive = false;
        }
    }


}
