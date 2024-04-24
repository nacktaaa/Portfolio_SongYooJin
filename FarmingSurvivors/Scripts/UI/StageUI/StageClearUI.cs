using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClearUI : MonoBehaviour
{
    public ShowingSlot showingSlot;
    public RectTransform rewardUIRoot;

    public void SetRewardSlot(SeedType seedType, SeedGrade seedGrade)
    {
        Crop crop = new Crop (seedType, seedGrade);
        showingSlot.frame.sprite = crop.gradeSetting.seedPackImage;
        showingSlot.icon.sprite = crop.cropSetting.seedSprite;
    }

    
}
