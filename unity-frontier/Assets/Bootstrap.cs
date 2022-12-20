using System.Runtime.InteropServices;
using Avatar;
using CityBuilding;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils.Injection;

public class Bootstrap : InjectableBehaviour
{
    [SerializeField] private Text label;

    [Inject] private BuilderModel _builder;
    [Inject] private CharacterModel _characters;
    [Inject] private AvatarModel _avatar;
    [Inject] private AccountModel _account;
    [Inject] private QuestModel _quests;


    [DllImport("__Internal")]
    private static extern void SignIn();

    private void Start()
    {
#if false//UNITY_WEBGL && !UNITY_EDITOR
        SignIn();
#else
        HandleWalletId("frontiergame.testnet");
#endif
    }


    public void HandleWalletId(string value)
    {
        _account.Id = value;
        label.text = "Authenticated as " + value;
        LoadServices();
    }
    public async void LoadServices()
    {
        
        await _builder.Load(_account.Id);
        label.text = "Build data loaded..";
        
        await _avatar.Load();
        label.text = "Avatar data loaded..";

        await _quests.Load();
        label.text = "Quests loaded..";

        SceneManager.LoadScene("MainArea");
    }
}