using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateForever : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    [SerializeField]
    private float rotationSpeed = 50f;

        void Update()
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
}