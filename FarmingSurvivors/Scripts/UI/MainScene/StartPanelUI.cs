using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StartPanelUI : UIPanel
{
    public RectTransform mainButtons;

    RectTransform originMainButtons;

    private void Awake() {
        originMainButtons = mainButtons;
    }

    public void FromFarm()
    {
        Camera.main.DOOrthoSize(10, 0.5f);
        Camera.main.transform.DOMoveY(0, 0.5f);
        mainButtons.DOAnchorPosY(0, 0.3f);
    }
    public void GoFarm()
    {
        Camera.main.DOOrthoSize(8, 0.5f);
        Camera.main.transform.DOMoveY(-1.8f, 0.5f);
        mainButtons.DOKill();
        mainButtons.DOAnchorPosY(-600, 0.3f).OnComplete(()=> UIManager.Instance.mainSceneCanvas.ShowFarmUI());
    }
    
}
