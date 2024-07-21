using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RPSGame : MonoBehaviour
{
    public Button player1SubmitButton;
    public Button restartButton;
    public Button continueButton;

    public Text player1ChoiceText;
    public Text player2ChoiceText;
    public Text resultText;
    public GameObject player1Heart;
    public GameObject player2Heart;
    public GameObject player1NoBalance;
    public GameObject player2NoBalance;
    public GameObject[] player1BalanceIndicators;
    public GameObject[] player2BalanceIndicators;

    public Sprite attackSprite;
    public Sprite defendSprite;
    public Sprite healSprite;
    public Sprite throwSprite;
    public Sprite counterSprite;

    public Image player1DefendIcon;
    public Image player2DefendIcon;

    public int player1MaxHP = 12;
    public int player2MaxHP = 12;
    public int player1HP = 12;
    public int player2HP = 12;
    public int player1MaxBalance = 3;
    public int player2MaxBalance = 3;
    public int player1Balance = 3;
    public int player2Balance = 3;

    public int AtkDamage = 6;
    public int AtkToAtkDamage = 3;
    public int HealPoint = 7;
    public int ThrowDamage = 4;
    public int ThrowToThrowDamage = 2;
    public int CounterDamage = 4;
    public int CounterFailedDamage = 3;
    public int preDamage = 0;

    private string player1Choice = "";
    private string player2Choice = "";

    private bool player1Submitted = false;

    private int turnCount = 1;

    private bool player1DefendNextRound = false;
    private bool player2DefendNextRound = false;

    private bool player1BalanceZeroNextTurn = false;
    private bool player2BalanceZeroNextTurn = false;

    private bool player1SkipNextTurn = false;
    private bool player2SkipNextTurn = false;

    private Button[] allButtons;
    private Button[] player1Buttons = new Button[5];
    private Button[] player2Buttons = new Button[5];

    private Dictionary<Button, string> buttonActions = new Dictionary<Button, string>();

    private ColorBlock normalColorBlock;
    private ColorBlock selectedColorBlock;

    // 防御逻辑处理
    private float player1DefendChance = 0;
    private float player2DefendChance = 0;

    //キャラのHPバー
    public Slider player1HPSlider;
    public Slider player2HPSlider;
    public Slider player1HealPreviewSlider;
    public Slider player2DamagePreviewSlider;

    private RectTransform player1HandleRectTransform;
    private RectTransform player2HandleRectTransform;

    void Start()
    {
        allButtons = new Button[] {
            player1SubmitButton,
            restartButton,
            continueButton
        };

        List<Button> buttonList = new List<Button>(FindObjectsOfType<Button>());
        buttonList.Remove(player1SubmitButton);
        buttonList.Remove(restartButton);
        buttonList.Remove(continueButton);

        if (buttonList.Count < 10)
        {
            Debug.LogError("Button list must contain at least 10 buttons.");
            return;
        }

        buttonList.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        for (int i = 0; i < 5; i++)
        {
            player1Buttons[i] = buttonList[i];
            player2Buttons[i] = buttonList[i + 5];
        }

        // 获取初始颜色块
        normalColorBlock = player1Buttons[0].colors;

        ShuffleAndAssignActions();

        player1SubmitButton.onClick.AddListener(Player1Submit);
        restartButton.onClick.AddListener(RestartGame);

        resultText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);

        player1SubmitButton.interactable = false;

        player1DefendIcon.gameObject.SetActive(false);
        player2DefendIcon.gameObject.SetActive(false);

        player1NoBalance.SetActive(false);
        player2NoBalance.SetActive(false);

        EnablePlayer1Buttons(true);

        // スライダーの初期設定
        player1HPSlider.maxValue = player1MaxHP;
        player1HPSlider.value = player1HP;
        player2HPSlider.maxValue = player2MaxHP;
        player2HPSlider.value = player2HP;

        player1HealPreviewSlider.maxValue = player1MaxHP;
        player1HealPreviewSlider.value = player1HP;
        player1HealPreviewSlider.gameObject.SetActive(false);

        player2DamagePreviewSlider.maxValue = player2MaxHP;
        player2DamagePreviewSlider.value = player2HP;
        player2DamagePreviewSlider.gameObject.SetActive(false);

        // ハンドルのRectTransformを取得
        player1HandleRectTransform = player1HPSlider.handleRect.GetComponent<RectTransform>();
        player2HandleRectTransform = player2HPSlider.handleRect.GetComponent<RectTransform>();

    }

    void Update()
    {
        // HPの変動に応じてスライダーを更新
        player1HPSlider.value = player1HP;
        player2HPSlider.value = player2HP;

        // 更新心形scale
        UpdateHeartScale();

        // Debug専用、後で消す
        if (Input.GetKeyDown(KeyCode.K))
        {
            player2HP -= player2MaxHP;
        }
    }


    void Shuffle(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            string temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void ShuffleAndAssignActions()
    {
        List<string> actions = new List<string> { "Attack", "Defend", "Heal", "Throw", "Counter", "Attack", "Defend", "Heal", "Throw", "Counter" };
        Shuffle(actions);

        for (int i = 0; i < 5; i++)
        {
            AssignButtonFunctionAndImage(player1Buttons[i], actions[i], true);
            AssignButtonFunctionAndImage(player2Buttons[i], actions[i + 5], false);
        }

        ResetButtonColors();
    }

    void AssignButtonFunctionAndImage(Button button, string action, bool isPlayer1)
    {
        button.onClick.RemoveAllListeners();
        buttonActions[button] = action;
        button.onClick.AddListener(() => {
            if (isPlayer1)
                Player1Choose(action);
        });

        switch (action)
        {
            case "Attack":
                button.GetComponent<Image>().sprite = attackSprite;
                break;
            case "Defend":
                button.GetComponent<Image>().sprite = defendSprite;
                break;
            case "Heal":
                button.GetComponent<Image>().sprite = healSprite;
                break;
            case "Throw":
                button.GetComponent<Image>().sprite = throwSprite;
                break;
            case "Counter":
                button.GetComponent<Image>().sprite = counterSprite;
                break;
        }
    }

    void Player1Choose(string choice)
    {
        player1Choice = choice;
        player1ChoiceText.text = "Player 1 chose: " + choice;
        player1SubmitButton.interactable = true;

        // 预览伤害和治疗
        int damagePreview = player2HP;
        int healPreview = player1HP;

    if(player1Balance > 0)
    {
        switch (choice)
        {
            case "Attack":
                damagePreview = player2HP - AtkDamage;
                break;
            case "Throw":
                damagePreview = player2HP - ThrowDamage;
                break;
            case "Counter":
                damagePreview = player2HP - CounterDamage;
                break;
            case "Heal":
                healPreview = Mathf.Min(player1HP + HealPoint, player1MaxHP);
                break;
            default:
                break;
        }
    }

        player2DamagePreviewSlider.value = Mathf.Max(damagePreview, 0);
        player2DamagePreviewSlider.gameObject.SetActive(true);

        player1HealPreviewSlider.value = healPreview;
        player1HealPreviewSlider.gameObject.SetActive(true);

        player1NoBalance.SetActive(player1Balance == 0);
    }

    void Player1Submit()
    {
        player1Submitted = true;
        player1SubmitButton.interactable = false;
        EnablePlayer1Buttons(false);
        player2DamagePreviewSlider.gameObject.SetActive(false); // 隐藏伤害预览滑动条
        player1HealPreviewSlider.gameObject.SetActive(false); // 隐藏治疗预览滑动条
        StartCoroutine(CPUChoose());
    }

    IEnumerator CPUChoose()
    {
        List<string> cpuActions = new List<string>();
        foreach (var button in player2Buttons)
        {
            cpuActions.Add(buttonActions[button]);
        }

        int randomIndex = Random.Range(0, cpuActions.Count);
        player2Choice = cpuActions[randomIndex];
        player2ChoiceText.text = "Player 2 chose: " + player2Choice;

        // 改变选中按钮的颜色
        foreach (var button in player2Buttons)
        {
            if (buttonActions[button] == player2Choice)
            {
                var colors = button.colors;
                colors.normalColor = colors.selectedColor; // 使用selectedColor作为选中效果
                button.colors = colors;
                break;
            }
        }

        player2NoBalance.SetActive(player2Balance == 0);

        yield return new WaitForSeconds(1); // 等待

        CheckResult();
    }

    void CheckResult()
    {
        if (player1Submitted)
        {
            bool player1TookDamage = false;
            bool player2TookDamage = false;

            int player1HPChange = 0;
            int player2HPChange = 0;

            // 检查玩家是否因为平衡值为零导致动作无效
            if (player1BalanceZeroNextTurn)
            {
                player1SkipNextTurn = true;
                player1BalanceZeroNextTurn = false;
                player1Balance = player1MaxBalance; // 恢复平衡值
            }
            if (player2BalanceZeroNextTurn)
            {
                player2SkipNextTurn = true;
                player2BalanceZeroNextTurn = false;
                player2Balance = player2MaxBalance; // 恢复平衡值
            }

            // 检查是否需要跳过回合
            if (player1SkipNextTurn)
            {
                player1Choice = "None";
                player1SkipNextTurn = false;
            }
            if (player2SkipNextTurn)
            {
                player2Choice = "None";
                player2SkipNextTurn = false;
            }

            // 处理平衡机制
            if (player1Choice == "Attack" && (player2Choice == "Attack" || player2Choice == "Defend" || player2Choice == "Counter"))
            {
                player1Balance--;
            }
            if (player2Choice == "Attack" && (player1Choice == "Attack" || player1Choice == "Defend" || player1Choice == "Counter"))
            {
                player2Balance--;
            }
            if (player1Choice == "Attack" && player2Choice == "Heal")
            {
                player2Balance--;
            }
            if (player2Choice == "Attack" && player1Choice == "Heal")
            {
                player1Balance--;
            }
            if (player1Choice == "Throw" && (player2Choice == "Attack" || player2Choice == "Counter"))
            {
                player1Balance--;
            }
            if (player2Choice == "Throw" && (player1Choice == "Attack" || player1Choice == "Counter"))
            {
                player2Balance--;
            }
            if (player1Choice == "Counter" && (player2Choice == "Defend" || player2Choice == "Heal"))
            {
                player1Balance--;
            }
            if (player2Choice == "Counter" && (player1Choice == "Defend" || player1Choice == "Heal"))
            {
                player2Balance--;
            }
            if (player1Choice == "Heal" && (player2Choice == "Defend" || player2Choice == "Heal" || player2Choice == "Counter"))
            {
                player1Balance = Mathf.Min(player1Balance + 1, player1MaxBalance);
            }
            if (player2Choice == "Heal" && (player1Choice == "Defend" || player1Choice == "Heal" || player1Choice == "Counter"))
            {
                player2Balance = Mathf.Min(player2Balance + 1, player2MaxBalance);
            }

            // 如果平衡值为零，下一个回合动作无效
            if (player1Balance <= 0)
            {
                player1BalanceZeroNextTurn = true;
                player1Balance = 0; // 保持平衡值为零以触发下一回合无效
            }
            if (player2Balance <= 0)
            {
                player2BalanceZeroNextTurn = true;
                player2Balance = 0; // 保持平衡值为零以触发下一回合无效
            }

            // 处理玩家选择为“None”的情况
            if (player1Choice != "None" && player2Choice != "None")
            {
                // 攻击对攻击
                if (player1Choice == "Attack" && player2Choice == "Attack")
                {
                    player1HP -= AtkToAtkDamage;
                    player2HP -= AtkToAtkDamage;
                    player1HPChange = -AtkToAtkDamage;
                    player2HPChange = -AtkToAtkDamage;
                    player1TookDamage = true;
                    player2TookDamage = true;
                }
                else
                {
                    // Player1 攻击
                    if (player1Choice == "Attack")
                    {
                        preDamage = AtkDamage;
                        if (player2Choice == "Heal" || player2Choice == "Throw")
                        {
                            player2HP -= AtkDamage;
                            player2HPChange = -AtkDamage;
                            player2TookDamage = true;
                        }
                        else if (player2Choice == "Defend")
                        {
                            player1TookDamage = false;
                        }
                        else if (player2Choice == "Counter")
                        {
                            player1TookDamage = false;
                        }
                    }

                    // Player2 攻击
                    if (player2Choice == "Attack")
                    {
                        if (player1Choice == "Heal" || player1Choice == "Throw")
                        {
                            player1HP -= AtkDamage;
                            player1HPChange = -AtkDamage;
                            player1TookDamage = true;
                        }
                        else if (player1Choice == "Defend")
                        {
                            player2TookDamage = false;
                        }
                        else if (player1Choice == "Counter")
                        {
                            player2TookDamage = false;
                        }
                    }
                }

                // 防御逻辑处理
                if (player1Choice == "Defend")
                {
                    preDamage = 0;
                    if (player2Choice == "Attack")
                    {
                        player1TookDamage = false;
                    }
                    else if (player2Choice == "Throw")
                    {
                        player1DefendChance = Random.value;

                        if (player1DefendChance <= 0.25f)
                        {
                            player1TookDamage = false;
                        }
                        else
                        {
                            player1HP -= ThrowDamage;
                            player1HPChange = -ThrowDamage;
                            player1TookDamage = true;
                            player1Balance--;
                        }
                    }
                    else if (player2Choice == "Counter")
                    {
                        player1TookDamage = false;
                    }
                }

                if (player2Choice == "Defend")
                {
                    if (player1Choice == "Attack")
                    {
                        player2TookDamage = false;
                    }
                    else if (player1Choice == "Throw")
                    {
                        player2DefendChance = Random.value;

                        if (player2DefendChance <= 0.25f)
                        {
                            player2TookDamage = false;
                        }
                        else
                        {
                            player2HP -= ThrowDamage;
                            player2HPChange = -ThrowDamage;
                            player2TookDamage = true;
                            player2Balance--;
                        }
                    }
                    else if (player1Choice == "Counter")
                    {
                        player2TookDamage = false;
                    }
                }

                // 治疗逻辑
                if (player1Choice == "Heal")
                {
                    if (player2Choice == "Defend" || player2Choice == "Heal" || player2Choice == "Counter")
                    {
                        int healedAmount = Mathf.Min(player1MaxHP - player1HP, HealPoint);
                        player1HP += healedAmount;
                        player1HPChange = healedAmount;
                    }
                }
                if (player2Choice == "Heal")
                {
                    if (player1Choice == "Defend" || player1Choice == "Heal" || player1Choice == "Counter")
                    {
                        int healedAmount = Mathf.Min(player2MaxHP - player2HP, HealPoint);
                        player2HP += healedAmount;
                        player2HPChange = healedAmount;
                    }
                }

                // 投掷对投掷
                if (player1Choice == "Throw" && player2Choice == "Throw")
                {
                    player1HP -= ThrowToThrowDamage;
                    player2HP -= ThrowToThrowDamage;
                    player1HPChange = -ThrowToThrowDamage;
                    player2HPChange = -ThrowToThrowDamage;
                    player1TookDamage = true;
                    player2TookDamage = true;
                }
                else
                {
                    if (player1Choice == "Throw" && !(player2Choice == "Defend"))
                    {
                        if (player2Choice == "Heal")
                        {
                            player2HP -= ThrowDamage;
                            player2HPChange = -ThrowDamage;
                            player2TookDamage = true;
                        }
                    }
                    if (player2Choice == "Throw" && !(player1Choice == "Defend"))
                    {
                        if (player1Choice == "Heal")
                        {
                            player1HP -= ThrowDamage;
                            player1HPChange = -ThrowDamage;
                            player1TookDamage = true;
                        }
                    }
                }

                // 反击逻辑
                if (player1Choice == "Counter")
                {
                    preDamage = CounterDamage;
                    if (player2Choice == "Attack" || player2Choice == "Throw")
                    {
                        player2HP -= CounterDamage;
                        player2HPChange = -CounterDamage;
                        player2TookDamage = true;
                    }
                    if (player2Choice == "Defend" || player2Choice == "Heal")
                    {
                        player1HP -= CounterFailedDamage;
                        player1HPChange = -CounterFailedDamage;
                        player1TookDamage = true;
                    }
                }
                if (player2Choice == "Counter")
                {
                    if (player1Choice == "Attack" || player1Choice == "Throw")
                    {
                        player1HP -= CounterDamage;
                        player1HPChange = -CounterDamage;
                        player1TookDamage = true;
                    }
                    if (player1Choice == "Defend" || player1Choice == "Heal")
                    {
                        player2HP -= CounterFailedDamage;
                        player2HPChange = -CounterFailedDamage;
                        player2TookDamage = true;
                    }
                }

                // 更新防御图标
                player1DefendIcon.gameObject.SetActive(player1Choice == "Defend" && (player2Choice == "Attack" || (player2Choice == "Throw" && player1DefendChance <= 0.25f)));
                player2DefendIcon.gameObject.SetActive(player2Choice == "Defend" && (player1Choice == "Attack" || (player1Choice == "Throw" && player2DefendChance <= 0.25f)));
            }
            else
            {
                // 处理玩家无法行动时另一方的行动
                if (player1Choice == "None")
                {
                    ExecuteOpponentAction(player2Choice, ref player2HP, ref player1HP, ref player2HPChange, ref player1HPChange);
                }
                if (player2Choice == "None")
                {
                    ExecuteOpponentAction(player1Choice, ref player1HP, ref player2HP, ref player1HPChange, ref player2HPChange);
                }
            }

            player1HP = Mathf.Max(player1HP, 0);
            player2HP = Mathf.Max(player2HP, 0);

            // 更新平衡指示器和心形scale
            UpdateBalanceIndicators();
            UpdateHeartScale();
            CheckGameOver();

            // 在Debug.Log中显示每回合选择、HP值和平衡值
            Debug.Log($"--------------Turn {turnCount}--------------");
            Debug.Log($"Round Result: 【Player 1】 chose 【{player1Choice}】, 【Player 2】 chose 【{player2Choice}】");
            Debug.Log($"【Player 1】 HP: 【{player1HP}】, Balance: 【{player1Balance}】");
            Debug.Log($"【Player 2】 HP: 【{player2HP}】, Balance: 【{player2Balance}】");

            turnCount++;

            player1Choice = "";
            player2Choice = "";
            player1Submitted = false;

            player1ChoiceText.text = "";
            player2ChoiceText.text = "";

            player1SubmitButton.interactable = false;

            EnablePlayer1Buttons(true);

            ShuffleAndAssignActions();  // 在每轮结束后重新洗牌
        }
    }

    void ExecuteOpponentAction(string choice, ref int actorHP, ref int opponentHP, ref int actorHPChange, ref int opponentHPChange)
    {
        if (choice == "Attack")
        {
            opponentHP -= AtkDamage;
            opponentHPChange = -AtkDamage;
        }
        else if (choice == "Defend")
        {
            // No immediate effect
        }
        else if (choice == "Heal")
        {
            int healedAmount = Mathf.Min(player1MaxHP - actorHP, HealPoint);
            actorHP += healedAmount;
            actorHPChange = healedAmount;
        }
        else if (choice == "Throw")
        {
            opponentHP -= ThrowDamage;
            opponentHPChange = -ThrowDamage;
        }
        else if (choice == "Counter")
        {
            actorHP -= CounterFailedDamage;
            actorHPChange = -CounterFailedDamage;
        }
    }

    void UpdateHeartScale()
    {
        if (player1Heart != null)
        {
            // 计算 player1Heart 的 scale
            float player1Scale = player1HP > 0 ? (1.0f - (player1MaxHP - player1HP) * 0.05f) * 70 : 0f; // max scale:70
            player1Heart.transform.localScale = new Vector3(player1Scale, player1Scale, player1Scale);
        }

        if (player2Heart != null)
        {
            // 计算 player2Heart 的 scale
            float player2Scale = player2HP > 0 ? (1.0f - (player2MaxHP - player2HP) * 0.05f) * 70 : 0f;
            player2Heart.transform.localScale = new Vector3(player2Scale, player2Scale, player2Scale);
        }
    }

    void ResetButtonColors()
    {
        foreach (Button button in player1Buttons)
        {
            button.colors = normalColorBlock;
        }
        foreach (Button button in player2Buttons)
        {
            button.colors = normalColorBlock;
        }
    }

    void EnablePlayer1Buttons(bool enable)
    {
        foreach (Button button in player1Buttons)
        {
            button.interactable = enable;
        }
    }

    void EnablePlayer2Buttons(bool enable)
    {
        foreach (Button button in player2Buttons)
        {
            button.interactable = enable;
        }
    }

    void UpdateBalanceIndicators()
    {
        // 更新Player1的平衡指示器
        for (int i = 0; i < player1MaxBalance; i++)
        {
            if (i < player1Balance)
                player1BalanceIndicators[i].SetActive(true);
            else
                player1BalanceIndicators[i].SetActive(false);
        }

        // 更新Player2的平衡指示器
        for (int i = 0; i < player2MaxBalance; i++)
        {
            if (i < player2Balance)
                player2BalanceIndicators[i].SetActive(true);
            else
                player2BalanceIndicators[i].SetActive(false);
        }

        player1NoBalance.SetActive(player1Balance == 0);
        player2NoBalance.SetActive(player2Balance == 0);
    }

    void CheckGameOver()
    {
        if (player1HP <= 0 && player2HP <= 0)
        {
            resultText.text = "It's a Tie!";
            resultText.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
            EndGame();
        }
        else if (player1HP <= 0)
        {
            resultText.text = "Player 2 Wins the Game!";
            resultText.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
            EndGame();
        }
        else if (player2HP <= 0)
        {
            resultText.text = "Player 1 Wins the Game!";
            resultText.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(true);
            EndGame();
        }
    }

    void EndGame()
    {
        EnablePlayer1Buttons(false);
        EnablePlayer2Buttons(false);
        player1SubmitButton.interactable = false;

        foreach (Button button in player1Buttons)
        {
            button.gameObject.SetActive(false);
        }
        foreach (Button button in player2Buttons)
        {
            button.gameObject.SetActive(false);
        }
    }

    void RestartGame()
    {
        turnCount = 1;

        player1HP = player1MaxHP;
        player2HP = player2MaxHP;
        player1Balance = player1MaxBalance;
        player2Balance = player2MaxBalance;
        player1Choice = "";
        player2Choice = "";
        player1Submitted = false;
        player1DefendNextRound = false;
        player2DefendNextRound = false;
        player1BalanceZeroNextTurn = false; // 重置平衡为零的状态
        player2BalanceZeroNextTurn = false; // 重置平衡为零的状态
        player1SkipNextTurn = false; // 重置跳过回合的状态
        player2SkipNextTurn = false; // 重置跳过回合的状态

        // 更新平衡指示器和心形scale
        UpdateBalanceIndicators();
        UpdateHeartScale();

        foreach (Button button in player1Buttons)
        {
            button.gameObject.SetActive(true);
        }
        foreach (Button button in player2Buttons)
        {
            button.gameObject.SetActive(true);
        }

        EnablePlayer1Buttons(true);
        EnablePlayer2Buttons(true);

        player1SubmitButton.interactable = false;

        resultText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        player1DefendIcon.gameObject.SetActive(false);
        player2DefendIcon.gameObject.SetActive(false);

        player1NoBalance.SetActive(false);
        player2NoBalance.SetActive(false);

        ShuffleAndAssignActions();  // 重启游戏时重新洗牌
    }
}