using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingItem : MonoBehaviour
{
    TextMeshProUGUI text;
    Transform dragComponent;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        dragComponent = transform.Find("DragComponent");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetSprite(Sprite newSprite)
    {
        // TODO: Check if this occurs before start
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = newSprite;

        SpriteRenderer dragRenderer = transform.Find("DragComponent").GetComponent<SpriteRenderer>();
        dragRenderer.sprite = newSprite;
    }

    public void SetPosition(Vector2 newPosition)
    {
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        dragComponent.position = transform.position;
    }

    public void SetDragPosition(Vector2 newPosition)
    {
        // TODO: Fix the logic here so it doesn't have to be relative
        dragComponent.position = new Vector3(newPosition.x,
            newPosition.y, transform.position.z - 1);
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public void SetCount(int count)
    {
        text.text = count.ToString();
    }
}
