﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour {
    public float transitionMultiplier = 2.5f;
    Scripts scripts;

    private void Awake() {
        scripts = FindObjectOfType<Scripts>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            // on escape pressed
            if (scripts != null && scripts.player != null && scripts.turnManager != null && !scripts.turnManager.isMoving) { 
                // if in game and not moving
                scripts.SaveGameData();
                scripts.SavePersistentData();
                // save data first
            }
            SceneManager.LoadScene("Menu");
            // exit back to the menu scene
        }
    }
}