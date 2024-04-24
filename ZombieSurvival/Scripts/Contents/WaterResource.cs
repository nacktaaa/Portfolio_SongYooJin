using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
public class WaterResource : MonoBehaviour, IDetectable
{
    public bool isVisible = true;
    Button drinkButton;
    Outline _outline;
    Coroutine drinkCoroutin;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _outline.enabled = false;
        drinkButton = GetComponentInChildren<Button>();
        drinkButton.gameObject.SetActive(false);
        drinkButton.onClick.AddListener(DrinkButtonClick);
    }
    public void OnVisible(bool _on)
    {
        isVisible = _on;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(isVisible)
        {
            Vector3 dir = transform.position - Camera.main.transform.position;
            drinkButton.transform.rotation = Quaternion.LookRotation(dir);
            Managers.UI.curActionButton = drinkButton.gameObject;
            drinkButton.gameObject.SetActive(true);
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
    public void DrinkButtonClick()
    {
        drinkCoroutin = StartCoroutine(DrinkWater());
    }
    IEnumerator DrinkWater()
    {
        Managers.Sound.PlayEffectSound(Define.EffectSound.DrinkWater);
        Managers.UI.ShowSlider();
        float count = 0;
        while(count < Managers.Game.player.stat.CurActionSpeed)
        {
            count += Time.deltaTime;
            Managers.UI.SetSlider(count/Managers.Game.player.stat.CurActionSpeed);
            if(Input.anyKey)
            {
                StopCoroutine(drinkCoroutin);
                Managers.Sound.StopEffectSound();
                Managers.UI.HideSlider();
            }
            yield return null;
        }
        Managers.Game.player.stat.Thirst -= 100f;
        Managers.UI.HideSlider();
    }
    
}
