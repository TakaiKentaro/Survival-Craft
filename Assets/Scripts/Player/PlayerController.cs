using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviour
{
    #region Singleton
    static PlayerController _instance = null;

    public static bool IsEmpty
    {
        get { return _instance == null; }
    }

    public static PlayerController Instance
    {
        get
        {
            if (_instance == null)
            {
                System.Type type = typeof(PhotonManager);
                _instance = GameObject.FindObjectOfType(type) as PlayerController;
            }

            return _instance;
        }
    }
    #endregion

    [SerializeField, Tooltip("Playerのステータス")] PlayerStatus _status = new PlayerStatus();

    
}
