using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Logging;
using FishNet.Managing.Scened;
using FishNet.Transporting;
using FishNet.Utility;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


public class ClientConnectionHandler : MonoBehaviour
{       
    private NetworkManager _networkManager;

    [SerializeField] private ReplaceOption _replaceScenes = ReplaceOption.All;
    
    [SerializeField] private GameObject joinLobby;
    [SerializeField] private GameObject startServer;
    

    void Start() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Awake() {
        InitializeOnce();
    }

    private void InitializeOnce() {
        _networkManager = GetComponentInParent<NetworkManager>();
        if (_networkManager == null) {
            NetworkManager.StaticLogError($"NetworkManager not found on {gameObject.name} or any parent objects. DefaultScene will not work.");
            return;
        }
        //A NetworkManager won't be initialized if it's being destroyed.
        if (!_networkManager.Initialized) return;

        _networkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        _networkManager.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;
        _networkManager.SceneManager.OnLoadEnd += SceneManager_OnLoadEnd;
        _networkManager.ServerManager.OnAuthenticationResult += ServerManager_OnAuthenticationResult;
        
        LoadOfflineScene();
    }

    private void OnDestroy() {

        if (!ApplicationState.IsQuitting() && _networkManager != null && _networkManager.Initialized)
        {
            _networkManager.ClientManager.OnClientConnectionState -= ClientManager_OnClientConnectionState;
            _networkManager.ServerManager.OnServerConnectionState -= ServerManager_OnServerConnectionState;
            _networkManager.SceneManager.OnLoadEnd -= SceneManager_OnLoadEnd;
            _networkManager.ServerManager.OnAuthenticationResult -= ServerManager_OnAuthenticationResult;
        }
    }

    private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs obj) {
        Debug.Log("Client connected");

        if (obj.ConnectionState == LocalConnectionState.Stopped)
        {
            if (!_networkManager.IsServer)
                LoadOfflineScene();
        } 
    }

    private void ServerManager_OnAuthenticationResult(NetworkConnection arg1, bool authenticated) {
        // This is only for loading connection scenes.
        if (!authenticated) return;

        SceneLoadData sld = new SceneLoadData("Game");
        _networkManager.SceneManager.LoadConnectionScenes(arg1, sld);
    }

    private void ServerManager_OnServerConnectionState(ServerConnectionStateArgs obj) {
        // When server starts load online scene as global. Since this is a global scene clients will automatically join it when connecting.
        if (obj.ConnectionState == LocalConnectionState.Started) {
            if (!_networkManager.ServerManager.OneServerStarted()) return;

            //If here can load scene.
            SceneLoadData sld = new SceneLoadData("Game");
            sld.ReplaceScenes = _replaceScenes;
            _networkManager.SceneManager.LoadConnectionScenes(sld);
        }
        //When server stops load offline scene.
        else if (obj.ConnectionState == LocalConnectionState.Stopped)
        {
            LoadOfflineScene();
        }
    }

    private void SceneManager_OnLoadEnd(SceneLoadEndEventArgs obj) {
        bool onlineLoaded = false;
        foreach (Scene s in obj.LoadedScenes)
        {
            if (s.name == "Game")
            {
                onlineLoaded = true;
                break;
            }
        }

        //If online scene was loaded then unload offline.
        if (onlineLoaded)
            UnloadOfflineScene();
    }

    // Offline scene handling
    private void LoadOfflineScene() {
        if (UnitySceneManager.GetActiveScene().name == "Lobby")
            return;
        UnitySceneManager.LoadScene("Lobby");
    }

    private void UnloadOfflineScene() {
        Scene s = UnitySceneManager.GetSceneByName("Lobby");
        if (string.IsNullOrEmpty(s.name))
            return;

        UnitySceneManager.UnloadSceneAsync(s);
    }
}

