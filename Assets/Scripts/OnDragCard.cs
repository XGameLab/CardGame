using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnDragCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{       
    private Outline outline;
    public Vector2 originalPosition;// 画像の初期位置

    public GameObject Enemy_HP;
    public GameObject Player_HP;
    private int e_HP = 10;
    private int p_HP = 10;
    public static int e_Count = 0;
    public static int p_Count = 0;

    public static bool player_Turn_End = false;
    public static bool enemy_Turn_End = false;

    public static bool isSelected = false;    

    public static RectTransform CardPrefab;

    public void Start()
    {
        outline = GetComponent<Outline>();
        
        originalPosition = transform.localPosition;// 画像の初期位置を保存
    }

    public void Update()
    {
        PlayerTurnEnd();
        EnemyTurnEnd();
        BattleStart();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("----------OnBeginDrag----------");
        outline.enabled = true;

        CardPrefab = GetComponent<RectTransform>(); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        // MoveWhenDrag();
        isSelected = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("----------OnEndDrag----------");
        outline.enabled = false;

        isSelected = false;

        this.transform.localPosition = originalPosition;// 画像を元の位置に戻す

        PlayerMovement();
        EnemyMovement();

        Debug.Log("Player Count: " + p_Count);
        Debug.Log("Enemy Count: " + e_Count);
    }

    private void MoveWhenDrag()
    {
        // CanvasのRectTransformを取得
        RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        // マウスのスクリーン座標をCanvas内のローカル座標に変換
        Vector2 mousePosition = Input.mousePosition;
        Vector2 canvasPosition = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mousePosition, Camera.main, out canvasPosition);
        // Canvas内のローカル座標を世界座標に変換して、画像の位置を設定
        this.transform.localPosition = canvasPosition;
    }

    private void PlayerMovement()
    {
        if(gameObject.tag == "P_ATK" && MouseUIInteraction.isMouseOverUI == true)
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

    private void PlayerTurnEnd()
    {
        if(p_Count >= 3)
        {
            if(gameObject.tag == "P_ATK" || gameObject.tag == "P_DEF")
            {
                this.gameObject.SetActive(false);
            }
            player_Turn_End = true;
        }
    }

    private void EnemyTurnEnd()
    {
        if(e_Count >= 3)
        {
            if(gameObject.tag == "E_ATK" || gameObject.tag == "E_DEF")
            {
                this.gameObject.SetActive(false);
            }
            enemy_Turn_End = true;
        }
    }

    private void BattleStart()
    {
        if(player_Turn_End && enemy_Turn_End)
        {
            GameController.status_Now = GameController.game_Status.Battle;
            player_Turn_End = false;
            enemy_Turn_End = false;
            Debug.Log("OHHHHHHH! Battle Start!");//2回出るバグあり。。。
        }
    }
}
