using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDeath : MonoBehaviour
{
    [field: SerializeField] public float DeathDelay { get; set; } = 2f;
    [field: SerializeField] public bool DieOnStart { get; set; } = true;

    public bool CanDie { get; set; } = true;

    void Start() {
        if (!CanDie) return;
        if (!DieOnStart) return;

        StartDelayedDeath();
    }
    public void StartDelayedDeath() {
        StartCoroutine(Death());
    }
    private IEnumerator Death() {
        yield return new WaitForSeconds(DeathDelay);
        Destroy(gameObject);
    }
}
