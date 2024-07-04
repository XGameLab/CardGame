using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public GameObject panel;//パネルオブジェクトを入れる
    public GameObject option;//パネルの子のoptionを入れる
   
    public GameObject rawimage;//パネルの子のrowimageを入れる

    public Camera mainCamera;

    AudioSource audioSource;

    public AudioSource[] camAudioSources;
    public GameObject[] Buttons;


    void Start() {
        audioSource = GetComponent<AudioSource>();

        mainCamera = Camera.main;
    }

    public void SoundButton(){
       
            audioSource.PlayOneShot(audioSource.clip);
        }
    
    public void OnStart()
    {
        SceneManager.LoadScene("NewGameScene");
    }

    public void OnCredit()
    {
        rawimage.gameObject.SetActive (true);

        camAudioSources = mainCamera.GetComponents<AudioSource>();
        foreach(AudioSource camAudioSource in camAudioSources) 
        {
            camAudioSource.mute = true;
        }
        ButtonsFalse();
    }

    public void OnCreditBack()
    {
        rawimage.gameObject.SetActive (false);

        camAudioSources = mainCamera.GetComponents<AudioSource>();
        foreach(AudioSource camAudioSource in camAudioSources) 
        {
            camAudioSource.mute = false;
        }

        ButtonsTrue();
    }


    public void OnOption()
    {
       panel.gameObject.SetActive (true);
       option.gameObject.SetActive (true);
    }

    public void OnOptionBack()
    {
       panel.gameObject.SetActive (false);
       option.gameObject.SetActive (false);
    }

     public   void EndGame()
    {
        // ゲーム終了
        Application.Quit();
    }

    
    void  ButtonsFalse()
    {
        foreach(GameObject t in Buttons) 
        {
            t.gameObject.SetActive (false);
        }
    }

    void  ButtonsTrue()
    {
        foreach(GameObject t in Buttons) 
        {
            t.gameObject.SetActive (true);
        }
    }
}
