using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class SpawneManager : MonoBehaviourPunCallbacks // Photon Realtime 用のクラスを継承する
{
    [SerializeField] string _playerPrefabName = "PlayerPrefab";
    [SerializeField] Transform[] _spawnPositions;

    private void Start()
    {
        SpawnPlayer();
    }

    /// <summary>
    /// プレイヤーを生成する
    /// </summary>
    public void SpawnPlayer()
    {
        // プレイヤーをどこに spawn させるか決める
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;    // 自分の ActorNumber を取得する。なお ActorNumber は「1から」入室順に振られる。
        Debug.Log("My ActorNumber: " + actorNumber);
        Transform spawnPoint = _spawnPositions[actorNumber - 1];

        // プレイヤーを生成し、他のクライアントと同期する
        PhotonNetwork.IsMessageQueueRunning = true;
        GameObject player = PhotonNetwork.Instantiate(_playerPrefabName, spawnPoint.position, Quaternion.identity);

        if (actorNumber > PhotonNetwork.CurrentRoom.MaxPlayers - 1)
        {
            Debug.Log("Closing Room");
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }


}


