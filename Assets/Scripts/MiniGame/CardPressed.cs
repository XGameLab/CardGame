using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class CardPressed : MonoBehaviourPunCallbacks
{
    public bool isATK; // 攻撃カードかどうかのフラグ
    public bool isDEF; // 防御カードかどうかのフラグ
    public bool isHeal; // 回復カードかどうかのフラグ
    public bool isThrow; // 投げ技カードかどうかのフラグ
    public bool isCNT; // カウンターカードかどうかのフラグ

    public Texture atkTexture; // 攻撃カードのテクスチャ
    public Texture defTexture; // 防御カードのテクスチャ
    public Texture healTexture; // 回復カードのテクスチャ
    public Texture throwTexture; // 投げ技カードのテクスチャ
    public Texture cntTexture; // カウンターカードのテクスチャ
    public Texture defaultTex; // デフォルトのテクスチャ

    public static event System.Action OnSameTypeCardsMatched; // 同じタイプのカードが一致したときのイベント
    public static event System.Action OnDifferentTypeCardsMatched; // 異なるタイプのカードが一致したときのイベント

    private static CardPressed firstCard; // 最初にクリックされたカード
    private static CardPressed secondCard; // 二番目にクリックされたカード
    private RawImage rawImage;
    private Button button;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        button = GetComponent<Button>();

        if (ScoreManager.Instance == null)
        {
            Debug.LogError("ScoreManager インスタンスが見つかりません。");
        }

        if (rawImage == null)
        {
            Debug.LogError("RawImage コンポーネントが不足しています。");
            return;
        }

        if (button != null)
        {
            button.onClick.AddListener(OnCardClicked);
        }
        else
        {
            Debug.LogError("Button コンポーネントが不足しています。");
        }
    }

    void OnCardClicked()
    {
        int currentPlayer = ScoreManager.Instance.CurrentPlayer;
        bool isOfflineMode = ScoreManager.Instance.IsOfflineMode;

        // 現在のプレイヤーとネットワーク状態をチェックし、クリックを許可するかどうかを決定
        if (isOfflineMode || (currentPlayer == 1 && PhotonNetwork.IsMasterClient) || (currentPlayer == 2 && !PhotonNetwork.IsMasterClient))
        {
            HandleCardClick();
        }
        else
        {
            Debug.Log("カードクリックは許可されていません。CurrentPlayer: " + currentPlayer);
        }
    }

    private void HandleCardClick()
    {
        if (firstCard == null)
        {
            firstCard = this; // 最初にクリックされたカードを保存
            ChangeCardImage(isATK, isDEF, isHeal, isThrow, isCNT); // カードの画像を変更
        }
        else if (secondCard == null && firstCard != this)
        {
            secondCard = this; // 二番目にクリックされたカードを保存
            ChangeCardImage(isATK, isDEF, isHeal, isThrow, isCNT); // カードの画像を変更
            CompareCards(); // カードを比較
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

        Debug.Log($"カードがクリックされました。タイプ: {GetCardType(this)}");
    }

    void CompareCards()
    {
        if (firstCard != null && secondCard != null)
        {
            // カードのタイプを比較
            if ((firstCard.isATK && secondCard.isATK) ||
                (firstCard.isDEF && secondCard.isDEF) ||
                (firstCard.isHeal && secondCard.isHeal) ||
                (firstCard.isThrow && secondCard.isThrow) ||
                (firstCard.isCNT && secondCard.isCNT))
            {
                Debug.Log("2枚のカードは同じタイプです。");
                DisableCardInteractivity(firstCard); // カードのインタラクティビティを無効にする
                DisableCardInteractivity(secondCard);

                OnSameTypeCardsMatched?.Invoke(); // 同じタイプのカードが一致したイベントを発火
                ResetCards();
            }
            else
            {
                Debug.Log("2枚のカードは異なるタイプです。");
                OnDifferentTypeCardsMatched?.Invoke(); // 異なるタイプのカードが一致したイベントを発火
                StartCoroutine(ResetCardsAfterDelay(1f)); // 遅延後にカードをリセット
            }
        }
    }

    void DisableCardInteractivity(CardPressed card)
    {
        if (card.button != null)
        {
            card.button.interactable = false; // カードのインタラクティビティを無効にする
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
