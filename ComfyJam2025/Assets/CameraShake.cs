using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // call this shit like CameraShake.Instance.Shake();
    public static CameraShake Instance { get; private set; }

    private void Awake()
    {
        // ensure only a single instance every exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private float defaultShakeDuration = 0.2f;
    [SerializeField] private float defaultShakeMagnitude = 0.1f;
    

    public void Shake(float duration, float magnitude)
    {
        StopAllCoroutines(); // stop current shakes before starting this one
        StartCoroutine(DoShake(duration, magnitude));
    }

    // this is if u dont wanna specify args
    public void Shake()
    {
        Shake(defaultShakeDuration, defaultShakeMagnitude);
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;  // store otiginal position
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // random offset
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // shake = original posotion + offset
            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;
            
            // wait for next frame before doing next shake
            yield return null;
        }

        // when done, reset position to stored original
        transform.localPosition = originalPos;
    }
}