using UnityEngine;
using UnityEngine.UI;

public class PlayerBalanceAndHP : MonoBehaviour
{
    private int _player1HP;
    private int _player2HP;
    private int _player1Balance;
    private int _player2Balance;
    private const int maxHP = 11;
    private const int maxBalance = 3;

    public int MaxHP => maxHP;
    public int MaxBalance => maxBalance;
    
    public Slider player1HPSlider;
    public Slider player2HPSlider;
    public Slider player1HealPreviewSlider;
    public Slider player1DamagePreviewSlider;

    public GameObject[] player1BalanceIMG = new GameObject[maxBalance];
    public GameObject[] player2BalanceIMG = new GameObject[maxBalance];

    public GameObject player1Heart;
    public GameObject player2Heart;

    public GameObject player1NoBalance;
    public GameObject player2NoBalance;

    public int damagePreview;
    public int healPreview;

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
            Debug.LogError("Slider is not assigned.");
        }
    }

    private void UpdateBalanceIMG(GameObject[] balanceIMG, int balance)
    {
        for (int i = 0; i < balanceIMG.Length; i++)
        {
            if (balanceIMG[i] != null)
            {
                balanceIMG[i].SetActive(i < balance);
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
            heart.transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            Debug.LogError("Heart GameObject is not assigned.");
        }
    }

    public void UpdatePreview(int damage, int heal)
    {
        damagePreview = Mathf.Clamp(player2HP - damage, 0, maxHP);
        healPreview = Mathf.Clamp(player1HP + heal, 0, maxHP);

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
