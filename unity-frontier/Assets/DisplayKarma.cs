using System;
using Avatar;
using UnityEngine;
using UnityEngine.UI;
using Utils.Injection;

[RequireComponent(typeof(Text))]
public class DisplayKarma : InjectableBehaviour
{
    [Inject] private AvatarModel _model;
    private Text _text;

    void Start()
    {
        _text = GetComponent<Text>();
        _model.Updated.Add(OnUpdated);
        OnUpdated();
    }

    private void OnUpdated()
    {
        _text.text = _model.GetAttributeById("karma");
    }

    private void OnDestroy()
    {
        _model.Updated.Remove(OnUpdated);
    }
}
