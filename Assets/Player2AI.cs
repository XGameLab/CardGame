using UnityEngine;
using System.Collections.Generic;

public class Player2AI : MonoBehaviour
{
    private List<GameObject> player2Cards; // Player2的卡牌列表

    public void InitializePlayer2Cards(List<int> remainingCardIndices, GameObject[] cardPrefabs)
    {
        player2Cards = new List<GameObject>();

        foreach (int index in remainingCardIndices)
        {
            player2Cards.Add(cardPrefabs[index]);
        }
    }

    public string Player2Action()
    {
        if (player2Cards == null || player2Cards.Count == 0)
        {
            Debug.LogWarning("Player2 has no cards to choose from.");
            return null;
        }

        int randomIndex = Random.Range(0, player2Cards.Count);
        GameObject chosenCard = player2Cards[randomIndex];
        ButtonHandler buttonHandler = chosenCard.GetComponent<ButtonHandler>();

        if (buttonHandler != null)
        {
            string action = "";

            if (buttonHandler.isATK)
            {
                action = "ATK";
            }
            else if (buttonHandler.isDEF)
            {
                action = "DEF";
            }
            else if (buttonHandler.isHeal)
            {
                action = "Heal";
            }
            else if (buttonHandler.isThrow)
            {
                action = "Throw";
            }
            else if (buttonHandler.isCNT)
            {
                action = "CNT";
            }
            return action; // 返回Player2的行动
        }

        return null;
    }
}
