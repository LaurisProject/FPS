using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonLobbyCustomMatch : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    // Static variable with same datatype as the class that is gone be used as singleton to represent the single instance of the class
    public static PhotonLobbyCustomMatch lobby;

    public string roomName;
    public int roomSize;
    public GameObject roomListingPrefab;
    public Transform roomsPanel;
   

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
        // Rename player in photon server
        PhotonNetwork.NickName = "Player" + UnityEngine.Random.Range(0, 1000);

    }

    // this is called when there is a change availaible the lobby
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        RemoveRoomListings();
        foreach (RoomInfo room in roomList)
        {
            ListRoom(room);
        }
    }

    private void ListRoom(RoomInfo room)
    {
        if(room.IsOpen && room.IsVisible)
        {
            GameObject tempListing = Instantiate(roomListingPrefab, roomsPanel);
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.roomName = room.Name;
            tempButton.roomSize = room.MaxPlayers;
            tempButton.SetRoom();
        }
    }

    private void RemoveRoomListings()
    {
        while(roomsPanel.childCount != 0)
        {
            Destroy(roomsPanel.GetChild(0).gameObject);
        }
    }

    public void CreateRoom()
    {
        Debug.Log("Trying to create a new room");
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte) roomSize };
        // Passing variables as failsafe to stop creating duplicate rooms
        PhotonNetwork.CreateRoom(roomName, roomOps);
    }

    // If room with same name exists this would be called
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to create a new room but failed, there must already be aroom with the same name");
      // this will need to be changed to a text display in the room list to tell the user that the room name needs to be changed
    }

    public void OnRoomNameChanged(string nameIn)
    {
        roomName = nameIn;
    }

    public void OnRoomSizeChanged(string sizeIn)
    {
        roomSize = int.Parse(sizeIn);
    }

    public void JoinLobbyOnClick()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }
}
