using UnityEngine;
using System.Collections.Generic;

public class RPSWinRateCalculator : MonoBehaviour
{
    public int simulationsPerAction = 1000;
    public int player1MaxHP = 12;
    public int player2MaxHP = 12;

    public int AtkDamage = 6;
    public int AtkToAtkDamage = 3;
    public int HealPoint = 7;
    public int ThrowDamage = 4;
    public int ThrowToThrowDamage = 2;
    public int CounterDamage = 4;
    public int CounterFailedDamage = 3;
    public int MaxBalance = 3;

    public bool enableBalance = true;  // 手动选择是否开启平衡机制

    private List<string> actions = new List<string> { "Attack", "Defend", "Heal", "Throw", "Counter" };

    void Start()
    {
        CalculateWinRates();
    }

    void CalculateWinRates()
    {
        Dictionary<string, float> winRates = new Dictionary<string, float>();

        foreach (string action in actions)
        {
            int wins = 0;

            for (int i = 0; i < simulationsPerAction; i++)
            {
                if (SimulateGame(action))
                {
                    wins++;
                }
            }

            float winRate = (float)wins / simulationsPerAction * 100;
            winRates[action] = winRate;
        }

        foreach (var entry in winRates)
        {
            Debug.Log($"Action: {entry.Key}, Win Rate: {entry.Value}%");
        }
    }

