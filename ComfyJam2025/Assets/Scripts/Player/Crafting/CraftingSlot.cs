using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spot that you can drop the crafting items into
public class CraftingSlot : MonoBehaviour
{
    public ItemType itemType;

    private SpriteRenderer spriteRenderer;

    Sprite emptySprite;
    public bool hasItem = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        emptySprite = spriteRenderer.sprite;

        hasItem = false;
    }

    public void SetSprite(Sprite newSprite)
    {
        spriteRenderer.sprite = newSprite;
        hasItem = true;
    }

    public void ClearSprite()
    {
        spriteRenderer.sprite = emptySprite;

        hasItem = false;
    }
}
