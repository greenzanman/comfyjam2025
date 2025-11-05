using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // TODO: Make a more robust version, this is hacky
    public List<Sprite> itemSprites;
    public static GameManager instance;
    public static CenterStation centerStation;
    private Vector3 mousePos;

    void Awake()
    {
        if (instance == null)
            instance = this;

        // Check itemTextures validity
        if (itemSprites.Count != Enum.GetNames(typeof(ItemType)).Length)
        {
            Logger.Log("Item textures not properly assigned for all items", LogLevel.fatal);
        }

        Logger.Log("GameManager registered", LogLevel.info);
    }

    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);
    }

    public static Sprite GetSprite(ItemType itemType)
    {
        return instance.itemSprites[(int)itemType];
    }

    public static float GetTimeDilation()
    {
        float baseDilation = 1;

        if (Input.GetMouseButton(1))
        {
            baseDilation *= 5;
        }
        // If book open, return 0.2
        if (PlayerManager.instance.playerState == PlayerState.Crafting)
        {
            baseDilation *= 0.1f;
        }

        return baseDilation;
    }

    public static float GetDeltaTime()
    {
        return Time.deltaTime * GetTimeDilation();
    }

    public static Vector3 GetMousePos()
    {
        return instance.mousePos;
    }
}
