using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI p1ScoreText;
    public TextMeshProUGUI p2ScoreText;
    public GameObject p1Panel; 
    public GameObject p2Panel; 

    private int player1Score = 0;
    private int player2Score = 0;
    private int currentPlayer = 1;
    private bool isOfflineMode = false; // 标志位，跟踪当前是否为离线模式
    private Outline player1Outline;
    private Outline player2Outline;

    void Start()
    {
        // 获取玩家对象的Outline组件
        player1Outline = p1Panel.GetComponent<Outline>();
        player2Outline = p2Panel.GetComponent<Outline>();

        // 确保初始时只有玩家1的对象发光
        UpdatePlayerOutline();
    }

    void OnEnable()
    {
        CardPressed.OnSameTypeCardsMatched += OnCardMatch;
        CardPressed.OnDifferentTypeCardsMatched += OnCardMismatch;
        ChooseGameMode.OnGameOffline += SetOfflineMode;
        ChooseGameMode.OnGameOnline += SetOnlineMode;
    }

    void OnDisable()
    {
        CardPressed.OnSameTypeCardsMatched -= OnCardMatch;
        CardPressed.OnDifferentTypeCardsMatched -= OnCardMismatch;
        ChooseGameMode.OnGameOffline -= SetOfflineMode;
        ChooseGameMode.OnGameOnline -= SetOnlineMode;
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

        // 检查总分数
        if (player1Score + player2Score >= 10)
        {
            if (player1Score > player2Score)
            {
                Debug.Log("P1 Win");
            }
            else if (player2Score > player1Score)
            {
                Debug.Log("P2 Win");
            }
            else
            {
                Debug.Log("Tie");
            }
        }
    }

    void OnCardMismatch()
    {
        // 只有在离线模式时才切换玩家
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
        currentPlayer = 1; // 开始时总是Player 1
        UpdatePlayerOutline();
        Debug.Log("Offline mode activated.");
    }

    void SetOnlineMode()
    {
        isOfflineMode = false;
        currentPlayer = 1; // 始终保持Player 1
        UpdatePlayerOutline();
        Debug.Log("Online mode activated.");
    }

    void UpdatePlayerOutline()
    {
        if (player1Outline != null && player2Outline != null)
        {
            // 根据当前玩家切换发光效果
            player1Outline.enabled = (currentPlayer == 1);
            player2Outline.enabled = (currentPlayer == 2);
        }
    }
}
