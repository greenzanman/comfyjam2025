using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDropBase : MonoBehaviour
{
    public ItemType itemType;

    private float lifetime = 15;

    void Update()
    {
        // Simple lifetime for drops
        lifetime -= GameManager.GetDeltaTime();
        if (lifetime < 0)
        {
            Destroy(gameObject);
        }

        // TODO: Make this not suck
        if (Input.GetMouseButtonDown(0))
        {
            if ((GameManager.GetMousePos() - transform.position).sqrMagnitude < 1)
            {
                Pickup();
                Destroy(gameObject);
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
