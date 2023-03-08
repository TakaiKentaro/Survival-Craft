using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class SpawneManager : MonoBehaviourPunCallbacks // Photon Realtime �p�̃N���X���p������
{
    [SerializeField] string _playerPrefabName = "PlayerPrefab";
    [SerializeField] Transform[] _spawnPositions;

    private void Start()
    {
        SpawnPlayer();
    }

    /// <summary>
    /// �v���C���[�𐶐�����
    /// </summary>
    public void SpawnPlayer()
    {
        // �v���C���[���ǂ��� spawn �����邩���߂�
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;    // ������ ActorNumber ���擾����B�Ȃ� ActorNumber �́u1����v�������ɐU����B
        Debug.Log("My ActorNumber: " + actorNumber);
        Transform spawnPoint = _spawnPositions[actorNumber - 1];

        // �v���C���[�𐶐����A���̃N���C�A���g�Ɠ�������
        PhotonNetwork.IsMessageQueueRunning = true;
        GameObject player = PhotonNetwork.Instantiate(_playerPrefabName, spawnPoint.position, Quaternion.identity);

        if (actorNumber > PhotonNetwork.CurrentRoom.MaxPlayers - 1)
        {
            Debug.Log("Closing Room");
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }


}


