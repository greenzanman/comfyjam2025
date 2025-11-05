using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDeath : MonoBehaviour
{
    [SerializeField] private float deathDelay = 2f;
    void Start() {
        IEnumerator Death() {
            yield return new WaitForSeconds(deathDelay);
            Destroy(gameObject);
        }
        StartCoroutine(Death());
    }
}
