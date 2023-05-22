using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using FishNet.Connection;
using FishNet.Managing.Scened;

namespace FishNet.Discovery
{  
    public class ClientConnectionHandler : NetworkBehaviour
    {       
        [SerializeField] NetworkManager networkManager;
        [SerializeField] private GameObject joinLobby;
        [SerializeField] private GameObject startServer;

        void Start() {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public override void OnStartServer() {
            base.OnStartServer();

            // Hide the Join Lobby button
            joinLobby.SetActive(false);
            // Set start server button text to green
            startServer.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = Color.green;
            
            if (networkManager == null) {
                Debug.LogError("NetworkManager not found");
                return;
            }

            networkManager.SceneManager.OnClientLoadedStartScenes += OnClientLoadedIn;
        }

        void OnClientLoadedIn(NetworkConnection conn, bool asServer) {
            Debug.Log("Client connected");

            // Load the game scene for the client
            // SceneLoadData scene = new SceneLoadData("Game");
            // base.SceneManager.LoadConnectionScenes(conn, scene);      
        }
    }
}
