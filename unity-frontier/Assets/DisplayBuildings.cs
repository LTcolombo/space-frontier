using CityBuilding;
using UnityEngine;
using Utils.Injection;

public class DisplayBuildings : InjectableBehaviour
{
    [Inject] private BuilderModel _model;

    private void Start()
    {
        _model.Updated.Add(OnModelUpdated);
        OnModelUpdated();
    }

    private void OnModelUpdated()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        foreach (var building in _model.Buildings)
        {
            Instantiate(Resources.Load<GameObject>(FindPrefab(building.type)), transform).transform.localPosition =
                new Vector3(building.x * DisplayPlacement.CellSize, 0, building.y * DisplayPlacement.CellSize);
        }
    }

    private string FindPrefab(BuildingType type)
    {
        return _model.GetConfig(type).prefab;
    }

    private void OnDestroy()
    {
        _model.Updated.Remove(OnModelUpdated);
    }
}