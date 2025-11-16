using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDropBase : MonoBehaviour {
    public ItemType itemType;
    [SerializeField] private float bounceHeight = 0.05f;
    [SerializeField] private float bounceDuration = 0.5f;

    private Vector3 originalPosition;
    private enum DropState {
        dropping,
        dropped,
        picking
    }

    private DropState dropState = DropState.dropping;
    private SpriteRenderer spriteRenderer;
    public float age = 0;
    // If this wasn't produced by a deer
    public bool isOriginal = true;
    private float lifetime = 15;
    private Vector2 startPos;
    private float bumpHeight = 1;
    private float bumpWidth;

    private void Start() {
        bounceHeight = Mathf.Clamp(bounceHeight, 0.1f, 5.0f);
        bounceDuration = Mathf.Clamp(bounceDuration, 0.1f, 5.0f);

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        startPos = transform.position;
        bumpWidth = Random.value * 6 - 3;
    }

    private void Update() {
        // Simple lifetime for drops
        age += GameManager.GetDeltaTime();
        if (age >= lifetime - 2) // alpha changes
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.3f + 0.7f * ((int)(age * 2) % 2));
        }

        switch (dropState) {
            case DropState.dropping:
                transform.position = startPos + new Vector2(bumpWidth * age,
                    bumpHeight * (2 - (2.5f * age - 1) * (3 * age - 1)));
                if (age >= 1) {
                    dropState = DropState.dropped;
                    originalPosition = transform.position;
                }
                break;
            case DropState.dropped:
                // Calculate bounce height based on time and bounce duration
                float currentHeight = bounceHeight * Mathf.Sin(Mathf.PI * (age - 1) / bounceDuration);

                // Update position with bounce height
                transform.position = originalPosition + new Vector3(0, currentHeight, 0);

                if (age > lifetime)
                    Destroy(gameObject);
                break;
            case DropState.picking:
                spriteRenderer.color = new Color(1, 1, 1, 1 - 4 * age);
                transform.position += Vector3.up * GameManager.GetDeltaTime() * 7;
                if (age >= 0.5)
                    Destroy(gameObject);
                break;
        }

        // TODO: Make this not suck
        if (Input.GetMouseButtonDown(0) && dropState != DropState.picking) {
            if ((GameManager.GetMousePos() - transform.position).sqrMagnitude < 4) {
                Pickup();
                dropState = DropState.picking;
                age = 0;
            }
        }
    }

    protected virtual void Pickup() {
        PlayerManager.AddItem(itemType);
    }

    void OnMouseOver() {
        if (dropState != DropState.picking && age > 0.1f) {
            Pickup();
            dropState = DropState.picking;
            age = 0;
        }
    }
}
