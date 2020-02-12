using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class DelayStartWaitingRoomController : MonoBehaviourPunCallbacks
{
    /*this object must be attached to an object
    / in the waiting room for scene of your project.*/

    // Photon view for sending rpc that updates the timer
    private PhotonView myPhotonView;

    //scene navigation indexes
    [SerializeField] private int multiplayerSceneIndex;
    [SerializeField] private int menusSceneIndex;
    //number of players in the room out of the total room size
    private int playerCount;
    private int roomSize;
    [SerializeField] private int minPlayerToStart;

    // text variables for the holding the displays for the countdown timer and player count
    [SerializeField] private Text roomCountDisplay;
    [SerializeField] private Text timerToStartDisplay;

    //bool values for if the timer can count down
    private bool readyToCountDown;
    private bool readyToStart;
    private bool startingGame;
    //countdown timer reset variables
    private float timerToStartgame;
    private float notFullGameTimer;
    private float fullGameTimer;
    //countdown timer reset variables
    [SerializeField] private float maxWaitTime;
    [SerializeField] private float maxFullGameWaitTime;

    private void Start()
    {
        //intialize variables
        myPhotonView = GetComponent<PhotonView>();
        fullGameTimer = maxFullGameWaitTime;
        timerToStartgame = maxWaitTime;

        PlayerCountUpdate();
    }

    void PlayerCountUpdate()
    {
        //updates player count when players join the room
        //displays player count
        //triggers countdown timer
        playerCount = PhotonNetwork.PlayerList.Length;
        roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
        roomCountDisplay.text = playerCount + ":" + roomSize;

        if (playerCount == roomSize)
        {
            readyToStart = true;
        }
        else if (playerCount >= minPlayerToStart)
        {
            readyToCountDown = true;
        }
        else
        {
            readyToCountDown = false;
            readyToStart = true;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //called whenever a new player joins the room
        PlayerCountUpdate();
        //send master client countdown timer to all other players in order to sync time
        if (PhotonNetwork.IsMasterClient)
        {
            myPhotonView.RPC("RPC_SendTimer", RpcTarget.Others, timerToStartgame);
        }
    }

    [PunRPC]
   private void RPC_SendTimer(float timeIn)
    {
        //RPC for syncing the countdown timer to those that join after it has started the countdown
        timerToStartgame = timeIn;
        notFullGameTimer = timeIn;
        if(timeIn < fullGameTimer)
        {
            fullGameTimer = timeIn;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //called whenever a player leaves the room
        PlayerCountUpdate();
    }

    private void Update()
    {
        WaitingForMorePlayers();
    }

    private void WaitingForMorePlayers()
    {
        //If there is only one player in room the timer will stop and rest
        if(playerCount <= 1)
        {
            ResetTimer();
        }
        //when there is enough players in the room the start timer will begin countdown
        if (readyToStart)
        {
            fullGameTimer -= Time.deltaTime;
            timerToStartgame = fullGameTimer;
        }
        else if(readyToCountDown)
        {
            notFullGameTimer -= Time.deltaTime;
            timerToStartgame = notFullGameTimer;
        }

        //format and display countdown timer
        string tempTimer = string.Format("{0:00}", timerToStartgame);
        timerToStartDisplay.text = tempTimer;
        //if the countdown timer reaches 0 the game will then start
        if(timerToStartgame <= 0f)
        {
            if (startingGame)
                return;
            StartGame();
        }
    }

    private void StartGame()
    {
        //Multiplayer scene is loaded to start the game
        startingGame = true;
        if (!PhotonNetwork.IsMasterClient)
            return;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(multiplayerSceneIndex);
    }

    private void ResetTimer()
    {
        //resets the count down timer
        timerToStartgame = maxWaitTime;
        notFullGameTimer = maxWaitTime;
        fullGameTimer = maxFullGameWaitTime;
    }

    public void DelayCancel()
    {
        //public function paired to cancel button in waiting room scene
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(menusSceneIndex);
    }
}
