using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardPressed : MonoBehaviour
{
    public bool isATK;
    public bool isDEF;
    public bool isHeal;
    public bool isThrow;
    public bool isCNT;

    public Texture atkTexture;
    public Texture defTexture;
    public Texture healTexture;
    public Texture throwTexture;
    public Texture cntTexture;
    public Texture defaultTex;
    public static event System.Action OnSameTypeCardsMatched;
    public static event System.Action OnDifferentTypeCardsMatched;

    private static CardPressed firstCard;
    private static CardPressed secondCard;
    private RawImage rawImage;
    private Button button;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        button = GetComponent<Button>();

        if (rawImage == null)
        {
            Debug.LogError("RawImage component is missing.");
            return;
        }

        if (button != null)
        {
            button.onClick.AddListener(OnCardClicked);
        }
        else
        {
            Debug.LogError("Button component is missing.");
        }
    }

    void OnCardClicked()
    {
        if (firstCard == null)
        {
            firstCard = this;
            ChangeCardImage();
        }
        else if (secondCard == null && firstCard != this)
        {
            secondCard = this;
            ChangeCardImage();
            CompareCards();
        }
    }

    void ChangeCardImage()
    {
        if (isATK)
        {
            rawImage.texture = atkTexture;
        }
        else if (isDEF)
        {
            rawImage.texture = defTexture;
        }
        else if (isHeal)
        {
            rawImage.texture = healTexture;
        }
        else if (isThrow)
        {
            rawImage.texture = throwTexture;
        }
        else if (isCNT)
        {
            rawImage.texture = cntTexture;
        }
    }

    void CompareCards()
    {
        if (firstCard != null && secondCard != null)
        {
            // 比较卡牌种类
            if ((firstCard.isATK && secondCard.isATK) ||
                (firstCard.isDEF && secondCard.isDEF) ||
                (firstCard.isHeal && secondCard.isHeal) ||
                (firstCard.isThrow && secondCard.isThrow) ||
                (firstCard.isCNT && secondCard.isCNT))
            {
                Debug.Log("The two cards are of the same type.");
                DisableCardInteractivity(firstCard);
                DisableCardInteractivity(secondCard);

                // 触发匹配成功事件
                OnSameTypeCardsMatched?.Invoke();

                ResetCards();
            }
            else
            {
                Debug.Log("The two cards are of different types.");
                OnDifferentTypeCardsMatched?.Invoke(); // 触发匹配失败事件
                StartCoroutine(ResetCardsAfterDelay(1f));
            }
        }
    }

    void DisableCardInteractivity(CardPressed card)
    {
        if (card != null && card.button != null)
        {
            card.button.interactable = false; // 禁用卡牌的按钮交互
        }
    }

    void ResetCards()
    {
        firstCard = null;
        secondCard = null;
    }

    IEnumerator ResetCardsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (firstCard != null)
        {
            firstCard.rawImage.texture = defaultTex;
        }

        if (secondCard != null)
        {
            secondCard.rawImage.texture = defaultTex;
        }

        ResetCards();
    }
}
