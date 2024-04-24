using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StageMap;
using UnityEngine;
using UnityEngine.UI;

public class RestStageUI : MonoBehaviour
{
    public StageType stageType = StageType.Rest;
    public Slider slider;
    public Text text;
    public GameObject shine;
    private float fillspeed = 36f;
    private float duration = 1.5f;
    private WaitForSeconds delay = new WaitForSeconds(2f);

    public void Init()
    {
        fillspeed = GameManager.Instance.player.status.maxHp/3f;
        shine.GetComponent<Image>().color = new Color(1,1,1,0);
        // 플레이어 HP 바의 값 가져오기
        slider.value = GameManager.Instance.player.status.GetHPbarValue();
        // 텍스트 애니메이션 시작
        text.text = null;
        text.DOText("건강해지는 기분이다...!!!", duration, false);
        StopCoroutine(FillHPBar());
        StartCoroutine(FillHPBar());
    }

    IEnumerator FillHPBar()
    {
        while(slider.value < 1)
        {
            GameManager.Instance.player.status.HP += fillspeed * Time.deltaTime;
            slider.value = GameManager.Instance.player.status.GetHPbarValue();
            yield return null;
        }
        slider.value = 1;
        shine.GetComponent<Image>().color = new Color(1,1,1,0.3f);
        shine.GetComponent<Animator>().Play("shineEffect");
        yield return delay;
        GameManager.Instance.player.SetHPBar();
        GameManager.Instance.ExitStage(stageType);
    }

}
