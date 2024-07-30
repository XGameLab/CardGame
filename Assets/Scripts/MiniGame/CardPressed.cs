using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class CardPressed : MonoBehaviourPunCallbacks
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

        if (ScoreManager.Instance == null)
        {
            Debug.LogError("ScoreManager instance not found.");
        }

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
        int currentPlayer = ScoreManager.Instance.CurrentPlayer;
        bool isOfflineMode = ScoreManager.Instance.IsOfflineMode;

        // 检查当前玩家和网络状态，决定是否允许点击
        if (isOfflineMode || (currentPlayer == 1 && PhotonNetwork.IsMasterClient) || (currentPlayer == 2 && !PhotonNetwork.IsMasterClient))
        {
            HandleCardClick();
        }
        else
        {
            Debug.Log("Card click not allowed. CurrentPlayer: " + currentPlayer);
        }
    }

    private void HandleCardClick()
    {
        if (firstCard == null)
        {
            firstCard = this;
            ChangeCardImage(isATK, isDEF, isHeal, isThrow, isCNT);
        }
        else if (secondCard == null && firstCard != this)
        {
            secondCard = this;
            ChangeCardImage(isATK, isDEF, isHeal, isThrow, isCNT);
            CompareCards();
        }
    }

    void ChangeCardImage(bool atk, bool def, bool heal, bool thw, bool cnt)
    {
        if (atk)
        {
            rawImage.texture = atkTexture;
        }
        else if (def)
        {
            rawImage.texture = defTexture;
        }
        else if (heal)
        {
            rawImage.texture = healTexture;
        }
        else if (thw)
        {
            rawImage.texture = throwTexture;
        }
        else if (cnt)
        {
            rawImage.texture = cntTexture;
        }

        Debug.Log($"Card clicked. Type: {GetCardType(this)}");
    }

    void CompareCards()
    {
        if (firstCard != null && secondCard != null)
        {
            if ((firstCard.isATK && secondCard.isATK) ||
                (firstCard.isDEF && secondCard.isDEF) ||
                (firstCard.isHeal && secondCard.isHeal) ||
                (firstCard.isThrow && secondCard.isThrow) ||
                (firstCard.isCNT && secondCard.isCNT))
            {
                Debug.Log("The two cards are of the same type.");
                DisableCardInteractivity(firstCard);
                DisableCardInteractivity(secondCard);

                OnSameTypeCardsMatched?.Invoke();
                ResetCards();
            }
            else
            {
                Debug.Log("The two cards are of different types.");
                OnDifferentTypeCardsMatched?.Invoke();
                StartCoroutine(ResetCardsAfterDelay(1f));
            }
        }
    }

    void DisableCardInteractivity(CardPressed card)
    {
        if (card.button != null)
        {
            card.button.interactable = false;
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

    string GetCardType(CardPressed card)
    {
        if (card.isATK) return "ATK";
        if (card.isDEF) return "DEF";
        if (card.isHeal) return "Heal";
        if (card.isThrow) return "Throw";
        if (card.isCNT) return "CNT";
        return "Unknown";
    }
}
