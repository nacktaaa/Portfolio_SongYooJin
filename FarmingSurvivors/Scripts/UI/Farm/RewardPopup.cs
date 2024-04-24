using System.Collections;
using System.Collections.Generic;
using Equipment;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class RewardPopup : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    public RectTransform euipArea;
    public GameObject equipUI;
    List<GameObject> eqUIs = new List<GameObject>();

    public void Init(bool isAuto)
    {
        int resultGold = 0;

        foreach(var g in FarmManager.Instance.rewardGolds)
            resultGold += g;
        
        goldText.text = resultGold.ToString();

        GoldManager.Instance.AddGold(resultGold);

        foreach(var e in FarmManager.Instance.rewardEquips)
        {
            GameObject go = Instantiate(equipUI, euipArea);
            go.GetComponent<ShowingSlot>().SetSlot(e);
            eqUIs.Add(go);
            EquipManager.Instance.inventory.AddEquipmentInInventory(e.parts, e);
        }
        FarmManager.Instance.ClearReward();

        foreach(var s in FarmManager.Instance.farmSlots)
        {
            if(s.canHarvest)
            {
                s.CurGrowthPoint -= s.crop.gradeSetting.maxGrowthValue;
                s.Clear();
                if(isAuto)
                {
                    if(FarmManager.Instance.playerSeeds.Count > 0)
                        s.PlantSeed(FarmManager.Instance.PopSeedHighestGrade());
                }
            }
        }
    }

    public void ConfirmButtonClick()
    {
        this.gameObject.SetActive(false);
        foreach(var e in eqUIs)
            Destroy(e);
    }
    
}
