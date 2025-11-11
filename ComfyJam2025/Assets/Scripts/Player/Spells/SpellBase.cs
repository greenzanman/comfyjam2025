using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

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
    private void Start()
    {
    }

    // Display aiming indicators for spell
    public abstract void Aim();

    // Actually cast
    public abstract void Cast();
}
