using DefaultNamespace.Model;
using UnityEngine;
using Utils.Injection;

public class Follow : InjectableBehaviour
{
    [Inject] private InteractionModel _interaction;

    public Transform target;
    public Vector3 offset;

    public float cameraSize = 8f;

    void Update()
    {
        if (_interaction.Get() == InteractionState.Dialog)
            return;

        var targetSize = _interaction.Get() == InteractionState.Building ? 20 : cameraSize;

        var currentTargetPosition = _interaction.Get() == InteractionState.Building 
            ? new Vector3(-20, 0,-25) 
            : target.position;
        
        var targetPoint = currentTargetPosition + offset;
        transform.position += (targetPoint - transform.position) * Time.deltaTime;
        transform.LookAt(currentTargetPosition, Vector3.up);
        GetComponent<Camera>().orthographicSize +=
            (targetSize - GetComponent<Camera>().orthographicSize) * Time.deltaTime * 4;
        ;
    }
}