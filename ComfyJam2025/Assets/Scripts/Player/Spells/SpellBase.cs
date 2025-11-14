using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.VFX;

public enum DamageType
{
    None,
    Fire,
    Ice,
    Disintegrate
}

public abstract class SpellBase : MonoBehaviour
{
    public SpellType spellType;
    [SerializeField] protected VisualEffect spellVFX;
    [SerializeField] protected VisualEffect secondaryVFX;
    [SerializeField] protected DelayedDeath delayedDeath;
    protected float secondaryVFXcastDelay = 0.75f;
    protected const string VFX_EVENT_NAME = "OnAbilityCasted";
    protected bool canCastSpell = true;
    protected float spellLocationOffset = 3f;

    // Display aiming indicators for spell
    public abstract void Aim();

    // Actually cast
    public abstract void Cast();

    protected virtual void PlayVFX(Vector3 targetPosition, bool onlyPlaySecondary = false, Transform secondaryTransformTarget = null) {
        if (spellVFX != null && !onlyPlaySecondary) {
            VisualEffect vfx = Instantiate(spellVFX);
            vfx.transform.position = new Vector3(targetPosition.x, targetPosition.y + (-vfx.GetVector3("Direction").y / spellLocationOffset), 0f);
            vfx.SendEvent(VFX_EVENT_NAME);
        }
        if (secondaryVFX != null && canCastSpell) {
            StartCoroutine(DelayCastSecondaryVF(targetPosition, secondaryTransformTarget));
        }
        delayedDeath.StartDelayedDeath();
    }
    protected IEnumerator DelayCastSecondaryVF(Vector3 targetPosition, Transform secondaryTransformTarget = null) {
        canCastSpell = false;
        yield return new WaitForSeconds(secondaryVFXcastDelay);
        VisualEffect vfx2 = Instantiate(secondaryVFX, secondaryTransformTarget);
        vfx2.transform.position = new Vector3(targetPosition.x, targetPosition.y + (-vfx2.GetVector3("Direction").y / spellLocationOffset), 0f); ;
        vfx2.SendEvent(VFX_EVENT_NAME);
        canCastSpell = true;
    }
}
