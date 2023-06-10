using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class GameStateManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject keyPrefab;
    [SerializeField]
    private GameObject powerUpPrefab;

    [Header("Audio")]
    public AudioClip lobbyMusic;
    public AudioClip gameMusic;

    [Header("Timing")]
    [SerializeField]
    private float timeToStart = 30f;
    [SerializeField]
    private float timeToPlay = 200f;

    [SyncVar]
    private float timeRemaining = 0;
    [SyncVar]
    private float timeRemainingUntilDisconnect = 5f;
    [SyncVar]
    public string timerText = "Loading...";
    [SerializeField]
    private TextMeshProUGUI timerTextMesh;
    [SyncVar]
    public bool gameOn = false;
    [SyncVar]
    public string winText = "";

    [SyncVar]
    private int keysTotal = 0;
    [SyncVar]
    public int keysCollected = 0;

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (base.IsServer)
        {
            timeRemaining = timeToStart;
        } else {
            enabled = false;
            Destroy(gameObject);
        }
    }

    void Start()
    {

        // change BG music to lobby music
        GameObject.Find("BG Music").GetComponent<AudioSource>().clip = lobbyMusic;
        GameObject.Find("BG Music").GetComponent<AudioSource>().Play();
    }

    void Update()
    {       
        timerTextMesh.text = timerText;

        if (gameOn) {
            if (timeRemaining > 0) {
                bool hiderWin = HiderWin();
                if (hiderWin) {
                    Winner("Hiders win!");
                }

                int requiredKeys = keysTotal / 2;
                
                ChangeTime(-Time.deltaTime);

                if (keysCollected >= requiredKeys)
                {
                    SetTimerText("Time remaining: " + Mathf.Round(timeRemaining) + "\nAll keys collected, report to the building.");
                } else {
                    SetTimerText("Time remaining: " + Mathf.Round(timeRemaining) + "\nKeys collected: " + keysCollected + "/" + requiredKeys);
                }
            }
            else {
                IsTimeUp();
            }
        }
        else {
            if (winText != "") {
                SetTimerText(winText);
                timeRemainingUntilDisconnect -= Time.deltaTime;
                if (timeRemainingUntilDisconnect <= 0) {
                    Debug.Log("Disconnecting...");
                    Disconnect();
                }

            } else {
                // Only start if there is at least one object with tag Player and one Seeker
                if (GameObject.FindGameObjectsWithTag("Player").Length > 0 || GameObject.FindGameObjectsWithTag("Seeker").Length > 0)
                {
                    if (timeRemaining > 0)
                    {
                        ChangeTime(-Time.deltaTime);
                        SetTimerText("Starting in: " + Mathf.Round(timeRemaining));
                    }
                    else
                    {
                        Debug.Log(gameOn);
                        StartGame();
                    }
                } else {
                    SetTimerText("Waiting for players...");
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void StartGame() {
        if (gameOn) return;

        timerText = "Starting game...";
        timeRemaining = timeToPlay;
        gameOn = true;
        keysTotal = GameObject.FindGameObjectsWithTag("Player").Length * 10;
        keysCollected = 0;
        // Spawn keys
        for (int i = 0; i < keysTotal; i++)
        {
            GameObject key = Instantiate(keyPrefab);
            InstanceFinder.ServerManager.Spawn(key, null);
            key.transform.position = new Vector3(Random.Range(-376, 455), 16, Random.Range(-162, 286));
        }

        // Spawn crate powerups
        for (int i = 0; i < 40; i++)
        {
            GameObject crate = Instantiate(powerUpPrefab);
            InstanceFinder.ServerManager.Spawn(crate, null);
            crate.transform.position = new Vector3(Random.Range(-376, 455), 16, Random.Range(-162, 286));
        }

        // Place players randomly between z = 30 and z = 120 (x is -290 and y = 12)
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = new Vector3(-290, 12, Random.Range(30, 120));
            player.GetComponent<CharacterController>().enabled = true;
            // facing the +x direction
            player.transform.rotation = Quaternion.Euler(0, 90, 0);
       }

        // Place seekers randomly between z = 30 and z = 120 (x is 470 and y = 12)
        foreach (GameObject seeker in GameObject.FindGameObjectsWithTag("Seeker"))
        {
            seeker.GetComponent<CharacterController>().enabled = false;
            seeker.transform.position = new Vector3(470, 12, Random.Range(30, 120));
            seeker.GetComponent<CharacterController>().enabled = true;
            // facing the -x direction
            seeker.transform.rotation = Quaternion.Euler(0, -90, 0);
        }

        StartGameRpc();
    }

    [ObserversRpc]
    void StartGameRpc() 
    {
        // change BG music to clockmusic
        GameObject.Find("BG Music").GetComponent<AudioSource>().clip = gameMusic;
        GameObject.Find("BG Music").GetComponent<AudioSource>().Play();

        // Add a compass marker for each player
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            GameObject.Find("Compass").GetComponent<CompassHandler>().AddMarker(player.GetComponent<CompassMarker>());
        }

        // Add a compass marker for each key
        foreach (GameObject key in GameObject.FindGameObjectsWithTag("Key"))
        {
            GameObject.Find("Compass").GetComponent<CompassHandler>().AddMarker(key.GetComponent<CompassMarker>());
        }

        // Add a compass marker for the building
        GameObject building = GameObject.Find("Building");
        GameObject.Find("Compass").GetComponent<CompassHandler>().AddMarker(building.GetComponent<CompassMarker>());
    }

    // Check if hiders win
    bool HiderWin() {
        if (keysCollected >= keysTotal / 2) {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player.transform.position.x > -10.3 && player.transform.position.x < 20.6 && player.transform.position.z > 23 && player.transform.position.z < 63.8) {
                    return true;
                }
            }
        }

        return false;
    }

    // Is time up
    [ServerRpc(RequireOwnership = false)]
    public void IsTimeUp() {
        if (timeRemaining <= 0 && gameOn) {
            Winner("Time's Up. Seekers win!");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void Disconnect() {
        DisconnectRpc();
        // reset all variables
        gameOn = false;
        timeRemaining = timeToStart;
        timeRemainingUntilDisconnect = 5;
        timerText = "";
        winText = "";
        keysTotal = 0;
        keysCollected = 0;
    }
    [ObserversRpc]
    public void DisconnectRpc() {
        GameObject.Find("Main Camera").transform.parent = null;
        ClientManager.StopConnection();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
    }

    // When key is collected
    [ServerRpc(RequireOwnership = false)]
    public void KeyCollected(GameObject key) {
        keysCollected++;
        KeyCollectedRpc(key);
    }
    [ObserversRpc]
    public void KeyCollectedRpc(GameObject key) {        
        // Remove from compass UI
        GameObject.Find("Compass").GetComponent<CompassHandler>().RemoveMarker(key);
        Destroy(key);
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlaceSeeker(GameObject seeker)
    {
        PlaceSeekerRpc(seeker);
        seeker.GetComponent<CharacterController>().enabled = false;
        seeker.transform.position = new Vector3(Random.Range(-290, 470), 16, Random.Range(30, 120));
        seeker.GetComponent<CharacterController>().enabled = true;
    }
    [ObserversRpc]
    public void PlaceSeekerRpc(GameObject seeker)
    {
        Debug.Log("Placing seeker");
    }

    // Set timer text
    [ServerRpc(RequireOwnership = false)]
    public void SetTimerText(string text) {
        timerText = text;
    }

    // Set time
    [ServerRpc(RequireOwnership = false)]
    public void SetTime(float time) {
        timeRemaining = time;
    }

    // Change time
    [ServerRpc(RequireOwnership = false)]
    public void ChangeTime(float time) {
        timeRemaining += time;
    }

    // WINNER
    [ServerRpc(RequireOwnership = false)]
    public void Winner(string winText) {
        Debug.Log("win");
        gameOn = false;
        this.winText = winText;
    }
}
