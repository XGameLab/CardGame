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

    public string DecidePlayer2Action()
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
            if (buttonHandler.isATK) return "ATK";
            if (buttonHandler.isDEF) return "DEF";
            if (buttonHandler.isHeal) return "Heal";
            if (buttonHandler.isThrow) return "Throw";
            if (buttonHandler.isCNT) return "CNT";
        }

        return null;
    }
}
