using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossStageUI : BattleStageUI
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI bossNameText;
    public Slider bossHPSlider;

    private void Awake() {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void SetTimeCount(float timeCount)
    {
        int min = (int)timeCount / 60;
        int sec = (int)timeCount - min * 60;
        string text = String.Format("{0:00}:{1:00}", min, sec);

        timeText.text = text;
    }

    public void SetBossHPSlider(float value)
    {
        bossHPSlider.value = value;
    }
}
