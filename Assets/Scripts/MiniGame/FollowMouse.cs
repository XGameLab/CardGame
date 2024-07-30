
using UnityEngine;
using Photon.Pun;

public class FollowMouse : MonoBehaviourPunCallbacks, IPunObservable
{
    private Vector3 targetPosition;
    private ScoreManager scoreManager;

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager not found in the scene.");
        }
    }

    void Update()
    {
        if (scoreManager == null)
        {
            return;
        }

        int currentPlayer = scoreManager.CurrentPlayer; // 获取当前玩家

        // 判断对象由谁控制
        if (ShouldControlObject(currentPlayer))
        {
            FollowMouseMovement();
        }
        else
        {
            // 平滑移动到同步的位置
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10);
        }
    }

    bool ShouldControlObject(int currentPlayer)
    {
        if (scoreManager == null)
        {
            return false;
        }

        // 在离线模式下始终允许控制对象
        if (scoreManager.IsOfflineMode)
        {
            return true;
        }

        // currentPlayer == 1 时对象由主机控制
        if (currentPlayer == 1 && PhotonNetwork.IsMasterClient)
        {
            return true;
        }
        // currentPlayer == 2 时对象由客户端控制
        else if (currentPlayer == 2 && !PhotonNetwork.IsMasterClient)
        {
            return true;
        }
        return false;
    }

    void FollowMouseMovement()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane; // 设置适当的Z值

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = worldPosition;

         // 如果连接到Photon服务器并且不是离线模式，才发送RPC
        if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode)
        {
            photonView.RPC("SyncPosition", RpcTarget.Others, worldPosition);
        }
    }

    [PunRPC]
    void SyncPosition(Vector3 position)
    {
        targetPosition = position;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 发送数据到其他客户端
            stream.SendNext(transform.position);
        }
        else
        {
            // 接收数据并更新
            targetPosition = (Vector3)stream.ReceiveNext();
        }
    }
}
