using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Events;

public enum PlayerState
{
    Idle, // No spell ready, clicking zaps, can craft?
    Crafting,   // Crafting screen is up
    Casting,    // Spell is ready, cannot craft
}

public enum SpellType
{
    Dud = -1,
    Test = 0,
    FireWall = 1,
    CircleFreeze = 2,
    FireCone = 3,
    Graveyard = 4,
    WindPush = 5,
    WindRotate = 6,
    BurnCircle = 7,
    Sunbeam = 8,
    ZapSpell = 9,
    BlowerSpell = 10,
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public PlayerState playerState = PlayerState.Idle;

    private const float ZAP_RADIUS = 4;
    private CraftingManager craftingManager;
    private Vector2 CRAFTING_POSITION = new Vector2(22, -17);
    private Vector2 CRAFTING_POSITION_BUMP = new Vector2(22, -16);
    private int CRAFTING_WIDTH = 4;
    private Transform craftingButton;

    public List<ItemData> possibleItems;
    public Dictionary<ItemType, int> inventory = new Dictionary<ItemType, int>();
    [HideInInspector] public UnityEvent<ItemType> OnItemChange; // for inventory ui

    [SerializeField] private float spellLocationOffset = 3f; 

    [Header("TEMP VFX")]
    public List<VisualEffect> vfxs;
    private const string VFX_EVENT_NAME = "OnAbilityCasted";

    public List<GameObject> spellPrefabs;
    private Dictionary<SpellType, GameObject> spellPrefabMapping; // Internal mapping of prefabs for each spell

    // Small buffer to prevent accidentally doubleclicking
    private float spellBuffer = 0;
    private SpellBase currentSpell;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;

        //Logger.Log("PlayerManager registered", LogLevel.info);
        craftingManager = GetComponentInChildren<CraftingManager>();
        if (craftingManager == null) {
            Logger.Log("Failed to find crafting manager attached to player manager.", LogLevel.fatal);
        }
        craftingManager.gameObject.SetActive(false);

        craftingButton = transform.Find("InventoryButton");
        if (craftingButton == null)
        {
            Logger.Log("Inventory button is missing.", LogLevel.fatal);
        }

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
            if (spellPrefabMapping.ContainsKey(spellInfo.spellType))
            {
                
                Logger.Log($"Recieved duplicates for spell type {spellInfo.spellType}", LogLevel.error);
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


#if UNITY_EDITOR
        PlayerHacks();
#endif
    }

    private void PlayerHacks()
    {
        // Add ingredients
        if (Input.GetKeyDown(KeyCode.C))
        {
            foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
            {
                AddItem(itemType);
            }
        }
        // Kill all enemies
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (EnemyBase enemy in EnemyManager.GetEnemies().ToList())
            {
                enemy.TakeDamage(9999);
            }
        }
    }

    private void HandlePlayerClick()
    {

        // Pulling up or down crafting
        if ((playerState == PlayerState.Idle || playerState == PlayerState.Crafting) &&
            IsMouseOnInventoryButton())
        {
            craftingButton.position = CRAFTING_POSITION_BUMP;

            if (Input.GetMouseButtonDown(0))
            {
                if (playerState == PlayerState.Idle)
                {
                    SetCraftingState(true);
                }
                else if (playerState == PlayerState.Crafting)
                {
                    SetCraftingState(false);
                }
                return;
            }
        }
        else
        {
            craftingButton.position = CRAFTING_POSITION;
        }

        // Basic zaps
        if (playerState == PlayerState.Idle && Input.GetMouseButtonDown(0))
        {
            DebugManager.DisplayDebug("Strike:" + GameManager.GetMousePos().ToString());

            // Find closest enemy within a region
            EnemyBase closestEnemy = EnemyManager.GetClosestEnemy(GameManager.GetMousePos(), ZAP_RADIUS);
            if (closestEnemy)
            {
                VisualEffect randomVfx = Instantiate(vfxs[UnityEngine.Random.Range(0, vfxs.Count)]);
                randomVfx.transform.position = new Vector3(closestEnemy.transform.position.x, closestEnemy.transform.position.y + (-randomVfx.GetVector3("Direction").y/spellLocationOffset), 0f);
                randomVfx.SendEvent(VFX_EVENT_NAME);
                closestEnemy.TakeDamage(1);
            }
        } // Casting spell
        else if (playerState == PlayerState.Casting)
        {
            currentSpell.Aim();
            spellBuffer = Mathf.Max(0, spellBuffer - Time.deltaTime); // This doesn't matter on slowdown

            if (spellBuffer <= 0 && Input.GetMouseButtonDown(0))
            {
                currentSpell.Cast();
                playerState = PlayerState.Idle;
                DebugManager.DisplayDebug("Casting:" + GameManager.GetMousePos().ToString());
            }
        }
    }

    // If mouse is on the inventory button
    private bool IsMouseOnInventoryButton()
    {
        Vector2 mousePos = GameManager.GetMousePos();
        return mousePos.x < CRAFTING_POSITION.x + CRAFTING_WIDTH &&
        mousePos.x > CRAFTING_POSITION.x - CRAFTING_WIDTH &&
        mousePos.y < CRAFTING_POSITION.y + CRAFTING_WIDTH &&
        mousePos.y > CRAFTING_POSITION.y - CRAFTING_WIDTH;
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

        spellBuffer = 0.2f;
    }
}
