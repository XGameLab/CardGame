using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ScoreManager : MonoBehaviourPunCallbacks
{
    public static ScoreManager Instance { get; private set; }
    public TextMeshProUGUI gameModeText;
    public TextMeshProUGUI p1ScoreText;
    public TextMeshProUGUI p2ScoreText;
    public TextMeshProUGUI winnerText;
    public GameObject winnerObj;
    public GameObject exitObj; 
    public GameObject p1Panel;
    public GameObject p2Panel;
    public Button retryButton;
    public Button exitButton;

    private int player1Score = 0;
    private int player2Score = 0;
    private int currentPlayer = 1;
    public int CurrentPlayer
    {
        get { return currentPlayer; }
        private set { currentPlayer = value; } // 设置为 private 来保护
    }
    private bool isOfflineMode = false;
    public bool IsOfflineMode => isOfflineMode;
    private Outline player1Outline;
    private Outline player2Outline;

    public static event System.Action OnRetryGame;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        winnerText.text = "";
        winnerObj.SetActive(false);
        exitObj.SetActive(false);
        retryButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);

        retryButton.onClick.AddListener(RetryGame);
        exitButton.onClick.AddListener(OnExitButtonClicked);

        player1Outline = p1Panel.GetComponent<Outline>();
        player2Outline = p2Panel.GetComponent<Outline>();

        UpdatePlayerOutline();
    }

    void OnEnable()
    {
        CardPressed.OnSameTypeCardsMatched += OnCardMatch;
        CardPressed.OnDifferentTypeCardsMatched += OnCardMismatch;
        ChooseGameMode.OnGameOffline += SetOfflineMode;
        ChooseGameMode.OnGameOnline += SetOnlineMode;
        ChooseGameMode.OnGameStart += SetCurrentPlayer;
    }

    void OnDisable()
    {
        CardPressed.OnSameTypeCardsMatched -= OnCardMatch;
        CardPressed.OnDifferentTypeCardsMatched -= OnCardMismatch;
        ChooseGameMode.OnGameOffline -= SetOfflineMode;
        ChooseGameMode.OnGameOnline -= SetOnlineMode;
        ChooseGameMode.OnGameStart -= SetCurrentPlayer;
    }

    void OnCardMatch()
    {
        if (currentPlayer == 1)
        {
            player1Score += 1;
            p1ScoreText.text = player1Score.ToString();
            Debug.Log("Player 1 Score: " + player1Score);
        }
        else
        {
            player2Score += 1;
            p2ScoreText.text = player2Score.ToString();
            Debug.Log("Player 2 Score: " + player2Score);
        }

        // 同步分数到所有客户端
        if (!isOfflineMode)
        {
            photonView.RPC("UpdateScore", RpcTarget.Others, player1Score, player2Score);
        }

        // 检查胜利条件
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

            // 显示胜利信息
            winnerText.text = winnerMessage;
            // 仅在在线模式下调用RPC显示胜利信息
            if (!isOfflineMode)
            {
                photonView.RPC("ShowWinner", RpcTarget.All, winnerMessage);
            }
            else
            {
                ShowWinner(winnerMessage); // 在离线模式下直接调用本地方法
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

        Cursor.visible = true; // 隐藏光标
    }


    [PunRPC]
    void UpdateScore(int p1Score, int p2Score)
    {
        player1Score = p1Score;
        player2Score = p2Score;

        p1ScoreText.text = player1Score.ToString();
        p2ScoreText.text = player2Score.ToString();
        
        Debug.Log("Scores updated. Player 1: " + player1Score + ", Player 2: " + player2Score);
    }

    void OnCardMismatch()
    {
        if (isOfflineMode)
        {
            // 直接切换玩家
            SwitchPlayer();
        }
        else
        {
            // 客户端请求切换玩家
            if (!PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RequestPlayerSwitch", RpcTarget.MasterClient);
            }
            else
            {
                SwitchPlayer();
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
            // 直接调用本地更新方法
            UpdatePlayerOutline();
            Debug.Log("Switching to Player " + currentPlayer);
        }
        else
        {
            // 使用RPC同步给所有客户端
            photonView.RPC("UpdateCurrentPlayer", RpcTarget.All, currentPlayer);
        }
    }

    [PunRPC]
    void UpdateCurrentPlayer(int player)
    {
        currentPlayer = player;
        UpdatePlayerOutline();
        Debug.Log("Switching to Player " + currentPlayer);
    }

    void SetOfflineMode()
    {
        isOfflineMode = true;
        currentPlayer = 1;
        UpdatePlayerOutline();
        gameModeText.text = "オフライン モード";
        Debug.Log("Offline mode activated.");
    }

    void SetOnlineMode()
    {
        isOfflineMode = false;
        currentPlayer = 1;
        UpdatePlayerOutline();
        gameModeText.text = "オンライン モード";
        Debug.Log("Online mode activated.");
    }

    void SetCurrentPlayer(int player)
    {
        currentPlayer = player;
        UpdatePlayerOutline();
        Debug.Log("Current Player set to: " + currentPlayer);
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
        Cursor.visible = false; // 隐藏光标

        player1Score = 0;
        player2Score = 0;
        p1ScoreText.text = player1Score.ToString();
        p2ScoreText.text = player2Score.ToString();

        winnerText.text = "";
        winnerObj.SetActive(false);
        retryButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);

        UpdatePlayerOutline();

        Debug.Log("Game Reset!");

        OnRetryGame?.Invoke();
    }

    void OnExitButtonClicked()
    {
        if (isOfflineMode)
        {
            ShowExit();
        }
        else
        {
            photonView.RPC("ShowExit", RpcTarget.All);
        }
    }

    [PunRPC]
    void ShowExit()
    {   
        if(!isOfflineMode)
        {
            exitObj.SetActive(true);  // 使 exitObj 可见
            Debug.Log("Exit Object is now visible.");
        }  
    }
}
