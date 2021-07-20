﻿using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class StatSummoner : MonoBehaviour {
    [SerializeField] private GameObject plus;
    [SerializeField] private GameObject minus;
    [SerializeField] private GameObject square;
    [SerializeField] private GameObject negSquare;
    [SerializeField] private GameObject circle;
    [SerializeField] private TextMeshProUGUI playerDebug;
    [SerializeField] private TextMeshProUGUI enemyDebug;
    private readonly float xCoord = -10.5f;
    public readonly float xOffset = 0.65f;
    private readonly float highlightOffset = 0.85f;
    public readonly float diceOffset = 1f;
    private readonly float buttonXCoord = -11.7f;
    private readonly float buttonXOffset = -0.6f;
    public readonly float[] yCoords = { 8.77f, 7.77f, 6.77f, 5.77f };
    private readonly Vector2 baseDebugPos = new(-1.667f, 7.333f);
    private List<GameObject> existingStatSquares = new();
    public readonly Dictionary<string, List<Dice>> addedPlayerDice = new() {
        { "green", new List<Dice>() },
        { "blue", new List<Dice>() },
        { "red", new List<Dice>() },
        { "white", new List<Dice>() },
    };
    public readonly Dictionary<string, List<Dice>> addedEnemyDice = new() {
        { "green", new List<Dice>() },
        { "blue", new List<Dice>() },
        { "red", new List<Dice>() },
        { "white", new List<Dice>() },
    };
    public Dictionary<string, int> addedPlayerStamina = new() {
        { "green", 0 },
        { "blue", 0 },
        { "red", 0 },
        { "white", 0 },
    };

    public Dictionary<string, int> addedEnemyStamina = new() {
        { "green", 0 },
        { "blue", 0 },
        { "red", 0 },
        { "white", 0 },
    };
    private Scripts scripts;

    private void Start() {
        scripts = FindObjectOfType<Scripts>();
        if (PlayerPrefs.GetString("debug") == "on") {
            playerDebug.color = scripts.colors.disabled;
            enemyDebug.color = scripts.colors.disabled;
        }
        else {
            playerDebug.color = Color.black;
            enemyDebug.color = Color.black;
        }
        // set the debug color if needed
        SummonStaminaButtons();
        // create the stamina buttons
    }

    /// <summary>
    /// Summon the stats squares for the player.
    /// </summary>
    public void SummonStats() {
        foreach (GameObject stat in existingStatSquares) {
            Destroy(stat);
            // destroy all existing stats
        }
        existingStatSquares.Clear();
        // clear the array
        for (int i = 0; i < 4; i++) {
            GenerateForStat(i, scripts.colors.colorNameArr[i]);
            // generate stat squares for every stat
        }
        SetDebugInformationFor("player");
        SetDebugInformationFor("enemy");
        // set the debug information for both
    }

    /// <summary>
    /// Gets the sum of the specified stat from the player or enemy. 
    /// </summary>
    public int SumOfStat(string stat, string playerOrEnemy) {
        if (stat != "green" && stat != "blue" && stat != "red" && stat != "white") {
            // make sure they are getting a valid stat
            Debug.LogError("Invalid stat to get the sum of");
            return 0;
        }
        if (playerOrEnemy == "player") {
            // get for player
            if (scripts.enemy.woundList.Contains("knee") && stat == "blue" && scripts.enemy.enemyName.text != "Lich") { return 99; }
            // return 99 if enemy has knee wound (lich not affected by wounds)
            int sum = scripts.player.stats[stat] + scripts.player.potionStats[stat] + addedPlayerStamina[stat] + scripts.itemManager.neckletStats[stat];
            // get the sum of base stats + potion + stamina + necklet
            foreach (Dice dice in addedPlayerDice[stat]) {
                // add to the sum all the added die
                if (dice != null) { sum += dice.GetComponent<Dice>().diceNum; }
            }
            return sum;
            // return the end
        }
        if (playerOrEnemy == "enemy") {
            // get for enemy, similar process to getting from player
            if (scripts.player.woundList.Contains("knee") && stat == "blue") { return 99; }
            int sum = scripts.enemy.stats[stat] + addedEnemyStamina[stat];
            foreach (Dice dice in addedEnemyDice[stat]) {
                if (dice != null) { sum += dice.GetComponent<Dice>().diceNum; }
            }
            return sum;
        }
        Debug.LogError("Can only get the stats of a player or an enemy");
        return 0;
    }

    /// <summary>
    /// Gets the sum of the specified stat from the player or enemy, disregarding wounds and dice.
    /// </summary>
    public int RawSumOfStat(string stat, string playerOrEnemy) {
        if (stat != "green" && stat != "blue" && stat != "red" && stat != "white") {
            // make sure they are getting a valid stat
            Debug.LogError("Invalid stat to get the sum of");
            return 0;
        }
        if (playerOrEnemy == "player") {
            return scripts.player.stats[stat] + scripts.player.potionStats[stat] + addedPlayerStamina[stat] + scripts.itemManager.neckletStats[stat];
        }
        if (playerOrEnemy == "enemy") {
            return scripts.enemy.stats[stat] + addedEnemyStamina[stat];
        }
        Debug.LogError("Can only get the stats of a player or an enemy");
        return 0;
    }

    /// <summary>
    /// Instantiate the player's stamina buttons.
    /// </summary>
    private void SummonStaminaButtons() {
        for (int i = 0; i < 4; i++) {
            // 1 for every stat
            GameObject spawnedPlusButton = SpawnButton(plus, new Vector2(buttonXCoord, yCoords[i] - 0.01f));
            // - 0.01 is necessary beacuse otherwise it has a weird visual glitch
            GameObject spawnedMinusButton = SpawnButton(minus, new Vector2(buttonXCoord + buttonXOffset, yCoords[i]));
            // create the buttons
            spawnedPlusButton.GetComponent<StaminaButton>().stat = scripts.colors.colorNameArr[i];
            spawnedMinusButton.GetComponent<StaminaButton>().stat = scripts.colors.colorNameArr[i];
            // assign stats to the buttons
        }
    }

    /// <summary>
    /// Generate the stat squares for the given stat.
    /// </summary>
    private void GenerateForStat(int i, string colorName) {
        // could use scripts.colors.colorNameArr[i] instead of colorName but that takes up way more space and its much more confusing
        Color statColor = scripts.colors.colorArr[Array.IndexOf(scripts.colors.colorNameArr, colorName)];
        // get the color of the given colorname
        if (scripts.player.stats[colorName] + scripts.itemManager.neckletStats[colorName] + scripts.player.potionStats[colorName] + addedPlayerStamina[colorName] > 0) {
            // if player's stats are greater than 0
            int k0;
            for (k0 = 0; k0 < scripts.player.stats[colorName] + scripts.player.potionStats[colorName]; k0++) {
                SpawnGeneratedShape(i, statColor, k0, xCoord, xOffset, true, true);
            }
            // summon the positive stat squares at the proper place
            if (scripts.player.stats[colorName] + scripts.player.potionStats[colorName] + addedPlayerStamina[colorName] < 0) {
                // if total without necklet is negative, but total with necklet is positive
                for (int k1 = 0; k1 < 0 - Mathf.Abs(scripts.player.stats[colorName]) + scripts.itemManager.neckletStats[colorName] + scripts.player.potionStats[colorName] + addedPlayerStamina[colorName]; k1++) {
                    // create circles based on the number over the negative
                    SpawnGeneratedShape(i, statColor, k0 + k1, xCoord, xOffset, true, false);
                }
            }
            else {
                // spawn circles normally
                for (int k2 = 0; k2 < scripts.itemManager.neckletStats[colorName]; k2++) {
                    SpawnGeneratedShape(i, statColor, k0 + k2, xCoord, xOffset, true, false);
                }
            }
        }
        else {
            // stats are less than 0
            for (int k = 0; k < -(scripts.player.stats[colorName] + scripts.itemManager.neckletStats[colorName] + scripts.player.potionStats[colorName] + addedPlayerStamina[colorName]); k++) {
                SpawnGeneratedShape(i, statColor, k, xCoord, xOffset, false);
            }
            // create negative stat squares
        }
        if (addedPlayerStamina[colorName] > 0 && scripts.player.stats[colorName] + scripts.itemManager.neckletStats[colorName] + scripts.player.potionStats[colorName] + addedPlayerStamina[colorName] > 0) {
            // if player stamina is greater than 0 and total stats) are greater than 0
            if (scripts.player.stats[colorName] + scripts.itemManager.neckletStats[colorName] + scripts.player.potionStats[colorName] > 0) {
                // if player's total stats (without stamina) are greater than 0
                for (int j = scripts.player.stats[colorName] + scripts.itemManager.neckletStats[colorName] + scripts.player.potionStats[colorName]; j < scripts.player.stats[colorName] + scripts.itemManager.neckletStats[colorName] + scripts.player.potionStats[colorName] + addedPlayerStamina[colorName]; j++) {
                    GameObject addedStaminaSquare = SpawnGeneratedShape(i, scripts.colors.yellow, j, xCoord, xOffset, true);
                    Vector3 position = addedStaminaSquare.transform.position;
                    position = new Vector2(position.x - 0.01f, position.y);
                    addedStaminaSquare.transform.position = position;
                    // move it over a tiny bit
                    addedStaminaSquare.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    // make sure the sorting order is higher than that of other squares
                    // THIS IS THE EASY TO FIX A VISUAL GLITCH INVOLVING NECKLETS AND NEGATIVES, IT WORKS SO DON'T TOUCH IT
                }
                // make yellow squares in the correct places
            }
            else {
                // player's total stats w/o stamina are less than 0
                for (int j = 0; j < scripts.player.stats[colorName] + scripts.itemManager.neckletStats[colorName] + scripts.player.potionStats[colorName] + addedPlayerStamina[colorName]; j++) {
                    SpawnGeneratedShape(i, scripts.colors.yellow, j, xCoord, xOffset, true);
                }
                // make yellow squares in the correct place
            }
        }
        if (scripts.enemy.stats[colorName] + addedEnemyStamina[colorName] > 0) {
            for (int l = 0; l < scripts.enemy.stats[colorName]; l++) {
                SpawnGeneratedShape(i, statColor, l, -xCoord + 1, -xOffset, true);
            }
        }
        else {
            for (int l = 0; l < -(scripts.enemy.stats[colorName] + addedEnemyStamina[colorName]); l++) {
                SpawnGeneratedShape(i, statColor, l, -xCoord + 1, -xOffset, false);
            }
        }
        if (addedEnemyStamina[colorName] > 0 && scripts.enemy.stats[colorName] + addedEnemyStamina[colorName] > 0) {
            if (scripts.enemy.stats[colorName] > 0) {
                for (int n = scripts.enemy.stats[colorName]; n < scripts.enemy.stats[colorName] + addedEnemyStamina[colorName]; n++) {
                    SpawnGeneratedShape(i, scripts.colors.yellow, n, -xCoord + 1, -xOffset, true);
                }
            }
            else {
                for (int n = 0; n < scripts.enemy.stats[colorName] + addedEnemyStamina[colorName]; n++) {
                    SpawnGeneratedShape(i, scripts.colors.yellow, n, -xCoord + 1, -xOffset, true);
                }
            }
        }
        // enemy stat spawning is super similar to player
    }

    /// <summary>
    /// Spawn a generated shape with given information.
    /// </summary>
    /// <param name="i">The index (0-3) of the stat (green, blue...)</param>
    /// <param name="statColor">The color of which to give to the shape.</param>
    /// <param name="k">The number of stat squares generated for the stat (so offsets can be applied).</param>
    /// <param name="coord">The base x-coordinate at which to create the stats.</param>
    /// <param name="offset">The offset of which to apply between each square.</param>
    /// <param name="isPositive">true to make a positive square, false to make a negative square.</param>
    /// <param name="isSquare">true to create a square, false to create a circle.</param>
    private GameObject SpawnGeneratedShape(int i, Color statColor, int k, float coord, float offset, bool isPositive, bool isSquare=true) {
        Vector3 instantiationsPos = new Vector2(coord + (k * offset), yCoords[i]);
        // set where the shape will be created
        GameObject spawnedShape;
        if (isPositive) {
            spawnedShape = isSquare ? Instantiate(square, instantiationsPos, Quaternion.identity) : Instantiate(circle, new Vector2(instantiationsPos.x, instantiationsPos.y), Quaternion.identity);
        }
        else { 
            if (instantiationsPos.x <= 0) { spawnedShape = Instantiate(negSquare, instantiationsPos, Quaternion.identity);  }
            else { 
                spawnedShape = Instantiate(negSquare, instantiationsPos, Quaternion.identity);  
                spawnedShape.GetComponent<SpriteRenderer>().flipX = true;
            }
            // depending on stat type and position, set the correct sprite.
        }
        spawnedShape.GetComponent<SpriteRenderer>().color = statColor;
        // give the die the correct player
        spawnedShape.transform.parent = transform;
        // move to correct position
        existingStatSquares.Add(spawnedShape);
        // add it to the array
        return spawnedShape;
    }

    /// <summary>
    /// Make a stamina +/- button.
    /// </summary>
    private GameObject SpawnButton(GameObject buttonType, Vector3 instantiationPos) {
        GameObject spawnedButton = Instantiate(buttonType, instantiationPos, Quaternion.identity);
        // create a button
        spawnedButton.transform.parent = transform;
        // child the button to this stat summoner
        return spawnedButton;
        // return the created button
    }

    /// <summary>
    /// Remove all attached die and stamina.
    /// </summary>
    public void ResetDiceAndStamina() {
        foreach (GameObject dice in scripts.diceSummoner.existingDice) {
            // fade out every die
            StartCoroutine(dice.GetComponent<Dice>().FadeOut());
        }
        foreach (string key in addedPlayerDice.Keys) { 
            // clear die and stamina from each stat
            addedPlayerDice[key].Clear();
            addedEnemyDice[key].Clear();
            addedPlayerStamina[key] = 0;
            addedEnemyStamina[key] = 0;
        }
        SetDebugInformationFor("player");
        SetDebugInformationFor("enemy");
        // set the debug information
    }

    /// <summary>
    /// Add a die to player's stat.
    /// </summary>
    public void AddDiceToPlayer(string addTo, Dice dice) {
        addedPlayerDice[addTo].Add(dice);
    }

    /// <summary>
    /// Add a die to enemy's stat.
    /// </summary>
    public void AddDiceToEnemy(string addTo, Dice dice) {
        addedEnemyDice[addTo].Add(dice);
    }

    /// <summary>
    /// Return the outermost player's x coordinate to add dice onto.
    /// </summary>
    public float OutermostPlayerX(string statType, string optionalDiceOffsetStatToMultiplyBy = null) {
        if (optionalDiceOffsetStatToMultiplyBy == null) { optionalDiceOffsetStatToMultiplyBy = statType; }
        // not setting the optional variable will just default it to the base stat type
        return xCoord + ((Mathf.Abs(scripts.player.stats[statType] + scripts.player.potionStats[statType] + scripts.itemManager.neckletStats[statType] + addedPlayerStamina[statType]) - 1) * xOffset + highlightOffset + diceOffset * addedPlayerDice[optionalDiceOffsetStatToMultiplyBy].Count);
        // sum everything to get the offset
    }

    /// <summary>
    /// Get the outermost enemy's x coordinate to add dice onto.
    /// </summary>
    public float OutermostEnemyX(string statType) {
        return -xCoord + 1 + ((Mathf.Abs(scripts.enemy.stats[statType]) + addedEnemyStamina[statType] - 1) * -xOffset)  - highlightOffset - diceOffset * (addedEnemyDice[statType].Count - 1);
        // similar to outermostplayerx
    }

    /// <summary>
    /// Set the debug information for player or enemy.
    /// </summary>
    public void SetDebugInformationFor(string playerOrEnemy) {
        if (scripts.tutorial == null) {
            if (playerOrEnemy == "player") {
                float furthest = (new[] { OutermostPlayerX("green"), OutermostPlayerX("blue"), OutermostPlayerX("red"), OutermostPlayerX("white") }).Max();
                playerDebug.transform.position = furthest >= -3.8 ? new Vector2(furthest + 1.333f, baseDebugPos.y) : new Vector2(baseDebugPos.x, baseDebugPos.y);
                // if the outermost position to too far, start moving the debug for plaer over
                playerDebug.text = "("+SumOfStat("green", "player")+")\n("+SumOfStat("blue", "player")+")\n("+SumOfStat("red", "player")+")\n("+SumOfStat("white", "player")+")";
            }
            else if (playerOrEnemy == "enemy") {
                enemyDebug.text = "("+SumOfStat("green", "enemy")+")\n("+SumOfStat("blue", "enemy")+")\n("+SumOfStat("red", "enemy")+")\n("+SumOfStat("white", "enemy")+")";
            }
            // ends up looking like
            // (2)
            // (2)
            // (1)
            // (2)
            // for example
            else { Debug.Log("error"); }
            scripts.enemy.TargetBest();
        }
    }

    /// <summary>
    /// Shift the dice of a given stat by a given amount.
    /// </summary>    
    public void ShiftDiceAccordingly(string stat, int shiftAmount) {
        foreach (Dice dice in addedPlayerDice[stat]) {
            // for every die in the specified stat
            Vector3 position = dice.transform.position;
            position = new Vector2(position.x + xOffset * shiftAmount, position.y);
            dice.transform.position = position;
            // shift the die by the specified amount
            dice.instantiationPos = position;
            // update the instantiation position
        }
        SetDebugInformationFor("player");
    }
}