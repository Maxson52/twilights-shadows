using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using FishNet.Object;

namespace FishNet.Discovery
{
    public class onClicks : NetworkBehaviour
    {
        public GameObject StartSearch;

        [SerializeField]
        private NetworkDiscovery networkDiscovery;

        private readonly List<IPEndPoint> _endPoints = new List<IPEndPoint>();

        private void Start()
        {
            // show cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (networkDiscovery == null) networkDiscovery = FindObjectOfType<NetworkDiscovery>();

            networkDiscovery.ServerFoundCallback += (endPoint) =>
            {
                Debug.Log("Server found!");

                string ipAddress = endPoint.Address.ToString();
                InstanceFinder.ClientManager.StartConnection(ipAddress);

                // Stop searching for servers
                networkDiscovery.StopSearchingForServers();
            };
        }

        public void StartServer() {
            InstanceFinder.ServerManager.StartConnection();
            FindObjectOfType<NetworkDiscovery>().StartAdvertisingServer();

            // Enable NetworkDiscoveryHUD
            GameObject.Find("NetworkManager").GetComponent<NetworkDiscoveryHUD>().enabled = true;
        }

        public void StartSearching() {
            if (networkDiscovery.IsSearching) return;
            FindObjectOfType<NetworkDiscovery>().StartSearchingForServers();

            // Set TMP text to Searching... and update the dots every second
            StartSearch.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Searching";
            InvokeRepeating("UpdateDots", 0, 1);
        }

        private void UpdateDots() {
            if (StartSearch.GetComponentInChildren<TMPro.TextMeshProUGUI>().text == "Searching...") {
                StartSearch.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Searching";
            } else {
                StartSearch.GetComponentInChildren<TMPro.TextMeshProUGUI>().text += ".";
            }
        }
    }
}