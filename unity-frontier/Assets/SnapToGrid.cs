using System;
using UnityEngine;

[ExecuteInEditMode]
public class SnapToGrid : MonoBehaviour
{
    void Update()
    {
        var transformPosition = transform.position;
        transform.position = new Vector3(
            (float)Math.Round(transformPosition.x),
            (float)Math.Round(transformPosition.y),
            (float)Math.Round(transformPosition.z));
    }
}