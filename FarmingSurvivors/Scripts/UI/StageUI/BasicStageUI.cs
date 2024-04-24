using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BasicStageUI : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public Slider EXPslider;

    private void Start() {
        SetEXPSlider(GameManager.Instance.player.status.GetEXPbarValue());
    }
    public void SetEXPSlider(float value)
    {
        EXPslider.value = value;
    }

}
