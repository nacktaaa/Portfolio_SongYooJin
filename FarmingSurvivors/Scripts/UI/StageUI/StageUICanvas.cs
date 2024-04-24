using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StageUICanvas : MonoBehaviour
{
    public BasicStageUI basicStageUI;
    public BattleStageUI battleStageUI;
    public BossStageUI bossStageUI;
    public LevelUpPanel levelUpPanel;
    public GoldStageUI goldStageUI;
    public StoreStageUI storeStageUI;
    public RestStageUI restStageUI;
    public GameObject stageClearPanel;
    public GameObject gameOverPanel;
    public ChapterClearUI chapterClearUI;
    public RevivalPopup revivalPopup;

    public Button generateButton;
    public Button revivalButton;

    public Image infoImage1;
    public Image infoImage2;

    private void Start() 
    {
        HideAllPanel();

        infoImage1.GetComponent<InfoButtonImage>().SetInfoImage();
        infoImage2.GetComponent<InfoButtonImage>().SetInfoImage();
    }

    // UI 전환
    public void HideAllPanel()
    {
        basicStageUI.gameObject.SetActive(false);
        battleStageUI.gameObject.SetActive(false);
        bossStageUI.gameObject.SetActive(false);
        goldStageUI.gameObject.SetActive(false);
        levelUpPanel.gameObject.SetActive(false);
        storeStageUI.gameObject.SetActive(false);
        restStageUI.gameObject.SetActive(false);
        stageClearPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        chapterClearUI.gameObject.SetActive(false);
    }
    public void ShowBattleStageUI()
    {
        basicStageUI.gameObject.SetActive(true);
        battleStageUI.gameObject.SetActive(true);
    }
    public void ShowGoldStageUI()
    {
        basicStageUI.gameObject.SetActive(true);
        goldStageUI.gameObject.SetActive(true);
    }
    public void ShowBossStageUI()
    {
        basicStageUI.gameObject.SetActive(true);
        bossStageUI.gameObject.SetActive(true);
    }

    // stageClearPanel '다음으로' 버튼 
    public void NextStageButton()
    {
        stageClearPanel.SetActive(false);
        GameManager.Instance.ExitStage(GameManager.Instance.curStage);
    }

    // GameOverPanel > 돌아가기 버튼(메인 화면으로)
    public void RetrunToMainSceneButton()
    {
        GameManager.Instance.ExitChapter();
    }

    // 스킬 상점 스테이지 UI 노출
    public void ShowStoreStageUI()
    {
        storeStageUI.gameObject.SetActive(true);
    }

    public void ShowRestStageUI()
    {
        restStageUI.gameObject.SetActive(true);
    }

    // 게임 오버 판넬
    public void ShowGameOverPanel(Sprite character)
    {
        gameOverPanel.transform.GetChild(3).GetComponent<Image>().sprite = character;
        revivalButton.interactable = GameManager.Instance.useRevival ? false : true;
        gameOverPanel.SetActive(true);
    }

    // 게임 오버 판넬 > 돌아가기 버튼(메인 화면으로)
    public void ReturnButton()
    {
        //Managers.Game.battleStage.Clear();
        GameManager.Instance.ExitChapter();
    }

    // 스테이지 클리어 판넬
    public void ShowStageClearPanel()
    {
        stageClearPanel.SetActive(true);
    }

    public void ShowChapterClearPanel()
    {
        chapterClearUI.gameObject.SetActive(true);
    }

    public void LoadNextChapter()
    {
        GameManager.Instance.LoadNextChapter();
    }

    public void RevivalButton()
    {
        revivalPopup.gameObject.SetActive(true);
    }

    public void RevivalByGoldButton()
    {
        GameManager.Instance.ContinueStageWithGold();
    }

    public void GamePause()
    {
        GameManager.Instance.Pause();
    }

    public void GameResume()
    {
        GameManager.Instance.Resume();
    }

}
