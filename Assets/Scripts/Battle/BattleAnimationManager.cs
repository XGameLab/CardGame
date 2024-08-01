using UnityEngine;

public class BattleAnimationManager : MonoBehaviour
{
    public Animator playerAnimator; // プレイヤーのアニメーター
    public Animator[] enemyAnimators; // 敵のアニメーターの配列
    public ObjectShake objectShake; // 物体を揺らす効果
    public AudioManager audioManager; // 音声管理

    private void Start()
    {
        // キャラクターのデフォルトのアニメーション状態を初期化
        SetDefaultAnimationStates();
    }

    public void SetDefaultAnimationStates()
    {
        SetTriggerIfExists(playerAnimator, "P1IdleTrigger"); // プレイヤーのアイドル状態にトリガーを設定
        SetTriggerIfExists(GetCurrentEnemyAnimator(), "P2IdleTrigger"); // 敵のアイドル状態にトリガーを設定
        UpdateEnemyVisibility(); // 敵の可視性を更新
    }

    public void PlayAnimation(string action, bool isPlayer)
    {
        switch (action)
        {
            case "ATK":
                PlayAttackAnimation(isPlayer); // 攻撃アニメーションを再生
                break;
            case "DEF":
                PlayDefendAnimation(isPlayer); // 防御アニメーションを再生
                break;
            case "Heal":
                PlayHealAnimation(isPlayer); // 回復アニメーションを再生
                break;
            case "Throw":
                PlayThrowAnimation(isPlayer); // 投げるアニメーションを再生
                break;
            case "CNT":
                PlayCounterAnimation(isPlayer); // カウンターアニメーションを再生
                break;
        }
    }

    public void PlayAttackAnimation(bool isPlayer)
    {
        if (isPlayer)
        {
            SetTriggerIfExists(playerAnimator, "P1ATKTrigger"); // プレイヤーの攻撃トリガーを設定
            audioManager.PlaySound("P1ATKTrigger"); // 攻撃音を再生
        }
        else
        {
            SetTriggerIfExists(GetCurrentEnemyAnimator(), "P2ATKTrigger"); // 敵の攻撃トリガーを設定
        }
        objectShake.TriggerShake(); // 物体を揺らす
    }

    public void PlayDefendAnimation(bool isPlayer)
    {
        if (isPlayer)
        {
            SetTriggerIfExists(playerAnimator, "P1DEFTrigger"); // プレイヤーの防御トリガーを設定
            audioManager.PlaySound("P1DEFTrigger"); // 防御音を再生
        }
        else
        {
            SetTriggerIfExists(GetCurrentEnemyAnimator(), "P2DEFTrigger"); // 敵の防御トリガーを設定
        }
    }

    public void PlayHealAnimation(bool isPlayer)
    {
        if (isPlayer)
        {
            SetTriggerIfExists(playerAnimator, "P1HealTrigger"); // プレイヤーの回復トリガーを設定
            audioManager.PlaySound("P1HealTrigger"); // 回復音を再生
        }
        else
        {
            SetTriggerIfExists(GetCurrentEnemyAnimator(), "P2HealTrigger"); // 敵の回復トリガーを設定
        }
    }

    public void PlayThrowAnimation(bool isPlayer)
    {
        if (isPlayer)
        {
            SetTriggerIfExists(playerAnimator, "P1ThrowTrigger"); // プレイヤーの投げるトリガーを設定
            audioManager.PlaySound("P1ThrowTrigger"); // 投げる音を再生
        }
        else
        {
            SetTriggerIfExists(GetCurrentEnemyAnimator(), "P2ThrowTrigger"); // 敵の投げるトリガーを設定
        }
        objectShake.TriggerShake(); // 物体を揺らす
    }

    public void PlayCounterAnimation(bool isPlayer)
    {
        if (isPlayer)
        {
            SetTriggerIfExists(playerAnimator, "P1CNTTrigger"); // プレイヤーのカウンタートリガーを設定
            audioManager.PlaySound("P1CNTTrigger"); // カウンター音を再生
        }
        else
        {
            SetTriggerIfExists(GetCurrentEnemyAnimator(), "P2CNTTrigger"); // 敵のカウンタートリガーを設定
        }
        objectShake.TriggerShake(); // 物体を揺らす
    }

    private Animator GetCurrentEnemyAnimator()
    {
        int index = GameStateManager.lastSelectedIndex; // 最後に選択された敵のインデックス
        // Debug.Log("index: " + index);
        if (index >= 0 && index < enemyAnimators.Length)
        {
            Animator currentAnimator = enemyAnimators[index]; // 現在の敵のアニメーター
            // Debug.Log("Current enemy animator: " + currentAnimator.name);

            // SpriteRenderer を取得して現在の Sprite 名を出力
            SpriteRenderer spriteRenderer = currentAnimator.GetComponent<SpriteRenderer>();
            // if (spriteRenderer != null)
            // {
            //     Debug.Log("Current sprite: " + spriteRenderer.sprite.name);
            // }
            if (spriteRenderer == null)
            {
                Debug.LogWarning("SpriteRenderer が " + currentAnimator.name + " に見つかりません。");
            }

            return currentAnimator;
        }
        else
        {
            Debug.LogWarning("無効な敵アニメーターのインデックス、デフォルトを使用します。");
            return enemyAnimators[0]; // デフォルトのアニメーターを返す
        }
    }

    private void SetTriggerIfExists(Animator animator, string triggerName)
    {
        if (HasTrigger(animator, triggerName))
        {
            animator.SetTrigger(triggerName); // トリガーを設定
        }
        else
        {
            Debug.LogWarning($"アニメーターには {triggerName} というトリガーがありません。");
        }
    }

    private bool HasTrigger(Animator animator, string triggerName)
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger && param.name == triggerName)
            {
                return true;
            }
        }
        return false;
    }

    public void SetAnimatorEnabled(bool enabled, bool isPlayer)
    {
        if (isPlayer)
        {
            playerAnimator.enabled = enabled; // プレイヤーのアニメーターを有効/無効にする
        }
        else
        {
            GetCurrentEnemyAnimator().enabled = enabled; // 敵のアニメーターを有効/無効にする
        }
    }

    public void TriggerExitAnimation(bool isPlayer)
    {
        if (isPlayer)
        {
            playerAnimator.SetTrigger("ExitTrigger"); // プレイヤーの退出トリガーを設定
        }
        else
        {
            GetCurrentEnemyAnimator().SetTrigger("ExitTrigger"); // 敵の退出トリガーを設定
        }
    }

    public void ResetWhenRestart()
    {
        ResetTriggers(playerAnimator); // プレイヤーのトリガーをリセット
        foreach (var animator in enemyAnimators)
        {
            ResetTriggers(animator); // 敵のトリガーをリセット
        }

        SetDefaultAnimationStates(); // デフォルトのアニメーション状態を設定

        TriggerExitAnimation(true); // プレイヤーの退出アニメーションをトリガー
        TriggerExitAnimation(false); // 敵の退出アニメーションをトリガー

        SetAnimatorEnabled(true, true); // プレイヤーのアニメーターを有効にする
        SetAnimatorEnabled(true, false); // 敵のアニメーターを有効にする
    }

    public void ResetTriggers(Animator animator)
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name); // トリガーをリセット
            }
        }
    }

    private void UpdateEnemyVisibility()
    {
        int currentIndex = GameStateManager.lastSelectedIndex; // 現在選択されている敵のインデックス
        for (int i = 0; i < enemyAnimators.Length; i++)
        {
            enemyAnimators[i].gameObject.SetActive(i == currentIndex); // 敵の表示状態を更新
        }
    }
}
