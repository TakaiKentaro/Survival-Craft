using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// ���r�[����
/// </summary>
public class PhotonManager : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// �X�e�[�^�X
    /// </summary>
    public enum PhotonState
    {
        INIT,
        CONNECTED,
        IN_LOBBY,
        READY,
        IN_GAME,
        DISCONNECTED,
        WAITING,
    }

    #region Singleton
    static PhotonManager _instance = null;

    public static bool IsEmpty
    {
        get { return _instance == null; }
    }

    public static PhotonManager Instance
    {
        get
        {
            if (_instance == null)
            {
                System.Type type = typeof(PhotonManager);
                _instance = GameObject.FindObjectOfType(type) as PhotonManager;
            }

            return _instance;
        }
    }
    #endregion

    public delegate void RoomJoinCallback(); //���[���ɓ��������ɌĂ΂�鏈��
    public delegate void GameStartCallback(); //�Q�[�����n�܂������ɌĂ΂�鏈��
    public delegate void GameStopCallback();�@//�Q�[�����~�܂������ɌĂ΂�鏈��
    public delegate void GameEventCallback(int id, int evt); //�C�x���g���������鎞�ɌĂ΂�鏈��
    public delegate void RoomUpdateCallback(List<RoomInfo> roomList); //���[�����ꗗ���X�V���ꂽ���ɌĂ΂�鏈��

    [SerializeField, Tooltip("�ꏏ�ɒ�����PhotonView")] PhotonView _photonView;
    [SerializeField, Tooltip("1�̃��[���ɉ��l�܂ł�")] int _maxPlayerInRoom = 4;
    [SerializeField, Tooltip("�����I�ɐڑ�")] bool _isAutoConnect = true;
    [SerializeField, Tooltip("�����Ń��[���ɓ���")] bool _isAutoJoin = true;
    [SerializeField, Tooltip("�����N�}�ɂȂ�")] bool _isRankMatching = true;
    [SerializeField, Tooltip("Photon�ŋ��L����郆�[�U���")] UserParam _mySelf = new UserParam();
    [SerializeField, Tooltip("�}�b�`���O�p�����t")] string _onlineKeyword = "";

    [Tooltip("���[�����ꗗ")] List<RoomInfo> _roomList = null;
    [Tooltip("Photon�̐ڑ�State")] PhotonState State;

    [Tooltip("Connect����FPS")] const int FPSRate = 60;

    /// <summary>
    /// �Q�[�������ǂ���
    /// </summary>
    static public bool IsGameNow
    {
        get
        {
            return _instance && _instance.State == PhotonState.IN_GAME;
        }
    }

    [Tooltip("�l�b�g���[�N�ŋ��L���郆�[�U�[�f�[�^�B�@���r�[�Ń}�b�`���O���Ɏg�p������")] List<UserParam> _player = new List<UserParam>();
    public UserParam GetPlayer(int id)
    {
        return _player[id];
    }

    [Tooltip("��ԊǗ�")] List<int> _playerStatus = new List<int>();

    //�R�[���o�b�N�Q
    RoomJoinCallback _roomJoinCallback;
    GameStartCallback _gameStartCallback;
    GameStopCallback _gameStopCallback;
    GameEventCallback _eventCallback;


#if UNITY_EDITOR
    public RoomUpdateCallback RoomUpdate { get; set; }
