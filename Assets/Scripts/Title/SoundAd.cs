using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SoundAd : MonoBehaviour
{
     //Audioミキサーを入れるとこです
    [SerializeField] AudioMixer audioMixer;

    //それぞれのスライダーを入れるとこです。。
    [SerializeField] Slider BgmSlider;
    // [SerializeField] Slider SeSlider;
    
    public TextMeshProUGUI BgmVolumeText;
    // public TextMeshProUGUI SeVolumeText;
    private AudioSource audioSource;
    public GameObject eventsystem;//消したくないオブジェクト
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
	}
    void Start()
    {
        //ミキサーのvolumeにスライダーのvolumeを入れてます。

        //BGM
        audioMixer.GetFloat("BGM", out float bgmVolume);
        // //SE
        // audioMixer.GetFloat("SE", out float seVolume);

        // スライダーのイベントリスナーを追加
        BgmSlider.onValueChanged.AddListener(SetBGMVolume);
        // SeSlider.onValueChanged.AddListener(SetSEVolume);

        SetBGMVolume(audioSource.volume);
    }

    void Update()
    {
        //BGNのパーセント表示
        float BgmVolume = BgmSlider.value * 100;
        BgmVolumeText.text = Mathf.RoundToInt(BgmVolume).ToString() + "%";
        
        // //SEのパーセント表示
        // float SeVolume = SeSlider.value * 100;
        // SeVolumeText.text = "Volume: " + Mathf.RoundToInt(SeVolume).ToString() + "%";
        
    }

    public void SetBGMVolume(float volume)
    {
        audioSource.volume = volume ; // スライダーの値を0-1の範囲に変換
        audioMixer.SetFloat("BGM", volume);
    }

    // public void SetSEVolume(float volume)
    // {
    //     audioMixer.SetFloat("SE", volume);
    // }

}