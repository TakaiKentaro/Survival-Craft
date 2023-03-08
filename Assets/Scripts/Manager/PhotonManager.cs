using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// ロビー生成
/// </summary>
public class PhotonManager : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// ステータス
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

    public delegate void RoomJoinCallback(); //ルームに入った時に呼ばれる処理
    public delegate void GameStartCallback(); //ゲームが始まった時に呼ばれる処理
    public delegate void GameStopCallback();　//ゲームが止まった時に呼ばれる処理
    public delegate void GameEventCallback(int id, int evt); //イベント処理をする時に呼ばれる処理
    public delegate void RoomUpdateCallback(List<RoomInfo> roomList); //ルーム情報一覧が更新された時に呼ばれる処理

    [SerializeField, Tooltip("一緒に着けるPhotonView")] PhotonView _photonView;
    [SerializeField, Tooltip("1つのルームに何人までか")] int _maxPlayerInRoom = 4;
    [SerializeField, Tooltip("自動的に接続")] bool _isAutoConnect = true;
    [SerializeField, Tooltip("自動でルームに入る")] bool _isAutoJoin = true;
    [SerializeField, Tooltip("ランクマになる")] bool _isRankMatching = true;
    [SerializeField, Tooltip("Photonで共有されるユーザ情報")] UserParam _mySelf = new UserParam();
    [SerializeField, Tooltip("マッチング用合言葉")] string _onlineKeyword = "";

    [Tooltip("ルーム情報一覧")] List<RoomInfo> _roomList = null;
    [Tooltip("Photonの接続State")] PhotonState State;

    [Tooltip("Connect時のFPS")] const int FPSRate = 60;

    /// <summary>
    /// ゲーム中かどうか
    /// </summary>
    static public bool IsGameNow
    {
        get
        {
            return _instance && _instance.State == PhotonState.IN_GAME;
        }
    }

    [Tooltip("ネットワークで共有するユーザーデータ達　ロビーでマッチング時に使用する情報")] List<UserParam> _player = new List<UserParam>();
    public UserParam GetPlayer(int id)
    {
        return _player[id];
    }

    [Tooltip("状態管理")] List<int> _playerStatus = new List<int>();

    //コールバック群
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
        //コールバックは都合のいいところの関数を設定する
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
    /// ルームに入ったときに呼ばれる処理
    /// </summary>
    void RoomJoin()
    {
        Debug.Log($"RoomJoin");
    }

    /// <summary>
    /// ゲームが始まった時に呼ばれる処理
    /// </summary>
    void GameStart()
    {
        Debug.Log($"ゲーム開始");
    }

    /// <summary>
    /// ゲームが止まった時に呼ばれる処理
    /// </summary>
    void GameStop()
    {
        Debug.Log($"ゲーム停止");
    }

    /// <summary>
    /// イベント処理
    /// </summary>
    /// <param name="id"></param>
    /// <param name="evt"></param>
    void GameEvent(int id, int evt)
    {
        Debug.Log($"GameEvent: From{id} - Event:{evt}");
    }

    /// <summary>
    /// 接続処理
    /// </summary>
    public void Connect()
    {
        Debug.Log($"Connect");

        //FPS調整
        PhotonNetwork.SendRate = FPSRate;
        PhotonNetwork.SerializationRate = FPSRate;

        PhotonNetwork.NickName = _mySelf.Name;
        PhotonNetwork.ConnectUsingSettings(); //エディタで設定したようにPhotonに接続します

        State = PhotonState.WAITING;
    }

    /// <summary>
    /// 接続終了
    /// </summary>
    public void Disconnect()
    {
        PhotonNetwork.Disconnect(); //このクライアントをPhotonサーバーと、Roomに残るプロセスから回線切断します。
    }

    /// <summary>
    /// ロビーに入る
    /// NOTE: 外部から呼ばれることはない
    /// </summary>
    void JoinLobby()
    {
        PhotonNetwork.JoinLobby(); //マスターサーバー上で、デフォルトロビーに入ります。
    }

    /// <summary>
    /// 部屋を作る
    /// </summary>
    public void CreateRoom()
    {
        Debug.Log($"CreatRoom");
        RoomOptions roomOptions = new RoomOptions(); //ルームを作成しますが、ルームがすでに存在している場合は失敗します。マスタサーバでのみ呼び出すことができます。

        //カスタムプロパティ
        List<string> properties = new List<string>();
        properties = properties.Concat(_mySelf.GetPropertiesString()).ToList(); //ユーザーの名前とランクをリスト化
        var roomProp = _mySelf.CreateHashTable(); //ユーザーのカスタムプロパティを取得

        //UserParam以外で部屋に情報を共有したい場合ここに追加

        //ここは変更不可
        roomProp["GameState"] = 0;
        roomProp["Keyword"] = _onlineKeyword;
        properties = properties.Concat(new string[] { "GameState", "Keyword" }).ToList(); //ゲームステートとキーワードをリスト化し結合

        roomOptions.IsVisible = true; //このルームがロビーに登録されているかどうかを定義します。そうでない場合は、ランダムに入室されません。
        roomOptions.MaxPlayers = (Byte)_maxPlayerInRoom;
        roomOptions.CustomRoomProperties = roomProp; //ルームのカスタムプロパティをroomPropに設定
        roomOptions.CustomRoomPropertiesForLobby = properties.ToArray(); //ロビーにリストされるカスタムルームのプロパティを定義する
        PhotonNetwork.CreateRoom(Guid.NewGuid().ToString(), roomOptions, TypedLobby.Default); //ルームを作成しますが、ルームがすでに存在している場合は失敗する。マスタサーバでのみ呼び出し可能。
    }

    /// <summary>
    /// 部屋から出る
    /// </summary>
    public void LeaveRoom()
    {
        Debug.Log("LeaveRoom");
        PhotonNetwork.LeaveRoom(); //現在のRoomを退室して、マスターサーバーに戻ります。
    }

    /// <summary>
    /// 指定した部屋に入る
    /// </summary>
    /// <param name="roomName"></param>
    /// <returns></returns>
    public bool Matching(string roomName)
    {
        Debug.Log($"Select Matching: {roomName}");

        return PhotonNetwork.JoinRoom(roomName); //roomnameでルームに参加して、成功時に??OnJoinedRoom()を呼び出します。これは、ロビーには影響されません。
    }

    /// <summary>
    /// 適当な部屋に入る
    /// </summary>
    /// <returns></returns>
    public bool RandomMatching()
    {
        Debug.Log($"RandomMatching");

        //入れる部屋を探す
        var list = MatchingFilter(_isRankMatching);

        if (list.Count > 0)
        {
            Debug.Log($"部屋があったので適当にはいる");
            return PhotonNetwork.JoinRoom(list[UnityEngine.Random.Range(0, list.Count)].Name);
        }

        Debug.Log($"入れる部屋がなかった");
        return false;
    }

    /// <summary>
    /// マッチング時にルーム情報をフィルタする
    /// NOTE: ここいじってマッチング設定変えれる
    /// </summary>
    /// <param name="isRankMatching"></param>
    /// <returns></returns>
    List<RoomInfo> MatchingFilter(bool isRankMatching)
    {
        //同じランク帯、またはフレンドルームを見つける
        return _roomList.Where(info =>
        {
            //入れない部屋は除外
            if (info.PlayerCount >= _maxPlayerInRoom) return false;
            if (int.Parse(info.CustomProperties["GameState"].ToString()) == 1) return false;

            //合言葉を使っているかどうか
            if (_onlineKeyword != "")
            {
                if (info.CustomProperties["Keyword"] != null)
                {
                    if (_onlineKeyword == info.CustomProperties["Keyword"].ToString()) return true;
                }

                return false; //フレンドマッチはキーワードマッチ以外禁止
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
    /// 状態更新
    /// </summary>
    void Update()
    {
        switch (State)
        {
            //接続開始
            case PhotonState.CONNECTED:
                //接続したらすぐロビーに入る
                JoinLobby();
                State = PhotonState.WAITING;
                break;
            //ロビーで部屋選択
            case PhotonState.IN_LOBBY:
                {
                    //ルームリストの取得に間があるので待機
                    if (_roomList == null) break;

                    //オートマッチングなら勝手に部屋の作成、入室を行う
                    if (_isAutoJoin)
                    {
                        //現在のロビーにいる人のルームリストを取得、取得したルームからマッチング相手を検索
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
            //ゲーム中
            case PhotonState.IN_GAME:
                break;
        }
    }

    /// <summary>
    /// ユーザー情報の更新
    /// </summary>
    void UpdateUserStatus()
    {
        Debug.Log("UpdateUserStatus");
        Photon.Realtime.Room room = PhotonNetwork.CurrentRoom; //現在いるルームを取得。ルームにいない場合はnull

        if (room == null)
        {
            return;
        }

        int index = 0;
        _player.Clear(); //全ての要素を削除
        foreach (var pl in room.Players.Values)
        {
            if (pl.CustomProperties["GUID"] == null) continue;

            _player.Add(new UserParam());
            _player[index++].UpdateHashTable(pl.CustomProperties);
        }
    }

    /// <summary>
    /// 揃ったか確認
    /// </summary>
    void CheckRoomStatus()
    {
        bool isGameStart = false;

        //揃っていたら開始
        if (PhotonNetwork.CurrentRoom.PlayerCount >= _maxPlayerInRoom) isGameStart = true;

        if (!isGameStart) return;

        //ゲームを開始したので乱入を禁止にする
        PhotonNetwork.CurrentRoom.CustomProperties["GameState"] = 1;
        //開始時点のTickを記録
        PhotonNetwork.CurrentRoom.CustomProperties["GameStartTime"] = PhotonNetwork.Time;　//Photonのネットワークタイムを入力
        PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties); //入室しているルームのアップデートを行う

        //ゲーム開始をコールする
        SendGameStart();
    }

    //Photon以外の対応

    /// <summary>
    /// 合言葉のセット
    /// </summary>
    /// <param name="kwd"></param>
    public void SetKeyword(string kwd)
    {
        _onlineKeyword = kwd;
    }

    //イカPhotonからのコールバック

    /// <summary>
    /// Photonに接続した時
    /// </summary>
    public override void OnConnectedToMaster()　//クライアントがMaster Serverに接続されていて、マッチメイキングやその他のタスクを行う準備が整ったときに呼び出される
    {
        Debug.Log("OnConnectedToMaster");
        State = PhotonState.CONNECTED;
    }

    /// <summary>
    /// ロビーに入った時
    /// </summary>
    public override void OnJoinedLobby() //マスターサーバーのロビーに入るときに呼び出される
    {
        Debug.Log("OnJoinedLobby:" + PhotonNetwork.CurrentLobby.Name);

        //カスタムプロパティ
        var userProp = _mySelf.CreateHashTable();
        userProp["GUID"] = Guid.NewGuid().ToString();
        PhotonNetwork.SetPlayerCustomProperties(userProp);

        //相手が来るまで待つように同期を行う
        State = PhotonState.IN_LOBBY;

        //MasterCliantと同じSceneをロード
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    /// <summary>
    /// ルームに入った時
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        base.OnJoinedRoom();

        _roomJoinCallback?.Invoke();

        //相手が来るまで待つよう同期する
        State = PhotonState.READY;
        UpdateUserStatus();
        CheckRoomStatus();
    }

    /// <summary>
    /// ルームから抜けた時
    /// </summary>
    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
        base.OnLeftRoom();

        State = PhotonState.IN_LOBBY;
    }

    /// <summary>
    /// PUN2でルーム取得
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
    /// 相手の切断
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        //相手が切断したので、勝ちにする
    }

    //RPC

    /// <summary>
    /// イベント
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
