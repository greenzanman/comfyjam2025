using System.Collections.Generic;
using UnityEngine;


public class GraveyardSpell : SpellBase
{
    public GameObject GraveyardPrefab;
    private Transform AimHighlight;
    public void Start()
    {
        AimHighlight = transform.Find("AimingCircle");
    }
    public override void Aim()
    {
        AimHighlight.position = GameManager.GetMousePos();
    }

    public override void Cast()
    {
        // Tell AudioManager to play spell sfx
        AudioManager.instance.PlaySpellSound(SpellSound.Summon);

        PlayVFX(AimHighlight.position);
        // Create graveyard
        Instantiate(GraveyardPrefab, AimHighlight.position, Quaternion.identity);
    }
}