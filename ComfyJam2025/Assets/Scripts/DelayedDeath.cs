using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDeath : MonoBehaviour
{
    [field: SerializeField] public float DeathDelay { get; set; } = 2f;

    public bool CanDie { get; set; } = true;

    void Start() {
        if (!CanDie) return;

        IEnumerator Death() {
            yield return new WaitForSeconds(DeathDelay);
            Destroy(gameObject);
        }
        StartCoroutine(Death());
    }
}
