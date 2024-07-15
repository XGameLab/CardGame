using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RPSGame : MonoBehaviour
{
    public Button player1SubmitButton;
    public Button restartButton;
    public Button winButton;
    public Button continueButton;

    public Text player1ChoiceText;
    public Text player2ChoiceText;
    public Text resultText;
    public Text player1HPText;
    public Text player2HPText;
    public Text player1BalanceText;
    public Text player2BalanceText;

    public Text player1HPChangeText;
    public Text player2HPChangeText;

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

    private GameStateManager gameStateManager;
    // public int currentStageIndex; // 手动指定当前场景编号

    void Start()
    {
        ClearDebugLog();

        allButtons = new Button[] {
            player1SubmitButton,
            restartButton,
            winButton,
            continueButton
        };

        List<Button> buttonList = new List<Button>(FindObjectsOfType<Button>());
        buttonList.Remove(player1SubmitButton);
        buttonList.Remove(restartButton);
        buttonList.Remove(winButton);
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
        winButton.gameObject.SetActive(false);
        player1HPChangeText.gameObject.SetActive(false);
        player2HPChangeText.gameObject.SetActive(false);

        player1SubmitButton.interactable = false;

        player1DefendIcon.gameObject.SetActive(false);
        player2DefendIcon.gameObject.SetActive(false);

        UpdateHPText();
        UpdateBalanceText();
        EnablePlayer1Buttons(true);

        // 获取GameStateManager实例
        gameStateManager = GameStateManager.Instance;
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
        player1HPChangeText.gameObject.SetActive(false);
        player1SubmitButton.interactable = true;
    }

    void Player1Submit()
    {
        player1Submitted = true;
        player1SubmitButton.interactable = false;
        EnablePlayer1Buttons(false);
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

        player2HPChangeText.gameObject.SetActive(false);

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

            player1HPChangeText.text = player1HPChange > 0 ? "+" + player1HPChange : player1HPChange.ToString();
            player2HPChangeText.text = player2HPChange > 0 ? "+" + player2HPChange : player2HPChange.ToString();
            player1HPChangeText.gameObject.SetActive(true);
            player2HPChangeText.gameObject.SetActive(true);

            UpdateHPText();
            UpdateBalanceText();
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

    void UpdateHPText()
    {
        player1HPText.text = "1P HP: " + player1HP;
        player2HPText.text = "2P HP: " + player2HP;
    }

    void UpdateBalanceText()
    {
        player1BalanceText.text = "1P Balance: " + player1Balance;
        player2BalanceText.text = "2P Balance: " + player2Balance;
    }

    void CheckGameOver()
    {
        if (player1HP <= 0 && player2HP <= 0)
        {
            resultText.text = "It's a Tie!";
            resultText.gameObject.SetActive(true);
            EndGame();
        }
        else if (player1HP <= 0)
        {
            resultText.text = "Player 2 Wins the Game!";
            resultText.gameObject.SetActive(true);
            EndGame();
        }
        else if (player2HP <= 0)
        {
            resultText.text = "Player 1 Wins the Game!";
            resultText.gameObject.SetActive(true);
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

        int lastSelectedIndex = GameStateManager.lastSelectedIndex;
        // 记录胜负情况
        if (player1HP > player2HP)
        {
            if(lastSelectedIndex != 2 && lastSelectedIndex != 3)
            {
                winButton.gameObject.SetActive(true);
                RecordWin(true); // Player1 wins
            }
            else
            {
                continueButton.gameObject.SetActive(true);
                RecordWin(true);
            }
        }
        else
        {
            restartButton.gameObject.SetActive(true);
            RecordWin(false); // Player2 wins or it's a tie
        }
    }

    // 在EndGame方法中记录胜负情况时，使用gameStateManager.lastSelectedIndex
    void RecordWin(bool player1Wins)
    {
        int lastSelectedIndex = GameStateManager.lastSelectedIndex;
        if (lastSelectedIndex >= 0 && lastSelectedIndex < gameStateManager.isStageCleared.Length)
        {
            gameStateManager.isStageCleared[lastSelectedIndex] = player1Wins;
            Debug.Log($"Stage {lastSelectedIndex} cleared: {player1Wins}");
        }
        else
        {
            Debug.LogWarning($"Invalid stage index {lastSelectedIndex}.");
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

        UpdateHPText();
        UpdateBalanceText();

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
        winButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);

        player1HPChangeText.gameObject.SetActive(false);
        player2HPChangeText.gameObject.SetActive(false);

        ShuffleAndAssignActions();  // 重启游戏时重新洗牌
        ClearDebugLog();
    }

    // 清除Debug.Log方法
    void ClearDebugLog()
    {
        var assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
