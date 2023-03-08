using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class PlayerHit : MonoBehaviourPunCallbacks
{
    [SerializeField] int _playerHp = 100;
    [SerializeField] Slider _hpSlider;

    PhotonView _view;

    private void Start()
    {
        _view = GetComponent<PhotonView>();
        if (_view)
        {
            if(_view.IsMine)
            {
                _hpSlider.maxValue = _playerHp;
                _hpSlider.value = _playerHp;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!_view.IsMine) return;

        if(collision.gameObject.CompareTag("Enemy"))
        {
            DamageHit(PhotonNetwork.LocalPlayer.ActorNumber, 1);
        }
        
    }

    public void DamageHit(int playerId, int damage)
    {
        _playerHp -= damage;
        _hpSlider.value = _playerHp;
        Dead();
        object[] parameters = new object[] { _playerHp };
        _view.RPC("SyncLife", RpcTarget.All, parameters);
    }

    [PunRPC]
    void SyncLife(int currentLife)
    {
        _playerHp = currentLife;
        _hpSlider.value = _playerHp;
        Dead();
    }

    void Dead()
    {
        if (_playerHp <= 0)
        {
            Debug.Log("Ž€–S");
            gameObject.SetActive(false);
        }
    }
}
