using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [System.Serializable]
    public struct ButtonScenePair
    {
        public Button button;      // シーンをロードするボタン
        public string sceneName;   // 手动指定场景名称 // ロードするシーン名
    }

    [SerializeField] private ButtonScenePair[] buttonScenePairs; // ボタンとシーン名のペア

    private void Awake()
    {
        // 各ボタンにシーンロードのイベントを登録
        foreach (var pair in buttonScenePairs)
        {
            if (pair.button != null && !string.IsNullOrEmpty(pair.sceneName))
            {
                // ボタンがクリックされたときに対応するシーンをロード
                pair.button.onClick.AddListener(() => OnButtonClick(pair.sceneName));
            }
        }
    }

    // ボタンがクリックされたときに呼び出される
    private void OnButtonClick(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName); // 指定されたシーンをロード
        }
    }
}
