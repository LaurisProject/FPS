using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class AvatarCombat : MonoBehaviour
{
    // Component attached to this gameobject. PhotonView allows us to send RPCs.
    private PhotonView PV;
    // Component attached to this gameobject. AvatarSetup sets up player related variables.
    private AvatarSetup avatarSetup;
    // Transform that determines the start point for the raycast.
    public Transform rayOrigin;
    // Display the player health
    public Text healthDisplay;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        avatarSetup = GetComponent<AvatarSetup>();
        healthDisplay = GameSetup.GS.healthDisplay;
    }

    void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            PV.RPC("RPC_Shooting", RpcTarget.All);
        }
        healthDisplay.text = avatarSetup.playerHealth.ToString();
    }

    [PunRPC]
    void RPC_Shooting()
    {
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin.position, rayOrigin.TransformDirection(Vector3.forward), out hit, 1000))
        {
            Debug.DrawRay(rayOrigin.position, rayOrigin.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did hit");

            if (hit.transform.CompareTag("Avatar"))
            {
                // When we hit another avatar, get its health and subtract with our damage amount.
                hit.transform.gameObject.GetComponent<AvatarSetup>().playerHealth -= avatarSetup.playerDamage;
            }
        }
        else
        {
            Debug.DrawRay(rayOrigin.position, rayOrigin.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }

    }
}
