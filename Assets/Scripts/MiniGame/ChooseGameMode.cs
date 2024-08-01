using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class ChooseGameMode : MonoBehaviourPunCallbacks
{
    public Toggle toggleOffline; // オフラインモードの切り替えトグル
    public Toggle toggleOnline; // オンラインモードの切り替えトグル
    public Button startButton; // ゲーム開始ボタン
    public Canvas canvasStart; // ゲーム開始のキャンバス
    public RawImage hostIMG; // ホストプレイヤーの画像
    public RawImage clientIMG; // クライアントプレイヤーの画像
    public TMP_Text statusText; // ステータステキスト

    public static event System.Action OnGameOffline; // オフラインゲームのイベント
    public static event System.Action OnGameOnline; // オンラインゲームのイベント
    public static event System.Action<int> OnGameStart; // ゲーム開始時のイベント

    [SerializeField] private TMP_Dropdown dropdown; // ドロップダウンメニュー

    private bool isConnecting; // サーバー接続中フラグ

    void Start()
    {
        hostIMG.gameObject.SetActive(false); // 初期状態でホスト画像を非表示
        clientIMG.gameObject.SetActive(false); // 初期状態でクライアント画像を非表示

        UpdateStatusText(); // ステータステキストを更新

        SetOppositeState(toggleOffline.isOn); // 初期状態に基づいてトグル状態を設定
        toggleOffline.onValueChanged.AddListener((value) => SetOppositeState(value));
        toggleOnline.onValueChanged.AddListener((value) => SetOppositeState(!value));
        startButton.onClick.AddListener(OnStartButtonClick); // 開始ボタンにイベントを追加
        dropdown.gameObject.SetActive(false); // 初期状態でドロップダウンメニューを非表示
    }

    void Update()
    {
        OnToggleSelected(); // トグルの選択状態を監視
        UpdateStatusText(); // ステータステキストを更新
    }

    void OnToggleValueChanged()
    {
        UpdateStatusText(); // トグルが変更されたときにステータステキストを更新
    }

    void UpdateStatusText()
    {
        if (toggleOffline.isOn)
        {
            statusText.text = "合計10点で終了"; // オフラインモードのステータス表示
        }
        else if (toggleOnline.isOn)
        {
            statusText.text = "先に10点取ろう！"; // オンラインモードのステータス表示
        }
    }

    void SetOppositeState(bool state)
    {
        canvasStart.gameObject.SetActive(true); // ゲーム開始キャンバスを表示
        toggleOffline.onValueChanged.RemoveAllListeners(); // トグルのリスナーを削除
        toggleOnline.onValueChanged.RemoveAllListeners();

        toggleOffline.isOn = state; // トグルの状態を設定
        toggleOnline.isOn = !state;

        toggleOffline.onValueChanged.AddListener((value) => SetOppositeState(value)); // 再度リスナーを追加
        toggleOnline.onValueChanged.AddListener((value) => SetOppositeState(!value));
    }

    void OnStartButtonClick()
    {
        Cursor.visible = false; // カーソルを非表示にする
        
        if (toggleOffline.isOn)
        {
            Debug.Log("オフラインゲームを開始...");
            OnGameOffline?.Invoke(); // オフラインゲームのイベントを発火
            dropdown.gameObject.SetActive(false); // ドロップダウンメニューを非表示
        }
        else if (toggleOnline.isOn)
        {
            Debug.Log("オンラインゲームを開始...");
            OnGameOnline?.Invoke(); // オンラインゲームのイベントを発火
            dropdown.gameObject.SetActive(true); // ドロップダウンメニューを表示

            int currentPlayer = 1;
            OnGameStart?.Invoke(currentPlayer); // ゲーム開始イベントを発火

            if (PhotonNetwork.IsConnectedAndReady)
            {
                if (dropdown.value == 0)
                {
                    CreateRoom(); // ルームを作成
                }
                else
                {
                    JoinRoom(); // ルームに参加
                }
            }
            else
            {
                isConnecting = true;
                PhotonNetwork.ConnectUsingSettings(); // Photonサーバーに接続
                Debug.Log("Photonサーバーに接続中...");
            }
        }

        canvasStart.gameObject.SetActive(false); // ゲーム開始キャンバスを非表示
    }

    void OnToggleSelected()
    {
        dropdown.gameObject.SetActive(!toggleOffline.isOn); // オフラインモードの場合はドロップダウンメニューを非表示
    }

    void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
        Debug.Log("ルームを作成中...");
    }

    void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("ランダムルームに参加中...");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon マスターサーバーに接続されました。");
        if (isConnecting)
        {
            isConnecting = false;
            if (dropdown.value == 0)
            {
                CreateRoom(); // ルームを作成
            }
            else
            {
                JoinRoom(); // ルームに参加
            }
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("ロビーに参加しました。");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("ルームに参加しました: " + PhotonNetwork.CurrentRoom.Name);
        if (PhotonNetwork.IsMasterClient)
        {
            hostIMG.gameObject.SetActive(true); // ホスト画像を表示
        }
        else
        {
            clientIMG.gameObject.SetActive(true); // クライアント画像を表示
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("ルームが正常に作成されました。");
        hostIMG.gameObject.SetActive(true); // ルーム作成後にホスト画像を表示
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"ルーム参加に失敗しました: {message}");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"ルーム作成に失敗しました: {message}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("ランダムルームへの参加に失敗しました。新しいルームを作成します...");
        CreateRoom(); // ルームを作成
    }
}
