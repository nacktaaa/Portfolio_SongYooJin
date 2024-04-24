using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 게임 화면에 두 종류의 인벤토리 UI가 존재한다.
/// 월드에서 습득할 수 있는 아이템을 보여주는 StorageInvenUI
/// 플레이어가 소지한 아이템을 보여주는 PlayerInvenUI
/// 해당 클래스에서 두 인벤토리UI 객체를 관리한다.
/// 그외 UI 와 관련한 기능 작성
/// </summary>
public class UIManager 
{
    public StorageInvenUI storageInvenUI;
    public PlayerInvenUI playerInvenUI;
    public DragSlot dragSlot;
    public SlotOutline slotOutline;
    public Slot selectSlot;
    public GameObject slider;
    public GameObject magazineCanvas;
    public GameObject curActionButton;
    public GameObject sleepCanvas;
    public GameObject deathCanvas;
    public GameObject menuCanvas;

    ItemInspector itemInspector;
    FlagUI flagUI;
    Color transparentColor = new Color(0,0,0,0); // 아이콘이미지의 투명화를 위한 컬러 변수

    public void Init()
    {
        GameObject invenUIPrefab = Resources.Load<GameObject>("Prefabs/UI/InventoryUI");
        GameObject sliderPrefab = Resources.Load<GameObject>("Prefabs/UI/Slider");
        GameObject sleepPanelPrefab = Resources.Load<GameObject>("Prefabs/UI/SleepCanvas");
        GameObject flagUIPrefab = Resources.Load<GameObject>("Prefabs/UI/FlagUI");
        GameObject magazinePrefab = Resources.Load<GameObject>("Prefabs/UI/MagazineCanvas");
        GameObject menuCanvasPrefab = Resources.Load<GameObject>("Prefabs/UI/MenuCanvas"); 

        GameObject go = GameObject.Instantiate(invenUIPrefab);
        go.name = "@InventoryUI";
        
        slider = GameObject.Instantiate(sliderPrefab);
        sleepCanvas = GameObject.Instantiate(sleepPanelPrefab);
        deathCanvas = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/DeathCanvas"));
        magazineCanvas = GameObject.Instantiate(magazinePrefab);
        flagUI = GameObject.Instantiate(flagUIPrefab).GetComponent<FlagUI>();
        menuCanvas = GameObject.Instantiate(menuCanvasPrefab);

        magazineCanvas.SetActive(false);
        menuCanvas.SetActive(false);

        slider.GetComponentInChildren<Slider>().value = 0;
        slider.SetActive(false);
        
        sleepCanvas.GetComponentInChildren<Image>().color = transparentColor;
        sleepCanvas.SetActive(false);
        deathCanvas.SetActive(false);

        storageInvenUI = go.GetComponentInChildren<StorageInvenUI>();
        playerInvenUI = go.GetComponentInChildren<PlayerInvenUI>();
        dragSlot = go.GetComponentInChildren<DragSlot>();
        slotOutline = go.GetComponentInChildren<SlotOutline>();
        itemInspector = go.GetComponentInChildren<ItemInspector>();
        storageInvenUI.gameObject.SetActive(false);
        playerInvenUI.gameObject.SetActive(false);

        flagUI.Init();
        storageInvenUI.Init();
        playerInvenUI.Init();
        dragSlot.Init();
        slotOutline.Init();
        itemInspector.Init();
    }
    public void OnUpdate()
    {
         if(Input.GetKeyDown(KeyCode.Escape))
         {
            if(!Managers.Game.isPause)
            {
                Managers.Game.isPause = true;
                AudioListener.pause = true;
                menuCanvas.SetActive(true);
            }
            else
            {
                Managers.Game.isPause = false;
                AudioListener.pause = false;
                MenuManager menu = menuCanvas.GetComponent<MenuManager>();
                menu.StartMenuCanvas.SetActive(true);
                menu.loadCanvas.SetActive(false);
                menu.optionCanvas.SetActive(false);
                menuCanvas.SetActive(false);
            }
         }

         if(Managers.Input.Inven)
        {
            if(!Managers.UI.playerInvenUI.isOpen)
            {
                ShowInvenUI(Managers.Inven.playerItems);
                Managers.Sound.PlayEffectSound(Define.EffectSound.OpenBag);
            }
            else
            {
                HideInvenUI();
                Managers.Sound.PlayEffectSound(Define.EffectSound.CloseBag);
            }
        }
         if(IsInvenUIOpen(Define.InvenType.World))
         {
            float distance = 
                Vector3.Distance(Managers.Inven.selectedStorage.transform.position, Managers.Game.player.transform.position);
            if(Mathf.Abs(distance) > 1.5f)
                HideInvenUI(Define.InvenType.World);
         }
         if(!EventSystem.current.IsPointerOverGameObject())
         {
            if(curActionButton!=null)
            {
                curActionButton.SetActive(false);
                curActionButton = null;
            }
         }

    }

