using UnityEngine;
using System.Collections.Generic;

public class Player2AI : MonoBehaviour
{
    private List<GameObject> player2Cards; // Player2のカードリスト
    public BattleAnimationManager battleAnimationManager; // バトルアニメーション管理の参照

    public void InitializePlayer2Cards(List<int> remainingCardIndices, GameObject[] cardPrefabs)
    {
        player2Cards = new List<GameObject>();

        foreach (int index in remainingCardIndices)
        {
            player2Cards.Add(cardPrefabs[index]); // Player2のカードリストにカードを追加
        }
    }

    public string DecidePlayer2Action()
    {
        if (player2Cards == null || player2Cards.Count == 0)
        {
            Debug.LogWarning("Player2 has no cards to choose from."); // Player2に選べるカードがない場合の警告
            return null;
        }

        int randomIndex = Random.Range(0, player2Cards.Count); // ランダムにカードを選択
        GameObject chosenCard = player2Cards[randomIndex];
        ButtonHandler buttonHandler = chosenCard.GetComponent<ButtonHandler>(); // 選択されたカードのButtonHandlerを取得

        if (buttonHandler != null)
        {
            string action = null;
            if (buttonHandler.isATK) action = "ATK";
            else if (buttonHandler.isDEF) action = "DEF";
            else if (buttonHandler.isHeal) action = "Heal";
            else if (buttonHandler.isThrow) action = "Throw";
            else if (buttonHandler.isCNT) action = "CNT";

            if (action != null)
            {
                battleAnimationManager.PlayAnimation(action, false); // Player2のアニメーションを再生
            }
            
            return action;
        }

        return null; // 適切なアクションが見つからなかった場合
    }
}
