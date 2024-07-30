using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class ChooseGameMode : MonoBehaviourPunCallbacks
{
    public Toggle toggleOffline;
    public Toggle toggleOnline;
    public Button startButton;
    public Canvas canvasStart;
    public RawImage hostIMG;
    public RawImage clientIMG;
    public TMP_Text statusText;

    public static event System.Action OnGameOffline;
    public static event System.Action OnGameOnline;
    public static event System.Action<int> OnGameStart;

    [SerializeField] private TMP_Dropdown dropdown;

    private bool isConnecting;

    void Start()
    {
        hostIMG.gameObject.SetActive(false);
        clientIMG.gameObject.SetActive(false);

        UpdateStatusText();

        SetOppositeState(toggleOffline.isOn);
        toggleOffline.onValueChanged.AddListener((value) => SetOppositeState(value));
        toggleOnline.onValueChanged.AddListener((value) => SetOppositeState(!value));
        startButton.onClick.AddListener(OnStartButtonClick);
        dropdown.gameObject.SetActive(false);
    }

    void Update()
    {
        OnToggleSelected();
        UpdateStatusText();
    }

    void OnToggleValueChanged()
    {
        UpdateStatusText();
    }

    void UpdateStatusText()
    {
        if (toggleOffline.isOn)
        {
            statusText.text = "合計10点で終了";
        }
        else if (toggleOnline.isOn)
        {
            statusText.text = "先に10点取ろう！";
        }
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
            Debug.Log("Starting offline game...");
            OnGameOffline?.Invoke();
            dropdown.gameObject.SetActive(false);
        }
        else if (toggleOnline.isOn)
        {
            Debug.Log("Starting online game...");
            OnGameOnline?.Invoke();
            dropdown.gameObject.SetActive(true);

            int currentPlayer = 1;
            OnGameStart?.Invoke(currentPlayer);

            if (PhotonNetwork.IsConnectedAndReady)
            {
                if (dropdown.value == 0)
                {
                    CreateRoom();
                }
                else
                {
                    JoinRoom();
                }
            }
            else
            {
                isConnecting = true;
                PhotonNetwork.ConnectUsingSettings(); // 连接到Photon服务器
                Debug.Log("Connecting to Photon server...");
            }
        }

        canvasStart.gameObject.SetActive(false);
    }

    void OnToggleSelected()
    {
        dropdown.gameObject.SetActive(!toggleOffline.isOn);
    }

    void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
        Debug.Log("Creating a room...");
    }

    void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Joining a random room...");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server.");
        if (isConnecting)
        {
            isConnecting = false;
            if (dropdown.value == 0)
            {
                CreateRoom();
            }
            else
            {
                JoinRoom();
            }
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby.");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
        if (PhotonNetwork.IsMasterClient)
        {
            hostIMG.gameObject.SetActive(true); // 显示主机图像
        }
        else
        {
            clientIMG.gameObject.SetActive(true); // 显示客户端图像
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created successfully.");
        hostIMG.gameObject.SetActive(true); // 在房间成功创建后显示 hostIMG
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Failed to join room: {message}");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Failed to create room: {message}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join random room. Creating a new room...");
        CreateRoom();
    }
}
