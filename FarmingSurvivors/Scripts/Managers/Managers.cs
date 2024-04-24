
using System;
using DG.Tweening;
using Equipment;
using StageMap;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// 여러 Manager를 하나의 클래스로 묶어서 관리
/// 싱글톤이며 어플리케이션 전체 관리 
/// 다른 매니저들을 static으로 들고 있게 하여
/// 외부에서 Managers를 통해 여러 매니저에 접근 가능하도록 한다
/// </summary>
public class Managers : SingletoneBehaviour<Managers>
{
     public bool IsTitleLoaded = false;

    protected override void Awake()
    {
        base.Awake();
        
        if (Instance != null)
            if(Instance != this)
                return;

        if(PlayerPrefs.HasKey("Gold")) 
            PlayerPrefs.SetInt("Gold", 500000);
        else
            PlayerPrefs.SetInt("Gold", 500);

        Application.targetFrameRate = 30;

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnLoaded;

        DataManager.Instance.Init(); // 데이터 먼저..
        SaveManager.Instance.Init();  
        SoundManager.Instance.Init();
        EquipManager.Instance.Init();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        switch (arg0.buildIndex)
        {
            case 0:    // MainScene
                UIManager.Instance.Init();
                FarmManager.Instance.Init();
                SoundManager.Instance.PlayBGM(BGMSounds.Intro);
                break;
            case 1:    // PlayScene
                {
                    GameManager.Instance.Init();
                    LevelUpManager.Instance.Init(DataManager.Instance);
                }
                break;
        }
    }
    private void OnSceneUnLoaded(Scene arg0)
    {
        DOTween.KillAll();
        switch (arg0.buildIndex)
        {
            case 0:    // MainScene
                FarmManager.Instance.Clear();
                SaveManager.Instance.Save();
                break;
            case 1:    // PlayScene
                GameManager.Instance.Clear();
                SaveManager.Instance.Save();
                PoolManager.Instance.Clear();
                break;
        }
    }
    private void OnApplicationQuit()
    {
        SaveManager.Instance.OnApplicationQuit();
    }
    void OnApplicationPause(bool isPaused)
    {
    }
    public static void DateChanged()
    {
        AdManager.DateChanged();
    }

    public void QuitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
