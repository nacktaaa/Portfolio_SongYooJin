using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
public class SleepResource : MonoBehaviour, IDetectable
{
    public bool isVisible = true;
    Button sleepButton;
    Outline _outline;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _outline.enabled = false;
        sleepButton = GetComponentInChildren<Button>();
        sleepButton.gameObject.SetActive(false);
        sleepButton.onClick.AddListener(SleepButtonClick);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 메뉴 팝업
        if(isVisible)
        {
            Vector3 dir = transform.position - Camera.main.transform.position;
            sleepButton.transform.rotation = Quaternion.LookRotation(dir);
            Managers.UI.curActionButton = sleepButton.gameObject;
            sleepButton.gameObject.SetActive(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isVisible)
            _outline.enabled = true;
   }

    public void OnPointerExit(PointerEventData eventData)
    {
        _outline.enabled = false;
    }
    public void OnVisible(bool _on)
    {
        isVisible = _on;
    }

    public void SleepButtonClick()
    {
        if(Managers.Game.player.stat.Sleepiness < 0)
            return;

        StartCoroutine(Sleep());
    }
    
    IEnumerator Sleep()
    {
        Managers.Sound.PlayEffectSound(Define.EffectSound.Ticking);
        Managers.Game.isSleep = true;
        float sleepCount = 20f;
        float fadeCount = 0f;
        while (fadeCount < 1 && Managers.Game.isSleep)
        {
            fadeCount += Time.deltaTime;
            Managers.UI.ShowSleepCanvas(fadeCount);

            yield return null;
        }
        fadeCount = 1f;
        Time.timeScale = 10f;
        while(sleepCount >= 0 && Managers.Game.isSleep)
        {
            sleepCount -= Time.deltaTime;
            yield return null;
        }

        Time.timeScale = 1f;
        
        while(fadeCount > 0 && Managers.Game.isSleep)
        {
            fadeCount -= Time.deltaTime;
            Managers.UI.ShowSleepCanvas(fadeCount);
            yield return null;
        }
        Managers.Game.player.stat.Sleepiness -= 100f;
        Managers.Game.player.stat.HP += 30;
        Managers.Game.flag.ChangeFlagStep(Define.FlagType.Bleeding, isPlus : false);
        Managers.Game.flag.ChangeFlagStep(Define.FlagType.Injury, isPlus : false);
        Managers.Game.flag.ChangeFlagStep(Define.FlagType.Sick, isPlus : false);

        fadeCount = 0;
        Managers.UI.CloseSleepCanvas();
        Managers.Game.isSleep = false;
    }

}
