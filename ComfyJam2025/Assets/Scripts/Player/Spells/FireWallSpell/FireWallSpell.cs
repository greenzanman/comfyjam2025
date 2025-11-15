using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class FireWallSpell : SpellBase
{
    public GameObject FireWallPrefab;
    private Transform AimHighlight;
    public void Start()
    {
        AimHighlight = transform.Find("AimingRect");
    }
    public override void Aim()
    {
        Vector2 mousePos = GameManager.GetMousePos();
        AimHighlight.position = mousePos;
        AimHighlight.rotation = Quaternion.Euler(Vector3.forward *
            Mathf.Atan(mousePos.y / mousePos.x) * Mathf.Rad2Deg);
    }

    public override void Cast()
    {
        // Tell AudioManager to play spell sfx
        AudioManager.instance.PlaySpellSound(SpellSound.FireWall);

        // Create firewall
        PlayVFX(AimHighlight.position);
        Instantiate(FireWallPrefab, AimHighlight.position, AimHighlight.rotation);
    }
}