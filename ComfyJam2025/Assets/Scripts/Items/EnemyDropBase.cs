using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDropBase : MonoBehaviour
{
    public ItemType itemType;

    private enum DropState
    {
        dropping,
        dropped,
        picking
    }

    private DropState dropState = DropState.dropping;
    private SpriteRenderer spriteRenderer;
    private float age = 0;
    private float lifetime = 15;
    private Vector2 startPos;
    private float bumpHeight = 1;
    private float bumpWidth;
    
    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        startPos = transform.position;
        bumpWidth = Random.value * 2 - 1;
    }

    private void Update()
    {
        // Simple lifetime for drops
        age += GameManager.GetDeltaTime();
        if (age >= lifetime - 2) // alpha changes
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.3f + 0.7f * ((int)(age * 2) % 2));
        }

        switch (dropState)
        {
            case DropState.dropping:
                transform.position = startPos + new Vector2(bumpWidth * age,
                    bumpHeight * (1 - (2 * age - 1) * (2 * age - 1)));
                if (age >= 1)
                    dropState = DropState.dropped;
                break;
            case DropState.dropped:
                if (age > lifetime)
                    Destroy(gameObject);
                break;
            case DropState.picking:
                spriteRenderer.color = new Color(1, 1, 1, 1 - 2 * age);
                transform.position += Vector3.up * GameManager.GetDeltaTime() * 2;
                if (age >= 0.5)
                    Destroy(gameObject);
                break;
        }

        // TODO: Make this not suck
        if (Input.GetMouseButtonDown(0) && dropState != DropState.picking)
        {
            if ((GameManager.GetMousePos() - transform.position).sqrMagnitude < 4)
            {
                Pickup();
                dropState = DropState.picking;
                age = 0;
            }
        }
    }

    protected virtual void Pickup()
    {
        PlayerManager.AddItem(itemType);
    }
}
public enum ItemType {
    NONE,
    RIND,
    GREEN_LEAF,
    ORANGE_LEAF,
    RED_MUSH,
    BLUE_MUSH,
    GREEN_MUSH,
    SUNFLOWER,
    ANTLER,
    ACORN,
    FEATHER
}
