using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    private (ItemType, ItemType, ItemType, SpellType)[] spellRecipes = {

        ( ItemType.RIND, ItemType.RIND, ItemType.RIND, SpellType.Dud),
        ( ItemType.ACORN, ItemType.ACORN, ItemType.ACORN, SpellType.ZapSpell),
        ( ItemType.GREEN_LEAF, ItemType.GREEN_LEAF, ItemType.GREEN_LEAF, SpellType.WindPush),
        ( ItemType.RED_MUSH, ItemType.RED_MUSH, ItemType.RED_MUSH, SpellType.Sunbeam),


        ( ItemType.ACORN, ItemType.ACORN, ItemType.GREEN_LEAF, SpellType.FireWall),
        ( ItemType.ACORN, ItemType.ACORN, ItemType.RED_MUSH, SpellType.Sunbeam),
        ( ItemType.ACORN, ItemType.ACORN, ItemType.RIND, SpellType.Graveyard),

        ( ItemType.GREEN_LEAF, ItemType.GREEN_LEAF, ItemType.ACORN, SpellType.WindRotate),
        ( ItemType.GREEN_LEAF, ItemType.GREEN_LEAF, ItemType.RED_MUSH, SpellType.FireWall),
        ( ItemType.GREEN_LEAF, ItemType.GREEN_LEAF, ItemType.RIND, SpellType.Sunbeam),

        ( ItemType.RED_MUSH, ItemType.RED_MUSH, ItemType.ACORN, SpellType.ZapSpell),
        ( ItemType.RED_MUSH, ItemType.RED_MUSH, ItemType.GREEN_LEAF, SpellType.FireCone),
        ( ItemType.RED_MUSH, ItemType.RED_MUSH, ItemType.RIND, SpellType.WindPush),

        ( ItemType.RIND, ItemType.RIND, ItemType.ACORN, SpellType.CircleFreeze),
        ( ItemType.RIND, ItemType.RIND, ItemType.GREEN_LEAF, SpellType.FireWall),
        ( ItemType.RIND, ItemType.RIND, ItemType.RED_MUSH, SpellType.FireCone),

        ( ItemType.RIND, ItemType.ACORN, ItemType.RED_MUSH, SpellType.BlowerSpell),
        ( ItemType.RIND, ItemType.ACORN, ItemType.GREEN_LEAF, SpellType.CircleFreeze),
        ( ItemType.ACORN, ItemType.GREEN_LEAF, ItemType.RED_MUSH, SpellType.BurnCircle),
        ( ItemType.RIND, ItemType.GREEN_LEAF, ItemType.RED_MUSH, SpellType.ZapSpell),
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

    [Header("ITEM SLOT VISUAL POSITION")]
    [SerializeField] private float initialStartingPos = 0;
    [SerializeField] private float rows = 4;
    [SerializeField] private float columns = 1;
    [SerializeField] private float moveUp = -6;

    void Start()
    {

        // Create each item display
        foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
        {
            GameObject newItem = Instantiate(craftingItemPrefab, OFFSCREEN, Quaternion.identity);
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
        //DebugManager.RegisterConsoleVar("DrawInventoryHitbox", 0);
    }

    void Update()
    {
        PlaceInventory();
        DragInventory();
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
        float pos = initialStartingPos;
        foreach (ItemType itemType in itemCounts.Keys)
        {

            craftingItems[itemType].SetPosition(new Vector2(rows + columns * (pos % columns), rows*2 + (int)(pos / columns) * moveUp));
            craftingItems[itemType].SetCount(itemCounts[itemType]);
            if (heldItem != null && heldItem.GetItemType() == itemType)
                craftingItems[itemType].SetCount(itemCounts[itemType] - 1);
            pos += 1;
        }
    }

    private void DragInventory()
    {
        Vector2 mousePos = GameManager.GetMousePos();
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
