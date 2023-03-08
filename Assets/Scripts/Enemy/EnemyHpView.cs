using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;

public class EnemyHpView : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] Slider _hpSlider;
    public int _hp = 10;

    PhotonView _photonView;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        _hpSlider.maxValue = _hp;
        _hpSlider.value = _hp;
    }

    public void DamageHp()
    {
        _hp--;
        _hpSlider.value = _hp;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_hpSlider.value);
        }
        else
        {
            _hpSlider.value = (int)stream.ReceiveNext();
        }
    }
}
