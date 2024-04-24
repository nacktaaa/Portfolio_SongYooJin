using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Managers : MonoBehaviour
{
    static Managers instance;
    static Managers Instance { get { return instance;}}

    DataManager data = new DataManager();
    GameManager game = new GameManager();
    InputManager input = new InputManager();
    InventoryManager inven = new InventoryManager();
    PoolManager pool = new PoolManager();
    SoundManager sound = new SoundManager();
    UIManager ui = new UIManager();

    public static DataManager Data {get {return Instance.data;}}
    public static GameManager Game {get {return Instance.game;}}
    public static InputManager Input {get { return Instance.input;}}
    public static InventoryManager Inven {get {return Instance.inven;}}
    public static PoolManager Pool {get {return Instance.pool;}}
    public static SoundManager Sound {get {return Instance.sound;}}
    public static UIManager UI {get {return Instance.ui;}}

    public static bool isInMenuScene = true;
    public static string saveDataPath;
    public static string saveDataName;
    public static GameObject playerPrefab;

    void Awake()
    {
        if(instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if ( go == null)
            {
                go = new GameObject("@Managers");
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go);
            instance = go.GetComponent<Managers>();
        }
        if(instance != this)
            Destroy(this.gameObject);

        saveDataPath = Application.persistentDataPath + "/SaveData/";
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnLoaded;
        SceneManager.sceneUnloaded += OnSceneUnLoaded;

        Data.Init();
        Sound.Init();
    }


    void OnDisable()
    {
        Game.Clear();
        Pool.Clear();
        Inven.Clear();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnLoaded;
    }

    private void OnApplicationQuit()
    {
        Game.Clear();
        Pool.Clear();
        Inven.Clear();
        Data.Clear();
        Sound.Clear();
    }
    private void OnSceneLoaded(Scene _scene, LoadSceneMode _loadSceneMode)
    {
        //Debug.Log($"LoadedScene : {_scene.name}");
        if(_scene.buildIndex == 1)
        {
            UI.Init();
            Pool.Init();
            Game.Init();
            Sound.PlayBGM(Define.BGM.Ambient);
            isInMenuScene = false;
        }
        else if(_scene.buildIndex == 0)
        {
            Sound.PlayBGM(Define.BGM.Intro);
            isInMenuScene = true;
        }
    }

    private void OnSceneUnLoaded(Scene _scene)
    {
        //Debug.Log("OnScneneUnLoaded " + _scene.name);
        if(_scene.buildIndex == 1)
        {
            Game.Clear();
            Pool.Clear();
            Inven.Clear();
            UI.Clear();
        }
    }

    private void Update()
    {
        if(isInMenuScene)
            return;

        Input.OnUpdate();
        Game.OnUpdate();
        UI.OnUpdate();
    }

}
