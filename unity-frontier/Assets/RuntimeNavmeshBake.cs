using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class RuntimeNavmeshBake : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
