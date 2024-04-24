using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class FarmInvenShowingSlot : MonoBehaviour
{
    public Crop crop;
    public Image packImage;
    public Image seedImage;
    public GameObject countBox;
    public TextMeshProUGUI countText;
    public int count = 1;
    protected Outline outline;

    private void Start() {
        outline = GetComponent<Outline>();
        outline.enabled= false;        
    }

    public void SetSlot(Crop crop)
    {
        this.crop = crop;
        packImage.sprite = crop.gradeSetting.seedPackImage;
        seedImage.sprite = crop.cropSetting.seedSprite;
        count = crop.count;

        if(count > 1)
        {
            countText.text = count.ToString();
            countBox.SetActive(true);
        }
        else
        {
            countBox.SetActive(false);
        }
    }

}
