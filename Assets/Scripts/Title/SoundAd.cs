using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SoundAd : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer; // オーディオミキサーの参照
    [SerializeField] Slider BgmSlider; // BGM用のスライダー
    // [SerializeField] Slider SeSlider; // SE用のスライダー（コメントアウトされている）

    public TextMeshProUGUI BgmVolumeText; // BGMのボリューム表示用のテキスト
    // public TextMeshProUGUI SeVolumeText; // SEのボリューム表示用のテキスト（コメントアウトされている）
    private AudioSource audioSource; // AudioSourceコンポーネント
    public GameObject eventsystem; // イベントシステムの参照

    void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // AudioSourceコンポーネントを取得
    }

    void Start()
    {
        // BGMの初期ボリュームを設定
        if (audioMixer != null)
        {
            audioMixer.GetFloat("BGM", out float bgmVolume); // AudioMixerからBGMのボリュームを取得
        }
        else
        {
            Debug.LogWarning("AudioMixer is not assigned."); // AudioMixerが割り当てられていない場合の警告
        }

        // スライダーのイベントリスナーを追加
        if (BgmSlider != null)
        {
            BgmSlider.onValueChanged.AddListener(SetBGMVolume); // スライダーの値が変化したときにSetBGMVolumeメソッドを呼び出す
        }
        else
        {
            Debug.LogWarning("BgmSlider is not assigned."); // BgmSliderが割り当てられていない場合の警告
        }

        // 初期のBGMボリュームを設定
        if (audioSource != null)
        {
            SetBGMVolume(audioSource.volume); // AudioSourceのボリュームを設定
        }
        else
        {
            Debug.LogWarning("AudioSource component is missing."); // AudioSourceコンポーネントが見つからない場合の警告
        }
    }

    void Update()
    {
        if (BgmSlider != null && BgmVolumeText != null)
        {
            float BgmVolume = BgmSlider.value * 100; // スライダーの値を百分率に変換
            BgmVolumeText.text = Mathf.RoundToInt(BgmVolume).ToString() + "%"; // テキストに表示
        }
    }

    public void SetBGMVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume; // AudioSourceのボリュームを設定
        }
        
        if (audioMixer != null)
        {
            audioMixer.SetFloat("BGM", volume); // AudioMixerのBGMボリュームを設定
        }
    }
}
