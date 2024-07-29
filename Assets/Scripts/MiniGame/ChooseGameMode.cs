using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChooseGameMode : MonoBehaviour
{
    public Toggle toggleOffline;
    public Toggle toggleOnline;
    public Button startButton;
    public Canvas canvasStart;

    public static event System.Action OnGameOffline;
    public static event System.Action OnGameOnline;
    public static event System.Action<int> OnGameStart; // 添加事件

    [SerializeField] private TMP_Dropdown dropdown;

    void Start()
    {
        SetOppositeState(toggleOffline.isOn);
        toggleOffline.onValueChanged.AddListener((value) => SetOppositeState(value));
        toggleOnline.onValueChanged.AddListener((value) => SetOppositeState(!value));
        startButton.onClick.AddListener(OnStartButtonClick);
        dropdown.gameObject.SetActive(false);
    }

    void Update()
    {
        OnToggleSelected();
    }

    void SetOppositeState(bool state)
    {
        canvasStart.gameObject.SetActive(true);
        toggleOffline.onValueChanged.RemoveAllListeners();
        toggleOnline.onValueChanged.RemoveAllListeners();

        toggleOffline.isOn = state;
        toggleOnline.isOn = !state;

        toggleOffline.onValueChanged.AddListener((value) => SetOppositeState(value));
        toggleOnline.onValueChanged.AddListener((value) => SetOppositeState(!value));
    }

    void OnStartButtonClick()
    {
        if (toggleOffline.isOn)
        {
            OnGameOffline?.Invoke();
            dropdown.gameObject.SetActive(false);
        }
        else if (toggleOnline.isOn)
        {
            OnGameOnline?.Invoke();
            dropdown.gameObject.SetActive(true);

            int currentPlayer = dropdown.value == 0 ? 1 : 2; // 0: Host, 1: Client
            OnGameStart?.Invoke(currentPlayer);
        }

        canvasStart.gameObject.SetActive(false);
    }

    void OnToggleSelected()
    {
        dropdown.gameObject.SetActive(!toggleOffline.isOn);
    }
}