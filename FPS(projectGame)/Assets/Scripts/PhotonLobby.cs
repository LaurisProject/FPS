using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    // Static variable with same datatype as the class that is gone be used as singleton to represent the single instance of the class
    public static PhotonLobby lobby;

    //  Button to allow players to search available rooms to connect with other players
    public GameObject battleButton;

    // To Cancel the match search.
    public GameObject cancelButton;

    // This function initialize the singleton
    private void Awake()
    {
     
        lobby = this; //creates the singleton as instance of the class, lives with in the Main menu screen.
    }

    // Start is called before the first frame update also to allow setup connection between players and the photon servers
    void Start()
    {
     
        PhotonNetwork.ConnectUsingSettings(); //Connects to Master photon server.
    }

    //  Gives feedback whether connection to master photon servers were successful or not.
    public override void OnConnectedToMaster()
    {

        Debug.Log("Player has connected to the Photon master server");
        // When master client load the scene , all other clients that connected to master client would load that scene
        PhotonNetwork.AutomaticallySyncScene = true;
        // Make battle button visible to user if connection is successful.
        battleButton.SetActive(true);
    }

    public void OnBattleButtonClick()
    {
        Debug.Log("Battle Button was clicked");
        battleButton.SetActive(false);
        cancelButton.SetActive(true);

        // Special function that looks at all available rooms and pick it at random.
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to join a random game but failed. There must be no open games available");

        // Sometimes joining room fails because there is no room. So we create new one.
        CreateRoom();
    }

    void CreateRoom()
    {
        Debug.Log("Trying to create a new room");
        int randomRoomName = Random.Range(0, 1000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte) MultiPlayerSettings.multiplayerSetting.maxPlayers };
        // Passing variables as failsafe to stop creating duplicate rooms
        PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);
    }

  

    // If room with same name exists this would be called
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to create a new room but failed, there must already be aroom with the same name");
        CreateRoom();
    }

    public void OnCancelButtonClicked()
    {
        Debug.Log("Cancel Button was clicked");
        cancelButton.SetActive(false);
        battleButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }
    
}
