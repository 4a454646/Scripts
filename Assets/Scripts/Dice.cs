﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Dice : MonoBehaviour {
    public int diceNum;
    public string diceType;
    public string statAddedTo;
    public bool moveable = true;
    public bool isAttached = false;
    public bool isRerolled = false;
    public string isOnPlayerOrEnemy = "none";
    public Vector3 instantiationPos;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer childSpriteRenderer;
    private Scripts scripts;
    private bool wasClickedRecently = false;

    private readonly WaitForSeconds[] rollTimes = { new(0.01f), new(0.03f), new(0.06f), new(0.09f), new(0.12f), new(0.15f), new(0.18f), new(0.21f), new(0.24f), new(0.3f) };
    // different times for rolling 

    private void Awake()  {
        // must be in awake, otherwise scripts not set fast enough
        scripts = FindObjectOfType<Scripts>();
        // assign the necessary sprite renderers
    }

    private void Start() {
        StartCoroutine(FadeIn());
    }

    private void OnMouseDown() {
        // as soon as the mouse button is pressed down
        if (scripts.tutorial != null) { 
            // if within the tutorial, make sure player can only do certain actions (so that they win)
            if (scripts.tutorial.isAnimating || scripts.tutorial.curIndex is 12 or 13)  {
                if (scripts.diceSummoner.CountUnattachedDice() == 6 && diceType == "red") { DiceDown(); }
                // only allow the red 6 to be picked
                else if (scripts.diceSummoner.CountUnattachedDice() == 4 && diceType == "green") { DiceDown(); }
                // then take the green
                else if (scripts.diceSummoner.CountUnattachedDice() == 2) { DiceDown(); }
                // after that it doesnt matter
                else { scripts.turnManager.SetStatusText("bad choice"); }
            }
        }
        else { DiceDown(); }
        // else just regular dice down
    }


    /// <summary>
    /// Handle what happens when the player presses down on a dice.
    /// </summary>
    private void DiceDown() { 
        if (moveable && !scripts.turnManager.isMoving) {
            // if the dice is still moveable
            scripts.soundManager.PlayClip("click0");
            // play sound clip
            childSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            // assign the child sprite renderer to be edited 
            scripts.highlightCalculator.ShowValidHighlights(gameObject.GetComponent<Dice>());
            // call the from class HighlightCalculator to show all valid highlights 
        }
        // TODO: LOOKS BETTER BUT CAUSES GLITCHES, FIX IN THE FUTURE
        
        // if (!moveable && isAttached && !isRerolled && isOnPlayerOrEnemy == "enemy") {
        //     // if an action can be performed on the dice (discard, reroll)
        //     if (!scripts.turnManager.isMoving || (scripts.turnManager.isMoving && scripts.turnManager.actionsAvailable)) {
        //         // if the situation permits action to occur on the die
        //         if (scripts.itemManager.discardableDieCounter > 0) {
        //             // if the enemy is wounded in the head and a die has not been discarded yet
        //             scripts.soundManager.PlayClip("click0");
        //             // play sound clip
        //             Color numTemp = spriteRenderer.color;
        //             Color baseTemp = childSpriteRenderer.color;
        //             numTemp.a -= 0.33f;
        //             spriteRenderer.color = numTemp;
        //             baseTemp.a -= 0.25f;
        //             childSpriteRenderer.color = baseTemp;
        //             // dim the colors of the die
        //         }
        //     }
        // }
        // if (isAttached && isOnPlayerOrEnemy == "player" && scripts.player.isCourageous && !scripts.turnManager.isMoving) {
        //     // if the player wants to Save a die via scroll of courage by discarding the others
        //     scripts.soundManager.PlayClip("click0");
        //     // play sound clip
        //     Color numTemp = spriteRenderer.color;
        //     Color baseTemp = childSpriteRenderer.color;
        //     numTemp.a -= 0.33f;
        //     spriteRenderer.color = numTemp;
        //     baseTemp.a -= 0.25f;
        //     childSpriteRenderer.color = baseTemp;
        //     // dim the colors of the die
        // }
    }
    
    private void OnMouseUp() {
        // self explanatory, tutorial restricts which dice can be picked, else is just normal
        if (scripts.tutorial != null) { 
            if (scripts.tutorial.isAnimating || scripts.tutorial.curIndex == 12 || scripts.tutorial.curIndex == 13) {
                if (scripts.diceSummoner.CountUnattachedDice() == 6 && diceType == "red") { DiceUp(); }
                else if (scripts.diceSummoner.CountUnattachedDice() == 4 && diceType == "green") { DiceUp(); }
                else if (scripts.diceSummoner.CountUnattachedDice() == 2) { DiceUp(); }
                else { scripts.turnManager.SetStatusText("bad choice"); }
            }
        }
        else { DiceUp(); }
    }

    private void OnMouseDrag() {
        if (scripts.tutorial != null) { 
            if (scripts.tutorial.isAnimating || scripts.tutorial.curIndex == 12 || scripts.tutorial.curIndex == 13) {
                if (scripts.diceSummoner.CountUnattachedDice() == 6 && diceType == "red") { DiceDrag(); }
                else if (scripts.diceSummoner.CountUnattachedDice() == 4 && diceType == "green") { DiceDrag(); }
                else if (scripts.diceSummoner.CountUnattachedDice() == 2) { DiceDrag(); }
                else { scripts.turnManager.SetStatusText("bad choice"); }
            }
        }
        else { DiceDrag(); }
    }

    /// <summary>
    /// Handle what happens when a dice is attempted to be dragged.
    /// </summary>
    private void DiceDrag() { 
        // when the mouse is dragged
        if (moveable && !scripts.turnManager.isMoving) {
            // if the dice can be moved
            spriteRenderer.sortingOrder = 3;
            childSpriteRenderer.sortingOrder = 2;
            // move the dice and its number to the front of the screen
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // get the mouse position via a function
            transform.position = new Vector2(mousePos.x, mousePos.y);
            // assign the transform position of the dice to be where the mouse cursor is
            // no need ot move child as the positions are tied together
        }
    }

    /// <summary>
    /// Handle what happens when the player releases a dice.
    /// </summary>
    private void DiceUp() {
        // when the mouse is released
        if (moveable && !scripts.turnManager.isMoving) {
            // if the dice can be moved
            scripts.soundManager.PlayClip("click1");
            // play sound clip
            if (wasClickedRecently) {
                // was clicked recently, so select the dice
                float xOverride;
                float yOverride;
                GameObject[] highlights = scripts.highlightCalculator.highlights;
                // 0: accuracy, 1: speed, 2: damage, 3: parry
                if (diceType == "yellow" || 
                    diceType == "green" && scripts.itemManager.PlayerHasWeapon("dagger") || diceType == "white" && Save.game.curCharNum == 3) { 
                    // yellow dice drop onto red by default
                    // green die + dagger drop onto red
                    // white die + char 3 do as well
                    xOverride = highlights[2].transform.position.x;
                    yOverride = highlights[2].transform.position.y;
                }
                else {
                    // regular die, so drop it to the regular position
                    int diceIndex = Array.IndexOf(scripts.colors.colorNameArr, diceType);
                    xOverride = highlights[diceIndex].transform.position.x;
                    yOverride = highlights[diceIndex].transform.position.y;
                }
                scripts.highlightCalculator.SnapToPosition(gameObject.GetComponent<Dice>(), instantiationPos, out moveable, out instantiationPos, xOverride, yOverride);
            }
            else { 
                // dice was not clicked recently, so start the timer
                StartCoroutine(ClickedTimer());
                scripts.highlightCalculator.SnapToPosition(gameObject.GetComponent<Dice>(), instantiationPos, out moveable, out instantiationPos);
            }
            // attempt to snap the position with a function defined in HighlightCalculator
            transform.position = instantiationPos;
            // set the transform position to be where the instantiation position is (snap back to the selection menu if it didn't get snapped in SnapToPosition)
            scripts.highlightCalculator.HideHighlights();
            // hide all the highlights
            spriteRenderer.sortingOrder = 1;
            childSpriteRenderer.sortingOrder = 0;
            // send the die to the background
        }
        if (!moveable && isAttached && !isRerolled && isOnPlayerOrEnemy == "enemy" && scripts.enemy.enemyName.text != "Lich") {
            //  && !scripts.turnManager.isMoving
            // if an action can be performed on the dice (discard, reroll)
            if (!scripts.turnManager.isMoving || scripts.turnManager.isMoving && scripts.turnManager.actionsAvailable) {
                // if the situation allows for an action to be performed
                if (Save.game.discardableDieCounter > 0) {
                    // if can discard from another source
                    if (scripts.turnManager.scimitarParry) { scripts.diceSummoner.breakOutOfScimitarParryLoop = true; }
                    // if source is from scimitarParry, break out of the waiting loop
                    DiscardFromEnemy();
                    // discard from the enemy
                    Save.game.discardableDieCounter--;
                    // decrease the counter for the number of die 
                    if (scripts.tutorial == null) { Save.SaveGame(); }
                }
                else if (scripts.enemy.woundList.Contains("chest")) {
                    // if enemy is wounded in the chest
                    Reroll();
                    // reroll the die
                }
            }
        }
        if (isAttached && isOnPlayerOrEnemy == "player" && scripts.player.isCourageous && scripts.turnManager.discardDieBecauseCourage) {
            // if discarding can and should discard die from courage
            DiscardFromPlayer();
            // do so 
        }
    }

    private IEnumerator ClickedTimer() { 
        if (!wasClickedRecently) { 
            wasClickedRecently = true;
            yield return scripts.delays[0.2f];
            wasClickedRecently = false;
        }
    }

    /// <summary>
    /// Discard this dice from the enemy.
    /// </summary>
    private void DiscardFromEnemy() {
        scripts.soundManager.PlayClip("click1");
        // play sound clip
        int index = scripts.statSummoner.addedEnemyDice[statAddedTo].IndexOf(this);
        // set variable index to be the index of where the die is
        scripts.turnManager.alterationDuringMove = true;
        // set necessary variables for the turnmanager
        scripts.statSummoner.addedEnemyDice[statAddedTo].Remove(this);
        scripts.diceSummoner.existingDice.Remove(gameObject);
        // remove the die from the lists
        Destroy(gameObject);
        // destroy the gameObject
        List<Dice> diceList = scripts.statSummoner.addedEnemyDice[statAddedTo];
        // assign reference variable for the enemy dice of the same category that just had a dice removed
        for (int i = index; i < diceList.Count; i++) {
            // for every dice at and after the index of where the removed die was
            diceList[i].transform.position = new Vector2(diceList[i].transform.position.x + scripts.statSummoner.diceOffset, diceList[i].transform.position.y);
            // shift the die so it fits properly after one was removed
            diceList[i].GetComponent<Dice>().instantiationPos = diceList[i].transform.position;
            // set the instantiation position for the dice
        }
        scripts.statSummoner.SetDebugInformationFor("enemy");
        // set the debug information
        Save.persistent.diceDiscarded++;
        Save.SavePersistent();
        // increment stats and Save them
    }

    /// <summary>
    /// Discard this die from the player.
    /// </summary>
    public void DiscardFromPlayer() {
        // very similar to discardfromenemy, just doesn't set certain variables in TurnManager and such
        int index = scripts.statSummoner.addedPlayerDice[statAddedTo].IndexOf(this);
        scripts.statSummoner.addedPlayerDice[statAddedTo].Remove(this);
        scripts.diceSummoner.existingDice.Remove(gameObject);
        Destroy(gameObject);
        List<Dice> diceList = scripts.statSummoner.addedPlayerDice[statAddedTo];
        for (int i = index; i < diceList.Count; i++) {
            diceList[i].transform.position = new Vector2(diceList[i].transform.position.x - scripts.statSummoner.diceOffset, diceList[i].transform.position.y);
            diceList[i].GetComponent<Dice>().instantiationPos = diceList[i].transform.position;
        }
        scripts.statSummoner.SetDebugInformationFor("player");
        scripts.diceSummoner.SaveDiceValues();
    }

    /// <summary>
    /// (Player only) Reroll an enemy's dice.
    /// </summary>
    private void Reroll() {
        // pretty self explanatory self explanatory
        Save.persistent.diceRerolled++;
        scripts.turnManager.alterationDuringMove = true;
        StartCoroutine(RerollAnimation());
        isRerolled = true;
        scripts.diceSummoner.SaveDiceValues();
        Save.SavePersistent();
    }

    /// <summary>
    /// Coroutine for playing the animation and rerolling the dice.
    /// </summary>
    public IEnumerator RerollAnimation(bool playSound=true) {
        // assign the spriterenderer reference
        for (int i = 0; i < 10; i++) {
            // 10 times
            yield return rollTimes[i];
            // wait for a set amount of time
            if (playSound) { scripts.soundManager.PlayClip("click0"); }
            // play sound clip if necessary
            int randNum = UnityEngine.Random.Range(1, 7);
            // get a random number for the dice 
            spriteRenderer.sprite = scripts.diceSummoner.numArr[randNum - 1].GetComponent<SpriteRenderer>().sprite;
            // assign the sprite to be the necessary sprite with the new number
            diceNum = randNum;
            // reassign the die's number
        }
        scripts.statSummoner.SetDebugInformationFor("player");
        scripts.statSummoner.SetDebugInformationFor("enemy");
        scripts.turnManager.RecalculateMaxFor("player");
        scripts.turnManager.RecalculateMaxFor("enemy");
        // set debug information and make sure that the player/enemy isn't aiming at something that they shouldn't be able to hit
        scripts.diceSummoner.SaveDiceValues();
    }

    /// <summary>
    /// Coroutine for decreasing the value of this die.
    /// </summary>
    public IEnumerator DecreaseDiceValue(bool wait = true) {
        if (wait) { yield return scripts.delays[1f]; }
        // wait if necessary
        if (diceNum == 1) { StartCoroutine(FadeOut()); }
        // fade it out if decreasing dice value to 0
        else {
            diceNum--;
            GetComponent<SpriteRenderer>().sprite = scripts.diceSummoner.numArr[diceNum - 1].GetComponent<SpriteRenderer>().sprite;
            // otherwise decrement value and set the proper sprite
        }
        scripts.statSummoner.SetDebugInformationFor("player");
        scripts.statSummoner.SetDebugInformationFor("enemy");
        // set the debug information
        scripts.diceSummoner.SaveDiceValues();
    }

    /// <summary>
    /// Sets this die's value to one.
    /// </summary>
    public void SetToOne() {
        // pretty self explanatory
        diceNum = 1;
        spriteRenderer.sprite = scripts.diceSummoner.numArr[0].GetComponent<SpriteRenderer>().sprite;
        scripts.statSummoner.SetDebugInformationFor("player");
        // this can only happen to player, so don't worry about enemies stuff
        scripts.diceSummoner.SaveDiceValues();
    }
    
    /// <summary>
    /// Coroutine for fading out this die.
    /// </summary>
    public IEnumerator FadeOut(bool wait=false, bool shiftOver = true) {
        if (wait) { yield return scripts.delays[0.55f]; }
        // wait if necessary
        Color numTemp = spriteRenderer.color;
        Color baseTemp = childSpriteRenderer.color;
        numTemp.a = 1;
        baseTemp.a = 1;
        // set them to 1 here because for some reason sometimes alpha starts at 2 and nothing works right
        // assign the necessary variables to manipulate the color
        for (int i = 0; i < 12; i++) {
            // 40 times
            yield return scripts.delays[0.005f];
            // wait a small duration
            numTemp.a -= 1/12f;
            spriteRenderer.color = numTemp;
            baseTemp.a -= 1/12f;
            childSpriteRenderer.color = baseTemp;
            // decrease the colors of the die and base
        }
        if (statAddedTo != "" && shiftOver) { 
            // if the die has already been attached (only needed to check for my cheat)
            if (isOnPlayerOrEnemy == "player") {
                // if the die is attached to the player
                for (int i = scripts.statSummoner.addedPlayerDice[statAddedTo].IndexOf(this)+1; i < scripts.statSummoner.addedPlayerDice[statAddedTo].Count; i++) { 
                    // for every die following this current die
                    GameObject curDie = scripts.statSummoner.addedPlayerDice[statAddedTo][i].gameObject;
                    Vector3 position = curDie.transform.position;
                    position = new Vector3(position.x - scripts.statSummoner.diceOffset, position.y, position.z);
                    curDie.transform.position = position;
                    // shift each die back one, because this die will decrease to 0 and fade out
                }
            }
            else if (isOnPlayerOrEnemy == "enemy") {
                // else the die is attached to the enemy
                for (int i = scripts.statSummoner.addedEnemyDice[statAddedTo].IndexOf(this)+1; i < scripts.statSummoner.addedEnemyDice[statAddedTo].Count; i++) { 
                    GameObject curDie = scripts.statSummoner.addedEnemyDice[statAddedTo][i].gameObject;
                    Vector3 position = curDie.transform.position;
                    position = new Vector3(position.x + scripts.statSummoner.diceOffset, position.y, position.z);
                    curDie.transform.position = position;
                }
                // same situation as above, shift the die (except forwards this time)
            }
            // else { print("something is wrong with this die"); }
        }
        try { scripts.statSummoner.addedPlayerDice[statAddedTo].Remove(this); } catch { }
        try { scripts.statSummoner.addedEnemyDice[statAddedTo].Remove(this); } catch { }
        // attempt to remove from the player/enemy, checking with if statements causes a plethora of bugs for no reason
        scripts.diceSummoner.existingDice.Remove(gameObject);
        // remove from existing die list so no errors later on
        Destroy(gameObject);
        // destroy the die
        scripts.diceSummoner.SaveDiceValues();
        // Save the dice values to the Save file
        scripts.statSummoner.SetDebugInformationFor("enemy");
        scripts.statSummoner.SetDebugInformationFor("player");
    }
    
    /// <summary>
    /// Coroutine for fading in a die.
    /// </summary>
    private IEnumerator FadeIn() {
        // very similar to fadeout
        spriteRenderer = GetComponent<SpriteRenderer>();
        childSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        Color numTemp = spriteRenderer.color;
        Color baseTemp = childSpriteRenderer.color;
        numTemp.a = 0;
        baseTemp.a = 0;
        spriteRenderer.color = numTemp;
        childSpriteRenderer.color = baseTemp;
        yield return scripts.delays[0.005f];
        for (int i = 0; i < 40; i++) {
            numTemp.a += 0.025f;
            spriteRenderer.color = numTemp;
            baseTemp.a += 0.025f;
            childSpriteRenderer.color = baseTemp;
            yield return scripts.delays[0.005f];
        }
    }
}