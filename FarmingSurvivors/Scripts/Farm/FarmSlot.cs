using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

public class FarmSlot : MonoBehaviour, IPointerClickHandler
{
    public int slotIdx = 0;
    BoxCollider2D col;
    public Sprite enableSprite;
    [SerializeField]
    int curGrowthPoint;
    [SerializeField]
    string cropName;
    [SerializeField]
    string cropGrade;
    public bool isEnable = false;
    public bool isEmpty = true;
    public bool isSelected = false;
    public bool canHarvest = false;
    public SpriteRenderer cropImage;
    public GameObject priceCanvas;
    public Crop crop;
    Color overPointColor;
    SpriteRenderer spr;
    public int CurGrowthPoint
    {
        get { return curGrowthPoint;}
        set
        {
            curGrowthPoint = value;
            if(crop != null)
            {
                if(curGrowthPoint >= crop.gradeSetting.maxGrowthValue)
                {
                    if(!canHarvest)
                    {
                        canHarvest = true;
                        cropImage.sprite = crop.cropSetting.growUpSprites[4];
                        FarmManager.Instance.MakeReward(crop);
                    }
                }
                else if (curGrowthPoint >= crop.gradeSetting.maxGrowthValue * 0.75f) 
                    cropImage.sprite = crop.cropSetting.growUpSprites[3];
                else if (curGrowthPoint >= crop.gradeSetting.maxGrowthValue * 0.5f)
                    cropImage.sprite = crop.cropSetting.growUpSprites[2];
                else if (curGrowthPoint >= crop.gradeSetting.maxGrowthValue * 0.25f)
                    cropImage.sprite = crop.cropSetting.growUpSprites[1];
                else
                    cropImage.sprite = crop.cropSetting.growUpSprites[0];
            }
        }
    }


    public void Init() 
    {
        ColorUtility.TryParseHtmlString("#D98880", out overPointColor);

        spr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        col.enabled = false;
        if(isEnable)
            spr.sprite = enableSprite;
        if(!isEmpty)
            cropImage.gameObject.SetActive(true);
        else    
            cropImage.gameObject.SetActive(false);

        if(crop != null)
        {
            cropName = crop.cropSetting.cropName;
            cropGrade = crop.gradeSetting.grade.ToString();
        }

        RefreshGroundColor();
            
        priceCanvas.SetActive(false);
    }
    public void RefreshGroundColor()
    {
        if(crop == null)
        {
            if(CurGrowthPoint > 0)
                spr.color = overPointColor;
            else
                spr.color= Color.white;
        }
        else
            spr.color = Color.white;
    }

    public void PlantSeed(Crop crop)
    {
        isEmpty = false;
        this.crop = crop;
        this.cropImage.sprite = crop.cropSetting.growUpSprites[0];
        cropImage.gameObject.SetActive(true);
        CurGrowthPoint = CurGrowthPoint;
        RefreshGroundColor();
        this.cropName = crop.cropSetting.cropName;
        this.cropGrade = crop.gradeSetting.grade.ToString();
        FarmManager.Instance.RemoveSeeds(crop);
    }

    public void Clear()
    {
        isEmpty = true;
        isSelected = false;
        canHarvest = false;
        crop = null;
        RefreshGroundColor();
        priceCanvas.SetActive(false);
        cropImage.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(isEnable && isEmpty)
        {
            if(FarmManager.Instance.selectInvenSlot != null)
            {
                PlantSeed(FarmManager.Instance.selectInvenSlot.crop);                
            }
            
        }
        else if(isEnable && !isEmpty)
        {
            if(FarmManager.Instance.selectCompost != null)
            {
                PutCompost();
                FarmManager.Instance.selectCompost.UnSelected();
                FarmManager.Instance.selectCompost = null;
            }
        }
        else if(!isEnable)
        {
            if(isSelected)
            {
                if (GoldManager.Instance.CurrentGold >= FarmManager.Instance.GetFarmPrice())
                {
                    GoldManager.Instance.SubtractGold(FarmManager.Instance.GetFarmPrice());
                    isEnable = true;
                    spr.sprite = enableSprite;
                    FarmManager.Instance.selectSlot = null;
                    HidePrice();
                }
            }
            else
            {
                priceCanvas.SetActive(true);
                priceCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = FarmManager.Instance.GetFarmPrice().ToString() + "<sprite name=\"Coin\">";
                
                if (GoldManager.Instance.CurrentGold >= FarmManager.Instance.GetFarmPrice())
                    priceCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
                else 
                    priceCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;

                if(FarmManager.Instance.selectSlot != null && FarmManager.Instance.selectSlot != this)
                {
                    FarmManager.Instance.selectSlot.priceCanvas.SetActive(false);
                    FarmManager.Instance.selectSlot.isSelected = false;
                }
                    
                FarmManager.Instance.selectSlot = this;
                isSelected = true;
            }

        }
    }

    private void PutCompost()
    {
        CurGrowthPoint += FarmManager.Instance.selectCompost.growthPoint;
        GoldManager.Instance.SubtractGold(FarmManager.Instance.selectCompost.price);
    }

    public void HidePrice()
    {
        if(priceCanvas.activeSelf)
        {
            priceCanvas.SetActive(false);
            isSelected = false;
        }
    }

    public void EnableCol()
    {
        col.enabled = true;
    }

    public void DisableCol()
    {
        col.enabled = false;
    }
}
