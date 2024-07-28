using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonHandler : MonoBehaviour
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

    public float resetTime = 2.0f; // 指定时间（秒）

    public delegate void ButtonPressedHandler(string buttonType);
    public static event ButtonPressedHandler OnButtonPressed;

    private Button button;
    private RawImage rawImage;
    private Texture originalTexture; // 用于存储原先的纹理

    void Start()
    {
        button = GetComponent<Button>();
        rawImage = GetComponent<RawImage>();
        
        if (button != null)
        {
            button.onClick.AddListener(NotifyButtonPressed);
        }

        if (rawImage != null)
        {
            originalTexture = rawImage.texture; // 保存初始纹理
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
            button.interactable = false; // 禁用按钮
            StartCoroutine(ResetTextureAfterTime(resetTime)); // 启动协程来恢复原先的纹理
        }
    }

    IEnumerator ResetTextureAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        if (rawImage != null)
        {
            rawImage.texture = originalTexture;
            button.interactable = true; // 禁用按钮
        }
    }
}
