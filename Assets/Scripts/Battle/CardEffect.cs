using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CardEffect : MonoBehaviour
{
    public GameObject[] cardPrefabs; // 卡牌种类预制体数组
    public RectTransform drawPosition; // 抽牌位置
    public RectTransform[] handPositions; // 手牌位置数组
    public RectTransform[] selectedPositions; // 被选中卡牌的位置数组
    public float drawDuration = 0.5f; // 动画持续时间
    public float drawInterval = 0.5f; // 发牌间隔时间
    public Canvas canvas; // 父 Canvas 对象
    public int cardsToDraw = 5; // 要抽取的卡牌数量
    public float moveDuration = 0.25f; // 移动动画的持续时间
    public Button submitButton; // 提交按钮

    public Player2AI player2AI; // 引用Player2AI脚本
    public BattleInfoManager battleInfoManager;

    private int currentHandPositionIndex = 0; // 当前手牌位置索引
    private List<int> availableCardIndices; // 可用卡牌索引列表
    private List<GameObject> currentHandCards; // 当前手牌中的卡牌
    private List<GameObject> cardPool; // 卡牌对象池
    private GameObject selectedCard; // 当前选中的卡牌

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
        InitializeCardIndices();
        currentHandCards = new List<GameObject>();
        cardPool = new List<GameObject>();

        // 预加载卡牌对象池
        foreach (GameObject prefab in cardPrefabs)
        {
            GameObject card = Instantiate(prefab, canvas.transform);
            card.SetActive(false);
            cardPool.Add(card);

            // 添加按钮点击事件监听器
            Button cardButton = card.GetComponent<Button>();
            if (cardButton != null)
            {
                cardButton.onClick.AddListener(() => OnCardClicked(card));
            }
        }

        // 为提交按钮添加点击事件监听器
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitButtonClicked);
            submitButton.interactable = false; // 初始状态禁用提交按钮
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) && !battleInfoManager.isGameOver)
        {
            StartCoroutine(DrawCards(cardsToDraw));
        }

        // 检测数字键的按下
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            SelectCardByIndex(0); // 选择第一张卡牌
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            SelectCardByIndex(1); // 选择第二张卡牌
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            SelectCardByIndex(2); // 选择第三张卡牌
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            SelectCardByIndex(3); // 选择第四张卡牌
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            SelectCardByIndex(4); // 选择第五张卡牌
        }

        // 检测Enter键的按下
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && selectedCard != null)
        {
            OnEnterClicked(); // 调用提交按钮的点击处理函数
        }
    }

    void InitializeCardIndices()
    {
        availableCardIndices = new List<int>();
        for (int i = 0; i < cardPrefabs.Length; i++)
        {
            availableCardIndices.Add(i);
        }
        currentHandPositionIndex = 0;
    }

    // 通过索引选择卡牌
    void SelectCardByIndex(int index)
    {
        if (index >= 0 && index < currentHandCards.Count)
        {
            GameObject card = currentHandCards[index];
            OnCardClicked(card); // 调用卡牌点击处理函数

            // 获取按钮类型并触发事件
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
            yield return new WaitForSeconds(drawInterval); // 等待指定的时间间隔
        }

        // 初始化Player2的卡牌
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

        // 获取当前手牌位置
        RectTransform currentHandPosition = handPositions[currentHandPositionIndex];

        // 随机选择一种未被抽取的卡牌
        int randomIndex = Random.Range(0, availableCardIndices.Count);
        int cardIndex = availableCardIndices[randomIndex];
        availableCardIndices.RemoveAt(randomIndex); // 移除已抽取的卡牌索引

        // 从对象池中获取卡牌
        GameObject newCard = cardPool[cardIndex];
        newCard.SetActive(true);
        RectTransform cardRectTransform = newCard.GetComponent<RectTransform>();

        if (cardRectTransform == null)
        {
            Debug.LogError("CardPrefab does not have a RectTransform component.");
            return;
        }

        // 设置卡牌的初始位置
        cardRectTransform.anchoredPosition = drawPosition.anchoredPosition;

        // 使用 LeanTween 移动卡牌到当前手牌位置
        LeanTween.move(cardRectTransform, currentHandPosition.anchoredPosition, drawDuration).setEase(LeanTweenType.easeOutQuad);

        // 你可以在这里添加更多动画效果，比如旋转、缩放等
        LeanTween.rotateZ(newCard, 0, drawDuration).setEase(LeanTweenType.easeOutQuad);
        LeanTween.scale(newCard, Vector3.one, drawDuration).setEase(LeanTweenType.easeOutQuad);

        // Debug.Log("Card is being moved to position: " + currentHandPositionIndex);

        // 更新索引，指向下一个手牌位置
        currentHandPositionIndex++;

        // 将新卡牌加入当前手牌列表
        currentHandCards.Add(newCard);
    }

    public void ResetHand()
    {
        // 复位当前手牌中的所有卡牌
        foreach (GameObject card in currentHandCards)
        {
            RectTransform cardRectTransform = card.GetComponent<RectTransform>();
            if (cardRectTransform != null)
            {
                LeanTween.move(cardRectTransform, drawPosition.anchoredPosition, moveDuration).setEase(LeanTweenType.easeOutQuad);
            }
            card.SetActive(false); // 隐藏卡牌而不是销毁
        }

        // 清空当前手牌列表
        currentHandCards.Clear();

        // 清除选中的卡牌
        selectedCard = null;

        // 重新初始化卡牌索引
        InitializeCardIndices();

        // 禁用提交按钮
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
            // 还原先前选中的卡牌位置
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
            // 移动新选中的卡牌位置
            int cardIndex = currentHandCards.IndexOf(card);
            if (cardIndex >= 0 && cardIndex < selectedPositions.Length)
            {
                RectTransform cardRectTransform = card.GetComponent<RectTransform>();
                if (cardRectTransform != null)
                {
                    LeanTween.move(cardRectTransform, selectedPositions[cardIndex].anchoredPosition, moveDuration).setEase(LeanTweenType.easeOutQuad);
                    selectedCard = card;

                    // 启用提交按钮
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
        // 禁用提交按钮
        if (submitButton != null)
        {
            submitButton.interactable = false;
        }

        selectedCard = null;
        StartCoroutine(ResetCards());
    }

    void OnEnterClicked()
    {
        // 禁用提交按钮
        if (submitButton != null)
        {
            submitButton.interactable = false;
        }

        if (battleInfoManager != null)
        {
            battleInfoManager.OnSubmitButtonClicked();
        }
        selectedCard = null;
        StartCoroutine(ResetCards());
    }

    IEnumerator ResetCards()
    {
        // 按倒序复位所有卡牌位置
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

        // 隐藏所有卡牌并清空当前手牌列表
        foreach (GameObject card in currentHandCards)
        {
            card.SetActive(false); // 隐藏卡牌而不是销毁
        }
        currentHandCards.Clear();

        // 清除选中的卡牌
        selectedCard = null;

        // 重新初始化卡牌索引
        InitializeCardIndices();

        Debug.Log("All cards have been reset.");
    }
}
