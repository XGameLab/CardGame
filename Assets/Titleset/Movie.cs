using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class Movie : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;

    void Start()
    {
         this. videoPlayer.Play();
    }

   
}