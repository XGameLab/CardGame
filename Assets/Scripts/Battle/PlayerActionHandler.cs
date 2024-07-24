using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerActionHandler : MonoBehaviour
{
    public PlayerBalanceAndHP playerBalanceAndHP;
    public BattleInfoManager battleInfoManager;
    public BattleAnimationManager battleAnimationManager;

    private Dictionary<string, Dictionary<string, System.Action>> actionHandlers;
    public event Action<string> OnActionHandled;
    public int atkDamage;
    public int throwDamage;
    public int cntDamage;
    public int healingAmount;

    public bool player1CannotAct;
    public bool player2CannotAct;
    public bool isPlayer1Win;

    void Start()
    {
        isPlayer1Win = false;
        InitializeActionHandlers();
    }

    void Update()
    {
        CheckGameOver();
    }

    void InitializeActionHandlers()
    {
        actionHandlers = new Dictionary<string, Dictionary<string, System.Action>>();

        actionHandlers["ATK"] = new Dictionary<string, System.Action>
        {
            { "ATK", () => 
                {
                    playerBalanceAndHP.player1HP -= atkDamage / 2;
                    playerBalanceAndHP.player1Balance -= 1;
                    playerBalanceAndHP.player2HP -= atkDamage / 2;
                    playerBalanceAndHP.player2Balance -= 1;
                }
            },
            { "DEF", () => playerBalanceAndHP.player1Balance -= 1 },
            { "Heal", () => 
                {
                    playerBalanceAndHP.player2HP -= atkDamage;
                    playerBalanceAndHP.player2Balance -= 1;
                }
            },
            { "Throw", () => 
                {
                    playerBalanceAndHP.player2HP -= atkDamage;
                    playerBalanceAndHP.player2Balance -= 1;
                }
            },
            { "CNT", () => 
                {
                    playerBalanceAndHP.player1HP -= cntDamage;
                    playerBalanceAndHP.player1Balance -= 1;
                }
            },
        };

        actionHandlers["DEF"] = new Dictionary<string, System.Action>
        {
            { "ATK", () => playerBalanceAndHP.player2Balance -= 1 },
            { "DEF", () => Debug.Log("Nothing happens.") },
            { "Heal", () => 
                {
                    playerBalanceAndHP.player2HP += healingAmount;
                    playerBalanceAndHP.player2Balance += 1;
                } 
            },
            { "Throw", () => 
                {
                    playerBalanceAndHP.player1HP -= throwDamage;
                    playerBalanceAndHP.player1Balance -= 1;
                }
            },
            { "CNT", () => 
                {
                    playerBalanceAndHP.player2HP -= cntDamage / 2;
                    playerBalanceAndHP.player2Balance -= 1;
                }
            },
        };

        actionHandlers["Heal"] = new Dictionary<string, System.Action>
        {
            { "ATK", () => 
                {
                    playerBalanceAndHP.player1HP -= atkDamage;
                    playerBalanceAndHP.player1Balance -= 1;
                }
            },
            { "DEF", () => 
                {
                    playerBalanceAndHP.player1HP += healingAmount;
                    playerBalanceAndHP.player1Balance += 1;
                }
            },
            { "Heal", () => 
                {
                    playerBalanceAndHP.player1HP += healingAmount;
                    playerBalanceAndHP.player1Balance += 1;
                    playerBalanceAndHP.player2HP += healingAmount;
                    playerBalanceAndHP.player2Balance += 1;
                }
            },
            { "Throw", () => 
                {
                    playerBalanceAndHP.player1HP -= throwDamage;
                    playerBalanceAndHP.player1Balance -= 1;
                }
            },  
            { "CNT", () => 
                {
                    playerBalanceAndHP.player1HP += healingAmount;
                    playerBalanceAndHP.player1Balance += 1;
                    playerBalanceAndHP.player2HP -= cntDamage / 2;
                    playerBalanceAndHP.player2Balance -= 1;
                }
            },         
        };

        actionHandlers["Throw"] = new Dictionary<string, System.Action>
        {
            { "ATK", () => 
                {
                    playerBalanceAndHP.player1HP -= atkDamage;
                    playerBalanceAndHP.player1Balance -= 1;
                }
            },
            { "DEF", () => 
                {
                    playerBalanceAndHP.player2HP -= throwDamage;
                    playerBalanceAndHP.player2Balance -= 1;
                }
            },
            { "Heal", () => 
                {
                    playerBalanceAndHP.player2HP -= throwDamage;
                    playerBalanceAndHP.player2Balance -= 1;
                }
            },
            { "Throw", () => 
                {
                    playerBalanceAndHP.player1HP -= throwDamage / 2;
                    playerBalanceAndHP.player2HP -= throwDamage / 2;
                }
            }, 
            { "CNT", () => 
                {
                    playerBalanceAndHP.player1HP -= cntDamage;
                    playerBalanceAndHP.player1Balance -= 1;
                }
            }, 
        };

        actionHandlers["CNT"] = new Dictionary<string, System.Action>
        {
            { "ATK", () => 
                {
                    playerBalanceAndHP.player2HP -= cntDamage;
                    playerBalanceAndHP.player2Balance -= 1;
                }
            },
            { "DEF", () => 
                {
                    playerBalanceAndHP.player1HP -= cntDamage / 2;
                    playerBalanceAndHP.player1Balance -= 1;
                }
            },
            { "Heal", () => 
                {
                    playerBalanceAndHP.player1HP -= cntDamage / 2;
                    playerBalanceAndHP.player1Balance -= 1;
                    playerBalanceAndHP.player2HP += healingAmount;
                    playerBalanceAndHP.player2Balance += 1;
                }
            },
            { "Throw", () => 
                {
                    playerBalanceAndHP.player2HP -= cntDamage;
                    playerBalanceAndHP.player2Balance -= 1;
                }
            },
            { "CNT", () => Debug.Log("Nothing happens.") }, 
        };
    }

    public void HandlePlayerActions(string player1Action, string player2Action)
    {
        Debug.Log("Player1 chose action: " + player1Action);
        Debug.Log("Player2 chose action: " + player2Action);

        if (player1CannotAct)
        {
            Debug.Log("Player1 cannot act this turn.");
            HandlePlayerCannotAct("Player1", player2Action);
            player1CannotAct = false;
            if (playerBalanceAndHP.player1HP > 0)
            {
                RestoreBalance("Player1");
            }
        }
        else if (player2CannotAct)
        {
            Debug.Log("Player2 cannot act this turn.");
            HandlePlayerCannotAct("Player2", player1Action);
            player2CannotAct = false;
            if (playerBalanceAndHP.player2HP > 0)
            {
                RestoreBalance("Player2");
            }
        }
        else
        {
            if (actionHandlers.ContainsKey(player1Action) && actionHandlers[player1Action].ContainsKey(player2Action))
            {
                actionHandlers[player1Action][player2Action].Invoke();
                // 添加动画播放
                PlayActionAnimation(player1Action, true);
                PlayActionAnimation(player2Action, false);
            }
            else
            {
                Debug.Log("Unhandled combination: Player1 (" + player1Action + "), Player2 (" + player2Action + ")");
            }

            if (playerBalanceAndHP.player1Balance == 0 || playerBalanceAndHP.player2Balance == 0)
            {
                if (playerBalanceAndHP.player1Balance == 0 && playerBalanceAndHP.player2Balance == 0)
                {
                    // 双方平衡同时为0的特殊处理逻辑
                    player1CannotAct = true;
                    player2CannotAct = true;
                    battleInfoManager.battleInfoText.text = "自分: " + battleInfoManager.submitText + "\n相手: " + battleInfoManager.player2ActionText + "\n↑双方次回行動不可";
                    battleInfoManager.player1CannotMoveText = "失敗";
                    battleInfoManager.player2CannotMoveText = "失敗";
                    battleAnimationManager.SetAnimatorEnabled(false, true); // 禁用双方的动画器
                    battleAnimationManager.SetAnimatorEnabled(false, false); // 禁用双方的动画器
                }
                else
                {
                    if (playerBalanceAndHP.player1Balance == 0)
                    {
                        player1CannotAct = true;
                        battleInfoManager.battleInfoText.text = "自分: " + battleInfoManager.submitText + "\n↑次回行動不可" + "\n相手: " + battleInfoManager.player2ActionText;
                        battleInfoManager.player1CannotMoveText = "失敗";
                        battleAnimationManager.SetAnimatorEnabled(false, true); // 禁用Player1的动画器
                    }

                    if (playerBalanceAndHP.player2Balance == 0)
                    {
                        player2CannotAct = true;
                        battleInfoManager.battleInfoText.text = "自分: " + battleInfoManager.submitText  + "\n相手: " + battleInfoManager.player2ActionText + "\n↑次回行動不可";
                        battleInfoManager.player2CannotMoveText = "失敗";
                        battleAnimationManager.SetAnimatorEnabled(false, false); // 禁用Player2的动画器
                    }
                }
            }
            else
            {
                battleInfoManager.player1CannotMoveText = null;
                battleInfoManager.player2CannotMoveText = null;
                battleAnimationManager.SetAnimatorEnabled(true, true); // 启用Player1的动画器
                battleAnimationManager.SetAnimatorEnabled(true, false); // 启用Player2的动画器
            }

        }

        Debug.Log("Player1 HP: 【" + playerBalanceAndHP.player1HP + "】, Player2 HP: 【" + playerBalanceAndHP.player2HP + "】");
        Debug.Log("Player1 Balance: 【" + playerBalanceAndHP.player1Balance + "】, Player2 Balance: 【" + playerBalanceAndHP.player2Balance + "】");

        playerBalanceAndHP.UpdateHPSliders();
        playerBalanceAndHP.UpdateBalance();
    }

    private void PlayActionAnimation(string action, bool isPlayer)
    {
        Animator animator = isPlayer ? battleAnimationManager.playerAnimator : battleAnimationManager.enemyAnimator;
        if (!animator.isActiveAndEnabled)
        {
            animator.enabled = true;
        }

        battleAnimationManager.ResetTriggers(animator); // 确保重置触发器

        switch (action)
        {
            case "ATK":
                battleAnimationManager.PlayAttackAnimation(isPlayer);
                break;
            case "DEF":
                battleAnimationManager.PlayDefendAnimation(isPlayer);
                break;
            case "Heal":
                battleAnimationManager.PlayHealAnimation(isPlayer);
                break;
            case "Throw":
                battleAnimationManager.PlayThrowAnimation(isPlayer);
                break;
            case "CNT":
                battleAnimationManager.PlayCounterAnimation(isPlayer);
                break;
        }
    }

    private void CheckGameOver()
    {
        if (playerBalanceAndHP.player1HP == 0 && playerBalanceAndHP.player2HP == 0)
        {
            isPlayer1Win = false;
            battleInfoManager.GameOver("自分: " + battleInfoManager.submitText  + "\n相手: " + battleInfoManager.player2ActionText + "\n【It's a Tie】");
        }
        else if (playerBalanceAndHP.player1HP == 0 && playerBalanceAndHP.player2HP != 0)
        {
            isPlayer1Win = false;
            battleInfoManager.GameOver("自分: " + battleInfoManager.submitText  + "\n相手: " + battleInfoManager.player2ActionText + "\n【Player2 Wins】");
        }
        else if (playerBalanceAndHP.player1HP != 0 && playerBalanceAndHP.player2HP == 0)
        {
            isPlayer1Win = true;
            battleInfoManager.GameOver("自分: " + battleInfoManager.submitText  + "\n相手: " + battleInfoManager.player2ActionText + "\n【Player1 Wins】");
        }
        else
        {
            isPlayer1Win = false;
            battleInfoManager.isGameOver = false;
        }
    }

    private void RestoreBalance(string player)
    {
        if (player == "Player1" && playerBalanceAndHP.player1HP > 0)
        {
            playerBalanceAndHP.player1Balance = 3;
            battleAnimationManager.TriggerExitAnimation(true);
            battleAnimationManager.SetAnimatorEnabled(true, true);
        }
        else if (player == "Player2" && playerBalanceAndHP.player2HP > 0)
        {
            playerBalanceAndHP.player2Balance = 3;
            battleAnimationManager.TriggerExitAnimation(false);
            battleAnimationManager.SetAnimatorEnabled(true, false);
        }
    }

    void HandlePlayerCannotAct(string player, string opponentAction)
    {
        if (player == "Player1")
        {
            switch (opponentAction)
            {
                case "ATK":
                    playerBalanceAndHP.player1HP -= atkDamage;
                    break;
                case "Throw":
                    playerBalanceAndHP.player1HP -= throwDamage;
                    break;
                case "Heal":
                    playerBalanceAndHP.player2HP += healingAmount;
                    playerBalanceAndHP.player2Balance += 1;
                    break;
                case "DEF":
                    Debug.Log("Nothing happens.");
                    break;
                case "CNT":
                    playerBalanceAndHP.player2HP -= cntDamage / 2;
                    playerBalanceAndHP.player2Balance -= 1;
                    break;
            }
        }
        else if (player == "Player2")
        {
            switch (opponentAction)
            {
                case "ATK":
                    playerBalanceAndHP.player2HP -= atkDamage;
                    break;
                case "Throw":
                    playerBalanceAndHP.player2HP -= throwDamage;
                    break;
                case "Heal":
                    playerBalanceAndHP.player1HP += healingAmount;
                    playerBalanceAndHP.player1Balance += 1;
                    break;
                case "DEF":
                    Debug.Log("Nothing happens.");
                    break;
                case "CNT":
                    playerBalanceAndHP.player1HP -= cntDamage / 2;
                    playerBalanceAndHP.player1Balance -= 1;
                    break;
            }
        }
    }
}
