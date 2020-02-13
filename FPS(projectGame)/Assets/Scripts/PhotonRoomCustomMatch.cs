using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using UnityEngine.UI;

public class PhotonRoomCustomMatch : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    // Create singleton of the room
    public static PhotonRoomCustomMatch room;

    // Used for sending messages from one client to other using RPC calls
    private PhotonView PV;

    public bool isGameLoaded;
    public int currentScene;


    // Player info
    private Player[] photonPlayers;
    public int playersInRoom;
    public int myNumberInRoom;
    public int playersInGame;

    public Vector3 spawnPosition;

    // Delayed start
    private bool readyToCount;
    private bool readyToStart;
    public float startingTime;
    private float lessThanMaxPlayers;
    private float atMaxPlayer;
    private float timeToStart;

    public GameObject lobbyGO;
    public GameObject roomGO;
    public Transform playersPanel;
    public GameObject playerListingPrefab;
    public GameObject startButton;

    // Create our singleton
    private void Awake()
    {
        //Sets up singleton statement 
        if (PhotonRoomCustomMatch.room == null) // if there is no PhotonRoom, this sets up a room
        {
            PhotonRoomCustomMatch.room = this;
        }
        else
        {
            if (PhotonRoomCustomMatch.room != this)// if there is a room already created, destroy this room and replace singleton with new instance.
            {
                Destroy(PhotonRoomCustomMatch.room.gameObject);
                PhotonRoomCustomMatch.room = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // OnEnable and OnDisable created because since we inheriting from other classes we need to override those

    public override void OnEnable()
    {
        // Subscribe to functions
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;

    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        readyToCount = false;
        readyToStart = false;
        lessThanMaxPlayers = startingTime;
        atMaxPlayer = 6;
        timeToStart = startingTime;

    }
    // Update is called once per frame related to delayed start
    void Update()
    {
        if (MultiPlayerSettings.multiplayerSetting.delayStart)
        {
            if (playersInRoom == 1)
            {
                RestartTimer();
            }
            if (!isGameLoaded)
            {
                if (readyToStart)
                {
                    atMaxPlayer -= Time.deltaTime;
                    lessThanMaxPlayers = atMaxPlayer;
                    timeToStart = atMaxPlayer;
                }
                else if (readyToCount)
                {
                    lessThanMaxPlayers -= Time.deltaTime;
                    timeToStart = lessThanMaxPlayers;
                }
                Debug.Log("Display time to start to the players " + timeToStart);
                if (timeToStart <= 0)
                {
                    StartGame();
                }
            }
        }

    }

    void ListPlayers()
    {
        if (PhotonNetwork.InRoom)
        {
            foreach(Player player in PhotonNetwork.PlayerList)
            {
                GameObject tempListing = Instantiate(playerListingPrefab, playersPanel); //creates a new player listing for each player
                Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
                tempText.text = player.NickName;
            }
        }
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We are now in a room");
        // Get list of the players

        lobbyGO.SetActive(false);
        roomGO.SetActive(true);
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        ClearPlayerListings();
        ListPlayers();

        photonPlayers = PhotonNetwork.PlayerList;
        // Save the list of the players including ourselves
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;

        // Rename player in photon server
        PhotonNetwork.NickName = myNumberInRoom.ToString();
        if (MultiPlayerSettings.multiplayerSetting.delayStart)
        {
            // Display list of players in room within max number of players room can hold. To be edited later.
            Debug.Log("Displayer players in room out of max players possible (" + playersInRoom + ":" + MultiPlayerSettings.multiplayerSetting.maxPlayers + ")");
            if (playersInRoom > 1)
            {
                readyToCount = true;
            }
            // Whether our room is full
            if (playersInRoom == MultiPlayerSettings.multiplayerSetting.maxPlayers)
            {
                // If room is full
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                    return;
                // If we are the master client close the room so no one else can join until we re-open it.
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
       /* else
        {
            StartGame();
        }*/
    }

    void ClearPlayerListings()
    {
        for(int i = playersPanel.childCount -1; i >= 0; i--)
        {
            Destroy(playersPanel.GetChild(i).gameObject);
            //this starts at the bottom of panel and removes each child 
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("A new player has joined the room");
        ClearPlayerListings();
        ListPlayers();
        photonPlayers = PhotonNetwork.PlayerList;
        // Number of players increased when new player joined
        playersInRoom++;
        // When we have delayed start
        if (MultiPlayerSettings.multiplayerSetting.delayStart)
        {
            Debug.Log("Displayer players in room out of max players possible (" + playersInRoom + ":" + MultiPlayerSettings.multiplayerSetting.maxPlayers + ")");
            if (playersInRoom > 1)
            {
                // If there is more than one player start the counter
                readyToCount = true;
            }
            // Check if room is full
            if (playersInRoom == MultiPlayerSettings.multiplayerSetting.maxPlayers)
            {
                readyToStart = true;
                // If we are not the master client
                if (!PhotonNetwork.IsMasterClient)
                    return;
                // If we are hosting the game then close the room
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }

    }

    public void StartGame()
    {
        isGameLoaded = true;
        // Master host check
        if (!PhotonNetwork.IsMasterClient)
            return;
        // If it is master host keep going
        if (MultiPlayerSettings.multiplayerSetting.delayStart)
        {
            // Close the room once game begins
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        PhotonNetwork.LoadLevel(MultiPlayerSettings.multiplayerSetting.multiplayerScene);
    }

    void RestartTimer()
    {
        lessThanMaxPlayers = startingTime;
        timeToStart = startingTime;
        atMaxPlayer = 6;
        readyToCount = false;
        readyToStart = false;
    }


    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;
        if (currentScene == MultiPlayerSettings.multiplayerSetting.multiplayerScene)
        {
            isGameLoaded = true;
            //for delay start game
            if (MultiPlayerSettings.multiplayerSetting.delayStart)
            {
                // Send RPC Command to master client which is host
                PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
            }
            //for non delayed start game
            else
            {
                RPC_CreatePlayer();
            }
        }

    }

    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        // Increment number of players in game if RPC command loaded the game scene
        playersInGame++;
        // To make sure we are not creating duplicate player objects
        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            PV.RPC("RPC_CreatePlayer", RpcTarget.All);
        }

    }

    [PunRPC]
    private void RPC_CreatePlayer()
    {
        // Creates player network controller but not player character
        // Instantiate player prefab across the network
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), spawnPosition, Quaternion.identity, 0);
    }

    // This method is called when a player leaves the room. Host migration is handled by Photon plugin itself.
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        // When a player leaves the room, we can get access to that player and use its information for various other operations.
        Debug.Log(otherPlayer.NickName + " has left the game");
        playersInRoom--;
        ClearPlayerListings();
        ListPlayers();
    }

}

