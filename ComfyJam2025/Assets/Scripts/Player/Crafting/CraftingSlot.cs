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

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        emptySprite = spriteRenderer.sprite;

        hasItem = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetSprite(Sprite newSprite)
    {
        spriteRenderer.sprite = newSprite;
        // Hacky fix
        transform.localScale = new Vector3(3, 3, 3);
        hasItem = true;
    }

    public void ClearSprite()
    {
        spriteRenderer.sprite = emptySprite;
        transform.localScale = Vector3.one;

        hasItem = false;
    }
}
