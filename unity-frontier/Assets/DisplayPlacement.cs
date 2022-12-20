using System.Collections;
using System.Collections.Generic;
using Avatar;
using CityBuilding;
using DefaultNamespace.Model;
using UnityEngine;
using UnityEngine.AI;
using Utils.Injection;

public class DisplayPlacement : InjectableBehaviour
{
    [Inject] private InteractionModel _interaction;
    [Inject] private BuilderModel _model;
    [Inject] private AccountModel _account;

    public GameObject gridElement;

    private GameObject _preview;

    public const float CellSize = 2.5f;

    public Material cell;
    public Material blocked;
    public Material available;
    private GameObject[,] _cells;

    private IEnumerator Start()
    {
        _interaction.Updated.Add(OnModeUpdated);

        yield return new WaitForSeconds(1);
    }

    private void OnModeUpdated()
    {
        if (_interaction.Get() == InteractionState.Building)
        {
            if (_cells == null)
            {
                _cells = new GameObject[_model.OccupiedData.GetLength(0), _model.OccupiedData.GetLength(1)];
            }

            for (var i = 0; i < _model.OccupiedData.GetLength(0); i++)
            for (var j = 0; j < _model.OccupiedData.GetLength(1); j++)
            {
                if (_cells[i, j] == null)
                {
                    var obj = Instantiate(gridElement, transform, true);
                    obj.transform.localPosition = new Vector3((i + 0.5f) * CellSize, 0.5f, (j + 0.5f) * CellSize);
                    obj.transform.localScale *= CellSize;
                    _cells[i, j] = obj;
                }
                else
                    _cells[i, j].SetActive(true);
            }

            _preview = Instantiate(Resources.Load<GameObject>(_model.Current.prefab), transform);
            // _preview.gameObject.GetComponentInChildren<NavMeshObstacle>().enabled = false;
            foreach (var mesh in _preview.GetComponentsInChildren<MeshRenderer>())
                mesh.material = cell;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_model.Current != null && _interaction.Get() == InteractionState.Building)
        {
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (transform.GetComponent<Collider>().bounds.IntersectRay(mouseRay, out var distance))
            {
                var intersectionPoint = mouseRay.origin + mouseRay.direction * distance;
                var localPoint = transform.InverseTransformPoint(intersectionPoint);
                var snappedPosition = localPoint.Snap(CellSize);


                var state = available;
                var cellPos = snappedPosition / CellSize;

                List<GameObject> cells = new List<GameObject>();
                
                for (var i = (int)cellPos.x - _model.Current.width / 2;
                     i < (int)cellPos.x + _model.Current.width / 2;
                     i++)
                for (var j = (int)cellPos.z - _model.Current.height / 2;
                     j < (int)cellPos.z + _model.Current.height / 2;
                     j++)
                {
                    
                    if (i < 0 || i >= _model.OccupiedData.GetLength(0) || j < 0 ||
                        j >= _model.OccupiedData.GetLength(1))
                    {
                        state = blocked;
                        break;
                    }
                    cells.Add(_cells[i, j]);

                    if (_model.OccupiedData[i, j] != 0)
                    {
                        state = blocked;
                        break;
                    }

                }


                if (Input.GetMouseButtonDown(0) && state == available)
                {
                    PutBuilding(snappedPosition);
                    return;
                }

                for (var i = 0; i < _cells.GetLength(0); i++)
                for (var j = 0; j < _cells.GetLength(1); j++)
                {
                    _cells[i, j].GetComponent<MeshRenderer>().material =
                        cells.Contains(_cells[i, j])
                            ? state
                            : _model.OccupiedData[i, j] == 0
                                ? cell
                                : blocked;
                }

                _preview.transform.localPosition = snappedPosition;
            }
        }
    }

    void PutBuilding(Vector3 snappedPosition)
    {
        var cellPos = snappedPosition / CellSize;
        _model.PutBuilding(_account.Id, (int)cellPos.x, (int)cellPos.z);

        var obj = Instantiate(Resources.Load<GameObject>(_model.Current.prefab), transform);
        obj.transform.localPosition = snappedPosition;

        for (var i = 0; i < _cells.GetLength(0); i++)
        for (var j = 0; j < _cells.GetLength(1); j++)
        {
            _cells[i, j].SetActive(false);
        }

        _interaction.Set(InteractionState.Walking);
    }


    private void OnDestroy()
    {
        _interaction.Updated.Remove(OnModeUpdated);
    }
}