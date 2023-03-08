using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class PlayerActionCotroller : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] GameObject _attackCollider;
    [SerializeField] GameObject _attacFakeCollider;

    [Header("Guard")]
    [SerializeField] GameObject _guardCollider;

    [Header("Status")]
    [SerializeField] float _intervalTime;

    bool _check;
    PhotonView _view;

    private void Start()
    {
        _view = GetComponent<PhotonView>();
        if (_view)
        {
            if (_view.IsMine)
            {
                // �������i�����ő��삵�ē������j�I�u�W�F�N�g�̏ꍇ�̂� Rigidbody, Animator ���g��
                _attacFakeCollider.SetActive(true);
                _attackCollider.SetActive(false);
                _guardCollider.SetActive(false);
            }
        }

        
    }

    private void Update()
    {
        if (!_view.IsMine) return;

        if (!_check)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                _attacFakeCollider.SetActive(false);
                _attackCollider.SetActive(true);
                _check = true;
                StartCoroutine("ActionInterval");
            }
        }
        if (Input.GetButtonDown("Fire2"))
        {
            _guardCollider.SetActive(true);
            _check = true;
        }
        if (Input.GetButtonUp("Fire2"))
        {
            _guardCollider.SetActive(false);
            _check = false;
        }
    }

    IEnumerator ActionInterval()
    {
        yield return new WaitForSeconds(_intervalTime);
        _check = false;
        _attacFakeCollider.SetActive(true);
        _attackCollider.SetActive(false);
    }
}
