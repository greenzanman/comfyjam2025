using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    const int INVENTORY_DEPTH = -2;
    public GameObject craftingItemPrefab;
    private Vector3 OFFSCREEN = new Vector3(-20, -20, INVENTORY_DEPTH);

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

            craftingItems[itemType].SetPosition(new Vector2(1 + 2 * pos, -2));
            craftingItems[itemType].SetCount(itemCounts[itemType]);
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
                if ((craftingItem.GetPosition() - mousePos).sqrMagnitude < 1)
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
                    if (utils.FlatSqrDistance(slot.transform.position, mousePos) < 1)
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
}
