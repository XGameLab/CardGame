using UnityEngine;
using UnityEngine.UI;

public class ChooseGameMode : MonoBehaviour
{
    public Toggle toggleOffline;
    public Toggle toggleOnline;
    public Button startButton;
    public Canvas canvasStart;

    public static event System.Action OnGameOffline;
    public static event System.Action OnGameOnline;

    void Start()
    {
        // 初始化Toggle状态
        SetOppositeState(toggleOffline.isOn);
        
        // 监听Toggle Offline的状态变化
        toggleOffline.onValueChanged.AddListener((value) => SetOppositeState(value));
        
        // 监听Toggle Online的状态变化
        toggleOnline.onValueChanged.AddListener((value) => SetOppositeState(!value));

        // 监听Start按钮点击事件
        startButton.onClick.AddListener(OnStartButtonClick);
    }

    void SetOppositeState(bool state)
    {
        canvasStart.gameObject.SetActive(true);

        // 禁用监听器，防止无限循环
        toggleOffline.onValueChanged.RemoveAllListeners();
        toggleOnline.onValueChanged.RemoveAllListeners();

        // 设置Toggle的状态
        toggleOffline.isOn = state;
        toggleOnline.isOn = !state;

        // 恢复监听器
        toggleOffline.onValueChanged.AddListener((value) => SetOppositeState(value));
        toggleOnline.onValueChanged.AddListener((value) => SetOppositeState(!value));
    }

    void OnStartButtonClick()
    {
        // 根据Toggle的状态更新文本内容
        if (toggleOffline.isOn)
        {
            OnGameOffline?.Invoke();
        }
        else if (toggleOnline.isOn)
        {
            OnGameOnline?.Invoke();
        }

        // 隐藏Canvas
        canvasStart.gameObject.SetActive(false);
    }
}
