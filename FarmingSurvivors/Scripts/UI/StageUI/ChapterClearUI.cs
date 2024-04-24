using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChapterClearUI : MonoBehaviour
{
    public Button nextChapterButton;
    public RectTransform rewardRoot;
    public ShowingSlot showingSlot;
    public Text growthText;

    public void SetNextChapterButton()
    {
        if(GameManager.Instance.curChapter == "3")
            nextChapterButton.gameObject.SetActive(false);
        else
            nextChapterButton.gameObject.SetActive(true);
    }

    public void SetRewardSlot(SeedType seedType, SeedGrade seedGrade)
    {
        Crop crop = new Crop (seedType, seedGrade);
        showingSlot.frame.sprite = crop.gradeSetting.seedPackImage;
        showingSlot.icon.sprite = crop.cropSetting.seedSprite;

        growthText.text = null;
        growthText.DOText("밭이 충분히 성장했다..!!!", 2f, false);
    }

    
}
