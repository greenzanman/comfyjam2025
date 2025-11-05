using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDropBase : MonoBehaviour
{
    public ItemType itemType;

    private float lifetime = 15;
    // Start is called before the first frame update
    void Start()
    {
        
    }

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
            if ((GameManager.GetMousePos() - transform.position).sqrMagnitude < 4)
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
