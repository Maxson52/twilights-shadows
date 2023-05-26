using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class PlayerSelection : NetworkBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private List<GameObject> playerPrefabs = new List<GameObject>();

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
    }

    public void SpawnHider() {
        Debug.Log("SpawnHider");
        canvas.SetActive(false);
        Spawn(0, LocalConnection);
    }

    public void SpawnSeeker() {
        Debug.Log("SpawnSeeker");
        canvas.SetActive(false);
        Spawn(1, LocalConnection);
    }

    [ServerRpc(RequireOwnership = false)]
    void Spawn(int index, NetworkConnection conn) {
        GameObject player = Instantiate(playerPrefabs[index], Vector3.zero, Quaternion.identity);
        Spawn(player, conn);
    }
}
