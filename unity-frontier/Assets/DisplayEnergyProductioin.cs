using System.Collections;
using System.Collections.Generic;
using CityBuilding;
using Unity.VisualScripting;
using UnityEngine;
using Utils.Injection;

public class DisplayEnergyProductioin : InjectableBehaviour
{
    [Inject] private BuilderModel _model;
    void Start()
    {
        Redraw();
        
        _model.Updated.Add(Redraw);
    }

    void Redraw()
    {
        if (GetComponent<RectTransform>() != null)
            transform.localScale = new Vector3(_model.GetEnergyGeneration(), 1, 1);
        // var light = GetComponent<Light>();
        // if (light != null){
        //     light.intensity = 3 * _model.GetEnergyGeneration();
        //     light.range = 3 * _model.GetEnergyGeneration();
        // }
    }

    void OnDestroy()
    {
        _model.Updated.Remove(Redraw);
    }
}
