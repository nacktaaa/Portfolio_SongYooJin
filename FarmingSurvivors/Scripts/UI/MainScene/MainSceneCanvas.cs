using System.Collections;
using System.Collections.Generic;
using Equipment;
using StageMap;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneCanvas : MonoBehaviour
{
    public GameObject titlePanel;
    public GameObject topInfoPanel;
    public GameObject continueCheckPopup;
    public Button topReturnButton;
    public UIPanel[] panels;
    public Sprite[] characterSprites;

    const int START = 0;
    const int CHARACTER_SELECT = 1;
    const int EQUIPMENT_INFO = 2;
    const int CHAPTER_SELECT = 3;
    const int FARM = 4;
    const int STORE = 5;

    Image characterImage;   // 캐릭터 선택창의 이미지 컴포넌트. 스프라이트 변경을 위함
    UIPanel curPanel;       // 현재 보여지고 있는 판넬을 저장 
    int curSelect = 0;      // 현재 선택된 캐릭터의 스프라이트를 인덱스로 저장
    CharacterSelectUI ch;

    private void Awake()
    {
        topInfoPanel.SetActive(false);
        foreach (var panel in panels)
            panel.gameObject.SetActive(false);

        if (!Managers.Instance.IsTitleLoaded)
        {
            titlePanel.SetActive(true);
            Managers.Instance.IsTitleLoaded = true;
        }
        else
        {
            titlePanel.SetActive(false);
            panels[START].gameObject.SetActive(true);
            topReturnButton.gameObject.SetActive(false);
            topInfoPanel.SetActive(true);
        }

        Transform child = panels[CHARACTER_SELECT].transform.GetChild(1);
        characterImage = child.transform.GetChild(0).GetComponent<Image>();


    }

    // titlePanel
    public void TouchToContinueButton()
    {
        titlePanel.SetActive(false);
        panels[START].gameObject.SetActive(true);
        topReturnButton.gameObject.SetActive(false);
        topInfoPanel.gameObject.SetActive(true);
        curPanel = panels[START];
    }

    // startPanel 
    public void PlayButton()
    {
        if (PlayerPrefs.HasKey("Map"))   // 추후 변경 예정
            continueCheckPopup.SetActive(true);
        else
            NewGameButton();
    }

    public void ContinueButton()
    {
        // 저장된 게임 불러오기 
        GameManager.Instance.isContinue = true;
        SceneManager.LoadScene(1);
    }

    public void NewGameButton()
    {
        // 캐릭터 선택 판넬로
        SaveManager.Instance.DeleteStageData();
        panels[START].gameObject.SetActive(false);
        continueCheckPopup.SetActive(false);
        panels[CHARACTER_SELECT].gameObject.SetActive(true);
        topReturnButton.gameObject.SetActive(true);
        curPanel = panels[CHARACTER_SELECT];
        characterImage.sprite = characterSprites[curSelect];
        ch = panels[CHARACTER_SELECT] as CharacterSelectUI;
        ch.SetDescription((Character)curSelect);
    }

    // characterSelectPanel
    public void NextButton()
    {
        curSelect = (curSelect + 1) % characterSprites.Length;
        GameManager.Instance.NextCharacter();
        characterImage.sprite = characterSprites[curSelect];
        ch.SetDescription((Character)curSelect);

    }
    public void PreviousButton()
    {
        curSelect = (curSelect + characterSprites.Length - 1) % characterSprites.Length;
        GameManager.Instance.PreviousCharacter();
        characterImage.sprite = characterSprites[curSelect];

        ch.SetDescription((Character)curSelect);
    }
    public void TouchCharacterImage()
    {
        panels[CHARACTER_SELECT].gameObject.SetActive(false);
        panels[EQUIPMENT_INFO].gameObject.SetActive(true);
        panels[EQUIPMENT_INFO].GetComponent<EquipmentInfoPanel>().Init();
        EquipManager.Instance.inventory.InventoryChanged();
        EquipManager.Instance.inventory.MountedChanged();
        curPanel = panels[EQUIPMENT_INFO];
    }
    public void CharacterSelectButton()
    {
        GameManager.Instance.curCharacter = characterSprites[curSelect].name;
        panels[CHARACTER_SELECT].gameObject.SetActive(false);
        panels[CHAPTER_SELECT].gameObject.SetActive(true);
        curPanel = panels[CHAPTER_SELECT];
    }

    // chapterSelectPanel
    public void StartChapter(int idx)
    {
        // 플레이씬으로 전환
        // 게임매니저에 챕터 인덱스 전달 -> 게임매니저에서 챕터 초기화 및 세팅
        SceneManager.LoadScene(1);
        GameManager.Instance.mapConfig = DataManager.Instance.ChapterConfigs[idx - 1];
    }

    // topInfoPanel
    public void ReturnButton()
    {
        if(curPanel == panels[FARM])
        {
            panels[START].GetComponent<StartPanelUI>().FromFarm();
            FarmManager.Instance.HidePriceSlots();
            FarmManager.Instance.DisableFarm();
        }
            
        curPanel.gameObject.SetActive(false);
        curPanel.previousPanel.gameObject.SetActive(true);
        curPanel = curPanel.previousPanel;
        
        if(curPanel == panels[START])
        {
            topInfoPanel.SetActive(true);
            topReturnButton.gameObject.SetActive(false);
        }
    }

    public void ShowFarmUI()
    {
        panels[START].gameObject.SetActive(false);
        panels[FARM].gameObject.SetActive(true);
        panels[FARM].GetComponent<FarmUIPanel>().SetInvenSlots();
        FarmManager.Instance.EnableFarm();
        topReturnButton.gameObject.SetActive(true);
        curPanel = panels[FARM];
    }
    public void ShowStoreUI()
    {
        panels[STORE].gameObject.SetActive(true);
        topReturnButton.gameObject.SetActive(true);
        curPanel = panels[STORE];
    }

    public void ExitButton()
    {
        Managers.Instance.QuitApp();
    }
}
