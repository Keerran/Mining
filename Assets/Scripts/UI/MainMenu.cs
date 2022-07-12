using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Managing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject initialScreen;

    [SerializeField]
    private TMP_InputField _ipField;
    [SerializeField]
    private Button _backButton;

    private NetworkManager _networkManager;
    private EventSystem _eventSystem;
    private GameObject _openScreen;
    private Stack<(GameObject, GameObject)> _history;

    void Awake()
    {
        _networkManager = FindObjectOfType<NetworkManager>();
        _eventSystem = FindObjectOfType<EventSystem>();
        _history = new Stack<(GameObject, GameObject)>();

        foreach(Transform child in transform)
            if(child != _backButton.transform)
                child.gameObject.SetActive(false);

        OpenScreen(initialScreen);


        _ipField.onSubmit.AddListener(JoinGame);
    }

    public void HostGame()
    {
        if(_networkManager.ServerManager.StartConnection())
        {
            SceneManager.LoadScene("SampleScene");
            _networkManager.ClientManager.StartConnection();
        }
    }

    public void OpenScreen(GameObject screen)
    {
        OpenScreen(screen, true);
    }

    public void OpenScreen(GameObject screen, bool pushHistory)
    {
        if(_openScreen != null)
        {
            _openScreen.SetActive(false);
            if(pushHistory)
                _history.Push((_openScreen, _eventSystem.currentSelectedGameObject));
        }
        
        screen.SetActive(true);
        _openScreen = screen;

        _backButton.enabled = _history.Count > 0;
        _eventSystem.SetSelectedGameObject(screen.transform.GetChild(0).gameObject);
    }

    public void Back()
    {
        if(_history.TryPop(out var result))
        {
            var (screen, focus) = result;
            OpenScreen(screen, false);
            _eventSystem.SetSelectedGameObject(focus);
        }
    }

    public void JoinGame()
    {
        JoinGame(_ipField.text);
    }

    private void JoinGame(string address)
    {
        SceneManager.LoadScene("SampleScene");
        _networkManager.ClientManager.StartConnection(address);
    }
}