    public void ShowInvenUI(List<Item> items, Define.InvenType type = Define.InvenType.Player)
    {
        switch(type)
        {
            case Define.InvenType.Player :
            {
                playerInvenUI.isOpen = true;
                playerInvenUI.gameObject.SetActive(true);
                playerInvenUI.SetSlots(items);
            }
                break;
            case Define.InvenType.World :
            {
                storageInvenUI.isOpen = true;
                storageInvenUI.gameObject.SetActive(true);
                storageInvenUI.SetSlots(items);
            }
                break;
        }
    }

    public void HideInvenUI(Define.InvenType type = Define.InvenType.Player)
    {
        slotOutline.DeselectImage();
        HideInspectorUI();

        switch(type)
        {
            case Define.InvenType.Player :
            {
                if(playerInvenUI.isOpen)
                {
                    playerInvenUI.Close();
                }
            }
                break;
            case Define.InvenType.World :
            {
                if(storageInvenUI.isOpen)
                {
                    storageInvenUI.Close();
                }
            }
                break;
        }
    }

    public void ShowInspectorUI(Slot _slot, Define.InvenType _type = Define.InvenType.Player)
    {
        selectSlot = _slot;
        itemInspector.ShowItemInspector(selectSlot, _type);
    }

    public void HideInspectorUI()
    {
        slotOutline.DeselectImage();
        itemInspector.HideItemInspector();
    }

    public void AddFlagUI(Define.FlagType statusEffect)
    {
        flagUI.AddFlagIcon(statusEffect);
    }

    public void ChangeFlagUI(Define.FlagType statusEffect, int step)
    {
        flagUI.ChangeFlagIcon(statusEffect, step);
    }

    public void RemoveFlagUI(Define.FlagType statusEffect)
    {
        flagUI.RemoveFlagIcon(statusEffect);
    }
    
    public void RemoveAllFlagUI()
    {
        flagUI.RemoveAllFlagIcon();
    }
    public bool IsInvenUIOpen(Define.InvenType _type = Define.InvenType.Player)
    {
        bool temp = playerInvenUI.isOpen;
        switch(_type)
        {
            case Define.InvenType.Player :
                temp = playerInvenUI.isOpen;
                break;
            case Define.InvenType.World :
                temp = storageInvenUI.isOpen;
                break;
        }
        return temp;
    }

    public void ShowSlider()
    {
        slider.transform.SetParent(Managers.Game.player.transform);
        slider.transform.rotation = Camera.main.transform.rotation;
        slider.gameObject.SetActive(true);
    }

    public void SetSlider(float _value)
    {
        slider.transform.localPosition = new Vector3 (0, 2, 0);
        Slider sliderConponent = slider.GetComponentInChildren<Slider>();
        sliderConponent.value = _value;
    }

    public void HideSlider()
    {
        slider.gameObject.SetActive(false);
        SetSlider(0);
    }

    public void ShowSleepCanvas(float alpha)
    {
        if(!sleepCanvas.activeSelf)
            sleepCanvas.SetActive(true);

        Image tempImag = sleepCanvas.GetComponentInChildren<Image>();
        tempImag.color = new Color(1,1,1, alpha);
    }

    public void CloseSleepCanvas()
    {
        sleepCanvas.GetComponentInChildren<Image>().color = transparentColor;
        sleepCanvas.SetActive(false);
    }

    public void ShowDeathCanvas(float alpha)
    {
        if(!deathCanvas.activeSelf)
            deathCanvas.SetActive(true);
        
        Image tempImag = deathCanvas.GetComponent<DeathCanvas>().deathPanel;
        tempImag.color = new Color(0,0,0, alpha);
    }

    public void RefreshMagazineUI()
    {
        string curMagazineText = 
            Managers.Inven.curMagazine == Managers.Inven.maxMagazine ? 
            $"<color=orange>{Managers.Inven.curMagazine}</color>" : $"{Managers.Inven.curMagazine}";

        magazineCanvas.GetComponentInChildren<TextMeshProUGUI>().text 
                = $"{curMagazineText} / {Managers.Inven.AmmoCount}";
        
    }

    public void Clear()
    {
        playerInvenUI.Clear();
    }
}
