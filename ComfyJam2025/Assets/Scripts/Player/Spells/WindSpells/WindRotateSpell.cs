using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class WindClockwiseSpell : SpellBase
{
    private BoxCollider2D windCollider;
    private Transform AimHighlight;
    [SerializeField] private float windStrength = 4f;

    public void Start()
    {
        AimHighlight = transform.Find("AimingRect");
        windCollider = AimHighlight.GetComponentInChildren<BoxCollider2D>();
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
        Vector2 windDirection = GameManager.GetMousePos().normalized;
        windDirection = new Vector2(windDirection.y, -windDirection.x);
        // Push enemies
        foreach (EnemyBase enemy in EnemyManager.GetEnemies()) {

            if (windCollider.OverlapPoint(enemy.GetPosition())) {
                PlayVFX(enemy.transform.position);
                enemy.Blow(windDirection, windStrength);
            }
        }
    }
}