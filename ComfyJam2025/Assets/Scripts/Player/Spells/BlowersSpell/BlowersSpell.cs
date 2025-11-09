using System.Collections.Generic;
using UnityEngine;


public class BlowersSpell : SpellBase
{
    public GameObject BlowersPrefab;

    private Transform AimHighlight;
    private Transform AimRect;
    public void Start()
    {
        AimHighlight = transform.Find("TargetCircle");
        AimRect = transform.Find("AimingRect");
    }
    public override void Aim()
    {
        Vector2 mousePos = GameManager.GetMousePos();
        AimHighlight.position = mousePos;
        AimRect.position = mousePos;
        AimRect.rotation = Quaternion.Euler(Vector3.forward *
            Mathf.Atan(mousePos.y / mousePos.x) * Mathf.Rad2Deg);
    }

    public override void Cast()
    {
        // Create graveyard
        GameObject blowersPrefab = Instantiate(BlowersPrefab, AimHighlight.position, Quaternion.identity);
        Vector2 windDirection = GameManager.GetMousePos().normalized;
        BlowersSelf blowers = blowersPrefab.GetComponent<BlowersSelf>();
        blowers.windDirection = windDirection;
        Destroy(gameObject);
    }
}