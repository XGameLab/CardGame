using UnityEngine;

public class BattleAnimationManager : MonoBehaviour
{
    public Animator playerAnimator;
    public Animator enemyAnimator;

    private void Start()
    {
        // 初始化角色的默认动画状态
        SetDefaultAnimationStates();
    }

    public void SetDefaultAnimationStates()
    {
        SetTriggerIfExists(playerAnimator, "P1IdleTrigger");
        SetTriggerIfExists(enemyAnimator, "P2IdleTrigger");
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
        }
        else
        {
            SetTriggerIfExists(enemyAnimator, "P2ATKTrigger");
        }
    }

    public void PlayDefendAnimation(bool isPlayer)
    {
        if (isPlayer)
        {
            SetTriggerIfExists(playerAnimator, "P1DEFTrigger");
        }
        else
        {
            SetTriggerIfExists(enemyAnimator, "P2DEFTrigger");
        }
    }

    public void PlayHealAnimation(bool isPlayer)
    {
        if (isPlayer)
        {
            SetTriggerIfExists(playerAnimator, "P1HealTrigger");
        }
        else
        {
            SetTriggerIfExists(enemyAnimator, "P2HealTrigger");
        }
    }

    public void PlayThrowAnimation(bool isPlayer)
    {
        if (isPlayer)
        {
            SetTriggerIfExists(playerAnimator, "P1ThrowTrigger");
        }
        else
        {
            SetTriggerIfExists(enemyAnimator, "P2ThrowTrigger");
        }
    }

    public void PlayCounterAnimation(bool isPlayer)
    {
        if (isPlayer)
        {
            SetTriggerIfExists(playerAnimator, "P1CNTTrigger");
        }
        else
        {
            SetTriggerIfExists(enemyAnimator, "P2CNTTrigger");
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
            enemyAnimator.enabled = enabled;
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
            enemyAnimator.SetTrigger("ExitTrigger");
        }
    }

    public void ResetWhenRestart()
    {
        ResetTriggers(playerAnimator);
        ResetTriggers(enemyAnimator);

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
}
