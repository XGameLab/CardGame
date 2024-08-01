using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CardEffect : MonoBehaviour
{
    public GameObject[] cardPrefabs; // カードプレハブの配列
    public RectTransform drawPosition; // カードを引く位置
    public RectTransform[] handPositions; // 手札の位置の配列
    public RectTransform[] selectedPositions; // 選択されたカードの位置の配列
    public float drawDuration = 0.5f; // アニメーションの持続時間
    public float drawInterval = 0.5f; // カードを引く間隔時間
    public Canvas canvas; // 親 Canvas オブジェクト
    public int cardsToDraw = 5; // 引くカードの枚数
    public float moveDuration = 0.25f; // 移動アニメーションの持続時間
    public Button submitButton; // 提出ボタン

    public Player2AI player2AI; // Player2AI スクリプトの参照
    public BattleInfoManager battleInfoManager; // BattleInfoManager スクリプトの参照
    public AudioManager audioManager; // AudioManager スクリプトの参照

    private int currentHandPositionIndex = 0; // 現在の手札位置インデックス
    private List<int> availableCardIndices; // 使用可能なカードインデックスのリスト
    private List<GameObject> currentHandCards; // 現在の手札のカードリスト
    private List<GameObject> cardPool; // カードのオブジェクトプール
    private GameObject selectedCard; // 現在選択されたカード

    string GetButtonType(ButtonHandler handler)
    {
        if (handler.isATK) return "ATK";
        if (handler.isDEF) return "DEF";
        if (handler.isHeal) return "Heal";
        if (handler.isThrow) return "Throw";
        if (handler.isCNT) return "CNT";
        return string.Empty;
    }

    void Start()
    {
        InitializeCardIndices(); // カードインデックスを初期化
        currentHandCards = new List<GameObject>(); // 手札のカードリストを初期化
        cardPool = new List<GameObject>(); // カードのオブジェクトプールを初期化

        // カードのオブジェクトプールを事前にロード
        foreach (GameObject prefab in cardPrefabs)
        {
            GameObject card = Instantiate(prefab, canvas.transform);
            card.SetActive(false); // 初期状態で非アクティブ
            cardPool.Add(card);

            // ボタンクリックイベントリスナーを追加
            Button cardButton = card.GetComponent<Button>();
            if (cardButton != null)
            {
                cardButton.onClick.AddListener(() => OnCardClicked(card));
            }
        }

        // 提出ボタンのクリックイベントリスナーを追加
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitButtonClicked);
            submitButton.interactable = false; // 初期状態で無効化
        }
    }

    void Update()
    {
        // Dキーが押された時にカードを引く
        if (Input.GetKeyDown(KeyCode.D) && !battleInfoManager.isGameOver)
        {
            StartCoroutine(DrawCards(cardsToDraw));
        }

        // 数字キーが押された時にカードを選択
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            SelectCardByIndex(0); // 最初のカードを選択
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            SelectCardByIndex(1); // 二番目のカードを選択
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            SelectCardByIndex(2); // 三番目のカードを選択
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            SelectCardByIndex(3); // 四番目のカードを選択
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            SelectCardByIndex(4); // 五番目のカードを選択
        }

        // Enterキーが押された時にカードを確定
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && selectedCard != null)
        {
            OnEnterClicked(); // 提出ボタンのクリック処理を実行
        }
    }

    void InitializeCardIndices()
    {
        availableCardIndices = new List<int>();
        for (int i = 0; i < cardPrefabs.Length; i++)
        {
            availableCardIndices.Add(i);
        }
        currentHandPositionIndex = 0; // 現在の手札位置インデックスをリセット
    }

    // インデックスでカードを選択
    void SelectCardByIndex(int index)
    {
        if (index >= 0 && index < currentHandCards.Count)
        {
            GameObject card = currentHandCards[index];
            OnCardClicked(card); // カードクリック処理を実行

            // ボタンタイプを取得してイベントをトリガー
            ButtonHandler buttonHandler = card.GetComponent<ButtonHandler>();
            if (buttonHandler != null)
            {
                string buttonType = GetButtonType(buttonHandler);
                buttonHandler.NotifyButtonPressed();
            }
        }
    }

    IEnumerator DrawCards(int numberOfCards)
    {
        for (int i = 0; i < numberOfCards && currentHandPositionIndex < handPositions.Length; i++)
        {
            DrawCard();
            yield return new WaitForSeconds(drawInterval); // 指定した間隔時間を待つ
        }

        // Player2のカードを初期化
        player2AI.InitializePlayer2Cards(availableCardIndices, cardPrefabs);
    }

    void DrawCard()
    {
        if (cardPrefabs == null || cardPrefabs.Length == 0 || drawPosition == null || handPositions == null || canvas == null)
        {
            Debug.LogError("CardPrefabs, DrawPosition, HandPositions, or Canvas is not set.");
            return;
        }

        if (availableCardIndices.Count == 0)
        {
            Debug.LogWarning("No more cards available to draw.");
            return;
        }

        // 現在の手札位置を取得
        RectTransform currentHandPosition = handPositions[currentHandPositionIndex];

        // ランダムにカードを選択
        int randomIndex = Random.Range(0, availableCardIndices.Count);
        int cardIndex = availableCardIndices[randomIndex];
        availableCardIndices.RemoveAt(randomIndex); // 選択したカードのインデックスをリストから削除

        // オブジェクトプールからカードを取得
        GameObject newCard = cardPool[cardIndex];
        newCard.SetActive(true);
        RectTransform cardRectTransform = newCard.GetComponent<RectTransform>();

        if (cardRectTransform == null)
        {
            Debug.LogError("CardPrefab does not have a RectTransform component.");
            return;
        }

        // カードの初期位置を設定
        cardRectTransform.anchoredPosition = drawPosition.anchoredPosition;

        // LeanTweenを使用してカードを手札位置に移動
        LeanTween.move(cardRectTransform, currentHandPosition.anchoredPosition, drawDuration).setEase(LeanTweenType.easeOutQuad);

        // その他のアニメーション効果を追加
        LeanTween.rotateZ(newCard, 0, drawDuration).setEase(LeanTweenType.easeOutQuad);
        LeanTween.scale(newCard, Vector3.one, drawDuration).setEase(LeanTweenType.easeOutQuad);

        // Debug.Log("Card is being moved to position: " + currentHandPositionIndex);

        // 次の手札位置にインデックスを更新
        currentHandPositionIndex++;

        // 新しいカードを現在の手札リストに追加
        currentHandCards.Add(newCard);
    }

    public void ResetHand()
    {
        // 現在の手札のすべてのカードをリセット
        foreach (GameObject card in currentHandCards)
        {
            RectTransform cardRectTransform = card.GetComponent<RectTransform>();
            if (cardRectTransform != null)
            {
                LeanTween.move(cardRectTransform, drawPosition.anchoredPosition, moveDuration).setEase(LeanTweenType.easeOutQuad);
            }
            card.SetActive(false); // カードを非表示にする
        }

        // 現在の手札リストをクリア
        currentHandCards.Clear();

        // 選択されたカードをクリア
        selectedCard = null;

        // カードインデックスを再初期化
        InitializeCardIndices();

        // 提出ボタンを無効化
        if (submitButton != null)
        {
            submitButton.interactable = false;
        }

        Debug.Log("Hand has been reset.");
    }

    void OnCardClicked(GameObject card)
    {
        if (selectedCard != card && selectedCard != null)
        {
            // 以前に選択されたカードの位置を復元
            int previousCardIndex = currentHandCards.IndexOf(selectedCard);
            if (previousCardIndex >= 0 && previousCardIndex < handPositions.Length)
            {
                RectTransform previousCardRectTransform = selectedCard.GetComponent<RectTransform>();
                if (previousCardRectTransform != null)
                {
                    LeanTween.move(previousCardRectTransform, handPositions[previousCardIndex].anchoredPosition, moveDuration).setEase(LeanTweenType.easeOutQuad);
                }
            }
        }

        if (selectedCard != card)
        {
            // 新しく選択されたカードの位置を移動
            int cardIndex = currentHandCards.IndexOf(card);
            if (cardIndex >= 0 && cardIndex < selectedPositions.Length)
            {
                RectTransform cardRectTransform = card.GetComponent<RectTransform>();
                if (cardRectTransform != null)
                {
                    LeanTween.move(cardRectTransform, selectedPositions[cardIndex].anchoredPosition, moveDuration).setEase(LeanTweenType.easeOutQuad);
                    selectedCard = card;

                    // 提出ボタンを有効化
                    if (submitButton != null)
                    {
                        submitButton.interactable = true;
                    }
                }
            }
        }
    }

    void OnSubmitButtonClicked()
    {
        // 提出ボタンを無効化
        if (submitButton != null)
        {
            submitButton.interactable = false;
        }

        selectedCard = null;
        StartCoroutine(ResetCards());
    }

    void OnEnterClicked()
    {
        // 提出ボタンを無効化
        if (submitButton != null)
        {
            submitButton.interactable = false;
        }

        if (battleInfoManager != null)
        {
            battleInfoManager.OnSubmitButtonClicked();
        }
        audioManager.PlayRandomBattleSE();
        selectedCard = null;
        StartCoroutine(ResetCards());
    }

    IEnumerator ResetCards()
    {
        // 逆順にすべてのカード位置をリセット
        for (int i = currentHandCards.Count - 1; i >= 0; i--)
        {
            GameObject card = currentHandCards[i];
            RectTransform cardRectTransform = card.GetComponent<RectTransform>();
            if (cardRectTransform != null)
            {
                LeanTween.move(cardRectTransform, drawPosition.anchoredPosition, moveDuration).setEase(LeanTweenType.easeOutQuad);
            }
            yield return new WaitForSeconds(drawInterval);
        }

        // すべてのカードを非表示にして手札リストをクリア
        foreach (GameObject card in currentHandCards)
        {
            card.SetActive(false); // カードを非表示にする
        }
        currentHandCards.Clear();

        // 選択されたカードをクリア
        selectedCard = null;

        // カードインデックスを再初期化
        InitializeCardIndices();

        Debug.Log("All cards have been reset.");
    }
}
