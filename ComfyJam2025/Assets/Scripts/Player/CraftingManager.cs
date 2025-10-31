using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    // TODO: Make a more robust version, this is hacky
    public List<Texture> itemTextures;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        // This version sucks
        Dictionary<ItemType, int> itemCounts = PlayerManager.instance.inventory;
        int pos = 1;
        foreach (ItemType itemType in itemCounts.Keys)
        {
            GUI.color = Color.white;
            GUI.Label(new Rect(60 * pos, 60, 32, 32), itemTextures[(int)itemType]);
            GUI.color = Color.black;
            GUI.Label(new Rect(60 * pos + 10, 90, 50, 50), itemCounts[itemType].ToString());
            pos += 1;
        }

    }
}
