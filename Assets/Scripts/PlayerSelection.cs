using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet.Object;
using FishNet.Connection;

public class PlayerSelection : NetworkBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private List<GameObject> playerPrefabs = new List<GameObject>();

    [SerializeField] private TextMeshProUGUI keyMaster;
    [SerializeField] private TextMeshProUGUI hunter;

    public override void OnStartClient() {
        base.OnStartClient();

        if (!base.IsOwner) {
            gameObject.SetActive(false);
        }
    }

    void Start() {
        // show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update() {
        // show cursor if canvas is active
        if (canvas.activeSelf) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (GameObject.Find("GameStateManager").GetComponent<GameStateManager>().gameOn) {
            // show Error and disabled the Buttons
            canvas.transform.Find("Error").gameObject.SetActive(true);
            canvas.transform.Find("Hider").gameObject.SetActive(false);
            canvas.transform.Find("Seeker").gameObject.SetActive(false);
        }

        // get number of Players
        int numberOfPlayers = GameObject.FindGameObjectsWithTag("Player").Length;
        int numberOfSeekers = GameObject.FindGameObjectsWithTag("Seeker").Length;

        // make buttons include number of players
        keyMaster.text = "Key Master (" + numberOfPlayers + ")";
        hunter.text = "Hunter (" + numberOfSeekers + ")";
    }

    public void SpawnHider() {
        canvas.SetActive(false);
        Spawn(0, LocalConnection);
    }

    public void SpawnSeeker() {
        canvas.SetActive(false);
        Spawn(1, LocalConnection);
    }

    [ServerRpc(RequireOwnership = false)]
    void Spawn(int index, NetworkConnection conn) {
        GameObject player = Instantiate(playerPrefabs[index], Vector3.zero, Quaternion.identity);
        Spawn(player, conn);
    }
}
