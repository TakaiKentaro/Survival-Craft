using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;


/// <summary>
/// Player��Ǐ]����J������ݒ肷��
/// </summary>
public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera _cvc;
    PhotonView _view;

    private void Start()
    {
        _view = GetComponent<PhotonView>();
        if (!_cvc)
        {
            _cvc = FindObjectOfType<CinemachineVirtualCamera>();
        }

        if (_view.IsMine)
        {
            _cvc.Follow = gameObject.transform;
        }
            
    }
}
