using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    private (ItemType, ItemType, ItemType, SpellType)[] spellRecipes = {
        ( ItemType.RIND, ItemType.ANTLER, ItemType.ANTLER, SpellType.BurnCircle),
        ( ItemType.RIND, ItemType.RIND, ItemType.RIND, SpellType.FireWall),
        ( ItemType.ANTLER, ItemType.ANTLER, ItemType.ANTLER, SpellType.CircleFreeze),
        ( ItemType.RIND, ItemType.ANTLER, ItemType.RIND, SpellType.FireCone),
        ( ItemType.FEATHER, ItemType.FEATHER, ItemType.FEATHER, SpellType.Graveyard),
        ( ItemType.FEATHER, ItemType.FEATHER, ItemType.ANTLER, SpellType.WindPush),
        ( ItemType.FEATHER, ItemType.FEATHER, ItemType.RIND, SpellType.WindRotate),
        ( ItemType.FEATHER, ItemType.RIND, ItemType.RIND, SpellType.ZapSpell),
        ( ItemType.FEATHER, ItemType.RIND, ItemType.ANTLER, SpellType.Sunbeam),
        ( ItemType.FEATHER, ItemType.ANTLER, ItemType.ANTLER, SpellType.BlowerSpell),
    };

    private int ConvertItemsToInt(ItemType item1, ItemType item2, ItemType item3)
    {
        return (int)Math.Pow(3, (int)item1) + (int)Math.Pow(3, (int)item2) + (int)Math.Pow(3, (int)item3);
    }

    private Dictionary<int, SpellType> spellDict;

    const int INVENTORY_DEPTH = 0;
    public GameObject craftingItemPrefab;
    private Vector3 OFFSCREEN = new Vector3(-40, -40, INVENTORY_DEPTH);

    // Items displayed in inventory
    private Dictionary<ItemType, CraftingItem> craftingItems = new Dictionary<ItemType, CraftingItem>();
    private CraftingItem heldItem = null;

    private List<CraftingSlot> craftingSlots = new List<CraftingSlot>();

    // Start is called before the first frame update
    void Start()
    {

        // Create each item display
        foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
        {
            GameObject newItem = Instantiate(craftingItemPrefab,
                OFFSCREEN, Quaternion.identity);
            newItem.transform.SetParent(transform);

            CraftingItem craftingComponent = newItem.GetComponent<CraftingItem>();

            if (craftingComponent == null)
            {
                Logger.Log("Crafting item prefab is missing component", LogLevel.error);
                break;
            }

            craftingComponent.SetSprite(GameManager.GetSprite(itemType));
            craftingComponent.SetItemType(itemType);

            craftingItems[itemType] = craftingComponent;
        }

        craftingSlots.Add(transform.Find("CraftingSlot1").GetComponent<CraftingSlot>());
        craftingSlots.Add(transform.Find("CraftingSlot2").GetComponent<CraftingSlot>());
        craftingSlots.Add(transform.Find("CraftingSlot3").GetComponent<CraftingSlot>());

        if (spellRecipes.Length <= 0) return;

        // Fill spell dict
        spellDict = new Dictionary<int, SpellType>();
        foreach ((ItemType, ItemType, ItemType, SpellType) recipe in spellRecipes)
        {
            int hash = ConvertItemsToInt(recipe.Item1, recipe.Item2, recipe.Item3);
            if (spellDict.ContainsKey(hash))
            {
                Logger.Log($"Recieved duplicate recipe for {recipe.Item4}", LogLevel.error);
            }
            else
            {
                spellDict[hash] = recipe.Item4;
            }
        }

        // Register some console variables
        DebugManager.RegisterConsoleVar("DrawInventoryHitbox", 0);
    }

    // Update is called once per frame
    void Update()
    {
        // Place inventory items along the screen
        PlaceInventory();

        DragInventory();
    }

    // Calculates and places initial positions of the inventory
    private void PlaceItems()
    {
        float screenHeight = Camera.main.orthographicSize;
        float screenWidth = screenHeight * Camera.main.aspect;
        Logger.Log(screenWidth.ToString(), LogLevel.error);
    }

    private void PlaceInventory()
    {
        // First, move everything off screen
        foreach (CraftingItem craftingItem in craftingItems.Values)
        {
            craftingItem.transform.position = OFFSCREEN;
        }

        // Then, place player known items
        Dictionary<ItemType, int> itemCounts = PlayerManager.instance.inventory;
        int pos = 0;
        foreach (ItemType itemType in itemCounts.Keys)
        {

            craftingItems[itemType].SetPosition(new Vector2(2 + 5 * (pos % 5), 4 + (int)(pos / 5) * 6));
            craftingItems[itemType].SetCount(itemCounts[itemType]);
            if (heldItem != null && heldItem.GetItemType() == itemType)
                craftingItems[itemType].SetCount(itemCounts[itemType] - 1);
            pos += 1;
        }
    }

    private void DragInventory()
    {
        Vector2 mousePos = GameManager.GetMousePos();
        // Clicking down
        if (Input.GetMouseButtonDown(0))
        {
            // TODO: heldItem should be separate from item Display
            foreach (CraftingItem craftingItem in craftingItems.Values)
            {
                if ((craftingItem.GetPosition() - mousePos).sqrMagnitude < 4)
                {
                    heldItem = craftingItem;
                    break;
                }
            }

            foreach (CraftingSlot slot in craftingSlots)
            {
                if (slot.hasItem && utils.FlatSqrDistance(slot.transform.position, mousePos) < 9)
                {
                    heldItem = craftingItems[slot.itemType];
                    PlayerManager.AddItem(slot.itemType);

                    slot.ClearSprite();
                    break;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (heldItem != null)
            {
                // Check if its near a slot
                foreach (CraftingSlot slot in craftingSlots)
                {
                    if (utils.FlatSqrDistance(slot.transform.position, mousePos) < 9)
                    {
                        UpdateSlot(slot, heldItem);
                        break;
                    }
                }
            }
            heldItem = null;
        }

        if (heldItem != null)
        {
            heldItem.SetDragPosition(mousePos);
        }
    }


    // TODO: Reset all slots when crafting is closed

    private void UpdateSlot(CraftingSlot slot, CraftingItem addedItem)
    {
        // Add back item it it held one
        if (slot.hasItem)
        {
            PlayerManager.AddItem(slot.itemType);
        }


        slot.SetSprite(GameManager.GetSprite(addedItem.GetItemType()));
        slot.itemType = addedItem.GetItemType();
        // Remove used item
        PlayerManager.RemoveItem(addedItem.GetItemType());
        CheckRecipes();
    }

    // Currently, just crafts the moment they're all filled
    private void CheckRecipes()
    {
        // Currently, just checks that there's a total of three ingredients
        int ingredientCount = 0;
        foreach (CraftingSlot slot in craftingSlots)
        {
            if (slot.hasItem)
                ingredientCount++;
        }

        if (ingredientCount == 3)
        {
            // Close crafting window
            PlayerManager.instance.SetCraftingState(false);

            PlayerManager.instance.AddSpell(GetSpell());

            // Clear crafting
            foreach (CraftingSlot slot in craftingSlots)
            {
                slot.ClearSprite();
            }
        }
    }

    private SpellType GetSpell()
    {
        // Assumes all slots have items
        int hash = ConvertItemsToInt(craftingSlots[0].itemType,
            craftingSlots[1].itemType, craftingSlots[2].itemType);

        if (spellDict.ContainsKey(hash))
        {
            return spellDict[hash];
        }
        else
        {
            return SpellType.Dud;
        }

        // TODO: Return a 'dud' spell
    }
}
