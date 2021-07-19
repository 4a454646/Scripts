﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Scripts : MonoBehaviour {
    [SerializeField] public Animator terrain;
    public Dice dice;
    public Arrow arrow;
    public Enemy enemy;
    public Music music;
    public Colors colors;
    public Player player;
    public MenuIcon menuIcon;
    public Tutorial tutorial;
    public BackToMenu backToMenu;
    public MenuButton menuButton;
    public TurnManager turnManager;
    public ItemManager itemManager;
    public DiceSummoner diceSummoner;
    public SoundManager soundManager;
    public LevelManager levelManager;
    public StatSummoner statSummoner;
    public TombstoneData tombstoneData;
    public CharacterSelector characterSelector;
    public HighlightCalculator highlightCalculator;
    private readonly float[] delayArr = { 0.0001f, 0.001f, 0.005f, 0.01f, 0.0125f, 0.02f, 0.025f, 0.03f, 0.033f, 0.05f, 0.1f, 0.15f, 0.2f, 0.25f, 0.4f, 0.45f, 0.5f, 0.55f, 0.6f, 0.65f, 0.75f, 0.8f, 1f, 1.15f, 1.25f, 1.5f, 1.55f, 2f, 2.5f, 3f };
    // array of delays to initiate waitforseconds with, this saves on memory
    public Dictionary<float, WaitForSeconds> delays = new();

    private void Start() {
        tutorial = FindObjectOfType<Tutorial>();
        if (tutorial == null) { Save.LoadGame(); }
        else { Save.LoadTutorial(); }
        Save.LoadPersistent();
        dice = FindObjectOfType<Dice>();
        arrow = FindObjectOfType<Arrow>();
        enemy = FindObjectOfType<Enemy>();
        colors = FindObjectOfType<Colors>();
        player = FindObjectOfType<Player>();
        menuIcon = FindObjectOfType<MenuIcon>();
        backToMenu = FindObjectOfType<BackToMenu>();
        menuButton = FindObjectOfType<MenuButton>();
        turnManager = FindObjectOfType<TurnManager>();
        itemManager = FindObjectOfType<ItemManager>();
        diceSummoner = FindObjectOfType<DiceSummoner>();
        soundManager = FindObjectOfType<SoundManager>();
        levelManager = FindObjectOfType<LevelManager>();
        statSummoner = FindObjectOfType<StatSummoner>();
        tombstoneData = FindObjectOfType<TombstoneData>();
        characterSelector = FindObjectOfType<CharacterSelector>();
        highlightCalculator = FindObjectOfType<HighlightCalculator>();
        foreach (float delay in delayArr) {
            delays.Add(delay, new WaitForSeconds(delay));
        }
        StartCoroutine(SaveAfterDelay());
    }

    private IEnumerator SaveAfterDelay() { 
        // set newgame to false after a delay so that stuff can load in if its true
        yield return delays[0.25f];
        if (player != null) { Save.game.newGame = false; }
        
        if (tutorial == null) { Save.SaveGame(); }
        music = FindObjectOfType<Music>();
        // also get the music here, because we need it to set up the singleton pattern first
    }

    public void OnApplicationQuit() { 
        if (player != null) { 
            if (tutorial == null) { Save.SaveGame(); } 
            Save.SavePersistent(); 
        }
    }
}