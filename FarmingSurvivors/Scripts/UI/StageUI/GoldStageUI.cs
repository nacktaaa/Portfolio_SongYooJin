using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoldStageUI : MonoBehaviour
{
    public TextMeshProUGUI timeCountText;
    public TextMeshProUGUI goldCountText;

    private void OnEnable() {
        GameManager.Instance.goldStage.GetGoldEventHandler -= SetGoldCount;
        GameManager.Instance.goldStage.GetGoldEventHandler += SetGoldCount;
    }
    private void OnDisable() {
        GameManager.Instance.goldStage.GetGoldEventHandler -= SetGoldCount;
    }
    
    public void SetTimeCount(float timeCount)
    {
        int min = (int)timeCount / 60;
        int sec = (int)timeCount - min * 60;
        string text = String.Format("{0:00}:{1:00}", min, sec);

        timeCountText.text = text;
    }

    public void SetGoldCount(int count)
    {
        goldCountText.text = count.ToString();
    }

}
