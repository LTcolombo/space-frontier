using UnityEngine;

public class GenerateTerrain : MonoBehaviour
{
    [SerializeField] private GameObject terrainPrefab;


    [SerializeField] private int mapSize = 10;
    [SerializeField] private int tileSize = 1;

    // Start is called before the first frame update
    void Start()
    {
        for (var i = -mapSize; i < mapSize; i++)
        for (var j = -mapSize; j < mapSize; j++)
        {
            Instantiate(terrainPrefab, new Vector3(tileSize * i + Random.Range(0, tileSize/4f), 0, tileSize * j + Random.Range(0, tileSize/4f)), Quaternion.identity, transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}