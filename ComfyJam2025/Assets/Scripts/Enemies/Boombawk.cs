using System.Collections;
using UnityEngine;

public class Boombawk : EnemyBase
{
    [Header("Boombawk Settings")]
    [SerializeField] private float moveSpeed = 0.5f;      // creeping speed
    [SerializeField] private float boomDamage = 450f;
    [SerializeField] private float boomRadius = 5f;       // tweak to 1/3 screen width
    [SerializeField] private float fuseTime = 3f;         // countdown once lit
    [SerializeField] private GameObject boomPreviewPrefab; // prefab for circle indicator

    private enum BoombawkState { Creeping, FuseLit, Exploded }
    private BoombawkState state = BoombawkState.Creeping;

    private Rigidbody2D rb;
    private CenterStation mama;
    private GameObject previewInstance;
    private float fuseTimer;

    protected override void Update()
    {
        base.Update();

        #if UNITY_EDITOR
            // 🔥 debug: press B to ignite this Boombawk
            if (Input.GetKeyDown(KeyCode.B))
            {
                Logger.Log("🔥 Manual burn trigger!", LogLevel.debug);
                Burn(3f); // duration doesn’t matter, just lights fuse
            }

            // ❄️ debug: press F to freeze/defuse
            if (Input.GetKeyDown(KeyCode.F))
            {
                Logger.Log("❄️ Manual freeze trigger!", LogLevel.debug);
                Freeze(2f);
            }
        #endif

    }

    protected override void InitializeEnemy()
    {
        Logger.Log($"Initializing Boombawk", LogLevel.debug);
        health = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        mama = GameManager.centerStation;

        // spawn preview circle (yellow transparent)
        if (boomPreviewPrefab)
        {
            previewInstance = Instantiate(boomPreviewPrefab, transform.position, Quaternion.identity);
            SetPreviewVisual(Color.yellow, 0.3f);
        }
    }

    protected override void Think()
    {
        switch (state)
        {
            case BoombawkState.Creeping:
                Creeping();
                break;

            case BoombawkState.FuseLit:
                FuseLit();
                break;
        }

        if (previewInstance)
            previewInstance.transform.position = transform.position;
    }

    private void Creeping()
    {
        if (mama == null) return;
        float dt = GameManager.GetDeltaTime();
        Vector2 targetPos = mama.transform.position;
        Vector2 newPos = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * dt);
        rb.MovePosition(newPos);
        Logger.Log($"Moving toward {targetPos} at {moveSpeed}", LogLevel.debug);
    }

    private void FuseLit()
    {
        fuseTimer -= GameManager.GetDeltaTime();
        if (fuseTimer <= 0)
        {
            StartCoroutine(Explode());
        }
    }

    private IEnumerator Explode()
    {
        state = BoombawkState.Exploded;
        if (previewInstance) Destroy(previewInstance);

        Logger.Log("💥 BOOMBAWK exploded!", LogLevel.info);

        // Damage nearby enemies (except itself)
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, boomRadius);
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyBase>();
            if (enemy && enemy != this)
                enemy.TakeDamage(boomDamage);
        }

        // Check damage to Mama (CenterStation)
        if (GameManager.centerStation != null)
        {
            float distToMama = Vector2.Distance(transform.position, GameManager.centerStation.transform.position);
            if (distToMama <= boomRadius)
            {
                PlayerManager.instance.TakeDamage(boomDamage);
                Logger.Log($"BOOMBAWK hit Mama! {boomDamage} damage dealt.", LogLevel.info);
            }
        }

        // Small delay to finish explosion FX
        yield return new WaitForSeconds(0.1f);

        Die();
    }

    public override void Burn(float burnDuration)
    {
        base.Burn(burnDuration);
        if (state == BoombawkState.Creeping)
        {
            // Light fuse
            state = BoombawkState.FuseLit;
            fuseTimer = fuseTime;
            SetPreviewVisual(Color.red, 1f);
            StartCoroutine(FlashPreview());
            rb.velocity = Vector2.zero;
        }
    }

    public override void Freeze(float freezeDuration)
    {
        base.Freeze(freezeDuration);

        if (state == BoombawkState.FuseLit)
        {
            // Defuse
            state = BoombawkState.Creeping;
            fuseTimer = 0;
            SetPreviewVisual(Color.yellow, 0.3f);
        }
    }

    private void SetPreviewVisual(Color color, float alpha)
    {
        if (!previewInstance) return;
        var renderer = previewInstance.GetComponent<SpriteRenderer>();
        if (renderer)
            renderer.color = new Color(color.r, color.g, color.b, alpha);
        previewInstance.transform.localScale = new Vector3(boomRadius * 2, boomRadius * 2, 1);
    }

    private IEnumerator FlashPreview()
    {
        if (!previewInstance) yield break;

        var renderer = previewInstance.GetComponent<SpriteRenderer>();
        for (int i = 0; i < 3; i++)
        {
            renderer.enabled = false;
            yield return new WaitForSeconds(0.25f);
            renderer.enabled = true;
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, boomRadius);
    }

    protected override void Die()
    {
        if (previewInstance)
            Destroy(previewInstance);
        base.Die();
    }
}
