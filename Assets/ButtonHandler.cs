using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public bool isATK;
    public bool isDEF;
    public bool isHeal;
    public bool isThrow;
    public bool isCNT;

    public delegate void ButtonPressedHandler(string buttonType);
    public static event ButtonPressedHandler OnButtonPressed;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(NotifyButtonPressed);
        }
    }

    void NotifyButtonPressed()
    {
        if (isATK && OnButtonPressed != null)
        {
            OnButtonPressed("ATK");
        }
        else if (isDEF && OnButtonPressed != null)
        {
            OnButtonPressed("DEF");
        }
        else if (isHeal && OnButtonPressed != null)
        {
            OnButtonPressed("Heal");
        }
        else if (isThrow && OnButtonPressed != null)
        {
            OnButtonPressed("Throw");
        }
        else if (isCNT && OnButtonPressed != null)
        {
            OnButtonPressed("CNT");
        }
    }
}