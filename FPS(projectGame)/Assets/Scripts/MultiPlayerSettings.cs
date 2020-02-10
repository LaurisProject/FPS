using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPlayerSettings : MonoBehaviour

{
    
    public static MultiPlayerSettings multiplayerSetting;

    //determines the delay type of the game

    public bool delayStart;
    public int maxPlayers;

    public int menuScene;

    // To call scenes from our scripts
    public int multiplayerScene;
    
    private void Awake()
    {
        if(MultiPlayerSettings.multiplayerSetting == null)
        {
            // If settings are not null
            MultiPlayerSettings.multiplayerSetting = this;
        }
        else
        {
            if(MultiPlayerSettings.multiplayerSetting != this)
            {
                // Then delete the game object if it doesnt match with instance
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

}
