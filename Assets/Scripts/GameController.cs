using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum game_Status{ButtonSelect, Battle}
    public static game_Status status_Now;
    public static game_Status last_Status;
    public GameObject[] all_Images;

    private float countDown_Time = 1;
    private int last_CountDown_Time = 0;

    // Start is called before the first frame update
    void Start()
    {
        status_Now = game_Status.ButtonSelect;
        last_Status = status_Now;
        Debug.Log("NOW: " + status_Now);
    }

    // Update is called once per frame
    void Update()
    {
        if(last_Status != status_Now)
        {
            Debug.Log("New Status: " + status_Now + "!!");
            last_Status = status_Now;
        }

        switch(status_Now)
        {
            case game_Status.ButtonSelect:
                break;
            case game_Status.Battle:
                countDown();
                // OnDragCard.p_Count = 0;
                // OnDragCard.e_Count = 0;
                break;
        }
    }
    // TURN END countDown 
    private void countDown()
    {
        if(countDown_Time > 0)
        {
            countDown_Time -= Time.deltaTime; 
            int secondsLeft = Mathf.RoundToInt(countDown_Time);
            if (secondsLeft != last_CountDown_Time)
            {
                last_CountDown_Time = secondsLeft;
                Debug.Log("CountDown: " + secondsLeft);
            }
        }
        else
        {
            Debug.Log("CountDown is OVER!");
            status_Now = game_Status.ButtonSelect;
            last_Status = status_Now;
            Debug.Log("New Status: " + status_Now);

            foreach(GameObject gameObject in all_Images)
            {
                gameObject.SetActive(true);
                Debug.Log("aaaaaaaaaaaaaaaaa");
            }
            countDown_Time = 1;
            last_CountDown_Time = 0;
        }
    }

}
