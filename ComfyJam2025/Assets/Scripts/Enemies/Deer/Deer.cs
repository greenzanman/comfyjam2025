using System.Collections.Generic;
using UnityEngine;

public class Deer : EnemyBase
{
    [SerializeField] private float moveSpeed = 1f;

    [SerializeField] private List<Sprite> sprites;
    enum DeerMode
    {
        Prancing,
        Grazing
    }
    DeerMode deerMode = DeerMode.Prancing;

    bool hasItem;

    private const float GRAZE_LENGTH = 4;
    private float grazeTimer = 0;
    private float spriteTimer = 0;
    ItemType currentItem;

    // Random diagonal direction
    private Vector3 pranceDirection;

    private float fieldWidth;
    private float fieldHeight;
    private SpriteRenderer spriteRenderer;

    protected override void InitializeEnemy()
    {
        health = maxHealth;

        spriteRenderer = transform.Find("Visual").GetComponent<SpriteRenderer>();
        pranceDirection = new Vector2(Random.value > 0.5f ? 1 : -1, Random.value > 0.5f ? 1 : -1);

        (fieldWidth, fieldHeight) = GameManager.GetScreenDimensions();
        fieldWidth /= 2;
        fieldHeight /= 2;
    }
    protected override void Think()
    {
        float deltaTime = GameManager.GetDeltaTime();
        spriteTimer += deltaTime;
        switch (deerMode)
        {
            case DeerMode.Prancing:
                transform.position += moveSpeed * deltaTime * pranceDirection;
                if (pranceDirection.x > 0 && transform.position.x > fieldWidth - 7 ||
                    pranceDirection.x < 0 && transform.position.x < -fieldWidth + 7)
                        pranceDirection.x = -pranceDirection.x;
                if (pranceDirection.y > 0 && transform.position.y > fieldHeight - 7 ||
                    pranceDirection.y < 0 && transform.position.y < -fieldHeight + 7)
                        pranceDirection.y = -pranceDirection.y;
                
                spriteRenderer.flipX = pranceDirection.x > 0;
                spriteRenderer.sprite = sprites[(int)(spriteTimer * 10) % 6]; // frames 0-5
                spriteRenderer.transform.localPosition = new Vector2(0, 2);

                
                EnemyDropBase[] drops = FindObjectsOfType<EnemyDropBase>();
                foreach (EnemyDropBase drop in drops)
                {
                    if (drop.age > 1 && drop.isOriginal && utils.FlatSqrDistance(drop.transform.position, GetPosition()) < 9)
                    {
                        hasItem = true;
                        currentItem = drop.itemType;
                        Destroy(drop.gameObject);
                        grazeTimer = GRAZE_LENGTH;
                        deerMode = DeerMode.Grazing;
                        break;
                    }
                }

                break;
            case DeerMode.Grazing:
                grazeTimer -= deltaTime;
                if (grazeTimer <= 0)
                {
                    deerMode = DeerMode.Prancing;
                    for (int i = 0; i < 3; i++)
                    {
                        GameObject drop = Instantiate(itemDropBasePrefab, transform.position + new Vector3(
                            Random.value * 6 - 3, Random.value * 6 - 3
                        ), Quaternion.identity);
                        drop.GetComponentInChildren<SpriteRenderer>().sprite = GameManager.GetSprite(currentItem);
                        drop.GetComponent<EnemyDropBase>().itemType = currentItem;
                        drop.GetComponent<EnemyDropBase>().isOriginal = false;
                    }
                    hasItem = false;
                    pranceDirection = new Vector2(Random.value > 0.5f ? 1 : -1, Random.value > 0.5f ? 1 : -1);
                }

                int grazingFrame = 6 + ((int)(spriteTimer * 6) % 2); // frame 6 or 7
                spriteRenderer.sprite = sprites[grazingFrame];
                spriteRenderer.transform.localPosition = Vector2.zero;
                break;
        }
    }

    protected override void Die()
    {
        if (hasItem)
        {
            GameObject drop = Instantiate(itemDropBasePrefab, transform.position, Quaternion.identity);
            drop.GetComponentInChildren<SpriteRenderer>().sprite = GameManager.GetSprite(currentItem);
            drop.GetComponent<EnemyDropBase>().itemType = currentItem;
        }

        // Inform manager
        EnemyManager.DeregisterEnemy(this);

        Destroy(gameObject);
    }
}
