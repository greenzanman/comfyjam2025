using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataSO", menuName = "Create New Item/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("DATA")]
    public ItemType itemType;
    [Range(0f,1f)] public float dropChance = 0f;

    [Header("VISUALS")]
    public Sprite itemSprite;
}
