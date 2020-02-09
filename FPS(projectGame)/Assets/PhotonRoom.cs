using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public static PhotonRoom room;
    private PhotonView PV;
    
   // public bool isGameLoaded;
    public int currentScene;
    public int multiplayScene;

    //player info
    //Player[] photonPlayers;
  //  public int playersInRoom;
    //public int myNumberInRoom;

   // public int playersInGame;

    //delayed start
    // private bool readyToCount;
    // private bool readyToStart;
    // public float startingTime;
    private void Awake()
    {
        //Sets up singleton statement 
        if(PhotonRoom.room == null) // if there is no PhotonRoom, this sets up a room
        {
            PhotonRoom.room = this;
        }
        else
        {
            if(PhotonRoom.room != this)// if there is a room already created, destroy this room.
            {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void OnEnable()
    {
        //subscribe to functions
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
        //PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    public void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        //readyToCount = false;
        //readyToStart = false;
       // lessThanMaxplayers = startingTime;
        //atMaxPlayers = 6;
       // timesToStart = startingTime;
    }
    // Update is called once per frame
    void Update()
    {

    }


    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;
        if(currentScene == multiplayScene)
        {
            //isGameLoaded = true;
            //for delay start game
            // if (MultiplayerSetting.multiplayerSetting.delayStart)
            //{
            //   PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
            //}
            //for non delaye start game
            //else
            {
                CreatePlayer();
            }
        }

    }

    private void CreatePlayer()
    {
        //creates player network controlley but not player character
        PhotonNetwork.Instantiate(Path.Combine("PhotonPreFabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity, 0);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We are now in a room");
        //photonPlayers = PhotonNetwork.PlayerList;
        //playersInRoom = photonPlayers.Length;
        //myNumberInRoom = playersInRoom;
        //PhotonNetwork.NickName = myNumberInRoom.ToString();
        StartGame();
    }

    private void StartGame()
    {
        //loads the multiplayer scene for all playhers
        //isGameLoaded = true;
        if (!PhotonNetwork.IsMasterClient) { return; }
        // if(MultiplayerSetting.multiplayerSetting.delayStart)
        // {
        //      PhotonNetwork.CurrentRoom.IsOpen = false;
        // }
        PhotonNetwork.LoadLevel(multiplayScene);

    }
}
