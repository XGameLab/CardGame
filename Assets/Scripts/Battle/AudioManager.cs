using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioClip attackSound; // 攻撃音
    public AudioClip defendSound; // 防御音
    public AudioClip healSound; // 回復音
    public AudioClip throwSound; // 投げる音
    public AudioClip counterSound; // カウンター音
    public AudioClip[] BattleSE; // 戦闘音効果の配列
    public AudioSource backupAudioSource; // バックアップ用の AudioSource
    public float playDelay = 0f; // 再生遅延（秒）
    public Button submitButton; // 送信ボタン

    private void Start()
    {
        // ボタンクリックイベントのバインド
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
                clip = attackSound; // 攻撃音
                break;
            case "P1DEFTrigger":
                clip = defendSound; // 防御音
                break;
            case "P1HealTrigger":
                clip = healSound; // 回復音
                break;
            case "P1ThrowTrigger":
                clip = throwSound; // 投げる音
                break;
            case "P1CNTTrigger":
                clip = counterSound; // カウンター音
                break;
            default:
                Debug.LogWarning("アクションに対応するサウンドが設定されていません: " + action);
                return;
        }

        // 遅延がある場合、遅延再生を使用
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
            Debug.LogWarning("AudioClip が null です。サウンドを再生できません。");
            return;
        }

        // GameStateManager を使用して音を再生する
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.PlaySoundEffect(clip);
        }
        // GameStateManager が使用できない場合、バックアップの AudioSource を使用して音を再生する
        else if (backupAudioSource != null)
        {
            backupAudioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("音を再生する方法がありません。");
        }
    }

    public void PlayRandomBattleSE()
    {
        if (BattleSE == null || BattleSE.Length == 0)
        {
            Debug.LogWarning("BattleSE の配列が空か null です。");
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
            Debug.LogWarning("選択された AudioClip が null です。");
        }
    }
}
