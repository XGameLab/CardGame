using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierArrows : MonoBehaviour
{
    #region Public Fields
    [Tooltip("Arrow Head")]
    public GameObject ArrowHeadPrefab;

    [Tooltip("Arrow Node")]
    public GameObject ArrowNodePrefab;

    [Tooltip("Arrow Node Numbers")]
    public int arrowNodeNum;

    [Tooltip("Scale Mutiplier of Arrow Nodes")]
    public float scaleFactor = 1f;
    #endregion

    #region Private Fields
    private RectTransform origin;
    private List<RectTransform> arrowNodes = new List<RectTransform>();
    private List<Vector2> controlPoints = new List<Vector2>();
    private readonly List<Vector2> controlPointFactors = new List<Vector2> { new Vector2(-0.3f, 0.8f), new Vector2(0.1f, 1.4f) };   
    #endregion

    #region Private Methods
    private void Awake()
    {
        this.origin = this.GetComponent<RectTransform>();

        for(int i=0; i<this.arrowNodeNum; i++)
        {
            this.arrowNodes.Add(Instantiate(this.ArrowNodePrefab, this.transform).GetComponent<RectTransform>());
        }
        this.arrowNodes.Add(Instantiate(this.ArrowHeadPrefab, this.transform).GetComponent<RectTransform>());
        this.arrowNodes.ForEach(a => a.GetComponent<RectTransform>().position = new Vector2(-1000, -1000));
        for(int i=0; i<4; i++)
        {
            this.controlPoints.Add(Vector2.zero);
        }
    }

    private void Update()
    {
        if (OnDragCard.isSelected == true)
        {
            // 获取拖拽卡片的世界空间中心位置
            Vector3 objectCenterWorldSpace = OnDragCard.CardPrefab.GetComponent<RectTransform>().transform.position;
            // 将世界空间坐标转换为本地坐标
            Vector2 objectCenterLocalPosition = this.origin.InverseTransformPoint(objectCenterWorldSpace);
            this.controlPoints[0] = new Vector2(objectCenterLocalPosition.x, objectCenterLocalPosition.y + 100f);

            // 将鼠标位置从屏幕空间转换为世界空间，然后转换为本地坐标
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mouseLocalPosition = this.origin.InverseTransformPoint(mouseWorldPosition);
            this.controlPoints[3] = new Vector2(mouseLocalPosition.x, mouseLocalPosition.y);

            this.controlPoints[1] = this.controlPoints[0] + (this.controlPoints[3] - this.controlPoints[0]) * this.controlPointFactors[0];
            this.controlPoints[2] = this.controlPoints[0] + (this.controlPoints[3] - this.controlPoints[0]) * this.controlPointFactors[1];

            for (int i = 0; i < this.arrowNodes.Count; i++)
            {
                var t = Mathf.Log(1f * i / (this.arrowNodes.Count - 1) + 1f, 2f);

                Vector2 bezierPosition = 
                    Mathf.Pow(1 - t, 3) * this.controlPoints[0] + 
                    3 * Mathf.Pow(1 - t, 2) * t * this.controlPoints[1] +
                    3 * (1 - t) * Mathf.Pow(t, 2) * this.controlPoints[2] + 
                    Mathf.Pow(t, 3) * this.controlPoints[3];

                // 将贝塞尔曲线的位置设置为本地坐标
                this.arrowNodes[i].localPosition = new Vector3(bezierPosition.x, bezierPosition.y, 0f);

                if (i > 0)
                {
                    var euler = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, this.arrowNodes[i].localPosition - this.arrowNodes[i - 1].localPosition));
                    this.arrowNodes[i].localRotation = Quaternion.Euler(euler);
                }

                var scale = this.scaleFactor * (1f - 0.03f * (this.arrowNodes.Count - 1 - i));
                this.arrowNodes[i].localScale = new Vector3(scale, scale, 1f);
            }
            this.arrowNodes[0].localRotation = this.arrowNodes[1].localRotation;
        }
        else
        {
            this.arrowNodes.ForEach(a => a.GetComponent<RectTransform>().localPosition = new Vector2(-1000, -1000));
        }
    }

    #endregion
}

// 这段C#代码定义了一个名为`BezierArrows`的脚本，用于创建贝塞尔曲线箭头。
// 它的实现原理是将鼠标拖动的位置作为控制点，计算贝塞尔曲线的各个节点的坐标，然后将箭头按照贝塞尔曲线绘制。
// 脚本中还包含了一些工具函数，如`Awake`和`Update`，用于初始化组件和更新箭头的位置。

// 以下是一些注意事项：

// 1. 代码中使用了`UnityEngine.Input`，这意味着脚本需要在游戏窗口中运行。
//    如果需要在非游戏环境中运行，需要移除对`Input.mousePosition`的依赖。

// 2. 代码中的`Update`方法中有一个`if`条件判断`OnDragCard.isSelected == true`，
//    这意味着脚本只在`OnDragCard`事件被选中时生效。
//    要使脚本在所有情况下都工作，可以移除这个条件判断，或者在`Update`方法中添加对`OnDragCard`事件的检查。

// 3. 代码中的`controlPointFactors`列表用于控制贝塞尔曲线中的控制点位置。
//    可以通过调整这个列表中的值来改变箭头在不同位置的弯曲程度。

// 4. 代码中的`scaleFactor`用于控制箭头的大小。可以通过调整这个值来改变箭头的大小。

// 5. 代码中的`arrowNodeNum`用于控制箭头的数量。可以通过调整这个值来创建不同数量的箭头。

// 6. 箭头在初始化时会移动到屏幕之外，可以通过修改`arrowNodes[i].position`来调整箭头的位置。

// 7. 脚本使用了`GetComponent<RectTransform>()`来获取`RectTransform`组件，
//    这是一个常见的Unity工具，用于获取游戏对象上的矩形transform。

// 8. 脚本使用了`Instantiate(this.ArrowNodePrefab)`来创建箭头节点，
//    这是一种常见的Unity工具，用于根据当前对象实例化一个新的游戏对象。

// 9. 脚本使用了`Vector2.SignedAngle`来计算旋转角度，这是一个常见的Unity工具，用于计算两个向量的夹角。

// 10. 脚本使用了`Quaternion.Euler`来计算旋转角度，这是一个常见的Unity工具，用于将欧拉角转换为四元数。

// 总之，这段代码实现了一个简单的贝塞尔曲线箭头效果，可以通过修改参数来改变箭头的外观和行为。

