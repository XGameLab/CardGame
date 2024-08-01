using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameStart : MonoBehaviourPunCallbacks
{
    public RectTransform[] cardPos; // カードの位置配列
    public GameObject[] cardPrefabs; // カードのプレハブ配列

    private int maxPerPrefab = 4; // 各プレハブの最大生成数
    private int[] prefabCounts; // 各プレハブの生成数を追跡する配列

    [SerializeField]
    private int seed; // ランダムシード、インスペクターで公開

    private const string SeedProperty = "GameSeed"; // シードのプロパティキー
    private bool isGameStarted = false; // ゲームが開始されたかどうかのフラグ

    void Start()
    {
        prefabCounts = new int[cardPrefabs.Length];
        PhotonNetwork.AddCallbackTarget(this); // コールバックターゲットを追加
    }

    void OnEnable()
    {
        ScoreManager.OnRetryGame += RetrySpawnCards; // リトライイベントにリスナーを追加
        ChooseGameMode.OnGameOffline += StartOfflineGame; // オフラインゲーム開始時のイベントリスナーを追加
        ChooseGameMode.OnGameOnline += StartOnlineGame; // オンラインゲーム開始時のイベントリスナーを追加
    }

    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this); // コールバックターゲットを削除
        ScoreManager.OnRetryGame -= RetrySpawnCards; // リトライイベントリスナーを削除
        ChooseGameMode.OnGameOffline -= StartOfflineGame; // オフラインゲーム開始リスナーを削除
        ChooseGameMode.OnGameOnline -= StartOnlineGame; // オンラインゲーム開始リスナーを削除
    }

    private void StartOfflineGame()
    {
        isGameStarted = true;
        GenerateSeedForOfflineMode(); // オフラインモード用のシードを生成
        SpawnCards(); // カードを生成
    }

    private void StartOnlineGame()
    {
        isGameStarted = true;
        // オンラインモードの処理はOnJoinedRoomで行う
    }

    private void GenerateAndSetSeed()
    {
        seed = Random.Range(int.MinValue, int.MaxValue);
        Debug.Log("ホストのシード: " + seed);

        ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable
        {
            { SeedProperty, seed }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties); // シードをルームプロパティに設定
    }

    private void GenerateSeedForOfflineMode()
    {
        seed = Random.Range(int.MinValue, int.MaxValue);
        Debug.Log("オフラインモードのシード: " + seed);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GenerateAndSetSeed(); // シードを生成して設定
            if (isGameStarted)
            {
                SpawnCards(); // カードを生成
            }
        }
        else
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(SeedProperty, out object seedValue))
            {
                seed = (int)seedValue;
                Debug.Log("クライアントがシードを受信: " + seed);
                if (isGameStarted)
                {
                    SpawnCards(); // カードを生成
                }
            }
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(SeedProperty))
        {
            seed = (int)propertiesThatChanged[SeedProperty];
            Debug.Log("プロパティから更新されたシード: " + seed);
            if (isGameStarted)
            {
                SpawnCards(); // カードを生成
            }
        }
    }

    private void SpawnCards()
    {
        // 既存のカードをクリア
        ResetGame();

        if (cardPrefabs.Length != 5)
        {
            Debug.LogError("プレハブ配列には正確に5つの要素が必要です。");
            return;
        }

        if (cardPos.Length != 20)
        {
            Debug.LogError("スポーンポイント配列には正確に20の要素が必要です。");
            return;
        }

        Random.InitState(seed);
        Debug.Log("使用されているシード: " + seed);

        foreach (var pos in cardPos)
        {
            bool cardSpawned = false;

            while (!cardSpawned)
            {
                int randomIndex = Random.Range(0, cardPrefabs.Length);

                if (prefabCounts[randomIndex] < maxPerPrefab)
                {
                    // 位置にある既存のカードをクリア
                    ClearPreviousCard(pos);

                    // PhotonNetwork.Instantiateを使用してネットワーク上にカードを生成
                    GameObject card;
                    if (PhotonNetwork.IsConnected)
                    {
                        card = PhotonNetwork.Instantiate(cardPrefabs[randomIndex].name, pos.position, Quaternion.identity);
                    }
                    else
                    {
                        card = Instantiate(cardPrefabs[randomIndex], pos.position, Quaternion.identity);
                    }
                    card.transform.SetParent(pos, false);

                    prefabCounts[randomIndex]++;
                    cardSpawned = true;
                }
            }
        }
    }

    private void ClearPreviousCard(RectTransform position)
    {
        foreach (Transform child in position)
        {
            if (child.gameObject.GetComponent<PhotonView>() != null)
            {
                PhotonNetwork.Destroy(child.gameObject);
            }
            else
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void ResetGame()
    {
        for (int i = 0; i < prefabCounts.Length; i++)
        {
            prefabCounts[i] = 0;
        }
    }

    private void RetrySpawnCards()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            GenerateAndSetSeed(); // シードを生成して設定
        }
        else if (!PhotonNetwork.IsConnected)
        {
            GenerateSeedForOfflineMode(); // オフラインモード用のシードを生成
        }

        SpawnCards(); // カードを生成
    }

    private void OnValidate()
    {
        if (Application.isPlaying && isGameStarted)
        {
            SpawnCards(); // カードを生成
        }
    }
}
