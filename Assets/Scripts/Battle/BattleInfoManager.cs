using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleInfoManager : MonoBehaviour
{
    public TMP_Text battleInfoText; // 戦闘情報テキスト
    public TMP_Text gameOverText; // ゲームオーバーテキスト
    public RawImage gameOverInfo; // ゲームオーバー情報
    public Button submitButton; // 送信ボタン
    public PlayerActionHandler playerActionHandler; // プレイヤーアクションハンドラー
    public Player2AI player2AI; // プレイヤー2のAI
    public PlayerBalanceAndHP playerBalanceAndHP; // プレイヤーのバランスとHP管理
    public BattleAnimationManager battleAnimationManager; // 戦闘アニメーション管理
    public string atkText; // 攻撃テキスト
    public string defText; // 防御テキスト
    public string healText; // 回復テキスト
    public string throwText; // 投げ技テキスト
    public string cntText; // 反撃テキスト
    public string submitText; // 送信テキスト
    public string player1CannotMoveText = null; // プレイヤー1が行動不能の場合のテキスト
    public string player2CannotMoveText = null; // プレイヤー2が行動不能の場合のテキスト
    public bool isGameOver = false; // ゲームオーバー状態かどうか
    public bool isNetBattle = false; // ネットバトルかどうか
    public GameObject[] stageBG; // ステージ背景の配列

    private string player1Action; // プレイヤー1のアクション
    public string player2ActionText; // プレイヤー2のアクションテキスト

    void Start()
    {
        UpdateStageBackground(); // ステージ背景を更新
    }

    void Update()
    {
        // ゲームオーバー時にRキーが押されたらゲームを再開
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
            isGameOver = false;
        }
    }

    void OnEnable()
    {
        ButtonHandler.OnButtonPressed += UpdateBattleInfo; // ボタン押下イベントに登録
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitButtonClicked); // 送信ボタンのクリックリスナーを追加
        }

        if (playerActionHandler != null)
        {
            playerActionHandler.OnActionHandled += UpdateBattleInfoText; // アクション処理イベントに登録
        }
    }

    void OnDisable()
    {
        ButtonHandler.OnButtonPressed -= UpdateBattleInfo; // ボタン押下イベントから登録解除
        if (submitButton != null)
        {
            submitButton.onClick.RemoveListener(OnSubmitButtonClicked); // 送信ボタンのクリックリスナーを削除
        }

        if (playerActionHandler != null)
        {
            playerActionHandler.OnActionHandled -= UpdateBattleInfoText; // アクション処理イベントから登録解除
        }
    }

    private void UpdateStageBackground()
    {
        int currentIndex = GameStateManager.lastSelectedIndex; // 現在の選択インデックス

        if (!isNetBattle)
        {
            // ステージ背景を更新
            for (int i = 0; i < stageBG.Length; i++)
            {
                stageBG[i].SetActive(i == currentIndex);
            }
        }
        else
        {
            stageBG[1].SetActive(true); // ネットバトルの場合、特定の背景をアクティブにする
        }
    }

    void UpdateBattleInfo(string buttonType)
    {
        if (playerBalanceAndHP == null)
        {
            Debug.LogError("PlayerBalanceAndHP が設定されていません。");
            return;
        }

        int damage = 0;
        int heal = 0;

        // ボタンの種類に応じてバトル情報を更新
        switch (buttonType)
        {
            case "ATK":
                battleInfoText.text = atkText; // 攻撃テキスト
                submitText = "攻撃";
                player1Action = "ATK";
                damage = playerActionHandler.atkDamage; // 攻撃のダメージ
                break;
            case "DEF":
                battleInfoText.text = defText; // 防御テキスト
                submitText = "防御";
                player1Action = "DEF";
                break;
            case "Heal":
                battleInfoText.text = healText; // 回復テキスト
                submitText = "回復";
                player1Action = "Heal";
                heal = playerActionHandler.healingAmount; // 回復量
                break;
            case "Throw":
                battleInfoText.text = throwText; // 投げ技テキスト
                submitText = "投げ技";
                player1Action = "Throw";
                damage = playerActionHandler.throwDamage; // 投げ技のダメージ
                break;
            case "CNT":
                battleInfoText.text = cntText; // 反撃テキスト
                submitText = "反撃";
                player1Action = "CNT";
                damage = playerActionHandler.cntDamage; // 反撃のダメージ
                break;
        }

        playerBalanceAndHP.ShowPreviewSliders(); // プレビューを表示
        playerBalanceAndHP.UpdatePreview(damage, heal); // プレビューを更新
    }

    public void OnSubmitButtonClicked()
    {
        if (player2AI == null)
        {
            Debug.LogError("Player2AI が設定されていません。");
            return;
        }

        string player2Action = player2AI.DecidePlayer2Action(); // プレイヤー2のアクションを決定

        // プレイヤー2のアクションに応じてテキストを設定
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
            playerActionHandler.HandlePlayerActions(player1Action, player2Action); // プレイヤーのアクションを処理
        }
        else
        {
            Debug.LogError("PlayerActionHandler が設定されていません。");
        }

        if (playerBalanceAndHP.player1Balance != 0 && playerBalanceAndHP.player2Balance != 0)
        {
            battleInfoText.text = "自分: " + submitText + player1CannotMoveText + "\n相手: " + player2ActionText + player2CannotMoveText;
        }

        playerBalanceAndHP.HidePreviewSliders(); // プレビューを非表示
        playerBalanceAndHP.UpdateHPSliders(); // HPスライダーを更新
        playerBalanceAndHP.UpdateBalance(); // バランスを更新
    }

    void UpdateBattleInfoText(string result)
    {
        battleInfoText.text += "\n" + result; // 戦闘結果を追加
    }

    public void GameOver(string message)
    {
        isGameOver = true; // ゲームオーバー状態に設定
        battleInfoText.text = message; // 戦闘情報テキストにメッセージを設定
        if (playerActionHandler.isPlayer1Win)
        {
            gameOverText.text = "You Win!";
            gameOverInfo.color = new Color(0.3f, 0.63f, 1f, 0.78f); // #50D9FF, 透明度200
        }
        else
        {
            gameOverText.text = "GameOver";
            gameOverInfo.color = new Color(1f, 0.3f, 0.3f, 0.78f); // #50D9FF, 透明度200
        }
    }

    public void RestartGame()
    {
        battleInfoText.text = "ゲーム再開"; // 戦闘情報テキストをリセット

        battleAnimationManager.ResetWhenRestart(); // アニメーションをリセット

        // プレイヤーのHPとバランスをリセット
        playerBalanceAndHP.player1HP = playerBalanceAndHP.MaxHP;
        playerBalanceAndHP.player2HP = playerBalanceAndHP.MaxHP;
        playerBalanceAndHP.player1Balance = playerBalanceAndHP.MaxBalance;
        playerBalanceAndHP.player2Balance = playerBalanceAndHP.MaxBalance;

        playerActionHandler.player1CannotAct = false;
        playerActionHandler.player2CannotAct = false;

        // スライダーとバランスインジケーターをリセット
        playerBalanceAndHP.UpdateHPSliders();
        playerBalanceAndHP.UpdateBalance();

        submitButton.interactable = true;

        // 必要なすべての状態と変数をリセット
        player1Action = null;
        player2ActionText = null;
        player1CannotMoveText = null;
        player2CannotMoveText = null;

        // カードの手札をリセット
        CardEffect cardEffect = GetComponent<CardEffect>();
        if (cardEffect != null)
        {
            cardEffect.ResetHand();
        }
    }
}
