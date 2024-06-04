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

    AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public void SoundButton(){
       
            audioSource.PlayOneShot(audioSource.clip);
        }
    
    public void OnStart()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnCredit()
    {
       rawimage.gameObject.SetActive (true);
    }

    public void OnCreditBack()
    {
       rawimage.gameObject.SetActive (false);
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
}
