using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleInfoManager : MonoBehaviour
{
    public TMP_Text battleInfoText;
    public TMP_Text gameOverText;
    public Button submitButton;
    public PlayerActionHandler playerActionHandler;
    public Player2AI player2AI;
    public PlayerBalanceAndHP playerBalanceAndHP;
    public BattleAnimationManager battleAnimationManager;
    public string atkText;
    public string defText;
    public string healText;
    public string throwText;
    public string cntText;
    public string submitText;
    public string player1CannotMoveText = null;
    public string player2CannotMoveText = null;
    public bool isGameOver = false;

    private string player1Action;
    public string player2ActionText;

    void Update()
    {
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
            isGameOver = false;
        }
    }

    void OnEnable()
    {
        ButtonHandler.OnButtonPressed += UpdateBattleInfo;
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitButtonClicked);
        }

        if (playerActionHandler != null)
        {
            playerActionHandler.OnActionHandled += UpdateBattleInfoText;
        }
    }

    void OnDisable()
    {
        ButtonHandler.OnButtonPressed -= UpdateBattleInfo;
        if (submitButton != null)
        {
            submitButton.onClick.RemoveListener(OnSubmitButtonClicked);
        }

        if (playerActionHandler != null)
        {
            playerActionHandler.OnActionHandled -= UpdateBattleInfoText;
        }
    }

    void UpdateBattleInfo(string buttonType)
    {
        if (playerBalanceAndHP == null)
        {
            Debug.LogError("PlayerBalanceAndHP is not assigned.");
            return;
        }

        int damage = 0;
        int heal = 0;

        switch (buttonType)
        {
            case "ATK":
                battleInfoText.text = atkText;
                submitText = "攻撃";
                player1Action = "ATK";
                damage = playerActionHandler.atkDamage;
                break;
            case "DEF":
                battleInfoText.text = defText;
                submitText = "防御";
                player1Action = "DEF";
                break;
            case "Heal":
                battleInfoText.text = healText;
                submitText = "回復";
                player1Action = "Heal";
                heal = playerActionHandler.healingAmount;
                break;
            case "Throw":
                battleInfoText.text = throwText;
                submitText = "投げ技";
                player1Action = "Throw";
                damage = playerActionHandler.throwDamage;
                break;
            case "CNT":
                battleInfoText.text = cntText;
                submitText = "反撃";
                player1Action = "CNT";
                damage = playerActionHandler.cntDamage;
                break;
        }

        playerBalanceAndHP.ShowPreviewSliders();
        playerBalanceAndHP.UpdatePreview(damage, heal);
    }

    void OnSubmitButtonClicked()
    {
        if (player2AI == null)
        {
            Debug.LogError("Player2AI is not assigned.");
            return;
        }

        string player2Action = player2AI.DecidePlayer2Action();

        switch (player2Action)
        {
            case "ATK":
                player2ActionText = "攻撃";
                break;
            case "DEF":
                player2ActionText = "防御";
                break;
            case "Heal":
                player2ActionText = "回復";
                break;
            case "Throw":
                player2ActionText = "投げ技";
                break;
            case "CNT":
                player2ActionText = "反撃";
                break;
            default:
                player2ActionText = "未知";
                Debug.LogError("Unknown action for Player2: " + player2Action);
                break;
        }

        if (playerActionHandler != null)
        {
            playerActionHandler.HandlePlayerActions(player1Action, player2Action);
        }
        else
        {
            Debug.LogError("PlayerActionHandler is not assigned.");
        }

        if (playerBalanceAndHP.player1Balance != 0 && playerBalanceAndHP.player2Balance != 0)
        {
            battleInfoText.text = "自分: " + submitText + player1CannotMoveText + "\n相手: " + player2ActionText + player2CannotMoveText;
        }

        playerBalanceAndHP.HidePreviewSliders();
        playerBalanceAndHP.UpdateHPSliders();
        playerBalanceAndHP.UpdateBalance();
    }

    void UpdateBattleInfoText(string result)
    {
        battleInfoText.text += "\n" + result;
    }

    public void GameOver(string message)
    {
        isGameOver = true;
        battleInfoText.text = message;
        if (playerActionHandler.isPlayer1Win)
        {
            gameOverText.text = "You Win!";
        }
        else
        {
            gameOverText.text = "GameOver";
        }
    }

    public void RestartGame()
    {
        battleInfoText.text = "ゲーム再開"; // Reset the battle info text

        battleAnimationManager.ResetWhenRestart();

        // Reset player HP and balance
        playerBalanceAndHP.player1HP = playerBalanceAndHP.MaxHP;
        playerBalanceAndHP.player2HP = playerBalanceAndHP.MaxHP;
        playerBalanceAndHP.player1Balance = playerBalanceAndHP.MaxBalance;
        playerBalanceAndHP.player2Balance = playerBalanceAndHP.MaxBalance;

        playerActionHandler.player1CannotAct = false;
        playerActionHandler.player2CannotAct = false;

        // Reset sliders and balance indicators
        playerBalanceAndHP.UpdateHPSliders();
        playerBalanceAndHP.UpdateBalance();

        submitButton.interactable = true;

        // Reset all other necessary states and variables
        player1Action = null;
        player2ActionText = null;
        player1CannotMoveText = null;
        player2CannotMoveText = null;

        // Reset card hand
        CardEffect cardEffect = GetComponent<CardEffect>();
        if (cardEffect != null)
        {
            cardEffect.ResetHand();
        }
    }
}
