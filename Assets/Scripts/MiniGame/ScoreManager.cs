using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI gameModeText;
    public TextMeshProUGUI p1ScoreText;
    public TextMeshProUGUI p2ScoreText;
    public TextMeshProUGUI winnerText;
    public GameObject winnerObj;
    public GameObject p1Panel; 
    public GameObject p2Panel; 
    public Button retryButton;

    private int player1Score = 0;
    private int player2Score = 0;
    private int currentPlayer = 1;
    private bool isOfflineMode = false;
    private Outline player1Outline;
    private Outline player2Outline;

    public static event System.Action OnRetryGame;

    void Start()
    {
        winnerText.text = "";
        winnerObj.SetActive(false);
        retryButton.gameObject.SetActive(false);
        retryButton.onClick.AddListener(RetryGame);

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
        ChooseGameMode.OnGameStart += SetCurrentPlayer; // 添加事件监听
    }

    void OnDisable()
    {
        CardPressed.OnSameTypeCardsMatched -= OnCardMatch;
        CardPressed.OnDifferentTypeCardsMatched -= OnCardMismatch;
        ChooseGameMode.OnGameOffline -= SetOfflineMode;
        ChooseGameMode.OnGameOnline -= SetOnlineMode;
        ChooseGameMode.OnGameStart -= SetCurrentPlayer; // 移除事件监听
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

        if (player1Score + player2Score >= 10)
        {
            winnerObj.SetActive(true);
            retryButton.gameObject.SetActive(true);
            if (player1Score > player2Score)
            {
                winnerText.text = "Winner: Player1";
            }
            else if (player2Score > player1Score)
            {
                winnerText.text = "Winner: Player2";
            }
            else
            {
                winnerText.text = "Tie Game!";
            }
        }
    }

    void OnCardMismatch()
    {
        if (isOfflineMode)
        {
            currentPlayer = (currentPlayer == 1) ? 2 : 1;
            UpdatePlayerOutline();
            Debug.Log("Switching to Player " + currentPlayer);
        }
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
        player1Score = 0;
        player2Score = 0;
        p1ScoreText.text = player1Score.ToString();
        p2ScoreText.text = player2Score.ToString();

        winnerText.text = "";
        winnerObj.SetActive(false);
        retryButton.gameObject.SetActive(false);

        UpdatePlayerOutline();

        Debug.Log("Game Reset!");

        OnRetryGame?.Invoke();
    }
}
