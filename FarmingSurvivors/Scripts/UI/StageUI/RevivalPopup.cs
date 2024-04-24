using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RevivalPopup : MonoBehaviour
{
    public Button AdButton;
    public Button GoldButton;
    Coroutine runningCoroutine = null;

    public void AdRevival()
    {
        if (SaveManager.Instance.PackagePurchased == 0)
        {
            if (runningCoroutine != null)
            {
                StopCoroutine(runningCoroutine);
            }
            StartCoroutine(AdManager.ShowAd(AdType.Revive));
        }
        else
        {
            GameManager.Instance.ContinueStage();
        }
    }
}