    bool SimulateGame(string player1Action)
    {
        int player1HP = player1MaxHP;
        int player2HP = player2MaxHP;
        int player1Balance = MaxBalance;
        int player2Balance = MaxBalance;
        bool player1BalanceZeroNextTurn = false;
        bool player2BalanceZeroNextTurn = false;
        bool player1SkipNextTurn = false;
        bool player2SkipNextTurn = false;

        while (player1HP > 0 && player2HP > 0)
        {
            string player2Action = actions[Random.Range(0, actions.Count)];

            if (enableBalance)
            {
                if (player1BalanceZeroNextTurn)
                {
                    player1SkipNextTurn = true;
                    player1BalanceZeroNextTurn = false;
                    player1Balance = MaxBalance;
                }
                if (player2BalanceZeroNextTurn)
                {
                    player2SkipNextTurn = true;
                    player2BalanceZeroNextTurn = false;
                    player2Balance = MaxBalance;
                }

                if (player1SkipNextTurn)
                {
                    player1Action = "None";
                    player1SkipNextTurn = false;
                }
                if (player2SkipNextTurn)
                {
                    player2Action = "None";
                    player2SkipNextTurn = false;
                }

                // 平衡机制
                if (player1Action == "Attack" && (player2Action == "Attack" || player2Action == "Defend" || player2Action == "Counter"))
                {
                    player1Balance--;
                }
                if (player2Action == "Attack" && (player1Action == "Attack" || player1Action == "Defend" || player1Action == "Counter"))
                {
                    player2Balance--;
                }
                if (player1Action == "Throw" && (player2Action == "Attack" || player2Action == "Counter"))
                {
                    player1Balance--;
                }
                if (player2Action == "Throw" && (player1Action == "Attack" || player1Action == "Counter"))
                {
                    player2Balance--;
                }
                if (player1Action == "Counter" && (player2Action == "Defend" || player2Action == "Heal"))
                {
                    player1Balance--;
                }
                if (player2Action == "Counter" && (player1Action == "Defend" || player1Action == "Heal"))
                {
                    player2Balance--;
                }
                if (player1Action == "Heal" && (player2Action == "Defend" || player2Action == "Heal" || player2Action == "Counter"))
                {
                    player1Balance = Mathf.Min(player1Balance + 1, 3);
                }
                if (player2Action == "Heal" && (player1Action == "Defend" || player1Action == "Heal" || player1Action == "Counter"))
                {
                    player2Balance = Mathf.Min(player2Balance + 1, 3);
                }

                if (player1Balance <= 0)
                {
                    player1BalanceZeroNextTurn = true;
                    player1Balance = 0;
                }
                if (player2Balance <= 0)
                {
                    player2BalanceZeroNextTurn = true;
                    player2Balance = 0;
                }
            }

            if (player1Action != "None" && player2Action != "None")
            {
                if (player1Action == "Attack" && player2Action == "Attack")
                {
                    player1HP -= AtkToAtkDamage;
                    player2HP -= AtkToAtkDamage;
                }
                else
                {
                    if (player1Action == "Attack")
                    {
                        if (player2Action == "Heal" || player2Action == "Throw")
                        {
                            player2HP -= AtkDamage;
                        }
                    }

                    if (player2Action == "Attack")
                    {
                        if (player1Action == "Heal" || player1Action == "Throw")
                        {
                            player1HP -= AtkDamage;
                        }
                    }
                }

                if (player1Action == "Defend")
                {
                    if (player2Action == "Throw")
                    {
                        if (Random.value > 0.25f) // 25%几率不受投掷伤害
                        {
                            player1HP -= ThrowDamage;
                        }
                    }
                }

                if (player2Action == "Defend")
                {
                    if (player1Action == "Throw")
                    {
                        if (Random.value > 0.25f) // 25%几率不受投掷伤害
                        {
                            player2HP -= ThrowDamage;
                        }
                    }
                }

                if (player1Action == "Heal" && (player2Action == "Defend" || player2Action == "Heal" || player2Action == "Counter"))
                {
                    player1HP = Mathf.Min(player1HP + HealPoint, player1MaxHP);
                }
                if (player2Action == "Heal" && (player1Action == "Defend" || player1Action == "Heal" || player1Action == "Counter"))
                {
                    player2HP = Mathf.Min(player2HP + HealPoint, player2MaxHP);
                }

                if (player1Action == "Throw" && player2Action == "Throw")
                {
                    player1HP -= ThrowToThrowDamage;
                    player2HP -= ThrowToThrowDamage;
                }
                else
                {
                    if (player1Action == "Throw" && player2Action != "Defend")
                    {
                        if (player2Action == "Heal")
                        {
                            player2HP -= ThrowDamage;
                        }
                    }
                    if (player2Action == "Throw" && player1Action != "Defend")
                    {
                        if (player1Action == "Heal")
                        {
                            player1HP -= ThrowDamage;
                        }
                    }
                }

                if (player1Action == "Counter")
                {
                    if (player2Action == "Attack" || player2Action == "Throw")
                    {
                        player2HP -= CounterDamage;
                    }
                    if (player2Action == "Defend" || player2Action == "Heal")
                    {
                        player1HP -= CounterFailedDamage;
                    }
                }
                if (player2Action == "Counter")
                {
                    if (player1Action == "Attack" || player1Action == "Throw")
                    {
                        player1HP -= CounterDamage;
                    }
                    if (player1Action == "Defend" || player1Action == "Heal")
                    {
                        player2HP -= CounterFailedDamage;
                    }
                }
            }
            else
            {
                if (player1Action == "None" && player2Action != "None")
                {
                    ExecuteOpponentAction(player2Action, ref player2HP, ref player1HP);
                }
                if (player2Action == "None" && player1Action != "None")
                {
                    ExecuteOpponentAction(player1Action, ref player1HP, ref player2HP);
                }
            }
        }

        return player1HP > 0 && player2HP <= 0;
    }

    void ExecuteOpponentAction(string choice, ref int actorHP, ref int opponentHP)
    {
        if (choice == "Attack")
        {
            opponentHP -= AtkDamage;
        }
        else if (choice == "Defend")
        {
            // No immediate effect
        }
        else if (choice == "Heal")
        {
            int healedAmount = Mathf.Min(player1MaxHP - actorHP, HealPoint);
            actorHP += healedAmount;
        }
        else if (choice == "Throw")
        {
            opponentHP -= ThrowDamage;
        }
        else if (choice == "Counter")
        {
            actorHP -= CounterFailedDamage;
        }
    }
}
