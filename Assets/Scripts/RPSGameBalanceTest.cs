using System;
using System.Collections.Generic;
using UnityEngine;

public class RPSGameBalanceTest : MonoBehaviour
{
    public int simulations = 10000;
    
    public int AtkDamage = 6;
    public int AtkToAtkDamage = 3;
    public int HealPoint = 7;
    public int ThrowDamage = 4;
    public int ThrowToThrowDamage = 2;
    public int CounterDamage = 4;
    public int CounterFailedDamage = 3;

    private string[] actions = { "Attack", "Defend", "Heal", "Throw", "Counter" };
    private Dictionary<string, float> actionEffectiveness = new Dictionary<string, float>();

    void Start()
    {
        SimulateGames();
        PrintResults();
    }

    void SimulateGames()
    {
        Dictionary<string, int> actionCounts = new Dictionary<string, int>();
        Dictionary<string, int> actionDamageDealt = new Dictionary<string, int>();

        foreach (var action in actions)
        {
            actionCounts[action] = 0;
            actionDamageDealt[action] = 0;
        }

        for (int i = 0; i < simulations; i++)
        {
            string player1Action = GetRandomAction();
            string player2Action = GetRandomAction();

            (int player1Damage, int player2Damage) = SimulateRound(player1Action, player2Action);

            actionCounts[player1Action]++;
            actionCounts[player2Action]++;
            actionDamageDealt[player1Action] += player2Damage;
            actionDamageDealt[player2Action] += player1Damage;
        }

        foreach (var action in actions)
        {
            actionEffectiveness[action] = (float)actionDamageDealt[action] / actionCounts[action];
        }
    }

    string GetRandomAction()
    {
        int index = UnityEngine.Random.Range(0, actions.Length);
        return actions[index];
    }

    (int, int) SimulateRound(string player1Action, string player2Action)
    {
        int player1Damage = 0;
        int player2Damage = 0;

        if (player1Action == "Attack" && player2Action == "Attack")
        {
            player1Damage = AtkToAtkDamage;
            player2Damage = AtkToAtkDamage;
        }
        else
        {
            if (player1Action == "Attack")
            {
                if (player2Action == "Heal" || player2Action == "Throw")
                {
                    player2Damage = AtkDamage;
                }
            }

            if (player2Action == "Attack")
            {
                if (player1Action == "Heal" || player1Action == "Throw")
                {
                    player1Damage = AtkDamage;
                }
            }

            if (player1Action == "Throw" && player2Action == "Throw")
            {
                player1Damage = ThrowToThrowDamage;
                player2Damage = ThrowToThrowDamage;
            }
            else
            {
                if (player1Action == "Throw" )
                {
                    if (player2Action == "Heal" || player2Action != "Defend")
                    {
                        player2Damage = ThrowDamage;
                    }
                }

                if (player2Action == "Throw" || player2Action != "Defend")
                {
                    if (player1Action == "Heal")
                    {
                        player1Damage = ThrowDamage;
                    }
                }
            }

            if (player1Action == "Counter")
            {
                if (player2Action == "Attack" || player2Action == "Throw")
                {
                    player2Damage = CounterDamage;
                }
                else if (player2Action == "Defend" || player2Action == "Heal")
                {
                    player1Damage = CounterFailedDamage;
                }
            }

            if (player2Action == "Counter")
            {
                if (player1Action == "Attack" || player1Action == "Throw")
                {
                    player1Damage = CounterDamage;
                }
                else if (player1Action == "Defend" || player1Action == "Heal")
                {
                    player2Damage = CounterFailedDamage;
                }
            }
        }

        return (player1Damage, player2Damage);
    }

    void PrintResults()
    {
        foreach (var action in actions)
        {
            Debug.Log($"{action} average damage dealt: {actionEffectiveness[action]}");
        }
    }
}
