using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

[RequireComponent(typeof(Light))]
public class Flicker : MonoBehaviour
{
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float effectStrength = 0.1f;
    private Light _light;
    private float _initialRange;
    private float _initialIntensity;

    void Start()
    {
        _light = GetComponent<Light>();
        _initialRange = _light.range;
        _initialIntensity = _light.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        _light.range = (1-effectStrength + Random.value * effectStrength) * _initialRange;
        _light.intensity = (1-effectStrength + Random.value * effectStrength) * _initialIntensity;
    }
}
