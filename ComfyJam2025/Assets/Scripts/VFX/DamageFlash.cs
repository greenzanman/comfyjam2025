using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour {
    [SerializeField] private Color _flashColor;
    [SerializeField] private float flashTime = 0.25f;
    public SpriteRenderer[] spriteRenderers;

    public void TriggerDamageFlash() {
        StartCoroutine(DamageFlasher());
    }
    private IEnumerator DamageFlasher() {
        SetFlashColor();
        float currentFlashAmount;
        float timer = 0f;
        yield return new WaitForSeconds(0.2f); // delay for damage numbers
        while (timer < flashTime) {
            timer += Time.deltaTime;
            currentFlashAmount = Mathf.Lerp(1f, 0f, (timer / flashTime));
            SetFlashAmount(currentFlashAmount);
            yield return null;
        }
    }
    private void SetFlashAmount(float amt) {
        for (int i = 0; i < spriteRenderers.Length; i++) {
            spriteRenderers[i].material.SetFloat("_FlashAmount", amt);
        }
    }
    private void SetFlashColor() {
        for (int i = 0; i < spriteRenderers.Length; i++) {
            spriteRenderers[i].material.SetColor("_FlashColor", _flashColor);
        }
    }
}
