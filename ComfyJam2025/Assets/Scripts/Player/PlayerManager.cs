using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PlayerState
{
    Idle, // No spell ready, clicking zaps, can craft?
    Crafting,   // Crafting screen is up
    Casting,    // Spell is ready, cannot craft
}

public enum ItemType
{
    Stem = 0, // TODO: HACKY ENUMS
    Rind = 1,
}

public enum SpellType
{
    Test = 0
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public PlayerState playerState = PlayerState.Idle;

    private CraftingManager craftingManager;

    public List<ItemData> possibleItems;
    public Dictionary<ItemType, int> inventory = new Dictionary<ItemType, int>();
    [HideInInspector] public UnityEvent<ItemType> OnItemChange; // for inventory ui

    public List<GameObject> spellPrefabs;
    private Dictionary<SpellType, GameObject> spellPrefabMapping; // Internal mapping of prefabs for each spell

    private SpellBase currentSpell;
    private void Awake()
    {
        if (instance == null)
            instance = this;

        Logger.Log("PlayerManager registered", LogLevel.info);
    }

    private void Start()
    {
        craftingManager = GetComponentInChildren<CraftingManager>();
        if (craftingManager == null)
        {
            Logger.Log("Failed to find crafting manager attached to player manager.", LogLevel.fatal);
        }
        craftingManager.gameObject.SetActive(false);

        // Build spellPrefabMapping
        spellPrefabMapping = new Dictionary<SpellType, GameObject>();
        foreach (GameObject spellPrefab in spellPrefabs)
        {
            SpellBase spellInfo = spellPrefab.GetComponent<SpellBase>();
            if (!spellInfo)
            {
                Logger.Log($"Recieved spell prefab without a spell base: {spellPrefab.name}", LogLevel.error);
                continue;
            }
            spellPrefabMapping.Add(spellInfo.spellType, spellPrefab);
        }    
        // Initialize inventory set
        // InitializeItems();
        
    }
    private void InitializeItems() {
        foreach (ItemData itemData in possibleItems) {
            inventory.Add(itemData.itemType, 0);
            //instance.OnItemChange.Invoke(itemData.itemType);
        }
    }
    // Increase inventory amount of one item by 1
    public static void AddItem(ItemType itemType) {

        if (instance.inventory.ContainsKey(itemType))
        {
            instance.inventory[itemType] += 1;
            //instance.OnItemChange.Invoke(itemType);
        }
        else
        {
            instance.inventory[itemType] = 1;
        }
    }

    public static void RemoveItem(ItemType itemType)
    {
        if (!instance.inventory.ContainsKey(itemType))
        {
            Logger.Log("Failed to remove item since it didn't exist", LogLevel.error);
        }
        instance.inventory[itemType] -= 1;
        if (instance.inventory[itemType] <= 0)
        {
            instance.inventory.Remove(itemType);
        }
    }

    private void Update()
    {
        HandlePlayerClick();
    }

    private void HandlePlayerClick()
    {

        // Pulling up crafting
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (playerState == PlayerState.Idle)
            {
                SetCraftingState(true);
            }
            else if (playerState == PlayerState.Crafting)
            {
                SetCraftingState(false);
            }
        }

        // Basic zaps
        if (playerState == PlayerState.Idle && Input.GetMouseButtonDown(0))
        {
            DebugManager.DisplayDebug("Strike:" + GameManager.GetMousePos().ToString());

            // Find closest enemy within a region
            EnemyBase closestEnemy = EnemyManager.GetClosestEnemy(GameManager.GetMousePos(), 1);
            if (closestEnemy)
            {
                closestEnemy.TakeDamage(1);
            }
        }
        else if (playerState == PlayerState.Casting)
        {
            currentSpell.Aim();

            if (Input.GetMouseButtonDown(0))
            {
                currentSpell.Cast();
                playerState = PlayerState.Idle;
                DebugManager.DisplayDebug("Casting:" + GameManager.GetMousePos().ToString());
            }
        }
    }

    public void SetCraftingState(bool state)
    {
        if (playerState == PlayerState.Idle && state)
        {
            playerState = PlayerState.Crafting;
            craftingManager.gameObject.SetActive(true);
        }
        else if (playerState == PlayerState.Crafting && !state)
        {
            playerState = PlayerState.Idle;
            craftingManager.gameObject.SetActive(false);
        }
        else
        {
            Logger.Log($"Attempted to make invalid crafting state transition: {state} from {playerState}", LogLevel.warn);
        }
    }

    public void AddSpell(SpellType spell)
    {
        if (!spellPrefabMapping.ContainsKey(spell))
        {
            Logger.Log($"Asked for a spelltype {spell} without a corresponding prefab", LogLevel.error);
        }

        // Start casting and change state
        GameObject newSpell = Instantiate<GameObject>(spellPrefabMapping[spell], Vector3.zero, Quaternion.identity);
        SpellBase spellInfo = newSpell.GetComponent<SpellBase>();
        currentSpell = spellInfo;

        playerState = PlayerState.Casting;

        Logger.Log($"Casting spell: {currentSpell.name}", LogLevel.info);
    }
}
