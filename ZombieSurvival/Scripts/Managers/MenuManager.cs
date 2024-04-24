using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    public GameObject StartMenuCanvas;
    public GameObject characterSelectCanvas;
    public GameObject loadCanvas;
    public GameObject optionCanvas;
    public GameObject[] playerPrefabs;
    public GameObject[] playerSamples;
    public GameObject saveFileButton;
    public GameObject curCanvas;
    public GameObject popup;
    string[] detailTexts;
    int curSampleIdx;
    public Transform saveFileHolder;
    TextMeshProUGUI detailText;
    public Image fadeImage;

    private void Awake()
    {
        curCanvas = StartMenuCanvas;

        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            curSampleIdx = 0;
            detailTexts = new string[playerSamples.Length];
            detailTexts[0] = "재빠르고 시야가 넓지만 잠이 많습니다.";               // Nary
            detailTexts[1] = "잠을 적게 자며 소식가이지만 이동 속도가 느립니다.";       // Susan
            detailTexts[2] = "용감하며 공격력이 강하지만 많이 먹습니다.";             // Bob
            detailText = characterSelectCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            detailText.text = detailTexts[curSampleIdx];

            fadeImage.transform.root.gameObject.SetActive(false);
            this.gameObject.SetActive(true);
        }
    }

    #region 메인화면 메뉴
    // StartCanvasButtons
    // 새로운 게임 버튼
    public void NewGameButton()
    {
        Camera.main.GetComponent<Animator>().SetTrigger("StartSelect");
        curCanvas.SetActive(false);
        characterSelectCanvas.SetActive(true); 
        curCanvas = characterSelectCanvas;
        Managers.Sound.PlayEffectSound(Define.EffectSound.Click);
    }

    // 불러오기 버튼
    public void LoadGameButton()
    {
        // 현재 캔버스를 loadCanvas로 변경
        curCanvas.SetActive(false);
        loadCanvas.SetActive(true);
        curCanvas = loadCanvas;

        // json으로 저장된 세이브 파일의 이름 로드
        List<string> saveFiles = Managers.Data.LoadSaveFiles();

        for(int i =0; i < saveFiles.Count; i ++)
        {   
            SaveFilebutton button = Instantiate(saveFileButton, saveFileHolder).GetComponent<SaveFilebutton>();
            button.fileName = saveFiles[(saveFiles.Count -1) - i];
            TextMeshProUGUI textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
            int sub = button.fileName.LastIndexOf(".");
            textMesh.text = button.fileName.Substring(0, sub);
        }

        Managers.Sound.PlayEffectSound(Define.EffectSound.Click);
    }

    // 옵션 버튼
    public void OptionButton()
    {
        curCanvas.SetActive(false);
        optionCanvas.SetActive(true);
        curCanvas = optionCanvas;
        Managers.Sound.PlayEffectSound(Define.EffectSound.Click);
    }
    // 나가기 버튼
    public void ExitButton()
    {
        Application.Quit();
    }

    // LoadCanvasButtons
    // 팝업 창 버튼
    // 세이브파일로 게임 시작
    public void StartBySaveFileButton()
    {
        Managers.Sound.PlayTriggerMusic(Define.TriggerSound.Stab1);
        StartCoroutine(LoadPlayScene());
    }

    public void CancelButton()
    {
        popup.SetActive(false);
        Managers.Sound.PlayEffectSound(Define.EffectSound.Click);
    }

    // CharacterSelectCanvasButtons
    // 이전 캐릭터 버튼
    public void PreviousButton()
    {
        playerSamples[curSampleIdx].SetActive(false);
        curSampleIdx--;
        if(curSampleIdx < 0)
            curSampleIdx = playerSamples.Length - 1;

        detailText.text = detailTexts[curSampleIdx];
        playerSamples[curSampleIdx].SetActive(true);
        Managers.Sound.PlayEffectSound(Define.EffectSound.Click);
    }
    // 다음 캐릭터 버튼
    public void NextButton()
    {
        playerSamples[curSampleIdx].SetActive(false);
        curSampleIdx = (curSampleIdx + 1) % playerSamples.Length;
        detailText.text = detailTexts[curSampleIdx];
        playerSamples[curSampleIdx].SetActive(true);
        Managers.Sound.PlayEffectSound(Define.EffectSound.Click);
    }
    // 이대로 시작 버튼
    public void StartButton()
    {
        Managers.playerPrefab = playerPrefabs[curSampleIdx];
        fadeImage.transform.root.gameObject.SetActive(true);
        Managers.saveDataName = null;
        Managers.Sound.PlayTriggerMusic(Define.TriggerSound.Stab1);
        
        StartCoroutine(LoadPlayScene());
    }
    // 돌아가기 버튼
    public void ReturnButton()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0 && curCanvas == characterSelectCanvas)
            Camera.main.GetComponent<Animator>().SetTrigger("Return");
        curCanvas.SetActive(false);
        StartMenuCanvas.SetActive(true);
        curCanvas = StartMenuCanvas;
        Managers.Sound.PlayEffectSound(Define.EffectSound.Click);
    }

    IEnumerator LoadPlayScene()
    {
        fadeImage.gameObject.SetActive(true);
        float count = 0;
        while ( count < 1)
        {
            count += Time.deltaTime;
            fadeImage.color = new Color(0,0,0, count);
            yield return null;
        }
        count = 1;
        Managers.Game.isPause = false;
        SceneManager.LoadScene(1);
    }
#endregion

    public void ContinueButton()
    {
        Managers.Game.isPause = false;
        AudioListener.pause = false;
        this.gameObject.SetActive(false);
    }

    public void ReturnMainButton()
    {
        SceneManager.LoadScene(0);
        AudioListener.pause = false;
        Managers.isInMenuScene = true;
    }

}
