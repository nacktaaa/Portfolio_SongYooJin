using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlagSlot : MonoBehaviour
{
    public bool isEmpty = true;
    public Image backImage;
    public Image stateImage;
    int flagStep = 1;
    public int FlagStep
    {
        get { return flagStep;}
        set
        {
            flagStep = value;
            flagStep = Mathf.Clamp(flagStep, 1, 3);
        }    
    }

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        backImage = transform.GetChild(0).GetComponent<Image>();
        stateImage = transform.GetChild(1).GetComponent<Image>();
        //Debug.Log("FlagSlot Init()");
    }

    

}
