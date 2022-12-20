using System.Linq;
using CityBuilding;
using UnityEngine;
using Utils.Injection;

public class DisplayCharacters : InjectableBehaviour
{
    [Inject] private CharacterModel _model;
    [Inject] private BuilderModel _buildings;

    private void Start()
    {
        _model.Updated.Add(OnModelUpdated);
        OnModelUpdated();
    }

    private void OnModelUpdated()
    {
        var i = 0;
        foreach (var character in _model.Characters)
        {
            i++;
            if (i <= transform.childCount)
                continue;

            var characterObj = Instantiate(Resources.Load<GameObject>(character), transform);
            characterObj.transform.localPosition = Vector3.forward * 3;

            var dialogue = characterObj.GetComponentInChildren<DialogueTrigger>();
            dialogue.initiator = character;
        }
    }

    private void OnDestroy()
    {
        _model.Updated.Remove(OnModelUpdated);
    }
}