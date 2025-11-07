using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
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
    Leaf
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public PlayerState playerState = PlayerState.Idle;

    private CraftingManager craftingManager;

    public List<ItemData> possibleItems;
    public Dictionary<ItemType, int> inventory = new Dictionary<ItemType, int>();
    [HideInInspector] public UnityEvent<ItemType> OnItemChange; // for inventory ui

    [Header("TEMP VFX")]
    public List<VisualEffect> vfxs;
    private const string VFX_EVENT_NAME = "OnAbilityCasted";

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
        InitializeItems();
    }
    private void InitializeItems() {
        foreach (ItemData itemData in possibleItems) {
            inventory.Add(itemData.itemType, 0);
            instance.OnItemChange.Invoke(itemData.itemType);
        }
    }
    // Increase inventory amount of one item by 1
    public static void AddItem(ItemType itemType) {

        if (instance.inventory.ContainsKey(itemType)) {
            instance.inventory[itemType] += 1;
            instance.OnItemChange.Invoke(itemType);
        }
    }
    private void Update()
    {
        HandlePlayerClick();
    }

    private void HandlePlayerClick()
    {
        // Basic zaps
        if (playerState == PlayerState.Idle && Input.GetMouseButtonDown(0))
        {
            DebugManager.DisplayDebug("Strike:" + GameManager.GetMousePos().ToString());

            // Find closest enemy within a region
            EnemyBase closestEnemy = EnemyManager.GetClosestEnemy(GameManager.GetMousePos(), 1);
            if (closestEnemy)
            {
                VisualEffect randomVfx = Instantiate(vfxs[UnityEngine.Random.Range(0, vfxs.Count)]);
                randomVfx.transform.position = new Vector3(closestEnemy.transform.position.x, closestEnemy.transform.position.y + (-randomVfx.GetVector3("Direction").y/2.75f), 0f);
                randomVfx.SendEvent(VFX_EVENT_NAME);
                closestEnemy.TakeDamage(1);
            }
        }

        // Pulling up crafting
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (playerState == PlayerState.Idle)
            {
                playerState = PlayerState.Crafting;
                craftingManager.gameObject.SetActive(true);
            }
            else
            {
                playerState = PlayerState.Idle;
                craftingManager.gameObject.SetActive(false);
            }
        }
    }
}
