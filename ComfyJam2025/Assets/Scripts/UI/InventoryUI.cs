using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private GameObject inventoryPanel;
    private bool isOpen = false;

    public void ToggleInventory() {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);
    }

    
}
