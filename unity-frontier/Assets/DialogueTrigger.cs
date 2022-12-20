using System;
using Avatar;
using CityBuilding;
using DefaultNamespace.Model;
using Tools.DataAssets.Nodes;
using UnityEngine;
using UnityEngine.AI;
using Utils.Injection;

[RequireComponent(typeof(Collider))]
public class DialogueTrigger : InjectableBehaviour
{
    [Inject] private InteractionModel _interaction;
    [Inject] private QuestModel _quests;

    private DialogueUI _ui;
    private static readonly int Talk = Animator.StringToHash("Talk");
    private DialogueGraph _graph;
    [HideInInspector] public string initiator;

    private void Start()
    {
        var parent = transform.parent;

        initiator = parent.name;

        var quest = _quests.FindQuest(initiator);
        var handleSelection = parent.GetComponent<HandleSelection>();
        var outline = parent.GetComponent<Outline>();
        if (quest != null) return;
        
        if (handleSelection)
            handleSelection.enabled = false;
        if (outline)
            outline.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var quest = _quests.FindQuest(initiator);
        if (quest == null)
            return;

        // _quests.Remove(quest);

        var nav = other.GetComponent<NavMeshAgent>();
        if (nav != null)
            nav.ResetPath();

        other.transform.LookAt(transform.parent);

        if (transform.parent.GetComponent<Animator>() != null)
            transform.parent.LookAt(other.transform);

        _ui = FindObjectOfType<DialogueUI>();

        _ui.answerSelected.RemoveAllListeners();
        _ui.answerSelected.AddListener(OnAnswerSelected);
        _graph = Resources.Load<DialogueGraph>(quest.id);
        _graph.Start();
        _ui.Setup(transform.parent);
        _ui.ShowChat(_graph.current);
        StartDialogue();
    }

    private void StartDialogue()
    {
        if (transform.parent.GetComponent<Animator>() != null)
            transform.parent.GetComponent<Animator>().SetTrigger(Talk);

        _ui.transform.GetChild(0).gameObject.SetActive(true);

        Camera.main.GetComponent<LookAtDialogue>().right = gameObject;

        _interaction.Set(InteractionState.Dialog);
    }

    private void OnAnswerSelected(int index)
    {
        _graph.SubmitAnswer(index);

        if (_graph.current == null)
            ExitDialogue();
        else
        {
            _ui.ShowChat(_graph.current);   
            
            if (transform.parent.GetComponent<Animator>() != null)
                transform.parent.GetComponent<Animator>().SetTrigger(Talk);
        }
    }

    private void ExitDialogue()
    {
        if (_ui != null)
        {
            _ui.answerSelected.RemoveAllListeners();
            _ui.transform.GetChild(0).gameObject.SetActive(false);
        }

        if (_interaction.Get() == InteractionState.Dialog)
            _interaction.Set(InteractionState.Walking);
    }

    public void OnTriggerExit(Collider other)
    {
        //ExitDialogue();
    }
}