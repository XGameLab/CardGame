using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnButtonClick : MonoBehaviour
{
    public GameObject Enemy_HP;
    public GameObject Player_HP;
    private int e_HP = 10;
    private int p_HP = 10;
    public static int e_Count = 0;
    public static int p_Count = 0;

    public static bool player_Turn_End = false;
    public static bool enemy_Turn_End = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // PlayerTurnEnd();
        // EnemyTurnEnd();
        // BattleStart();
    }

    public void Button_Click()
    {
        PlayerMovement();
        EnemyMovement();
    }

    private void PlayerMovement()
    {
        if(gameObject.tag == "P_ATK")
        {
            if(e_HP > 0)
            {
                e_HP--;
            }
            Enemy_HP.GetComponent<Text>().text = "HP: " + e_HP;

            if(p_Count < 3)
            {
                p_Count++;
                Debug.Log("行動 " + p_Count);
            }
            Debug.Log("Player's Attack!");
        }
            
        if(gameObject.tag == "P_DEF")
        {
            if(p_Count < 3)
            {
                p_Count++;
                Debug.Log("行動 " + p_Count);
            }
            Debug.Log("Player's Defense!");
        }
   
    }

    private void EnemyMovement()
    {
        if(gameObject.tag == "E_ATK")
        {
            if(p_HP > 0)
            {
                p_HP--;
            }
            Player_HP.GetComponent<Text>().text = "HP: " + p_HP;

            if(e_Count < 3)
            {
                e_Count++;
                Debug.Log("行動 " + e_Count);
            }
            Debug.Log("Enemy's Attack!");
        }
            
        if(gameObject.tag == "E_DEF")
        {
            if(e_Count < 3)
            {
                e_Count++;
                Debug.Log("行動 " + e_Count);
            }
            Debug.Log("Enemy's Defense!");
        }
    }

    // private void BattleStart()
    // {
    //     if(player_Turn_End && enemy_Turn_End)
    //     {
    //         GameController.status_Now = GameController.game_Status.Battle;
    //         player_Turn_End = false;
    //         enemy_Turn_End = false;
    //         Debug.Log("OHHHHHHH! Battle Start!");//2回出るバグあり。。。
    //     }
    // }

    // private void PlayerTurnEnd()
    // {
    //     if(p_Count >= 3)
    //     {
    //         if(gameObject.tag == "P_ATK" || gameObject.tag == "P_DEF")
    //         {
    //             this.gameObject.SetActive(false);
    //         }
    //         player_Turn_End = true;
    //     }
    // }

    // private void EnemyTurnEnd()
    // {
    //     if(e_Count >= 3)
    //     {
    //         if(gameObject.tag == "E_ATK" || gameObject.tag == "E_DEF")
    //         {
    //             this.gameObject.SetActive(false);
    //         }
    //         enemy_Turn_End = true;
    //     }
    // }

}
