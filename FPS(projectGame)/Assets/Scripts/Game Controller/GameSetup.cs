using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameSetup : MonoBehaviour
{
    public static GameSetup GS;
    public Text healthDisplay;
    public Transform[] spawnPoints;

    private void OnEnable()
    {
        if (GameSetup.GS == null)
        {
            GameSetup.GS = this;
        }
    }

    // Referenced in a disconnect button
    public void DisconnectPlayer()
    {
        StartCoroutine(DisconnectAndLoad());
    }

    IEnumerator DisconnectAndLoad()
    {
        // Disconnects the local player from the master client and the room
        PhotonNetwork.LeaveRoom();
        // Wait until we disconnect from the room
        while (PhotonNetwork.InRoom)
            yield return null;

        // Load the main menu scene
        SceneManager.LoadScene(MultiPlayerSettings.multiplayerSetting.menuScene);
    }
}
