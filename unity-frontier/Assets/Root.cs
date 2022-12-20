using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    private static GameObject Instance;
    void Start()
    {
        Instance = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
