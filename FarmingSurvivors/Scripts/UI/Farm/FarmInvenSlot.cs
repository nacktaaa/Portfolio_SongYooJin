using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class FarmInvenSlot : FarmInvenShowingSlot, IPointerClickHandler
{

    private void Start() {
        outline = GetComponent<Outline>();
        outline.enabled= false;        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(FarmManager.Instance.selectSlot != null)
            FarmManager.Instance.HidePriceSlots();
            
        if(FarmManager.Instance.selectInvenSlot == null)
        {
            Debug.Log($"select {crop.cropSetting.cropName}");
            Selected();
            FarmManager.Instance.selectInvenSlot = this;
        }
        else
        {
            if(FarmManager.Instance.selectInvenSlot != this)
            {
                Debug.Log($"select {crop.cropSetting.cropName}");
                Selected();
                FarmManager.Instance.selectInvenSlot.UnSelected();
                FarmManager.Instance.selectInvenSlot = this;
            }
            else
            {
                UnSelected();
                FarmManager.Instance.selectInvenSlot = null;
            }
        }
    }

    private void Selected()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(1.2f, 0.1f));
        sequence.Append(transform.DOScale(1f, 0.1f));
        outline.enabled = true;
    }

    public void UnSelected()
    {
        outline.enabled = false;
    }

}
