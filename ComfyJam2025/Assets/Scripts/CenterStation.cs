using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterStation : MonoBehaviour
{
    private void Awake()
    {
        GameManager.centerStation = this;
    }
}
