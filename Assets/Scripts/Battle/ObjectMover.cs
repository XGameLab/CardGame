using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectMover : MonoBehaviour
{
    public GameObject[] objectsToMove; // 移動させるオブジェクトの配列（エディタで設定）
    public Transform[] targetPositions; // ターゲット位置のTransform配列（エディタで設定）
    public GameObject[] startObjectsToMove; // ゲーム開始時に移動させるオブジェクトの配列（エディタで設定）
    public Transform[] startObjPositions; // ゲーム開始時のターゲット位置のTransform配列（エディタで設定）
    public GameObject[] controllableObjects; // 表示/非表示を制御するオブジェクトの配列（エディタで設定）
    public float moveDuration = 1f; // 移動時間
    public float startMoveDuration = 1f; // ゲーム開始時の移動時間
    public float stayDuration = 2f; // 2秒間停留
    public float waitSec = 3f; // 待機時間
    public BattleInfoManager battleInfoManager; // BattleInfoManager スクリプトの参照
    public PlayerActionHandler playerActionHandler; // PlayerActionHandler スクリプトの参照

    public GameObject hintObj; // ヒントオブジェクト
    public Transform hintPosition; // ヒントオブジェクトのターゲット位置
    public Button submitButton; // 提出ボタン
    private Vector3 hintObjInitialPosition; // ヒントオブジェクトの初期位置

    private Vector3[] initialPositions; // 初期位置を保存するための配列
    private Vector3[] startObjInitialPositions; // startObjectsToMoveの初期位置を保存する配列
    private bool isAtInitialPositions = true; // オブジェクトが初期位置にいるかどうかを示すフラグ
    private bool isAtTargetPositions = false; // オブジェクトがターゲット位置にいるかどうかを示すフラグ
    private Coroutine hintCoroutine; // ヒントのコルーチン
    private bool hintDismissed = false; // ヒントが非表示になったかどうかを示すフラグ

    private void Start()
    {
        if (objectsToMove.Length != targetPositions.Length)
        {
            Debug.LogError("Objects to move and target positions count do not match!");
            return;
        }

        // objectsToMoveの初期位置を保存
        initialPositions = new Vector3[objectsToMove.Length];
        for (int i = 0; i < objectsToMove.Length; i++)
        {
            if (objectsToMove[i] != null)
            {
                initialPositions[i] = objectsToMove[i].transform.position;
            }
        }

        // startObjectsToMoveの初期位置を保存
        startObjInitialPositions = new Vector3[startObjectsToMove.Length];
        for (int i = 0; i < startObjectsToMove.Length; i++)
        {
            if (startObjectsToMove[i] != null)
            {
                startObjInitialPositions[i] = startObjectsToMove[i].transform.position;
            }
        }

        // ヒントオブジェクトの初期位置を保存
        if (hintObj != null)
        {
            hintObjInitialPosition = hintObj.transform.position;
        }

        // 提出ボタンのクリックイベントリスナーを追加
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitButtonClicked);
        }

        // 開始時にオブジェクトをターゲット位置に移動してから戻る
        MoveStartObjects();
    }

    private void Update()
    {
        // Rキーが押されたときにオブジェクトを初期位置に移動
        if (Input.GetKey(KeyCode.R) && !isAtInitialPositions)
        {
            MoveObjectsToInitialPositions();
        }

        if (battleInfoManager != null)
        {
            // ゲームオーバー時にオブジェクトをターゲット位置に移動
            if (battleInfoManager.isGameOver && !isAtTargetPositions)
            {
                MoveObjectsToTargetPositions();
            }

            // ゲームオーバー時にヒントのタイマーを停止
            if (battleInfoManager.isGameOver)
            {
                if (hintCoroutine != null)
                {
                    StopCoroutine(hintCoroutine);
                    hintCoroutine = null;
                }
            }
            else
            {
                // ゲームが終了していない場合にヒントのタイマーを再開
                if (hintCoroutine == null && !hintDismissed)
                {
                    hintCoroutine = StartCoroutine(WaitForDKeyOrMoveHint());
                }
            }
        }

        // Enterキーが押されたときにヒントの動きをリセット
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (hintCoroutine != null)
            {
                StopCoroutine(hintCoroutine);
                hintCoroutine = null;
            }

            hintDismissed = false; // ヒントが非表示になった状態をリセット

            // 新しいヒントの動きタイマーを開始
            hintCoroutine = StartCoroutine(WaitForDKeyOrMoveHint());
        }

        ControlObjectVisibility(); // オブジェクトの表示/非表示を制御
    }

    private void MoveObjectsToTargetPositions()
    {
        for (int i = 0; i < objectsToMove.Length; i++)
        {
            if (objectsToMove[i] != null && targetPositions[i] != null)
            {
                LeanTween.move(objectsToMove[i], targetPositions[i].position, moveDuration).setEase(LeanTweenType.easeInOutQuad);
            }
        }
        isAtInitialPositions = false;
        isAtTargetPositions = true;
    }

    private void MoveObjectsToInitialPositions()
    {
        for (int i = 0; i < objectsToMove.Length; i++)
        {
            if (objectsToMove[i] != null)
            {
                LeanTween.move(objectsToMove[i], initialPositions[i], moveDuration).setEase(LeanTweenType.easeInOutQuad);
            }
        }
        isAtInitialPositions = true;
        isAtTargetPositions = false;
    }

    private void MoveStartObjects()
    {
        for (int i = 0; i < startObjectsToMove.Length; i++)
        {
            if (startObjectsToMove[i] != null && startObjPositions[i] != null)
            {
                int index = i;
                Vector3 initialPosition = startObjInitialPositions[index];
                Vector3 targetPosition = startObjPositions[index].position;

                LeanTween.move(startObjectsToMove[index], targetPosition, startMoveDuration)
                    .setEase(LeanTweenType.easeInOutQuad)
                    .setOnComplete(() =>
                    {
                        LeanTween.delayedCall(startObjectsToMove[index], stayDuration, () =>
                        {
                            LeanTween.move(startObjectsToMove[index], initialPosition, startMoveDuration).setEase(LeanTweenType.easeInOutQuad);
                        });
                    });
            }
        }
    }

    private void ControlObjectVisibility()
    {
        if (playerActionHandler != null)
        {
            bool shouldShow = playerActionHandler.isPlayer1Win; // プレイヤー1が勝利したかどうかを確認
            foreach (var obj in controllableObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(shouldShow); // オブジェクトの表示/非表示を設定
                }
            }
        }
    }

    private void OnSubmitButtonClicked()
    {
        // ヒントのタイマーを停止してリセット
        if (hintCoroutine != null)
        {
            StopCoroutine(hintCoroutine);
            hintCoroutine = null;
        }

        hintDismissed = false; // ヒントが非表示になった状態をリセット

        // 新しいヒントの動きタイマーを開始
        hintCoroutine = StartCoroutine(WaitForDKeyOrMoveHint());
    }

    private IEnumerator WaitForDKeyOrMoveHint()
    {
        float timer = 0f;
        bool dKeyPressed = false;

        // 7秒間待つかDキーが押されるのを待つ
        while (timer < waitSec)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                dKeyPressed = true;
                break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        if (dKeyPressed)
        {
            // Dキーが押された場合、ヒントを初期位置に戻し、タイマーを停止
            LeanTween.move(hintObj, hintObjInitialPosition, moveDuration).setEase(LeanTweenType.easeInOutQuad);
            hintDismissed = true; // ヒントが非表示になったことを示す
        }
        else if (!hintDismissed)
        {
            // Dキーが押されなかった場合、ヒントをターゲット位置に移動
            LeanTween.move(hintObj, hintPosition.position, moveDuration).setEase(LeanTweenType.easeInOutQuad);
        }

        hintCoroutine = null; // コルーチン変数をリセット
    }
}
