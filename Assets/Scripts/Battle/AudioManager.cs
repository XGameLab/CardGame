using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioClip attackSound;
    public AudioClip defendSound;
    public AudioClip healSound;
    public AudioClip throwSound;
    public AudioClip counterSound;
    public AudioClip[] BattleSE; // 战斗音效数组
    public AudioSource backupAudioSource; // 备用的 AudioSource
    public float playDelay = 0f; // 延迟时间（秒）
    public Button submitButton; // 提交按钮

    private void Start()
    {
        // 绑定按钮点击事件
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(PlayRandomBattleSE);
        }
    }

    public void PlaySound(string action)
    {
        AudioClip clip = null;

        switch (action)
        {
            case "P1ATKTrigger":
                clip = attackSound;
                break;
            case "P1DEFTrigger":
                clip = defendSound;
                break;
            case "P1HealTrigger":
                clip = healSound;
                break;
            case "P1ThrowTrigger":
                clip = throwSound;
                break;
            case "P1CNTTrigger":
                clip = counterSound;
                break;
            default:
                Debug.LogWarning("No sound mapped for action: " + action);
                return;
        }

        // 如果有延迟，则使用延迟播放
        if (playDelay > 0)
        {
            LeanTween.delayedCall(playDelay, () => PlaySoundEffect(clip));
        }
        else
        {
            PlaySoundEffect(clip);
        }
    }

    private void PlaySoundEffect(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null, cannot play sound.");
            return;
        }

        // 尝试使用 GameStateManager 播放音效
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.PlaySoundEffect(clip);
        }
        // 如果 GameStateManager 不可用，使用备用的 AudioSource 播放音效
        else if (backupAudioSource != null)
        {
            backupAudioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("No available method to play the sound.");
        }
    }

    public void PlayRandomBattleSE()
    {
        if (BattleSE == null || BattleSE.Length == 0)
        {
            Debug.LogWarning("BattleSE array is empty or null.");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, BattleSE.Length);
        AudioClip randomClip = BattleSE[randomIndex];
        
        if (randomClip != null)
        {
            PlaySoundEffect(randomClip);
        }
        else
        {
            Debug.LogWarning("Selected AudioClip is null.");
        }
    }
}
