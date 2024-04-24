using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : SingletoneBehaviour<UIManager>
{
    public MainSceneCanvas mainSceneCanvas;
    public EquipmentInfoPanel equipUI;
    public FarmUIPanel farmUI;
    public DragSlot dragSlot;
    
    public void Init()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            mainSceneCanvas = GameObject.FindObjectOfType<MainSceneCanvas>();
            equipUI = mainSceneCanvas.panels[2] as EquipmentInfoPanel;
            dragSlot = mainSceneCanvas.panels[2].gameObject.GetComponentInChildren<DragSlot>();
            dragSlot.Init();

            farmUI = mainSceneCanvas.panels[4] as FarmUIPanel;
        }
    }
}
