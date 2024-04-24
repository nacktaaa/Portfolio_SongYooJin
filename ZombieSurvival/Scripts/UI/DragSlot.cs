using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    [SerializeField]
    Image image;
    [SerializeField]
    Image iconImage;

    public Item item;
    public Slot slot;

    Transform originTr;
    Color transparentColor = new Color(0,0,0,0);

    public void Init()
    {
        originTr = transform;
        image = GetComponent<Image>();
        iconImage = transform.GetChild(0).GetComponent<Image>();
        image.color = transparentColor;
        iconImage.sprite = null;
    }

    public void CopySlot(Slot _slot)
    {
        image.color = Color.white; // 보이게 바꿈 
        slot = _slot;
        iconImage.sprite = slot.item.iconImage;  // 슬롯 아이콘 복사
        iconImage.gameObject.SetActive(true);
    }

    public void ClearDragSlot()
    {
        image.color = transparentColor; // 다시 안보이게
        iconImage.sprite = null;
        slot = null;
        iconImage.gameObject.SetActive(false);
        transform.position = originTr.position;
    } 
    
}
