using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour
{
    public PhotonView PV; // This was assigned manually to fix null reference. Should probably be assigned in the Awake method.
    public GameObject myAvatar;
    // The team the player belongs to.
    public int myTeam;

    void Start()
    {
        PV = GetComponent<PhotonView>();

        if (PV.IsMine)
        {
            // If this is the local player, send the RPC_GetTeam to the master client only.
            PV.RPC("RPC_GetTeam", RpcTarget.MasterClient);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If the myAvatar is null, it means we haven't yet spawn an avatar into the scene.
        if (myAvatar == null && myTeam != 0)
        {
            if (myTeam == 1)
            {
                int spawnPicker = Random.Range(0, GameSetup.GS.spawnPointsTeamOne.Length); // this goes to the game setup script and pulls the array of spawn points
                if (PV.IsMine)
                {
                    myAvatar = PhotonNetwork.Instantiate
                         (Path.Combine("PhotonPrefabs", "PlayerAvatar"),
                         GameSetup.GS.spawnPointsTeamOne[spawnPicker].position,
                         GameSetup.GS.spawnPointsTeamOne[spawnPicker].rotation, 0);
                }
            }
            else
            {
                int spawnPicker = Random.Range(0, GameSetup.GS.spawnPointsTeamTwo.Length); // this goes to the game setup script and pulls the array of spawn points
                if (PV.IsMine)
                {
                    myAvatar = PhotonNetwork.Instantiate
                         (Path.Combine("PhotonPrefabs", "PlayerAvatar"),
                         GameSetup.GS.spawnPointsTeamTwo[spawnPicker].position,
                         GameSetup.GS.spawnPointsTeamTwo[spawnPicker].rotation, 0);
                }
            }
        }
    }

    // This RPC is only called on the master client.
    [PunRPC]
    void RPC_GetTeam()
    {
        // When the local player sends this method to the master client, the master client then sends back to this local player the its team value.
        myTeam = GameSetup.GS.nextPlayersTeam;
        // Here is updating the nextPlayerTeam value, so when the next player joins, it will have a different team value.
        GameSetup.GS.UpdateTeam();
        // Syncronise every player with its team value.
        PV.RPC("RPC_SentTeam", RpcTarget.OthersBuffered, myTeam);
    }

    // This method will be called on every client.
    [PunRPC]
    void RPC_SentTeam(int whichTeam)
    {
        myTeam = whichTeam;
    }
}
