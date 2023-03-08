using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

[RequireComponent(typeof(Rigidbody), typeof(PhotonView))]
public class PlayerMoveController : MonoBehaviour
{
    [Header("ÉpÉâÉÅÅ[É^")]
    [SerializeField] float _movingSpeed = 0;
    [SerializeField] float _turnSpeed = 0;
    [SerializeField] float _jumpPower = 0;
    [SerializeField] float _isGroundedLength = 0;
    [SerializeField] Animator _anim;

    [Header("Tools")]
    [SerializeField] GameObject _hand;
    [SerializeField] GameObject _sword;
    [SerializeField] GameObject _axe;
    [SerializeField] GameObject _pickaxe;

    Rigidbody _rb;
    PhotonView _view;
    Transform _spawnPos;
    bool _isMove = false;
    float _time = 0f;
    string _name = "";
    void Start()
    {
        _view = GetComponent<PhotonView>();
        if (_view)
        {
            if (_view.IsMine)
            {
                _rb = GetComponent<Rigidbody>();
            }
        }
        //_spawnPos = GameObject.Find("PlayerSpawn").GetComponent<Transform>();
    }

    void Update()
    {
        if (!_view.IsMine) return;
        if (!_isMove)
        {
            Move();
        }
        if (_spawnPos)
        {
            Vector3 pos = _spawnPos.position;
            if (pos.y <= -5f)
            {
                gameObject.transform.position = _spawnPos.position;
            }
        }
    }

    private void Move()
    {
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        Vector3 dir = Vector3.forward * v + Vector3.right * h;

        if (dir == Vector3.zero)
        {
            _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
        }
        else
        {
            dir = Camera.main.transform.TransformDirection(dir);
            dir.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * _turnSpeed);

            Vector3 velo = dir.normalized * _movingSpeed;
            velo.y = _rb.velocity.y;
            _rb.velocity = velo;

        }
        if (_anim)
        {
            Vector3 velocity = _rb.velocity;

            if (velocity != new Vector3(0, 0, 0))
            {
                _anim.SetBool("IsWalk", true);
            }
            else
            {
                _anim.SetBool("IsWalk", false);
            }

        }
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            _rb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);

            _anim.SetBool("IsJump", true);

            _name = "IsJump";
            _time = 0.5f;
            StartCoroutine(IsCoroutine());
        }
        if (Input.GetButtonDown("Fire1") && IsGrounded())
        {
            _anim.SetBool("IsAttack", true);

            _isMove = true;
            _name = "IsAttack";
            _time = 0.5f;
            StartCoroutine(IsCoroutine());
        }
    }

    bool IsGrounded()
    {
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        Vector3 start = this.transform.position + col.center;
        Vector3 end = start + Vector3.down * _isGroundedLength;
        Debug.DrawLine(start, end);
        bool isGrounded = Physics.Linecast(start, end);
        return isGrounded;
    }

    IEnumerator IsCoroutine()
    {
        yield return new WaitForSeconds(_time);
        _anim.SetBool(_name, false);
        _isMove = false;
    }
}