using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private GameObject nullOverlay;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemAmountText;

    public void UpdateUI(Sprite spriteImage, int itemAmount) {
        if (itemAmount <= 0) {
            nullOverlay.SetActive(true);
        }
        else {
            nullOverlay.SetActive(false);
        }

        itemImage.sprite = spriteImage;
        itemAmountText.text = itemAmount.ToString("0");
    }
}
