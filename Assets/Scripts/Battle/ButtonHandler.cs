using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonHandler : MonoBehaviour
{
    public bool isATK; // 攻撃ボタンかどうか
    public bool isDEF; // 防御ボタンかどうか
    public bool isHeal; // 回復ボタンかどうか
    public bool isThrow; // 投げ技ボタンかどうか
    public bool isCNT; // 反撃ボタンかどうか

    public Texture atkTexture; // 攻撃時のテクスチャ
    public Texture defTexture; // 防御時のテクスチャ
    public Texture healTexture; // 回復時のテクスチャ
    public Texture throwTexture; // 投げ技時のテクスチャ
    public Texture cntTexture; // 反撃時のテクスチャ

    public float resetTime = 2.0f; // リセット時間（秒）

    public delegate void ButtonPressedHandler(string buttonType);
    public static event ButtonPressedHandler OnButtonPressed;

    private Button button; // ボタンコンポーネント
    private RawImage rawImage; // RawImageコンポーネント
    private Texture originalTexture; // 元のテクスチャ

    void Start()
    {
        button = GetComponent<Button>();
        rawImage = GetComponent<RawImage>();
        
        if (button != null)
        {
            button.onClick.AddListener(NotifyButtonPressed); // ボタン押下時のイベントリスナーを追加
        }

        if (rawImage != null)
        {
            originalTexture = rawImage.texture; // 初期テクスチャを保存
        }
    }

    public void NotifyButtonPressed()
    {
        if (rawImage != null)
        {
            if (isATK && OnButtonPressed != null)
            {
                OnButtonPressed("ATK");
                rawImage.texture = atkTexture;
                Debug.Log("It's ATK");
            }
            else if (isDEF && OnButtonPressed != null)
            {
                OnButtonPressed("DEF");
                rawImage.texture = defTexture;
                // Debug.Log("It's DEF");
            }
            else if (isHeal && OnButtonPressed != null)
            {
                OnButtonPressed("Heal");
                rawImage.texture = healTexture;
                // Debug.Log("It's Heal");
            }
            else if (isThrow && OnButtonPressed != null)
            {
                OnButtonPressed("Throw");
                rawImage.texture = throwTexture;
                // Debug.Log("It's throw");
            }
            else if (isCNT && OnButtonPressed != null)
            {
                OnButtonPressed("CNT");
                rawImage.texture = cntTexture;
                // Debug.Log("It's CNT");
            }
            else
            {
                Debug.Log("Something is going wrong...");
            }
            button.interactable = false; // ボタンを無効化
            StartCoroutine(ResetTextureAfterTime(resetTime)); // 指定時間後に元のテクスチャに戻すコルーチンを開始
        }
    }

    IEnumerator ResetTextureAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        if (rawImage != null)
        {
            rawImage.texture = originalTexture; // 元のテクスチャに戻す
            button.interactable = true; // ボタンを有効化
        }
    }
}
