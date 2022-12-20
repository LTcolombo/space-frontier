using Avatar;
using UnityEngine;
using UnityEngine.UI;
using Utils.Injection;

[RequireComponent(typeof(Text))]
public class DisplayDime : InjectableBehaviour
{
    [Inject] private FTModel _model;
    private Text _text;

    void Start()
    {
        _text = GetComponent<Text>();
        _model.Updated.Add(OnUpdated);
        _model.Update();
        OnUpdated();
    }

    private void OnUpdated()
    {
        _text.text = _model.Get().ToString();
    }

    private void OnDestroy()
    {
        _model.Updated.Remove(OnUpdated);
    }
}
