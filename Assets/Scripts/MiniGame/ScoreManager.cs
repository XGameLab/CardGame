using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ScoreManager : MonoBehaviourPunCallbacks
{
    public static ScoreManager Instance { get; private set; }
    public TextMeshProUGUI gameModeText; // ゲームモードのテキスト表示
    public TextMeshProUGUI p1ScoreText; // プレイヤー1のスコア表示
    public TextMeshProUGUI p2ScoreText; // プレイヤー2のスコア表示
    public TextMeshProUGUI winnerText; // 勝者のテキスト表示
    public GameObject winnerObj; // 勝者オブジェクト
    public GameObject exitObj; // 終了オブジェクト
    public GameObject p1Panel; // プレイヤー1のパネル
    public GameObject p2Panel; // プレイヤー2のパネル
    public Button retryButton; // リトライボタン
    public Button exitButton; // 終了ボタン

    private int player1Score = 0; // プレイヤー1のスコア
    private int player2Score = 0; // プレイヤー2のスコア
    private int currentPlayer = 1; // 現在のプレイヤー
    public int CurrentPlayer
    {
        get { return currentPlayer; }
        private set { currentPlayer = value; } // プライベートに設定して保護
    }
    private bool isOfflineMode = false; // オフラインモードフラグ
    public bool IsOfflineMode => isOfflineMode;
    private Outline player1Outline; // プレイヤー1のアウトライン
    private Outline player2Outline; // プレイヤー2のアウトライン

    public static event System.Action OnRetryGame; // リトライイベント

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject); // インスタンスが既に存在する場合、現在のオブジェクトを破棄
        }
        else
        {
            Instance = this; // シングルトンインスタンスの設定
        }
    }

    void Start()
    {
        winnerText.text = "";
        winnerObj.SetActive(false);
        exitObj.SetActive(false);
        retryButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);

        retryButton.onClick.AddListener(RetryGame); // リトライボタンのクリックイベントを追加
        exitButton.onClick.AddListener(OnExitButtonClicked); // 終了ボタンのクリックイベントを追加

        player1Outline = p1Panel.GetComponent<Outline>();
        player2Outline = p2Panel.GetComponent<Outline>();

        UpdatePlayerOutline(); // プレイヤーのアウトラインを更新
    }

    void OnEnable()
    {
        CardPressed.OnSameTypeCardsMatched += OnCardMatch; // カード一致時のイベントリスナーを追加
        CardPressed.OnDifferentTypeCardsMatched += OnCardMismatch; // カード不一致時のイベントリスナーを追加
        ChooseGameMode.OnGameOffline += SetOfflineMode; // オフラインモード設定のイベントリスナーを追加
        ChooseGameMode.OnGameOnline += SetOnlineMode; // オンラインモード設定のイベントリスナーを追加
        ChooseGameMode.OnGameStart += SetCurrentPlayer; // 現在のプレイヤー設定のイベントリスナーを追加
    }

    void OnDisable()
    {
        CardPressed.OnSameTypeCardsMatched -= OnCardMatch; // イベントリスナーを削除
        CardPressed.OnDifferentTypeCardsMatched -= OnCardMismatch; // イベントリスナーを削除
        ChooseGameMode.OnGameOffline -= SetOfflineMode; // イベントリスナーを削除
        ChooseGameMode.OnGameOnline -= SetOnlineMode; // イベントリスナーを削除
        ChooseGameMode.OnGameStart -= SetCurrentPlayer; // イベントリスナーを削除
    }

    void OnCardMatch()
    {
        if (currentPlayer == 1)
        {
            player1Score += 1;
            p1ScoreText.text = player1Score.ToString();
            Debug.Log("プレイヤー1のスコア: " + player1Score);
        }
        else
        {
            player2Score += 1;
            p2ScoreText.text = player2Score.ToString();
            Debug.Log("プレイヤー2のスコア: " + player2Score);
        }

        // スコアをすべてのクライアントに同期
        if (!isOfflineMode)
        {
            photonView.RPC("UpdateScore", RpcTarget.Others, player1Score, player2Score);
        }

        // 勝利条件をチェック
        if ((isOfflineMode && player1Score + player2Score >= 10) || (!isOfflineMode && (player1Score >= 10 || player2Score >= 10)))
        {
            string winnerMessage;
            if (player1Score > player2Score)
            {
                winnerMessage = "Winner: Player1";
            }
            else if (player2Score > player1Score)
            {
                winnerMessage = "Winner: Player2";
            }
            else
            {
                winnerMessage = "Tie Game!";
            }

            // 勝者のメッセージを表示
            winnerText.text = winnerMessage;
            if (!isOfflineMode)
            {
                photonView.RPC("ShowWinner", RpcTarget.All, winnerMessage); // 勝者の表示をすべてのクライアントに同期
            }
            else
            {
                ShowWinner(winnerMessage); // オフラインモードの場合はローカルで表示
            }
        }
    }

    [PunRPC]
    void ShowWinner(string winnerMessage)
    {
        winnerText.text = winnerMessage;
        winnerObj.SetActive(true);
        retryButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);

        Cursor.visible = true; // 光标を表示
    }

    [PunRPC]
    void UpdateScore(int p1Score, int p2Score)
    {
        player1Score = p1Score;
        player2Score = p2Score;

        p1ScoreText.text = player1Score.ToString();
        p2ScoreText.text = player2Score.ToString();
        
        Debug.Log("スコアが更新されました。プレイヤー1: " + player1Score + ", プレイヤー2: " + player2Score);
    }

    void OnCardMismatch()
    {
        if (isOfflineMode)
        {
            SwitchPlayer(); // オフラインモードではプレイヤーを切り替える
        }
        else
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RequestPlayerSwitch", RpcTarget.MasterClient); // マスタークライアントにプレイヤー切り替えをリクエスト
            }
            else
            {
                SwitchPlayer(); // マスタークライアントの場合は直接プレイヤーを切り替える
            }
        }
    }

    [PunRPC]
    void RequestPlayerSwitch()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SwitchPlayer();
        }
    }

    void SwitchPlayer()
    {
        currentPlayer = (currentPlayer == 1) ? 2 : 1;

        if (isOfflineMode)
        {
            UpdatePlayerOutline(); // オフラインモードではローカルでプレイヤーを更新
            Debug.Log("プレイヤーを切り替え: " + currentPlayer);
        }
        else
        {
            photonView.RPC("UpdateCurrentPlayer", RpcTarget.All, currentPlayer); // プレイヤー切り替えをすべてのクライアントに同期
        }
    }

    [PunRPC]
    void UpdateCurrentPlayer(int player)
    {
        currentPlayer = player;
        UpdatePlayerOutline(); // プレイヤーのアウトラインを更新
        Debug.Log("プレイヤーを切り替え: " + currentPlayer);
    }

    void SetOfflineMode()
    {
        isOfflineMode = true;
        currentPlayer = 1;
        UpdatePlayerOutline(); // プレイヤーのアウトラインを更新
        gameModeText.text = "オフライン モード";
        Debug.Log("オフラインモードが有効になりました。");
    }

    void SetOnlineMode()
    {
        isOfflineMode = false;
        currentPlayer = 1;
        UpdatePlayerOutline(); // プレイヤーのアウトラインを更新
        gameModeText.text = "オンライン モード";
        Debug.Log("オンラインモードが有効になりました。");
    }

    void SetCurrentPlayer(int player)
    {
        currentPlayer = player;
        UpdatePlayerOutline(); // プレイヤーのアウトラインを更新
        Debug.Log("現在のプレイヤー: " + currentPlayer);
    }

    void UpdatePlayerOutline()
    {
        if (player1Outline != null && player2Outline != null)
        {
            player1Outline.enabled = (currentPlayer == 1);
            player2Outline.enabled = (currentPlayer == 2);
        }
    }

    void RetryGame()
    {
        Cursor.visible = false; // 光标を非表示

        player1Score = 0;
        player2Score = 0;
        p1ScoreText.text = player1Score.ToString();
        p2ScoreText.text = player2Score.ToString();

        winnerText.text = "";
        winnerObj.SetActive(false);
        retryButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);

        UpdatePlayerOutline(); // プレイヤーのアウトラインを更新

        Debug.Log("ゲームがリセットされました。");

        OnRetryGame?.Invoke(); // リトライイベントを発行
    }

    void OnExitButtonClicked()
    {
        if (isOfflineMode)
        {
            ShowExit(); // オフラインモードではローカルで終了を表示
        }
        else
        {
            photonView.RPC("ShowExit", RpcTarget.All); // オンラインモードではRPCで終了を表示
        }
    }

    [PunRPC]
    void ShowExit()
    {
        exitObj.SetActive(true); // exitObjを表示
        Debug.Log("Exitオブジェクトが表示されました。");
    }
}
