using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;

    public List<ItemSlotUI> itemSlotUIs;

    private bool isOpen = false;

    private void Awake() {
        PlayerManager.instance.OnItemChange.AddListener(UpdateUI);
    }

    public void ToggleInventory() {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);
    }

    public void UpdateUI(ItemType itemType) {
        ItemData ID = PlayerManager.instance.possibleItems.FirstOrDefault(searching => searching.itemType == itemType);
        int indexOfID = PlayerManager.instance.possibleItems.IndexOf(ID);
        itemSlotUIs[indexOfID].gameObject.SetActive(true);
        itemSlotUIs[indexOfID].UpdateUI(ID.itemSprite, PlayerManager.instance.inventory[ID.itemType]);
    }
}
