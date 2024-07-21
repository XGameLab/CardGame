using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class BattleInfoManager : MonoBehaviour
{
    public TMP_Text battleInfoText;
    public Button submitButton;
    public PlayerActionHandler playerActionHandler; // 引用PlayerActionHandler脚本

    public string atkText;
    public string defText;
    public string healText;
    public string throwText;
    public string cntText;
    public string submitText; // 添加提交按钮显示的文本

    private string player1Action;

    void OnEnable()
    {
        ButtonHandler.OnButtonPressed += UpdateBattleInfo;
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitButtonClicked);
        }
    }

    void OnDisable()
    {
        ButtonHandler.OnButtonPressed -= UpdateBattleInfo;
        if (submitButton != null)
        {
            submitButton.onClick.RemoveListener(OnSubmitButtonClicked);
        }
    }

    void UpdateBattleInfo(string buttonType)
    {
        switch (buttonType)
        {
            case "ATK":
                battleInfoText.text = atkText;
                submitText = "攻撃";
                player1Action = "ATK";
                break;
            case "DEF":
                battleInfoText.text = defText;
                submitText = "防御";
                player1Action = "DEF";
                break;
            case "Heal":
                battleInfoText.text = healText;
                submitText = "回復";
                player1Action = "Heal";
                break;
            case "Throw":
                battleInfoText.text = throwText;
                submitText = "投げ技";
                player1Action = "Throw";
                break;
            case "CNT":
                battleInfoText.text = cntText;
                submitText = "反撃";
                player1Action = "CNT";
                break;
        }
    }

    void OnSubmitButtonClicked()
    {
        battleInfoText.text = "Wataは\n" + submitText + "\nを使った"; // 更新提交按钮点击时的文本
        StartCoroutine(ClearBattleInfoTextAfterDelay(2f)); // 启动协程，在2秒后清除文本

        // 调用Player2的行动
        string player2Action = FindObjectOfType<Player2AI>().Player2Action();

        // 调用PlayerActionHandler来处理并显示玩家行动
        if (playerActionHandler != null)
        {
            playerActionHandler.HandlePlayerActions(player1Action, player2Action);
        }
        else
        {
            Debug.LogError("PlayerActionHandler is not assigned.");
        }
    }

    IEnumerator ClearBattleInfoTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        battleInfoText.text = "キーボード\n「D」ドロー";
    }
}
