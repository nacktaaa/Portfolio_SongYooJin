using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum FarmInvenType
{
    Seed, Compost
}
public class FarmInvenTab : ToggleImageOnClick
{
    public FarmInvenType type;

    [SerializeField]
    FarmUIPanel farmUIPanel;

    override public void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        farmUIPanel.ShowTap(type, this);
    }
    
}
