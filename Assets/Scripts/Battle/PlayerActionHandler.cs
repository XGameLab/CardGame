using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerActionHandler : MonoBehaviour
{
    public PlayerBalanceAndHP playerBalanceAndHP; // プレイヤーのHPとバランス管理の参照
    public BattleInfoManager battleInfoManager; // バトル情報管理の参照
    public BattleAnimationManager battleAnimationManager; // バトルアニメーション管理の参照

    private Dictionary<string, Dictionary<string, System.Action>> actionHandlers; // アクションのハンドラ辞書
    public event Action<string> OnActionHandled; // アクションが処理されたときのイベント
    public int atkDamage; // 攻撃のダメージ量
    public int throwDamage; // 投げ技のダメージ量
    public int cntDamage; // 反撃のダメージ量
    public int healingAmount; // 回復量

    public bool player1CannotAct; // プレイヤー1が行動できないかどうかのフラグ
    public bool player2CannotAct; // プレイヤー2が行動できないかどうかのフラグ
    public bool isPlayer1Win; // プレイヤー1が勝ったかどうかのフラグ

    void Start()
    {
        isPlayer1Win = false;
        InitializeActionHandlers(); // アクションハンドラを初期化
    }

    void Update()
    {
        CheckGameOver(); // ゲームオーバーかどうかを確認
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
            Debug.Log("Player1 cannot act this turn."); // プレイヤー1はこのターン行動できない
            HandlePlayerCannotAct("Player1", player2Action);
            player1CannotAct = false;
            if (playerBalanceAndHP.player1HP > 0)
            {
                RestoreBalance("Player1");
            }
        }
        else if (player2CannotAct)
        {
            Debug.Log("Player2 cannot act this turn."); // プレイヤー2はこのターン行動できない
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
                // アニメーション再生を追加
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
                    // 両者のバランスが同時に0になる特別な処理ロジック
                    player1CannotAct = true;
                    player2CannotAct = true;
                    battleInfoManager.battleInfoText.text = "自分: " + battleInfoManager.submitText + "\n相手: " + battleInfoManager.player2ActionText + "\n↑双方次回行動不可";
                    battleInfoManager.player1CannotMoveText = "失敗";
                    battleInfoManager.player2CannotMoveText = "失敗";
                    battleAnimationManager.SetAnimatorEnabled(false, true); // 両者のアニメーターを無効化
                    battleAnimationManager.SetAnimatorEnabled(false, false); // 両者のアニメーターを無効化
                }
                else
                {
                    if (playerBalanceAndHP.player1Balance == 0)
                    {
                        player1CannotAct = true;
                        battleInfoManager.battleInfoText.text = "自分: " + battleInfoManager.submitText + "\n↑次回行動不可" + "\n相手: " + battleInfoManager.player2ActionText;
                        battleInfoManager.player1CannotMoveText = "失敗";
                        battleAnimationManager.SetAnimatorEnabled(false, true); // Player1のアニメーターを無効化
                    }

                    if (playerBalanceAndHP.player2Balance == 0)
                    {
                        player2CannotAct = true;
                        battleInfoManager.battleInfoText.text = "自分: " + battleInfoManager.submitText  + "\n相手: " + battleInfoManager.player2ActionText + "\n↑次回行動不可";
                        battleInfoManager.player2CannotMoveText = "失敗";
                        battleAnimationManager.SetAnimatorEnabled(false, false); // Player2のアニメーターを無効化
                    }
                }
            }
            else
            {
                battleInfoManager.player1CannotMoveText = null;
                battleInfoManager.player2CannotMoveText = null;
                battleAnimationManager.SetAnimatorEnabled(true, true); // Player1のアニメーターを有効化
                battleAnimationManager.SetAnimatorEnabled(true, false); // Player2のアニメーターを有効化
            }

        }

        Debug.Log("Player1 HP: 【" + playerBalanceAndHP.player1HP + "】, Player2 HP: 【" + playerBalanceAndHP.player2HP + "】");
        Debug.Log("Player1 Balance: 【" + playerBalanceAndHP.player1Balance + "】, Player2 Balance: 【" + playerBalanceAndHP.player2Balance + "】");

        playerBalanceAndHP.UpdateHPSliders(); // HPスライダーを更新
        playerBalanceAndHP.UpdateBalance(); // バランスを更新
    }

    private void PlayActionAnimation(string action, bool isPlayer)
    {
        Animator animator = isPlayer ? battleAnimationManager.playerAnimator : battleAnimationManager.enemyAnimators[GameStateManager.lastSelectedIndex];
        if (!animator.isActiveAndEnabled)
        {
            animator.enabled = true;
        }

        battleAnimationManager.ResetTriggers(animator); // トリガーをリセット

        // player1CannotAct 状態をチェックし、player1 が行動できない場合はアニメーションと音声をスキップ
        if (player1CannotAct && isPlayer)
        {
            Debug.Log("Player1 cannot act, skipping animation and sound.");
            return;
        }

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
