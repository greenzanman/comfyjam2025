using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SunbeamSpell : SpellBase
{
    public GameObject SunbeamPrefab;
    private Transform AimHighlight;
    public void Start()
    {
        AimHighlight = transform.Find("AimingCircle");
    }
    public override void Aim()
    {
        Vector2 mousePos = GameManager.GetMousePos();
        AimHighlight.position = mousePos;
    }

    public override void Cast()
    {
        // Create firewall
        Instantiate(SunbeamPrefab, AimHighlight.position, AimHighlight.rotation);
        Destroy(gameObject);
    }
}