﻿using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CharacterSelector : MonoBehaviour {
    [SerializeField] public int selectionNum;
    [SerializeField] public bool[] unlockedChars = new bool[4] { true, false, false, false };
    [SerializeField] public bool easy = false;
    [SerializeField] private Sprite[] icons;
    [SerializeField] private Sprite releasedButton;
    [SerializeField] private Sprite pressedButton;
    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject rightButton;
    [SerializeField] private TextMeshProUGUI quoteText;
    [SerializeField] private TextMeshProUGUI perkText;
    [SerializeField] public SimpleFadeIn simpleFadeIn;
    [SerializeField] public GameObject itemHider;
    private readonly string[] quotes = new string[4] {
        "- \"they say 68% of adventurers die of starvation...\"",
        "- \"what comedy is your defiance, beasts!\"",
        "- \"...breastplate costs a fortune; dodging is free...\"",
        "- \"honestly all the carnage is making me sleepy...\"",
    }; 
    private readonly string[] perks = new string[4] {
        "* Food restores more stamina",
        "* Gains a yellow die each round\n* Cannot use stamina",
        "* All white dice are set to 1\n* Gains 1 stamina upon inflicting a wound",
        "* White dice buff damage\n* Gains 3 stamina once wounded\n* As stamina reaches 10, wounds are cured and stamina is decreased by 10",
    }; 
    private bool preventPlayingFX = true;
    private Scripts scripts;
    
    private void Start() {
        scripts = FindObjectOfType<Scripts>();
        simpleFadeIn = FindObjectOfType<SimpleFadeIn>();
        // get necessary objects
        unlockedChars = Save.persistent.unlockedChars;
        easy = Save.persistent.easyMode;
        // pull in data from the Savefile
        if (easy) { scripts.itemManager.floorItems[2].GetComponent<Item>().UnHide(); }
        else { scripts.itemManager.floorItems[2].GetComponent<Item>().Hide(); }
        // hide the item if normal mode, if easy mode then show it
        selectionNum = 0;
        SetSelection(selectionNum);
        // select 0 and go to it
        StartCoroutine(AllowFX());
    }
    
    /// <summary>
    /// Only allow sound effects to be played after a short delay, preventing extra clicking.
    /// </summary>
    private IEnumerator AllowFX() { 
        yield return scripts.delays[0.45f];
        preventPlayingFX = false;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { 
            SetSelection(selectionNum - 1);
            ChangeToPressed("Left"); 
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) { 
            SetSelection(selectionNum + 1);
            ChangeToPressed("Right"); 
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow)) { 
            ChangeToReleased("Left");
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow)) { 
            ChangeToReleased("Right");
        }
        // depending on the input, shift the selection in that direction and shows a small animation
        else if (Input.GetKeyDown(KeyCode.Space)) { ToggleEasy(); }
        // space toggles easy mode
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) { 
            // enter selects the character
            if (unlockedChars[selectionNum]) { StartCoroutine(LoadMenuScene()); }
            // but only if its already unlocked
        }
    }

    /// <summary>
    /// Coroutine used to load the menu scene after the player locks in their character selection.
    /// </summary>
    private IEnumerator LoadMenuScene() { 
        scripts.soundManager.PlayClip("blip0");
        // play sfx (this is when selected)
        Save.persistent.newCharNum = selectionNum;
        // set the selection num
        Save.SavePersistent();
        // Save the selection num
        yield return scripts.delays[0.1f];
        // delay here, because i don't want a singleton and this allows blip to complete playing
        SceneManager.LoadScene("Menu");
        // load the menu scene after the delay
    }

    /// <summary>
    /// Select (preview) a character and display it to the player
    /// </summary>
    public void SetSelection(int num) {
        if (num is >= 0 and <= 3) {
            // only allow selections between the number of available characters
            selectionNum = num;
            // set the current selection num
            if (unlockedChars[selectionNum]) {
                // if the current selected character is unlocked
                itemHider.SetActive(false);
                // show that character's items
                quoteText.text = quotes[selectionNum];
                perkText.text = perks[selectionNum];
                // show that character's quote and perk
            }
            else {
                // current selected character is not unlocked
                itemHider.SetActive(true);
                // make sure to hide items
                quoteText.text = "beat game on previous character to unlock";
                perkText.text = "";
                // let player know that it's locked
            }
            GetComponent<SpriteRenderer>().sprite = icons[selectionNum];
            // set the character icon
            if (!preventPlayingFX) { scripts.soundManager.PlayClip("click0"); }
            // only play sfx if we want it 
            if (selectionNum == 0) { leftButton.transform.position = new Vector2(-8.53f, 20f); }
            // hide left button if we are the leftmost (first) character
            else { leftButton.transform.position = new Vector2(-8.53f, 1f); }
            // otherwise show the left button
            if (selectionNum == 3) { rightButton.transform.position = new Vector2(8.53f, 20f); }
            else { rightButton.transform.position = new Vector2(8.53f, 1f); }
            // same for the right button, but 
        }
        switch (num) {
            case 0:
                scripts.itemManager.floorItems[0].GetComponent<Item>().itemName = "harsh sword";
                scripts.itemManager.floorItems[0].GetComponent<Item>().modifier = "harsh";
                scripts.itemManager.floorItems[0].GetComponent<SpriteRenderer>().sprite = 
                    scripts.itemManager.GetItemSprite("sword");
                scripts.itemManager.floorItems[1].GetComponent<Item>().itemName = "steak";
                scripts.itemManager.floorItems[1].GetComponent<SpriteRenderer>().sprite = 
                    scripts.itemManager.GetItemSprite("steak");
                scripts.itemManager.floorItems[2].GetComponent<Item>().itemName = "torch";
                scripts.itemManager.floorItems[2].GetComponent<SpriteRenderer>().sprite = 
                    scripts.itemManager.GetItemSprite("torch");
                break;
            case 1:
                scripts.itemManager.floorItems[0].GetComponent<Item>().itemName = "common maul";
                scripts.itemManager.floorItems[0].GetComponent<Item>().modifier = "common";
                scripts.itemManager.floorItems[0].GetComponent<SpriteRenderer>().sprite = 
                    scripts.itemManager.GetItemSprite("maul");
                scripts.itemManager.floorItems[1].GetComponent<Item>().itemName = "armor";
                scripts.itemManager.floorItems[1].GetComponent<SpriteRenderer>().sprite = 
                    scripts.itemManager.GetItemSprite("armor");
                scripts.itemManager.floorItems[2].GetComponent<Item>().itemName = "helm of might";
                scripts.itemManager.floorItems[2].GetComponent<SpriteRenderer>().sprite = 
                    scripts.itemManager.GetItemSprite("helm_of_might");
                break;
            case 2:
                scripts.itemManager.floorItems[0].GetComponent<Item>().itemName = "quick dagger";
                scripts.itemManager.floorItems[0].GetComponent<Item>().modifier = "quick";
                scripts.itemManager.floorItems[0].GetComponent<SpriteRenderer>().sprite = 
                    scripts.itemManager.GetItemSprite("dagger");
                scripts.itemManager.floorItems[1].GetComponent<Item>().itemName = "boots of dodge";
                scripts.itemManager.floorItems[1].GetComponent<SpriteRenderer>().sprite = 
                    scripts.itemManager.GetItemSprite("boots_of_dodge");
                scripts.itemManager.floorItems[2].GetComponent<Item>().itemName = "ankh";
                scripts.itemManager.floorItems[2].GetComponent<SpriteRenderer>().sprite = 
                    scripts.itemManager.GetItemSprite("ankh");
                break;
            case 3:
                scripts.itemManager.floorItems[0].GetComponent<Item>().itemName = "ruthless mace";
                scripts.itemManager.floorItems[0].GetComponent<Item>().modifier = "ruthless";
                scripts.itemManager.floorItems[0].GetComponent<SpriteRenderer>().sprite = 
                    scripts.itemManager.GetItemSprite("mace");
                scripts.itemManager.floorItems[1].GetComponent<Item>().itemName = "cheese";
                scripts.itemManager.floorItems[1].GetComponent<SpriteRenderer>().sprite = 
                    scripts.itemManager.GetItemSprite("cheese");
                scripts.itemManager.floorItems[2].GetComponent<Item>().itemName = "kapala";
                scripts.itemManager.floorItems[2].GetComponent<SpriteRenderer>().sprite = 
                    scripts.itemManager.GetItemSprite("kapala");
                break;
        }
        // give the character items based on their class, even if its not unlocked because it will be hidden regardless
        scripts.itemManager.floorItems[0].GetComponent<Item>().Select(false);
        // select the first item so its not buggy
    }

    /// <summary>
    /// Changes a L/R Character Select button to its 'pressed' sprite.
    /// </summary>
    public void ChangeToPressed(string leftOrRight) {
        // set the button to be pressed down 
        if (leftOrRight == "Left") { leftButton.GetComponent<CharacterSwapButton>().spriteRenderer.sprite = pressedButton; }
        else { rightButton.GetComponent<CharacterSwapButton>().spriteRenderer.sprite = pressedButton; }
    }

    /// <summary>
    /// Changes a L/R Character Select button to its 'released' sprite.
    /// </summary>
    public void ChangeToReleased(string leftOrRight) {
        // make the button pop up
        if (leftOrRight == "Left") { leftButton.GetComponent<CharacterSwapButton>().spriteRenderer.sprite = releasedButton; }
        else { rightButton.GetComponent<CharacterSwapButton>().spriteRenderer.sprite = releasedButton; }
    }

    /// <summary>
    /// Toggles easy mode and handles the hiding.
    /// </summary>
    private void ToggleEasy() {
        if (!simpleFadeIn.lockChanges) {
            // don't allow toggle of easy if we are fading rn
            scripts.soundManager.PlayClip("click1");
            // play clip
            easy = !easy;
            // toggle the boolean
            StartCoroutine(simpleFadeIn.FadeHide()); 
            // fade to black and then back
            Save.persistent.easyMode = easy; 
            Save.SavePersistent();
            // apply it to the Save file so the next game will have the correct character
        }
    }
}
