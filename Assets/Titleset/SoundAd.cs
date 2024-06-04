using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundAd : MonoBehaviour
{
     //Audioミキサーを入れるとこです
    [SerializeField] AudioMixer audioMixer;

    //それぞれのスライダーを入れるとこです。。
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SESlider;

    public GameObject eventsystem;//消したくないオブジェクト
    void Awake()
    {
        DontDestroyOnLoad(eventsystem.gameObject);
	}
    void Start()
    {
        //ミキサーのvolumeにスライダーのvolumeを入れてます。

        //BGM
        audioMixer.GetFloat("BGM", out float bgmVolume);
        BGMSlider.value = bgmVolume;
        //SE
        audioMixer.GetFloat("SE", out float seVolume);
        SESlider.value = seVolume;
    }

    public void SetBGM(float volume)
    {
        audioMixer.SetFloat("BGM", volume);
    }

    public void SetSE(float volume)
    {
        audioMixer.SetFloat("SE", volume);
    }

}
