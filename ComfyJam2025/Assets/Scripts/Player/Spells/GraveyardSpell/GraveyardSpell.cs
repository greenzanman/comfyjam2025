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
        // Create graveyard
        Instantiate(GraveyardPrefab, AimHighlight.position, Quaternion.identity);
        Destroy(gameObject);
    }
}