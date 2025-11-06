using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
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

        // Place items
        // PlaceItems();

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

            craftingItems[itemType].SetPosition(new Vector2(2 + 5 * pos, 5 + (int)(pos / 5) * 3));
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
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (heldItem != null)
            {
                // Check if its near a slot
                foreach (CraftingSlot slot in craftingSlots)
                {
                    if (utils.FlatSqrDistance(slot.transform.position, mousePos) < 4)
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
            DebugManager.DisplayDebug("Here");
            heldItem.SetDragPosition(mousePos);
        }
    }


    // TODO: Reset all slots when crafting is closed

    private void UpdateSlot(CraftingSlot slot, CraftingItem addedItem)
    {
        // TODO: They're gone for good once placed?
        // Add back item it it held one
        // if (slot.hasItem)
        // {
        //     PlayerManager.AddItem(slot.itemType);
        // }


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

            PlayerManager.instance.AddSpell(SpellType.Test);

            // Clear crafting
            foreach (CraftingSlot slot in craftingSlots)
            {
                slot.ClearSprite();
            }
        }
    }
}