#endif
    // Start is called before the first frame update
    void Start()
    {
        //�R�[���o�b�N�͓s���̂����Ƃ���̊֐���ݒ肷��
        _roomJoinCallback = RoomJoin;
        _gameStartCallback = GameStart;
        _eventCallback = GameEvent;
        _gameStopCallback = GameStop;

        if (_isAutoConnect)
        {
            Connect();
        }
    }

    /// <summary>
    /// ���[���ɓ������Ƃ��ɌĂ΂�鏈��
    /// </summary>
    void RoomJoin()
    {
        Debug.Log($"RoomJoin");
    }

    /// <summary>
    /// �Q�[�����n�܂������ɌĂ΂�鏈��
    /// </summary>
    void GameStart()
    {
        Debug.Log($"�Q�[���J�n");
    }

    /// <summary>
    /// �Q�[�����~�܂������ɌĂ΂�鏈��
    /// </summary>
    void GameStop()
    {
        Debug.Log($"�Q�[����~");
    }

    /// <summary>
    /// �C�x���g����
    /// </summary>
    /// <param name="id"></param>
    /// <param name="evt"></param>
    void GameEvent(int id, int evt)
    {
        Debug.Log($"GameEvent: From{id} - Event:{evt}");
    }

    /// <summary>
    /// �ڑ�����
    /// </summary>
    public void Connect()
    {
        Debug.Log($"Connect");

        //FPS����
        PhotonNetwork.SendRate = FPSRate;
        PhotonNetwork.SerializationRate = FPSRate;

        PhotonNetwork.NickName = _mySelf.Name;
        PhotonNetwork.ConnectUsingSettings(); //�G�f�B�^�Őݒ肵���悤��Photon�ɐڑ����܂�

        State = PhotonState.WAITING;
    }

    /// <summary>
    /// �ڑ��I��
    /// </summary>
    public void Disconnect()
    {
        PhotonNetwork.Disconnect(); //���̃N���C�A���g��Photon�T�[�o�[�ƁARoom�Ɏc��v���Z�X�������ؒf���܂��B
    }

    /// <summary>
    /// ���r�[�ɓ���
    /// NOTE: �O������Ă΂�邱�Ƃ͂Ȃ�
    /// </summary>
    void JoinLobby()
    {
        PhotonNetwork.JoinLobby(); //�}�X�^�[�T�[�o�[��ŁA�f�t�H���g���r�[�ɓ���܂��B
    }

    /// <summary>
    /// ���������
    /// </summary>
    public void CreateRoom()
    {
        Debug.Log($"CreatRoom");
        RoomOptions roomOptions = new RoomOptions(); //���[�����쐬���܂����A���[�������łɑ��݂��Ă���ꍇ�͎��s���܂��B�}�X�^�T�[�o�ł̂݌Ăяo�����Ƃ��ł��܂��B

        //�J�X�^���v���p�e�B
        List<string> properties = new List<string>();
        properties = properties.Concat(_mySelf.GetPropertiesString()).ToList(); //���[�U�[�̖��O�ƃ����N�����X�g��
        var roomProp = _mySelf.CreateHashTable(); //���[�U�[�̃J�X�^���v���p�e�B���擾

        //UserParam�ȊO�ŕ����ɏ������L�������ꍇ�����ɒǉ�

        //�����͕ύX�s��
        roomProp["GameState"] = 0;
        roomProp["Keyword"] = _onlineKeyword;
        properties = properties.Concat(new string[] { "GameState", "Keyword" }).ToList(); //�Q�[���X�e�[�g�ƃL�[���[�h�����X�g��������

        roomOptions.IsVisible = true; //���̃��[�������r�[�ɓo�^����Ă��邩�ǂ������`���܂��B�����łȂ��ꍇ�́A�����_���ɓ�������܂���B
        roomOptions.MaxPlayers = (Byte)_maxPlayerInRoom;
        roomOptions.CustomRoomProperties = roomProp; //���[���̃J�X�^���v���p�e�B��roomProp�ɐݒ�
        roomOptions.CustomRoomPropertiesForLobby = properties.ToArray(); //���r�[�Ƀ��X�g�����J�X�^�����[���̃v���p�e�B���`����
        PhotonNetwork.CreateRoom(Guid.NewGuid().ToString(), roomOptions, TypedLobby.Default); //���[�����쐬���܂����A���[�������łɑ��݂��Ă���ꍇ�͎��s����B�}�X�^�T�[�o�ł̂݌Ăяo���\�B
    }

    /// <summary>
    /// ��������o��
    /// </summary>
    public void LeaveRoom()
    {
        Debug.Log("LeaveRoom");
        PhotonNetwork.LeaveRoom(); //���݂�Room��ގ����āA�}�X�^�[�T�[�o�[�ɖ߂�܂��B
    }

    /// <summary>
    /// �w�肵�������ɓ���
    /// </summary>
    /// <param name="roomName"></param>
    /// <returns></returns>
    public bool Matching(string roomName)
    {
        Debug.Log($"Select Matching: {roomName}");

        return PhotonNetwork.JoinRoom(roomName); //roomname�Ń��[���ɎQ�����āA��������??OnJoinedRoom()���Ăяo���܂��B����́A���r�[�ɂ͉e������܂���B
    }

    /// <summary>
    /// �K���ȕ����ɓ���
    /// </summary>
    /// <returns></returns>
    public bool RandomMatching()
    {
        Debug.Log($"RandomMatching");

        //����镔����T��
        var list = MatchingFilter(_isRankMatching);

        if (list.Count > 0)
        {
            Debug.Log($"�������������̂œK���ɂ͂���");
            return PhotonNetwork.JoinRoom(list[UnityEngine.Random.Range(0, list.Count)].Name);
        }

        Debug.Log($"����镔�����Ȃ�����");
        return false;
    }

    /// <summary>
    /// �}�b�`���O���Ƀ��[�������t�B���^����
    /// NOTE: �����������ă}�b�`���O�ݒ�ς����
    /// </summary>
    /// <param name="isRankMatching"></param>
    /// <returns></returns>
    List<RoomInfo> MatchingFilter(bool isRankMatching)
    {
        //���������N�сA�܂��̓t�����h���[����������
        return _roomList.Where(info =>
        {
            //����Ȃ������͏��O
            if (info.PlayerCount >= _maxPlayerInRoom) return false;
            if (int.Parse(info.CustomProperties["GameState"].ToString()) == 1) return false;

            //�����t���g���Ă��邩�ǂ���
            if (_onlineKeyword != "")
            {
                if (info.CustomProperties["Keyword"] != null)
                {
                    if (_onlineKeyword == info.CustomProperties["Keyword"].ToString()) return true;
                }

                return false; //�t�����h�}�b�`�̓L�[���[�h�}�b�`�ȊO�֎~
            }
            else
            {
                if (info.CustomProperties["Keyword"] != null)
                {
                    if (info.CustomProperties["Keyword"].ToString().Length > 0)
                    {
                        return false;
                    }
                }
            }

            if (isRankMatching)
            {
                if (_mySelf.Rank == (int)info.CustomProperties["Rank"]) return true;
                return false;
            }
            else
            {
                return true;
            }

        }).ToList();
    }

    /// <summary>
    /// ��ԍX�V
    /// </summary>
    void Update()
    {
        switch (State)
        {
            //�ڑ��J�n
            case PhotonState.CONNECTED:
                //�ڑ������炷�����r�[�ɓ���
                JoinLobby();
                State = PhotonState.WAITING;
                break;
            //���r�[�ŕ����I��
            case PhotonState.IN_LOBBY:
                {
                    //���[�����X�g�̎擾�ɊԂ�����̂őҋ@
                    if (_roomList == null) break;

                    //�I�[�g�}�b�`���O�Ȃ珟��ɕ����̍쐬�A�������s��
                    if (_isAutoJoin)
                    {
                        //���݂̃��r�[�ɂ���l�̃��[�����X�g���擾�A�擾�������[������}�b�`���O���������
                        if (RandomMatching())
                        {
                            State = PhotonState.WAITING;
                        }
                        else
                        {
                            CreateRoom();
                            State = PhotonState.IN_GAME;
                        }
                    }
                }
                break;
            //�Q�[����
            case PhotonState.IN_GAME:
                break;
        }
    }

    /// <summary>
    /// ���[�U�[���̍X�V
    /// </summary>
    void UpdateUserStatus()
    {
        Debug.Log("UpdateUserStatus");
        Photon.Realtime.Room room = PhotonNetwork.CurrentRoom; //���݂��郋�[�����擾�B���[���ɂ��Ȃ��ꍇ��null

        if (room == null)
        {
            return;
        }

        int index = 0;
        _player.Clear(); //�S�Ă̗v�f���폜
        foreach (var pl in room.Players.Values)
        {
            if (pl.CustomProperties["GUID"] == null) continue;

            _player.Add(new UserParam());
            _player[index++].UpdateHashTable(pl.CustomProperties);
        }
    }

    /// <summary>
    /// ���������m�F
    /// </summary>
    void CheckRoomStatus()
    {
        bool isGameStart = false;

        //�����Ă�����J�n
        if (PhotonNetwork.CurrentRoom.PlayerCount >= _maxPlayerInRoom) isGameStart = true;

        if (!isGameStart) return;

        //�Q�[�����J�n�����̂ŗ������֎~�ɂ���
        PhotonNetwork.CurrentRoom.CustomProperties["GameState"] = 1;
        //�J�n���_��Tick���L�^
        PhotonNetwork.CurrentRoom.CustomProperties["GameStartTime"] = PhotonNetwork.Time;�@//Photon�̃l�b�g���[�N�^�C�������
        PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties); //�������Ă��郋�[���̃A�b�v�f�[�g���s��

        //�Q�[���J�n���R�[������
        SendGameStart();
    }

    //Photon�ȊO�̑Ή�

    /// <summary>
    /// �����t�̃Z�b�g
    /// </summary>
    /// <param name="kwd"></param>
    public void SetKeyword(string kwd)
    {
        _onlineKeyword = kwd;
    }

    //�C�JPhoton����̃R�[���o�b�N

    /// <summary>
    /// Photon�ɐڑ�������
    /// </summary>
    public override void OnConnectedToMaster()�@//�N���C�A���g��Master Server�ɐڑ�����Ă��āA�}�b�`���C�L���O�₻�̑��̃^�X�N���s���������������Ƃ��ɌĂяo�����
    {
        Debug.Log("OnConnectedToMaster");
        State = PhotonState.CONNECTED;
    }

    /// <summary>
    /// ���r�[�ɓ�������
    /// </summary>
    public override void OnJoinedLobby() //�}�X�^�[�T�[�o�[�̃��r�[�ɓ���Ƃ��ɌĂяo�����
    {
        Debug.Log("OnJoinedLobby:" + PhotonNetwork.CurrentLobby.Name);

        //�J�X�^���v���p�e�B
        var userProp = _mySelf.CreateHashTable();
        userProp["GUID"] = Guid.NewGuid().ToString();
        PhotonNetwork.SetPlayerCustomProperties(userProp);

        //���肪����܂ő҂悤�ɓ������s��
        State = PhotonState.IN_LOBBY;

        //MasterCliant�Ɠ���Scene�����[�h
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    /// <summary>
    /// ���[���ɓ�������
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        base.OnJoinedRoom();

        _roomJoinCallback?.Invoke();

        //���肪����܂ő҂悤��������
        State = PhotonState.READY;
        UpdateUserStatus();
        CheckRoomStatus();
    }

    /// <summary>
    /// ���[�����甲������
    /// </summary>
    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
        base.OnLeftRoom();

        State = PhotonState.IN_LOBBY;
    }

    /// <summary>
    /// PUN2�Ń��[���擾
    /// </summary>
    /// <param name="roomList"></param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate");

        base.OnRoomListUpdate(roomList);

        _roomList = roomList;

#if UNITY_EDITOR
        RoomUpdate?.Invoke(_roomList);
#endif
    }

    /// <summary>
    /// ����̐ؒf
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        //���肪�ؒf�����̂ŁA�����ɂ���
    }

    //RPC

    /// <summary>
    /// �C�x���g
    /// </summary>
    [PunRPC]
    void EventCall(int id, int status)
    {
        _eventCallback?.Invoke(id, status);
    }

    public void SendEvent(int evt)
    {
        _photonView.RPC("EventCall", RpcTarget.All, PhotonNetwork.LocalPlayer.UserId, evt);
    }

    [PunRPC]
    void GameStartCall()
    {
        State = PhotonState.IN_GAME;
        _gameStartCallback?.Invoke();
    }
    void SendGameStart()
    {
        _photonView.RPC("GameStartCall", RpcTarget.All);
    }

    public void AutoJoin()
    {
        if (!_isAutoJoin) _isAutoJoin = true;
    }
}
