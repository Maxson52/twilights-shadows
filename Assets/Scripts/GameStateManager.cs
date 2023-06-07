using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class GameStateManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject keyPrefab;

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
    private float timeRemainingUntilDisconnect = 5f;
    [SyncVar]
    public string timerText = "Loading...";
    [SerializeField]
    private TextMeshProUGUI timerTextMesh;
    [SyncVar]
    public bool gameOn = false;
    public string winText = "";

    [SyncVar]
    private int keysTotal = 0;
    [SyncVar]
    public int keysCollected = 0;

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (!IsServer)
        {
            enabled = false;
        }
    }

    void Start()
    {
        timeRemaining = timeToStart;

        // change BG music to lobby music
        GameObject.Find("BG Music").GetComponent<AudioSource>().clip = lobbyMusic;
        GameObject.Find("BG Music").GetComponent<AudioSource>().Play();
    }

    void Update()
    {       
        timerTextMesh.text = timerText;

        if (gameOn) {
            if (timeRemaining > 0)
            {
                bool hiderWin = HiderWin();
                if (hiderWin) {
                    ServerHiderWin();
                }

                int requiredKeys = keysTotal / 2;
                
                timeRemaining -= Time.deltaTime;

                if (keysCollected >= requiredKeys)
                {
                    timerText = "Time remaining: " + Mathf.Round(timeRemaining) + "\nAll keys collected, report to the building.";
                } else {
                    timerText = "Time remaining: " + Mathf.Round(timeRemaining) + "\nKeys collected: " + keysCollected + "/" + requiredKeys;
                }
            }
            else
            {
                winText = "Time's up! Seekers win.";
                gameOn = false;
            }
        }
        else {
            if (winText != "") {
                timerText = winText;
                timeRemainingUntilDisconnect -= Time.deltaTime;
                if (timeRemainingUntilDisconnect <= 0) {
                    Debug.Log("Disconnecting...");
                    Disconnect();
                }

            } else {
                // Only start if there is at least one object with tag Player and one Seeker
                if (GameObject.FindGameObjectsWithTag("Player").Length > 0)
                {
                    if (timeRemaining > 0)
                    {
                        timeRemaining -= Time.deltaTime;
                        timerText = "Starting in: " + Mathf.Round(timeRemaining);
                    }
                    else
                    {
                        timerText = "Starting now...";
                        StartGame();
                        gameOn = true;
                    }
                } else {
                    timerText = "Waiting for players...";
                }
            }
        }
    }

    void StartGame() 
    {
        timeRemaining = timeToPlay;

        keysTotal = GameObject.FindGameObjectsWithTag("Player").Length * 10;
        keysCollected = 0;

        // change BG music to clockmusic
        GameObject.Find("BG Music").GetComponent<AudioSource>().clip = gameMusic;
        GameObject.Find("BG Music").GetComponent<AudioSource>().Play();
        
        // Spawn keys
        for (int i = 0; i < keysTotal; i++)
        {
            GameObject key = Instantiate(keyPrefab);
            key.transform.position = new Vector3(Random.Range(-376, 455), 12, Random.Range(-162, 286));
            GameObject.Find("Compass").GetComponent<CompassHandler>().AddKeyMarker(key.GetComponent<KeyMarker>());

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
    }

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

    [ObserversRpc]
    public void Disconnect() {
        GameObject.Find("Main Camera").transform.parent = null;
        ClientManager.StopConnection();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
    }

    [ServerRpc(RequireOwnership = false)]
    public void KeyCollected() {
        keysCollected++;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlaceSeeker(GameObject seeker)
    {
        Debug.Log("Placing seeker");
        seeker.GetComponent<CharacterController>().enabled = false;
        seeker.transform.position = new Vector3(Random.Range(-290, 470), 12, Random.Range(30, 120));
        seeker.GetComponent<CharacterController>().enabled = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerTurnOffFlashlight() {
        gameOn = false;
        winText = "Seekers win!";
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerHiderWin() {
        gameOn = false;
        winText = "Hiders win!";
    }
}
