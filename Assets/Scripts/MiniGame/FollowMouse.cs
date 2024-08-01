using UnityEngine;
using Photon.Pun;

public class FollowMouse : MonoBehaviourPunCallbacks, IPunObservable
{
    private Vector3 targetPosition; // 同期する位置
    private ScoreManager scoreManager; // スコアマネージャー

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>(); // シーンから ScoreManager を探す
        if (scoreManager == null)
        {
            Debug.LogError("シーン内に ScoreManager が見つかりません。");
        }
    }

    void Update()
    {
        if (scoreManager == null)
        {
            return;
        }

        int currentPlayer = scoreManager.CurrentPlayer; // 現在のプレイヤーを取得

        // オブジェクトの制御権を持っているかを判断
        if (ShouldControlObject(currentPlayer))
        {
            FollowMouseMovement(); // マウスの動きに追従
        }
        else
        {
            // 同期された位置にスムーズに移動
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10);
        }
    }

    bool ShouldControlObject(int currentPlayer)
    {
        if (scoreManager == null)
        {
            return false;
        }

        // オフラインモードでは常にオブジェクトを制御可能
        if (scoreManager.IsOfflineMode)
        {
            return true;
        }

        // currentPlayer == 1 でオブジェクトはマスタークライアントによって制御
        if (currentPlayer == 1 && PhotonNetwork.IsMasterClient)
        {
            return true;
        }
        // currentPlayer == 2 でオブジェクトはクライアントによって制御
        else if (currentPlayer == 2 && !PhotonNetwork.IsMasterClient)
        {
            return true;
        }
        return false;
    }

    void FollowMouseMovement()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane; // 適切なZ値を設定

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // オフセットを追加
        Vector3 offset = new Vector3(0.2f, -0.5f, 0f);
        worldPosition += offset;
        
        transform.position = worldPosition;

        // Photonサーバーに接続されており、オフラインモードでない場合にのみRPCを送信
        if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode)
        {
            photonView.RPC("SyncPosition", RpcTarget.Others, worldPosition); // 位置を同期
        }
    }

    [PunRPC]
    void SyncPosition(Vector3 position)
    {
        targetPosition = position; // 他のクライアントから受信した位置を設定
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 他のクライアントにデータを送信
            stream.SendNext(transform.position);
        }
        else
        {
            // データを受信して更新
            targetPosition = (Vector3)stream.ReceiveNext();
        }
    }
}
