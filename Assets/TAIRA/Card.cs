using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// カード処理クラス
/// </summary>
public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	
    private RectTransform cardTransform;
    [SerializeField] private float duration = 0.5f;//移動速度
    private float currentTime = 0.0f;
    private Vector2 Sposition; // スタート位置のレクト座標
    private Vector2 Eposition; // エンド位置
    private RectTransform endPositionRect; // エンド位置のRectTransform

    // Start(シーン開始時orインスタンス作成時に1回実行)
    void Start()
    {
        FindPos();
    }

    // Update(毎フレーム1回ずつ実行)
    void Update()
    {
        Move();
    }

     public void FindPos()//移動時のスタート位置とゴール位置を探す
    {
         RectTransform spositionRect = GameObject.Find("StartPosition").GetComponent<RectTransform>();
        if (spositionRect != null)
        {
            Sposition = spositionRect.anchoredPosition;
        }
        else
        {
            Debug.LogError("StartPosition オブジェクトが見つかりません。");
        }

        endPositionRect = GameObject.Find("EndPosition").GetComponent<RectTransform>();
        if (endPositionRect != null)
        {
            Eposition = endPositionRect.anchoredPosition;
        }
        else
        {
            Debug.LogError("EndPosition オブジェクトが見つかりません。");
        }

        cardTransform = GetComponent<RectTransform>();
    }

    public void Move()//カードの移動処理
    {
        if (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float t = Mathf.Clamp01(currentTime / duration);
            cardTransform.anchoredPosition = Vector2.Lerp(Sposition, Eposition, t);
        }
        else
        {
            currentTime = duration; // 現在時間を期間に設定して、移動を停止します。
            if (cardTransform.parent != endPositionRect)
            {
                cardTransform.SetParent(endPositionRect, false);
            }
        }

        if (Sposition == null)
    {
        Debug.LogError("Eposition が null です。");
        return;
    }

    }

	/// <summary>
	/// タップ開始時に実行
	/// IPointerDownHandlerが必要
	/// </summary>
	/// <param name="eventData">タップ情報</param>
	public void OnPointerDown (PointerEventData eventData)
	{
		Debug.Log ("カードがタップされました");
	}
	/// <summary>
	/// タップ終了時に実行
	/// IPointerUpHandlerが必要
	/// </summary>
	/// <param name="eventData">タップ情報</param>
	public void OnPointerUp (PointerEventData eventData)
	{
		Debug.Log ("カードへのタップを終了しました");
	}
}