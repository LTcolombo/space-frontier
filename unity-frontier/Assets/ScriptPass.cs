using UnityEngine;
using UnityEngine.AI;

public class ScriptPass : MonoBehaviour
{
    public GameObject fire;
    public GameObject npc;

    public void Run()
    {
        fire.SetActive(false);
        npc.SetActive(true);
        npc.GetComponent<NavMeshAgent>().SetDestination(Vector3.zero);
    }
}
