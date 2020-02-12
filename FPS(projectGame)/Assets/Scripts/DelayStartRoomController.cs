using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class DelayStartRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] private int waitingRoomSceneIndex;

    public override void OnEnable()
    {
        //register photon callback functions
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        //unregister to photon callback functions
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinedRoom() //callback function for when we successfully create or join a room
    {
        //called when our player joins the room
        // load into waiting room scene
        SceneManager.LoadScene(waitingRoomSceneIndex);
    }
}
