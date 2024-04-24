using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SlotOutline : MonoBehaviour
{
    Image selectImage;
    public void Init()
    {
        selectImage = GetComponent<Image>();
        selectImage.enabled = false;
    }

    public void SelectImage(Vector3 pos)
    {
        transform.position = pos;
        selectImage.enabled = true;
    }

    public void DeselectImage()
    {
        selectImage.enabled = false;
    }
}
