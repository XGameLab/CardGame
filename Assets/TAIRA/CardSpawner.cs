using System.Collections;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public GameObject[] cardPrefabs; // Cardプレハブの配列をインスペクターから設定
    public RectTransform canvasRectTransform; // キャンバスのRectTransformをインスペクターから設定
    private int cardCount = 0; // 生成したカードの数を追跡する変数

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && cardCount == 0)
        {
            // SpawnCardメソッドを1秒ごとに呼び出すように設定
            InvokeRepeating("SpawnCard", 0f, 0.5f);
        }
    }

    void SpawnCard()
    {
        if (cardCount < 5)
        {
            // ランダムなインデックスを配列から取得
            int randomIndex = Random.Range(0, cardPrefabs.Length);
            // ランダムなカードプレハブをインスタンス化
            GameObject newCard = Instantiate(cardPrefabs[randomIndex]);

            // カードをキャンバスの子要素に設定
            newCard.transform.SetParent(canvasRectTransform, false);

            // カードのRectTransformを取得
            RectTransform cardRect = newCard.GetComponent<RectTransform>();

            // カードの位置をキャンバスの中央に設定
            cardRect.anchoredPosition = Vector2.zero;

            // カードのサイズを設定（必要に応じて調整）
            cardRect.sizeDelta = new Vector2(100, 150);

            // カードの数を増やす
            cardCount++;

            // 5枚のカードが生成されたら、InvokeRepeatingをキャンセル
            if (cardCount >= 5)
            {
                CancelInvoke("SpawnCard");
            }
        }
    }
}
