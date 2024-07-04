using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [System.Serializable]
    public struct ButtonScenePair
    {
        public Button button;
        public string sceneName; // 手动指定场景名称
    }

    [SerializeField] private ButtonScenePair[] buttonScenePairs;

    private void Awake()
    {
        foreach (var pair in buttonScenePairs)
        {
            if (pair.button != null && !string.IsNullOrEmpty(pair.sceneName))
            {
                pair.button.onClick.AddListener(() => OnButtonClick(pair.sceneName));
            }
        }
    }

    private void OnButtonClick(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
