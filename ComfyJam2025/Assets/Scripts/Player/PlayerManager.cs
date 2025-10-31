using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle, // No spell ready, clicking zaps, can craft?
    Crafting,   // Crafting screen is up
    Casting,    // Spell is ready, cannot craft
}

public enum ItemType
{
    Stem = 0, // TODO: HACKY ENUMS
    Rind = 1
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public PlayerState playerState = PlayerState.Idle;

    private CraftingManager craftingManager;

    public Dictionary<ItemType, int> inventory = new Dictionary<ItemType, int>();

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
    }

    // Increase inventory amount of one item by 1
    public static void AddItem(ItemType itemType)
    {
        if (instance.inventory.ContainsKey(itemType))
        {
            instance.inventory[itemType] += 1;
        }
        else
        {
            instance.inventory[itemType] = 1;
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
