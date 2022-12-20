using UnityEngine;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Collider))]
public class HandleSelection : MonoBehaviour
{
    private Outline _outline;
    private Collider _selectionCollider;
    private Camera _camera;

    void Start()
    {
        _outline = GetComponent<Outline>();
        _camera = Camera.main;

        _selectionCollider = GetComponent<Collider>();
    }

    void Update()
    {
        var mouseRay = _camera.ScreenPointToRay(Input.mousePosition);
        if (_selectionCollider != null)
            _outline.enabled = _selectionCollider.bounds.IntersectRay(mouseRay, out _);
    }
}