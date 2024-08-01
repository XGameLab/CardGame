using UnityEngine;

public class BattleAnimationManager : MonoBehaviour
{
    public Animator playerAnimator;
    public Animator[] enemyAnimators;
    public ObjectShake objectShake;
    public AudioManager audioManager;

    private void Start()
    {
        // 初始化角色的默认动画状态
        SetDefaultAnimationStates();
    }

    public void SetDefaultAnimationStates()
    {
        SetTriggerIfExists(playerAnimator, "P1IdleTrigger");
        SetTriggerIfExists(GetCurrentEnemyAnimator(), "P2IdleTrigger");
        UpdateEnemyVisibility();
    }

    public void PlayAnimation(string action, bool isPlayer)
    {
        switch (action)
        {
            case "ATK":
                PlayAttackAnimation(isPlayer);
                break;
            case "DEF":
                PlayDefendAnimation(isPlayer);
                break;
            case "Heal":
                PlayHealAnimation(isPlayer);
                break;
            case "Throw":
                PlayThrowAnimation(isPlayer);
                break;
            case "CNT":
                PlayCounterAnimation(isPlayer);
                break;
        }
    }

    public void PlayAttackAnimation(bool isPlayer)
    {
        if (isPlayer)
        {
            SetTriggerIfExists(playerAnimator, "P1ATKTrigger");
            audioManager.PlaySound("P1ATKTrigger");
        }
        else
        {
            SetTriggerIfExists(GetCurrentEnemyAnimator(), "P2ATKTrigger");
        }
        objectShake.TriggerShake();
    }

    public void PlayDefendAnimation(bool isPlayer)
    {
        if (isPlayer)
        {
            SetTriggerIfExists(playerAnimator, "P1DEFTrigger");
            audioManager.PlaySound("P1DEFTrigger");
        }
        else
        {
            SetTriggerIfExists(GetCurrentEnemyAnimator(), "P2DEFTrigger");
        }
    }

    public void PlayHealAnimation(bool isPlayer)
    {
        if (isPlayer)
        {
            SetTriggerIfExists(playerAnimator, "P1HealTrigger");
            audioManager.PlaySound("P1HealTrigger");
        }
        else
        {
            SetTriggerIfExists(GetCurrentEnemyAnimator(), "P2HealTrigger");
        }
    }

    public void PlayThrowAnimation(bool isPlayer)
    {
        if (isPlayer)
        {
            SetTriggerIfExists(playerAnimator, "P1ThrowTrigger");
            audioManager.PlaySound("P1ThrowTrigger");
        }
        else
        {
            SetTriggerIfExists(GetCurrentEnemyAnimator(), "P2ThrowTrigger");
        }
        objectShake.TriggerShake();
    }

    public void PlayCounterAnimation(bool isPlayer)
    {
        if (isPlayer)
        {
            SetTriggerIfExists(playerAnimator, "P1CNTTrigger");
            audioManager.PlaySound("P1CNTTrigger");
        }
        else
        {
            SetTriggerIfExists(GetCurrentEnemyAnimator(), "P2CNTTrigger");
        }
        objectShake.TriggerShake();
    }

    private Animator GetCurrentEnemyAnimator()
    {
        int index = GameStateManager.lastSelectedIndex;
        // Debug.Log("index: " + index);
        if (index >= 0 && index < enemyAnimators.Length)
        {
            Animator currentAnimator = enemyAnimators[index];
            // Debug.Log("Current enemy animator: " + currentAnimator.name);

            // 获取 SpriteRenderer 并输出当前的 Sprite 名称
            SpriteRenderer spriteRenderer = currentAnimator.GetComponent<SpriteRenderer>();
            // if (spriteRenderer != null)
            // {
            //     Debug.Log("Current sprite: " + spriteRenderer.sprite.name);
            // }
            if (spriteRenderer == null)
            {
                Debug.LogWarning("No SpriteRenderer found on " + currentAnimator.name);
            }

            return currentAnimator;
        }
        else
        {
            Debug.LogWarning("Invalid enemy animator index, using default.");
            return enemyAnimators[0]; // 返回默认的动画
        }
    }

    private void SetTriggerIfExists(Animator animator, string triggerName)
    {
        if (HasTrigger(animator, triggerName))
        {
            animator.SetTrigger(triggerName);
        }
        else
        {
            Debug.LogWarning($"Animator does not have a trigger called {triggerName}");
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
            playerAnimator.enabled = enabled;
        }
        else
        {
            GetCurrentEnemyAnimator().enabled = enabled;
        }
    }

    public void TriggerExitAnimation(bool isPlayer)
    {
        if (isPlayer)
        {
            playerAnimator.SetTrigger("ExitTrigger");
        }
        else
        {
            GetCurrentEnemyAnimator().SetTrigger("ExitTrigger");
        }
    }

    public void ResetWhenRestart()
    {
        ResetTriggers(playerAnimator);
        foreach (var animator in enemyAnimators)
        {
            ResetTriggers(animator);
        }

        SetDefaultAnimationStates();

        TriggerExitAnimation(true); 
        TriggerExitAnimation(false); 

        SetAnimatorEnabled(true, true);
        SetAnimatorEnabled(true, false);
    }

    public void ResetTriggers(Animator animator)
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);
            }
        }
    }

    private void UpdateEnemyVisibility()
    {
        int currentIndex = GameStateManager.lastSelectedIndex;
        for (int i = 0; i < enemyAnimators.Length; i++)
        {
            enemyAnimators[i].gameObject.SetActive(i == currentIndex);
        }
    }
}
