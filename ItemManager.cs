﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System;

public class ItemManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI lootText;
    [SerializeField] public TextMeshProUGUI itemDesc;
    [SerializeField] private GameObject item;
    [SerializeField] public GameObject highlight;
    [SerializeField] public GameObject highlightedItem;
    [SerializeField] private Sprite[] commonItemSprites;
    [SerializeField] private Sprite[] rareItemSprites;
    [SerializeField] private Sprite[] weaponSprites;
    [SerializeField] private Sprite[] otherSprites;
    [SerializeField] public List<GameObject> floorItems;

    public string[] statArr = new string[] { "green", "blue", "red", "white" };
    public string[] statArr1 = new string[] { "accuracy", "speed", "damage", "parry" };
    public Dictionary<string, int> baseWeapon = new Dictionary<string, int>() { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } };
    public Dictionary<string, Dictionary<string, int>> weaponDict = new Dictionary<string, Dictionary<string, int>>() {
        { "dagger", new Dictionary<string, int>()   { { "green", 2 }, { "blue", 4 }, { "red", 0 }, { "white", 0 } } },
        { "flail", new Dictionary<string, int>()    { { "green", 0 }, { "blue", 3 }, { "red", 1 }, { "white", 0 } } },
        { "hatchet", new Dictionary<string, int>()  { { "green", 1 }, { "blue", 2 }, { "red", 2 }, { "white", 1 } } },
        { "mace", new Dictionary<string, int>()     { { "green", 1 }, { "blue", 3 }, { "red", 2 }, { "white", 0 } } },
        { "maul", new Dictionary<string, int>()     { { "green", -1 }, { "blue", -1 }, { "red", 3 }, { "white", 1 } } },
        { "montante", new Dictionary<string, int>() { { "green", 1 }, { "blue", 1 }, { "red", 3 }, { "white", 2 } } },
        { "scimitar", new Dictionary<string, int>() { { "green", 0 }, { "blue", 2 }, { "red", 1 }, { "white", 2 } } },
        { "rapier", new Dictionary<string, int>()   { { "green", 4 }, { "blue", 2 }, { "red", -1 }, { "white", 1 } } },
        { "spear", new Dictionary<string, int>()    { { "green", 2 }, { "blue", -1 }, { "red", 3 }, { "white", 1 } } },
        { "sword", new Dictionary<string, int>()    { { "green", 1 }, { "blue", 2 }, { "red", 1 }, { "white", 2 } } },
    };  
    private string[] weaponNames = new string[] { "dagger", "flail", "hatchet", "mace", "maul", "montante", "scimitar", "rapier", "spear", "sword" };
    public Dictionary<string, Dictionary<string, int>> modifierDict = new Dictionary<string, Dictionary<string, int>>() {
        { "accurate0", new Dictionary<string, int>() { { "green", 1 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } } },
        { "accurate1", new Dictionary<string, int>() { { "green", 2 }, { "blue", -1 }, { "red", 0 }, { "white", 0 } } },
        { "brisk0", new Dictionary<string, int>()    { { "green", 0 }, { "blue", 1 }, { "red", -1 }, { "white", 0 } } },
        { "brisk1", new Dictionary<string, int>()    { { "green", 0 }, { "blue", 1 }, { "red", 0 }, { "white", -1 } } },
        { "blunt0", new Dictionary<string, int>()    { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 1 } } },
        { "blunt1", new Dictionary<string, int>()    { { "green", 0 }, { "blue", 0 }, { "red", -1 }, { "white", 1 } } },
        { "common0", new Dictionary<string, int>()   { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } } },
        { "common1", new Dictionary<string, int>()   { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } } },
        { "common2", new Dictionary<string, int>()   { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } } },
        { "common3", new Dictionary<string, int>()   { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } } },
        { "common4", new Dictionary<string, int>()   { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } } },
        { "common5", new Dictionary<string, int>()   { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } } },
        { "heavy0", new Dictionary<string, int>()    { { "green", 0 }, { "blue", -1 }, { "red", 1 }, { "white", 0 } } },
        { "heavy1", new Dictionary<string, int>()    { { "green", 0 }, { "blue", -1 }, { "red", 0 }, { "white", 1 } } },
        { "nimble0", new Dictionary<string, int>()   { { "green", 0 }, { "blue", 1 }, { "red", 0 }, { "white", -1 } } },
        { "nimble1", new Dictionary<string, int>()   { { "green", 0 }, { "blue", 1 }, { "red", -1 }, { "white", 1 } } },
        { "precise0", new Dictionary<string, int>()  { { "green", 1 }, { "blue", 0 }, { "red", -1 }, { "white", 0 } } },
        { "precise1", new Dictionary<string, int>()  { { "green", 1 }, { "blue", -1 }, { "red", 0 }, { "white", 0 } } },
        { "ruthless0", new Dictionary<string, int>() { { "green", 1 }, { "blue", -1 }, { "red", 0 }, { "white", 1 } } },
        { "ruthless1", new Dictionary<string, int>() { { "green", 0 }, { "blue", -1 }, { "red", 1 }, { "white", 0 } } },
        { "stable0", new Dictionary<string, int>()   { { "green", -1 }, { "blue", 0 }, { "red", 0 }, { "white", 1 } } },
        { "stable1", new Dictionary<string, int>()   { { "green", 0 }, { "blue", -1 }, { "red", 0 }, { "white", 1 } } },
        { "sharp0", new Dictionary<string, int>()    { { "green", 0 }, { "blue", 0 }, { "red", 1 }, { "white", 0 } } },
        { "sharp1", new Dictionary<string, int>()    { { "green", 1 }, { "blue", -1 }, { "red", 1 }, { "white", 0 } } },
    };
    private string[] modifierNames = new string[] { "accurate0", "accurate1", "brisk0", "brisk1", "blunt0", "blunt1", "common0", "common1", "common2", "common3", "common4", "common5", "heavy0", "heavy1", "nimble0", "nimble1", "precise0", "precise1", "ruthless0", "ruthless1", "stable0", "stable1", "sharp0", "sharp1" };

    public Dictionary<string, string> descriptionDict = new Dictionary<string, string>() {
        {"armour", "protects from one hit"}, // finished
        {"arrow", "proceed to the next level"}, // finished
        {"ankh", "force new draft"}, // finished
        {"boots of dodge", "pay 1 stamina to become dodgy"}, // finished
        {"cheese", "3"}, // finished
        {"dagger", "green dice buff damage"}, // finished
        {"flail", "start with red die"}, // finished
        {"hatchet", "enemy can't choose yellow die"},  // finished
        {"helm of might", "pay 3 stamina to gain a yellow die"}, // finished
        {"kapala", "offer an item to become furious"}, // finished
        {"mace", "reroll all dice still to be picked"}, // finished
        {"maul", "any wound is deadly"}, // finished
        {"montante", ""}, // finished
        {"necklet", ""}, // finished
        {"phylactery", "gain leech buff once wounded"}, //
        {"potion", ""}, // finished
        {"rapier", "gain 3 stamina upon kill"}, // finished
        {"scimitar", "discard enemy's die upon parry"},  // finished
        {"scroll", ""}, // finished
        {"shuriken", "discard enemy's die"}, // finished
        {"skeleton key", "escape the fight"}, // finished
        {"spear", "always choose first die"}, // finished
        {"steak", "5"}, // finished
        {"sword", ""}, // finished
        {"torch", "find more loot"} // finished
    };
    public string[] neckletTypes = new string[] { "solidity", "rapidity", "strength", "defense", "arcane", "nothing" };
    public Dictionary<string, int> neckletStats = new Dictionary<string, int>() { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } };
    public Dictionary<string, int> neckletCounter = new Dictionary<string, int>() { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 }, { "arcane", 1 } };
    private string[] scrollTypes = new string[] { "fury", "haste", "dodge", "leech", "courage", "challenge", "nothing" };
    private string[] potionTypes = new string[] { "accuracy", "speed", "strength", "defense", "might", "life", "nothing" };
    private Sprite[] allSprites;
    private float itemSpacing = 1f;
    private float itemY = -5.3f;
    private Scripts scripts;
    public int col = 0;
    public List<GameObject> curList;
    public int discardableDieCounter = 0;
    public bool usedAnkh = false;
    public bool usedHelm = false;
    public bool usedBoots = false;
    public int numItemsDroppedForTrade = 0;

    void Start() {
        allSprites = commonItemSprites.ToArray().Concat(rareItemSprites.ToArray()).Concat(weaponSprites.ToArray()).Concat(otherSprites.ToArray()).ToArray();
        // create a list containing all of the sprites
        scripts = FindObjectOfType<Scripts>();
        curList = scripts.player.inventory;
        // assign the curlist variable for item selection navigation
        lootText.text = "";
        CreateWeaponWithStats("sword", "common", 2, 1, 2, 2);
        MoveToInventory(0, true);
        CreateItem("steak", "common");
        MoveToInventory(0, true);
        // create items based on class and move them into the inventory
        Select(curList, 0);
        // select the first item
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
            // if pressing one of the ctrl keys
            if (curList == scripts.player.inventory) { Select(floorItems, 0); }
            else if (curList == floorItems) { Select(scripts.player.inventory, 0); }
            // swap the curList (used for selection) to the other
            else { print("invalid list to select from"); }
            // something is wrong here
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            Select(curList, col - 1, false);
            // if pressing left, move the selection left
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            Select(curList, col + 1, false);
            // if moving right, move the selection right
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            highlightedItem.GetComponent<Item>().Use();
            // if pressing return or enter, use the item that is selected
        }
    } 

    /// <summary>
    /// Select an item from the given list at the given index. 
    /// </summary>
    /// <param name="itemList">The list of which to attempt to make a selection from.</param>
    /// <param name="c">The column number of which to attempt to make a selection from.</param>
    /// <param name="forceDifferentSelection">Whether or not to try to force a different selection if the column was not valid for the list.</param>
    public void Select(List<GameObject> itemList, int c, bool forceDifferentSelection=true) {
        if (c <= itemList.Count - 1 && c >= 0) {
            // if the column is within the bounds of the list
            itemList[c].GetComponent<Item>().Select();
            // select the object
            curList = itemList;
            col = c;
            // update the variables used for selection 
        }
        else {
            // column is invalid for the list
            if (forceDifferentSelection) {
                // if we want to force a different selection
                if (itemList.Count > 1) {
                    // if there is more than 1 item
                    itemList[col - 1].GetComponent<Item>().Select();
                    // select the next item over
                    curList = itemList;
                    col--;
                    // update the variables used for selection
                }
                else {
                    // player only has weapon
                    scripts.player.inventory[0].GetComponent<Item>().Select();
                    curList = scripts.player.inventory;
                    col = 0;
                    // select the weapon and update the variables used for selection.
                }
            }
            else {
                highlightedItem.GetComponent<Item>().Select();
                // not forcing a different selection, so select the item regardless
            }
        }
    }

    /// <summary>
    /// Create an item with specified itemtype.
    /// </summary>
    /// <param name="itemType">The type of item of which to create.</param>
    private void CreateItem(string itemType) {
        GameObject instantiatedItem = Instantiate(item, new Vector2(-2.75f + floorItems.Count * itemSpacing, itemY), Quaternion.identity);
        // create an item object at the correct position
        Sprite sprite = null;
        // create a variable of which we can place the sprite upon to depending on the item type
        if (itemType == "weapon") { sprite = weaponSprites[UnityEngine.Random.Range(0, weaponSprites.Length - 1)]; }
        else if (itemType == "common") { sprite = commonItemSprites[UnityEngine.Random.Range(0, commonItemSprites.Length - 1)]; }
        else if (itemType == "rare") { sprite = rareItemSprites[UnityEngine.Random.Range(0, rareItemSprites.Length - 1)]; }
        // depending on the type, give it a corresponding random sprite.
        else { print("bad item type to create"); }
        // can only create item types of weapon, common, and rare
        instantiatedItem.GetComponent<SpriteRenderer>().sprite = sprite;
        // give the sprite renderer the proper sprite 
        instantiatedItem.transform.parent = gameObject.transform;
        // make the item childed to this manager
        instantiatedItem.GetComponent<Item>().itemName = sprite.name.Replace("_", " ");
        instantiatedItem.GetComponent<Item>().itemType = itemType;
        // assign the attributes for the name and the type of the item 
        SetItemStatsImmediately(instantiatedItem);
        // if needed, immediately give the item its proper attributes
        floorItems.Add(instantiatedItem);
        // add the item to the array
    }

    /// <summary>
    /// Create an item with the specified name and itemtype.
    /// </summary>
    /// <param name="itemName">The name of the item of which to create.</param>
    /// <param name="itemType">The type of the item of which to create</param>
    public void CreateItem(string itemName, string itemType) {
        GameObject instantiatedItem = Instantiate(item, new Vector2(-2.75f + floorItems.Count * itemSpacing, itemY), Quaternion.identity);
        // instantiate the item
        instantiatedItem.GetComponent<SpriteRenderer>().sprite = allSprites[(from a in allSprites select a.name).ToList().IndexOf(itemName)];
        // give the item the proper sprite
        instantiatedItem.transform.parent = gameObject.transform;
        // make the item childed to this manager
        instantiatedItem.GetComponent<Item>().itemName = instantiatedItem.GetComponent<SpriteRenderer>().sprite.name.Replace("_", " ");
        instantiatedItem.GetComponent<Item>().itemType = itemType;
        // assign the attributes for the name and the type of the item
        SetItemStatsImmediately(instantiatedItem);
        // if needed, immediately give the item its proper attributes
        floorItems.Add(instantiatedItem);
        // add the item to the array
    }

    /// <summary>
    /// Instantly assign necessary attributes of items (like their modifier).
    /// </summary>
    /// <param name="instantiatedItem"></param>
    private void SetItemStatsImmediately(GameObject instantiatedItem) {
        // this needs to be done here rather than in Item.Start() or Awake() because the timing will be off and errors will be thrown
        if (instantiatedItem.GetComponent<Item>().itemName == "necklet") {
            instantiatedItem.GetComponent<Item>().modifier = neckletTypes[UnityEngine.Random.Range(0, 5)];
        }
        else if (instantiatedItem.GetComponent<Item>().itemName == "scroll") {
            instantiatedItem.GetComponent<Item>().modifier = scrollTypes[UnityEngine.Random.Range(0, scrollTypes.Length)];
        }
        else if (instantiatedItem.GetComponent<Item>().itemName == "potion") {
            instantiatedItem.GetComponent<Item>().modifier = potionTypes[UnityEngine.Random.Range(0, potionTypes.Length)];
        }
        // assign a modifier for a necklet, scroll, or potion
    }

    /// <summary>
    /// Create a item with random modifier and stats. 
    /// </summary>
    public void CreateRandomWeapon() {
        GameObject instantiatedItem = Instantiate(item, new Vector2(-2.75f + floorItems.Count * itemSpacing, itemY), Quaternion.identity);
        // instantiate the item
        int rand = UnityEngine.Random.Range(0, weaponNames.Length);
        // get a random variable of which we will pull item information from
        instantiatedItem.GetComponent<SpriteRenderer>().sprite = weaponSprites[rand];
        // get the sprite from the random number
        instantiatedItem.transform.parent = gameObject.transform;
        // child the item to this manager
        instantiatedItem.GetComponent<Item>().itemType = "weapon";
        // set the itemtype to be a weaon
        string modifier = modifierNames[UnityEngine.Random.Range(0, modifierNames.Length)];
        // pull a random modifier from the array
        instantiatedItem.GetComponent<Item>().modifier = modifier.Substring(0, modifier.Length - 1);
        // assign the modifier attribute (cut off the final letter, as modifiers are like 'common0' and 'common1')
        instantiatedItem.GetComponent<Item>().itemName = instantiatedItem.GetComponent<Item>().modifier + " " + instantiatedItem.GetComponent<SpriteRenderer>().sprite.name.Replace("_", " ");
        // concatenate the modifier with weapon name and assign the item's name attribute to be that
        baseWeapon = weaponDict[weaponNames[rand]];
        // get the base weapon's stats from the array of dictionaries gotten from the previous random number
        foreach (string key in statArr)
        { 
            // for every key in the stat array ("green", "blue", etc.)
            baseWeapon[key] += modifierDict[modifier][key]; 
            // add the modifier stats to the weapon stats
            if (baseWeapon[key] < 1) { baseWeapon[key] = -1; }
            // limit the item so it can't go down to -2 (not in the actual game, but in my modded version later i may do this)
        }
        instantiatedItem.GetComponent<Item>().weaponStats = baseWeapon;
        // assign the weapon stats to the weapon
        floorItems.Add(instantiatedItem);
        // add the item to the array
    }

    /// <summary>
    /// Create a weapon with specified name, modifier, and stats.
    /// </summary>
    /// <param name="weaponName">The name of the weapon of which to create.</param>
    /// <param name="modifier">The name of the modifier of which to apply to the weapon.</param>
    /// <param name="aim">The aim stat to give to the weapon.</param>
    /// <param name="spd">The spd stat to give to the weapon.</param>
    /// <param name="atk">The atk stat to give to the weapon.</param>
    /// <param name="def">The def stat to give to the weapon.</param>
    public void CreateWeaponWithStats(string weaponName, string modifier, int aim, int spd, int atk, int def) {
        GameObject instantiatedItem = Instantiate(item, new Vector2(-2.75f + floorItems.Count * itemSpacing, itemY), Quaternion.identity);
        // instantiaet the item
        Sprite sprite = null;
        // create an empty sprite variable to imprint on
        sprite = allSprites[(from a in allSprites select a.name).ToList().IndexOf(weaponName)];
        // get the sprite based on the weapon name
        instantiatedItem.GetComponent<SpriteRenderer>().sprite = sprite;
        // give the sprite based on the item name
        instantiatedItem.transform.parent = gameObject.transform;
        // child the item to this manager
        instantiatedItem.GetComponent<Item>().itemName = modifier + " " + sprite.name.Replace("_", " ");
        instantiatedItem.GetComponent<Item>().itemType = "weapon";
        instantiatedItem.GetComponent<Item>().modifier = modifier;
        baseWeapon["green"] = aim;
        baseWeapon["blue"] = spd;
        baseWeapon["red"] = atk;
        baseWeapon["white"] = def;
        instantiatedItem.GetComponent<Item>().weaponStats = baseWeapon;
        // assign the attributes of the item based on the given parameters
        floorItems.Add(instantiatedItem);
        // add the item to the array
    }

    /// <summary>
    /// Move the floor item at the specifed index into the player's inventory.
    /// </summary>
    /// <param name="index">The index from which to move the item.</param>
    /// <param name="starter">Whether the item is created instantly as a starter item or is a normal item.</param>
    public void MoveToInventory(int index, bool starter=false) {
        if (scripts.player.inventory.Count <= 7) {
            // if the player doesn't have 7 or more items
            if (!starter) { scripts.soundManager.PlayClip("click"); }
            // if the item is not the starter (so it doesn't instantly play a click), play the click sound
            if (floorItems[index].GetComponent<Item>().itemType == "weapon") { 
                // if the item being moved is a weapon 
                floorItems[index].transform.position = new Vector2(-2.75f, 3.16f);
                // move the item to the weapon slot
                scripts.player.stats = floorItems[index].GetComponent<Item>().weaponStats;
                // set the player's stats to be equal to that of the weapon
                if (!starter) {
                    // if the weapon is not a starter (so player already has a weapon)
                    scripts.turnManager.SetStatusText("you take " + floorItems[index].GetComponent<Item>().itemName.Split(' ')[1]);
                    // notify the player
                    Destroy(scripts.player.inventory[0]);
                    // destroy the previous weapon
                    scripts.player.inventory[0] = floorItems[index]; 
                    // add the new weapon to the player's inventory
                    scripts.statSummoner.SetDebugInformationFor("player");
                    scripts.statSummoner.SummonStats();
                    // set the debug information and summon the new stats
                }
                else {
                    scripts.player.inventory.Add(floorItems[index]);
                    // item is a starter, so just add it to the player's inventory
                }
                floorItems.RemoveAt(0);
                // remove the item at index 0 (which is the weapon, because the weapon is always created first)
                foreach(GameObject item in floorItems) {
                    item.transform.position = new Vector2(item.transform.position.x - 1f, itemY);
                    // for every item, shift it over now that an item has been removed
                }
                Select(curList, 0);
                // select the item at index 0
            }
            else {
                // not a weapon
                if (floorItems[index].GetComponent<Item>().itemName == "necklet") {
                    if (floorItems[index].GetComponent<Item>().modifier == "solidity") { 
                        neckletStats["green"] += neckletCounter["arcane"]; 
                        neckletCounter["green"]++; 
                    } 
                    else if (floorItems[index].GetComponent<Item>().modifier == "rapidity") { 
                        neckletStats["blue"] += neckletCounter["arcane"];
                        neckletCounter["blue"]++; 
                    } 
                    else if (floorItems[index].GetComponent<Item>().modifier == "strength") { 
                        neckletStats["red"] += neckletCounter["arcane"]; 
                        neckletCounter["red"]++; 
                    } 
                    else if (floorItems[index].GetComponent<Item>().modifier == "defense") { 
                        neckletStats["white"] += neckletCounter["arcane"]; 
                        neckletCounter["white"]++; 
                    }
                    else if (floorItems[index].GetComponent<Item>().modifier == "arcane") {
                        neckletCounter["arcane"]++;    
                        foreach (string stat in statArr) { 
                            neckletStats[stat] = neckletCounter["arcane"] * neckletCounter[stat]; 
                        }
                    } 
                    else if (floorItems[index].GetComponent<Item>().modifier == "nothing") {}
                    else { print("bad modifier"); }
                    // depending on the type of the necklet, modify the stats accordingly
                }
                if (!starter) 
                { 
                    if (itemType == "weapon") { 
                        scripts.turnManager.SetStatusText($"you take {scripts.itemManager.descriptionDict[itemName.Split(' ')[1]]}"); 
                    }
                    if (itemName == "necklet")  { 
                        if (modifier == "arcane") { scripts.turnManager.SetStatusText($"you take arcane necklet"); }
                        else { scripts.turnManager.SetStatusText($"you take {itemName} of {modifier}"); }
                    }
                    else if (itemName == "potion" || itemName == "scroll") { scripts.turnManager.SetStatusText($"you take {itemName} of {modifier}"); }
                    else { scripts.turnManager.SetStatusText($"you take {itemName}"); }
                    // notify the player of which item that they took
                }
                // if the item is not a starter item, notify the player that they have picked up the item
                floorItems[index].transform.position = new Vector2(-2.75f + itemSpacing * scripts.player.inventory.Count, 3.16f);
                // add the item to the proper location
                scripts.player.inventory.Add(floorItems[index]);
                // add the item to the player's inventory
                floorItems.RemoveAt(index);
                // and remove it from the floor
                for (int i = index; i < floorItems.Count; i++) {
                    // for every item after where the removed item was
                    floorItems[i].transform.position = new Vector2(floorItems[i].transform.position.x - 1f, itemY);
                    // shift it over to the proper location
                }
                Select(curList, index);
                // select the next item of where it was
            }
        }
    }

    /// <summary>
    /// Spawn the items after the enemy has died. 
    /// </summary>
    public void SpawnItems() {
        lootText.text = "loot:";
        // set the text 
        int torchCount = (from item in scripts.player.inventory where item.GetComponent<Item>().itemName == "torch" select item).Count();
        // count the number of torches
        int spawnCount = 3 + torchCount * 2  + scripts.levelManager.level;
        // create a spawn count 
        CreateRandomWeapon();
        // create a random weapon at index 0
        for (int i = 0; i < UnityEngine.Random.Range(0, spawnCount); i++) {
            CreateItem("common");
            // create a random number of items based on the spawn count
        }
        if (UnityEngine.Random.Range(0, 10) == 0) { CreateItem("rare"); }
        // low chance to produce rare
        CreateItem("arrow", "arrow");
        // create an arrow to move to the next level
    }

    /// <summary>
    /// Spawn the items for which the player can trade with.
    /// </summary>
    public void SpawnTraderItems() {
        lootText.text = "PLACEHOLDER TEXT";
        // set the test
        for (int i = 0; i < 3; i++) { CreateItem("common"); }
        // create 3 common items
        CreateItem("arrow", "arrow");
        // create the next level arrow
    }

    /// <summary>
    /// Hide floor items, preparing for deletion. MUST be kept separate from destroying them!
    /// </summary>
    public void HideItems() {
        lootText.text = "";
        // clear the text
        Select(scripts.player.inventory, 0);
        // select player's inventory
        foreach (GameObject test in floorItems) {
            // hide every item
            test.GetComponent<SpriteRenderer>().sprite = null;
        }
    }
    
    /// <summary>
    /// Destroy floor items after they are hidden. MUST be kept separate from hiding them!
    /// </summary>
    public void DestroyItems() {
        foreach (GameObject test in floorItems) {
            Destroy(test);
            // destroy every floor item
        }
        floorItems.Clear();
        // clear the array
    }

    /// <summary>
    /// Checks if the player has an item with specified item name.
    /// </summary>
    /// <param name="itemName">The item name to check for.</param>
    /// <returns>true if the item was found, false otherwise.</returns>
    public bool PlayerHas(string itemName) {
        return (from a in scripts.player.inventory select a.GetComponent<Item>().itemName).Contains(itemName);
    }

    /// <summary>
    /// Checks if the player has a weapon with specified item name.
    /// </summary>
    /// <param name="weaponName">The weapon name to check for.</param>
    /// <returns>true if the weapon was found, false otherwise.</returns>
    public bool PlayerHasWeapon(string weaponName) {
        return (from a in scripts.player.inventory select a.GetComponent<Item>().itemName).Contains(scripts.player.inventory[0].GetComponent<Item>().itemName.Split(' ')[1]);
    }

    /// <summary>
    /// Gets the first item in the player's inventory with given name.
    /// </summary>
    /// <param name="itemName">The item name to check for.</param>
    /// <returns>The GameObject that was found.</returns>
    public GameObject GetPlayerItem(string itemName) {
        try { return scripts.player.inventory[(from a in scripts.player.inventory select a.GetComponent<Item>().itemName).ToList().IndexOf(itemName)]; }
        catch { return null; }
    }

    /// <summary>
    /// Fade all torches that have ended their lifespan.
    /// </summary>
    public void AttemptFadeTorches() {
        foreach (GameObject item in scripts.player.inventory) {
            // for every item
            if (item.GetComponent<Item>().itemName == "torch") {
                // if the item is a torch
                if ($"{scripts.levelManager.level}-{scripts.levelManager.sub}" == item.GetComponent<Item>().modifier) {
                    // if the fade level matches the current level
                    scripts.turnManager.SetStatusText("your torch runs out");
                    // notify player
                    Destroy(scripts.player.inventory[scripts.player.inventory.IndexOf(item)]);
                    scripts.player.inventory[scripts.player.inventory.IndexOf(item)].GetComponent<Item>().Remove();
                    // remove the torch. later come back an add an animation to this. 
                }
            }
        }
    }
}
