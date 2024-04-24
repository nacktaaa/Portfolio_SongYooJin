using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CompostSlot : MonoBehaviour, IPointerClickHandler
{
    public int price = 1000;
    public int growthPoint = 25;
    public GameObject errorPopup;
    public TextMeshProUGUI priceText;
    bool isSelected = false;
    Outline outline;
    CanvasGroup canvasGroup;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(isSelected)
        {
            UnSelected();
            FarmManager.Instance.selectCompost = null;
            return;
        }

        if(GoldManager.Instance.CurrentGold < price)
            errorPopup.SetActive(true);
        else
        {
            Selected();
            FarmManager.Instance.selectCompost = this;
        }
    }

    private void Start() {
        outline = GetComponent<Outline>();
        canvasGroup = GetComponent<CanvasGroup>();
        priceText.text = price.ToString();
        outline.enabled = false;
    }
    private void Selected()
    {
        isSelected = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(1.2f, 0.1f));
        sequence.Append(transform.DOScale(1f, 0.1f));
        outline.enabled = true;
    }

    public void UnSelected()
    {
        isSelected = false;
        outline.enabled = false;
    }
}
