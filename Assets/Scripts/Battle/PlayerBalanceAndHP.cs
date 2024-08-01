using UnityEngine;
using UnityEngine.UI;

public class PlayerBalanceAndHP : MonoBehaviour
{
    private int _player1HP;
    private int _player2HP;
    private int _player1Balance;
    private int _player2Balance;
    private const int maxHP = 11; // 最大HP
    private const int maxBalance = 3; // 最大バランス

    public int MaxHP => maxHP;
    public int MaxBalance => maxBalance;
    
    public Slider player1HPSlider; // プレイヤー1のHPスライダー
    public Slider player2HPSlider; // プレイヤー2のHPスライダー
    public Slider player1HealPreviewSlider; // プレイヤー1の回復プレビュースライダー
    public Slider player1DamagePreviewSlider; // プレイヤー1のダメージプレビュースライダー

    public GameObject[] player1BalanceIMG = new GameObject[maxBalance]; // プレイヤー1のバランス画像
    public GameObject[] player2BalanceIMG = new GameObject[maxBalance]; // プレイヤー2のバランス画像

    public GameObject player1Heart; // プレイヤー1のハートオブジェクト
    public GameObject player2Heart; // プレイヤー2のハートオブジェクト

    public GameObject player1NoBalance; // プレイヤー1のバランスがない場合の表示オブジェクト
    public GameObject player2NoBalance; // プレイヤー2のバランスがない場合の表示オブジェクト

    public int damagePreview; // ダメージプレビュー
    public int healPreview; // 回復プレビュー

    public int player1HP
    {
        get => _player1HP;
        set
        {
            _player1HP = Mathf.Clamp(value, 0, maxHP);
            UpdateHPSlider(player1HPSlider, _player1HP);
            UpdateHeartScale(player1Heart, _player1HP, maxHP);
        }
    }

    public int player2HP
    {
        get => _player2HP;
        set
        {
            _player2HP = Mathf.Clamp(value, 0, maxHP);
            UpdateHPSlider(player2HPSlider, _player2HP);
            UpdateHeartScale(player2Heart, _player2HP, maxHP);
        }
    }

    public int player1Balance
    {
        get => _player1Balance;
        set
        {
            _player1Balance = Mathf.Clamp(value, 0, maxBalance);
            UpdateBalanceIMG(player1BalanceIMG, _player1Balance);
            player1NoBalance.SetActive(_player1Balance == 0);
        }
    }

    public int player2Balance
    {
        get => _player2Balance;
        set
        {
            _player2Balance = Mathf.Clamp(value, 0, maxBalance);
            UpdateBalanceIMG(player2BalanceIMG, _player2Balance);
            player2NoBalance.SetActive(_player2Balance == 0);
        }
    }

    void Start()
    {
        player1HP = maxHP;
        player2HP = maxHP;
        player1Balance = maxBalance;
        player2Balance = maxBalance;

        if (player1HPSlider != null)
        {
            player1HPSlider.maxValue = maxHP;
            player1HPSlider.value = player1HP;
        }

        if (player2HPSlider != null)
        {
            player2HPSlider.maxValue = maxHP;
            player2HPSlider.value = player2HP;
        }

        if (player1HealPreviewSlider != null)
        {
            player1HealPreviewSlider.maxValue = maxHP;
            player1HealPreviewSlider.value = player1HP;
            player1HealPreviewSlider.gameObject.SetActive(false);
        }

        if (player1DamagePreviewSlider != null)
        {
            player1DamagePreviewSlider.maxValue = maxHP;
            player1DamagePreviewSlider.value = player2HP;
            player1DamagePreviewSlider.gameObject.SetActive(false);
        }

        UpdateBalanceIMG(player1BalanceIMG, player1Balance);
        UpdateBalanceIMG(player2BalanceIMG, player2Balance);

        UpdateHeartScale(player1Heart, player1HP, maxHP);
        UpdateHeartScale(player2Heart, player2HP, maxHP);
    }

    private void UpdateHPSlider(Slider slider, int hp)
    {
        if (slider != null)
        {
            slider.value = hp;
        }
        else
        {
            Debug.LogError("Slider is not assigned."); // スライダーが割り当てられていない場合のエラーメッセージ
        }
    }

    private void UpdateBalanceIMG(GameObject[] balanceIMG, int balance)
    {
        for (int i = 0; i < balanceIMG.Length; i++)
        {
            if (balanceIMG[i] != null)
            {
                balanceIMG[i].SetActive(i < balance); // バランスに応じて画像を表示/非表示
            }
            else
            {
                Debug.LogError("Balance image is not assigned at index: " + i);
            }
        }
    }

    private void UpdateHeartScale(GameObject heart, int hp, int maxHP)
    {
        if (heart != null)
        {
            float scale = hp > 0 ? (1.0f - (maxHP - hp) * 0.05f) * 70 : 0f;
            heart.transform.localScale = new Vector3(scale, scale, scale); // HPに基づいてハートのスケールを変更
        }
        else
        {
            Debug.LogError("Heart GameObject is not assigned."); // ハートオブジェクトが割り当てられていない場合のエラーメッセージ
        }
    }

    public void UpdatePreview(int damage, int heal)
    {
        damagePreview = Mathf.Clamp(player2HP - damage, 0, maxHP); // ダメージプレビューを更新
        healPreview = Mathf.Clamp(player1HP + heal, 0, maxHP); // 回復プレビューを更新

        UpdateHPSlider(player1HealPreviewSlider, healPreview);
        UpdateHPSlider(player1DamagePreviewSlider, damagePreview);
    }

    public void ShowPreviewSliders()
    {
        if (player1HealPreviewSlider != null)
        {
            player1HealPreviewSlider.gameObject.SetActive(true);
        }

        if (player1DamagePreviewSlider != null)
        {
            player1DamagePreviewSlider.gameObject.SetActive(true);
        }
    }

    public void HidePreviewSliders()
    {
        if (player1HealPreviewSlider != null)
        {
            player1HealPreviewSlider.gameObject.SetActive(false);
        }

        if (player1DamagePreviewSlider != null)
        {
            player1DamagePreviewSlider.gameObject.SetActive(false);
        }
    }

    public void UpdateHPSliders()
    {
        UpdateHPSlider(player1HPSlider, _player1HP);
        UpdateHPSlider(player2HPSlider, _player2HP);
    }

    public void UpdateBalance()
    {
        player1Balance = Mathf.Clamp(player1Balance, 0, maxBalance);
        player2Balance = Mathf.Clamp(player2Balance, 0, maxBalance);
    }
}
