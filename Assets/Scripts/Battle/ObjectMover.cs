using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectMover : MonoBehaviour
{
    public GameObject[] objectsToMove; // 在编辑器中拖入需要移动的物体
    public Transform[] targetPositions; // 在编辑器中拖入目标位置的Transform
    public GameObject[] startObjectsToMove; // 在编辑器中拖入游戏开始时移动的物体
    public Transform[] startObjPositions; // 在编辑器中拖入游戏开始时的目标位置的Transform
    public GameObject[] controllableObjects; // 在编辑器中拖入需要控制显示/隐藏的物体
    public float moveDuration = 1f; // 移动时长
    public float startMoveDuration = 1f; // 游戏开始时的移动时长
    public float stayDuration = 2f; // 停留2秒
    public BattleInfoManager battleInfoManager;
    public PlayerActionHandler playerActionHandler; // 引用PlayerActionHandler脚本

    public GameObject hintObj; // 提示物体
    public Transform hintPosition; // 提示物体目标位置
    public Button submitButton; // 提交按钮
    private Vector3 hintObjInitialPosition; // 提示物体初始位置

    private Vector3[] initialPositions; // 用于存储初始位置
    private Vector3[] startObjInitialPositions; // 存储startObjectsToMove初始位置
    private bool isAtInitialPositions = true; // 标记物体是否在初始位置
    private bool isAtTargetPositions = false; // 标记物体是否在目标位置
    private Coroutine hintCoroutine;

    private void Start()
    {
        if (objectsToMove.Length != targetPositions.Length)
        {
            Debug.LogError("Objects to move and target positions count do not match!");
            return;
        }

        // 存储objectsToMove的初始位置
        initialPositions = new Vector3[objectsToMove.Length];
        for (int i = 0; i < objectsToMove.Length; i++)
        {
            if (objectsToMove[i] != null)
            {
                initialPositions[i] = objectsToMove[i].transform.position;
            }
        }

        // 存储startObjectsToMove的初始位置
        startObjInitialPositions = new Vector3[startObjectsToMove.Length];
        for (int i = 0; i < startObjectsToMove.Length; i++)
        {
            if (startObjectsToMove[i] != null)
            {
                startObjInitialPositions[i] = startObjectsToMove[i].transform.position;
            }
        }

        // 存储提示物体的初始位置
        if (hintObj != null)
        {
            hintObjInitialPosition = hintObj.transform.position;
        }

        // 为提交按钮添加点击事件监听器
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitButtonClicked);
        }

        // 开始时移动物体到目标位置然后返回
        MoveStartObjects();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R) && !isAtInitialPositions)
        {
            MoveObjectsToInitialPositions();
        }

        if (battleInfoManager != null)
        {
            if (battleInfoManager.isGameOver && !isAtTargetPositions)
            {
                MoveObjectsToTargetPositions();
            }

            // 新增：当游戏结束时停止计时并清零
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
                // 新增：游戏未结束时恢复计时
                if (hintCoroutine == null)
                {
                    hintCoroutine = StartCoroutine(WaitForDKeyOrMoveHint());
                }
            }
        }

        ControlObjectVisibility();
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
            bool shouldShow = playerActionHandler.isPlayer1Win;
            foreach (var obj in controllableObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(shouldShow);
                }
            }
        }
    }

    private void OnSubmitButtonClicked()
    {
        if (hintCoroutine != null)
        {
            StopCoroutine(hintCoroutine);
        }
        hintCoroutine = StartCoroutine(WaitForDKeyOrMoveHint());
    }

    private IEnumerator WaitForDKeyOrMoveHint()
    {
        float timer = 0f;
        bool dKeyPressed = false;

        while (timer < 7f)
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
            // 如果按下了D键，移动回初始位置
            LeanTween.move(hintObj, hintObjInitialPosition, moveDuration).setEase(LeanTweenType.easeInOutQuad);
        }
        else
        {
            // 如果没有按下D键，移动到目标位置但不返回
            LeanTween.move(hintObj, hintPosition.position, moveDuration).setEase(LeanTweenType.easeInOutQuad);
        }

        hintCoroutine = null; // 重置协程变量
    }
}
