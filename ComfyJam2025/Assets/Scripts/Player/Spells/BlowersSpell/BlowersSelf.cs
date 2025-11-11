using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowersSelf : MonoBehaviour
{
    public GameObject blowerPrefab;
    [SerializeField] private float blowerCount = 3;
    private List<Blower> blowerList = new List<Blower>();
    private float age;
    public Vector2 windDirection;
    [SerializeField] private float lifetime = 20;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            float spawnAngle = Mathf.PI * 2 / blowerCount * i;
            Vector2 spawnPos = 2 * new Vector2(Mathf.Cos(spawnAngle), Mathf.Sin(spawnAngle));
            Vector2 transformedPos = transform.position + new Vector3(spawnPos.x, spawnPos.y, 0);
            GameObject blowerObject = Instantiate(blowerPrefab, transformedPos, Quaternion.identity);
            Blower blower = blowerObject.GetComponent<Blower>();
            blower.basePos = transformedPos;
            blower.windDirection = windDirection;
            blowerList.Add(blower);
        }
    }

    // Update is called once per frame
    void Update()
    {
        age += GameManager.GetDeltaTime();
        if (age > lifetime)
        {
            foreach (Blower blower in blowerList)
                Destroy(blower.gameObject);
            Destroy(gameObject);

        }
    }
}
