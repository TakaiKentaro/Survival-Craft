using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(Rigidbody), typeof(PhotonView))]
public class EnemyController : MonoBehaviourPunCallbacks
{
    [SerializeField] int _ememyHp = 10;
    [SerializeField] float _enemyMoveSpeed;
    [SerializeField] Transform[] _playerPos;
    [SerializeField] Slider _enemyHpSlider;

    int _currentWaypointIndex;
    NavMeshAgent _navMeshAgent;
    Rigidbody _rb;
    PhotonView _view;

    private void Start()
    {
        _view = GetComponent<PhotonView>();
        if (_view)
        {
            _rb = GetComponent<Rigidbody>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _enemyHpSlider.maxValue = _ememyHp;
            _enemyHpSlider.value = _ememyHp;
            _navMeshAgent.speed = _enemyMoveSpeed;
        }

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
        _playerPos = new Transform[gameObjects.Length];

        for (int i = 0; i < gameObjects.Length; i++)
        {
            _playerPos[i] = gameObjects[i].transform;
        }

        StartCoroutine("MoveDestination");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            // �ړI�n�̔ԍ����P�X�V�i�E�ӂ���]���Z�q�ɂ��邱�ƂŖړI�n�����[�v�������j
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _playerPos.Length;
            // �ړI�n�����̏ꏊ�ɐݒ�
            _navMeshAgent.SetDestination(_playerPos[_currentWaypointIndex].position);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack"))
        {
            Damage(1);
        }
        if(other.CompareTag("Guard"))
        {
            // �ړI�n�̔ԍ����P�X�V�i�E�ӂ���]���Z�q�ɂ��邱�ƂŖړI�n�����[�v�������j
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _playerPos.Length;
            // �ړI�n�����̏ꏊ�ɐݒ�
            _navMeshAgent.SetDestination(_playerPos[_currentWaypointIndex].position);
        }
    }

    IEnumerator MoveDestination()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            _navMeshAgent.SetDestination(_playerPos[_currentWaypointIndex].position);
        }
    }

    public void Damage(int damage)
    {
        _ememyHp -= damage;
        _enemyHpSlider.value = _ememyHp;
        Dead();
        object[] parameters = new object[] { _ememyHp };
        _view.RPC("SyncLife", RpcTarget.All, parameters);
    }

    [PunRPC]
    void SyncLife(int currentLife)
    {
        _ememyHp = currentLife;
        _enemyHpSlider.value = _ememyHp;
        Dead();
    }

    void Dead()
    {
        if(_ememyHp <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
