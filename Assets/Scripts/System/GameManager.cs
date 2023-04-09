using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    Player player;
    ItemDataManager itemData;
    InventoryUI inventoryUI;
    CharacterBoxUI characterBoxUI;

    StatPoints statPoints;

    StrengthStats strStats;
    StaminaStats staStats;
    IntelligenceStats intStats;
    DefenseStats defStats;
    LuckyStats luckStats;

    public Player MainPlayer => player;

    public ItemDataManager ItemData => itemData;

    static GameManager instance = null;
    public static GameManager Inst => instance;

    public InventoryUI InvenUI => inventoryUI;

    public StatPoints StatPoints => statPoints;

    public CharacterBoxUI CharacterBoxUI => characterBoxUI;

    public StrengthStats StrStats => strStats;
    public StaminaStats StaStats => staStats;
    public IntelligenceStats IntStats => intStats;
    public DefenseStats DefStats => defStats;
    public LuckyStats LuckStats => luckStats;

    // UIÃ¢À» Ä×À» ¶§ -------------------------------------------------------------------------------------------------------------------------------------
    bool onUIMode;

    public bool OnUiMode
    {
        get => onUIMode;
        set
        {
            onUIMode = value;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Initialize();
    }

    private void Initialize()
    {
        player = FindObjectOfType<Player>();
        itemData = GetComponent<ItemDataManager>();
        inventoryUI = FindObjectOfType<InventoryUI>();
        statPoints = FindObjectOfType<StatPoints>();
        characterBoxUI = FindObjectOfType<CharacterBoxUI>();
        strStats = FindObjectOfType<StrengthStats>();
        staStats = FindObjectOfType<StaminaStats>();
        intStats = FindObjectOfType<IntelligenceStats>();
        defStats = FindObjectOfType<DefenseStats>();
        luckStats = FindObjectOfType<LuckyStats>();
    }

    private void Update()
    {
        //if (onUIMode)
        //{
        //    Time.timeScale = 0;
        //}
        //else
        //{
        //    Time.timeScale = 1;
        //}
    }
}
