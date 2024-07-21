using UnityEngine;

public class PlayerActionHandler : MonoBehaviour
{
    public void HandlePlayerActions(string player1Action, string player2Action)
    {
        Debug.Log("Player1 chose action: " + player1Action);
        Debug.Log("Player2 chose action: " + player2Action);

        // 根据玩家1和玩家2的行动显示不同的Debug.Log信息
        if (player1Action == "ATK" && player2Action == "DEF")
        {
            Debug.Log("Player1's attack was blocked by Player2's defense.");
        }
        else if (player1Action == "ATK" && player2Action == "Heal")
        {
            Debug.Log("Player1's attack interrupted Player2's heal.");
        }
        else if (player1Action == "ATK" && player2Action == "CNT")
        {
            Debug.Log("Player1's attack was countered by Player2.");
        }
        // 添加其他情况的处理逻辑
        else
        {
            Debug.Log("Unhandled combination: Player1 (" + player1Action + "), Player2 (" + player2Action + ")");
        }
    }
}
