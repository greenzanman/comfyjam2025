using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterStation : MonoBehaviour
{
    private void Awake()
    {
        GameManager.centerStation = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(new Vector2(1, 1) * Time.deltaTime);
        //DebugManager.DisplayDebug("Things are happening");
    }
}
