//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Outline))]
public class Storage : MonoBehaviour, IDetectable
{
    public int idx;
    public bool isOpened = false;
    bool isVisible = false;
    Outline _outline;

    public float curWeight = 0f;
    public float maxWeight;

    public List<Item> items = new List<Item>();

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _outline.enabled = false;
        maxWeight = Random.Range(8, 15);
    }

    
    public void OnPointerClick(PointerEventData eventData)
    {
        if(isVisible)
        {
            // 일단storageInvenUI를 켜고 아이템을 연결 
            Managers.Inven.SetStorage(this);
            Managers.UI.ShowInvenUI(items, Define.InvenType.World);
            isOpened = true;
            Managers.Sound.PlayEffectSound(Define.EffectSound.OpenStorage);
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

    public string GetWeightText()
    {
        string weightText = string.Format("{0:0.##}/{1:0.##}", curWeight, maxWeight);
        return weightText;
    }

}
