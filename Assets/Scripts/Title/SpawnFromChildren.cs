using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFromChildren : MonoBehaviour
{
    public List<Sprite> objectsToSpawn; // 生成するスプライトをインスペクターで指定
    public float spawnInterval = 2.0f; // 生成間隔をインスペクターで指定
    public int numberOfChildren = 5; // 子要素の数をインスペクターで指定
    public float objectsInterval; // 子要素の配置間隔
    public Vector3 firstChildPosition; // 一つ目の子要素の位置をインスペクターで指定
    public Vector3 fixedSize = new Vector3(1.0f, 1.0f, 1.0f); // 生成するオブジェクトのサイズをインスペクターで指定
    public float moveSpeed = 1.0f; // 生成するオブジェクトの移動速度と角度をインスペクターで指定
    public float moveAngle = 45.0f;
    public bool useRandomSprites = false; // ランダム化のフラグをインスペクターで指定
    public float destroyAfterSeconds = 5.0f; // 生成したオブジェクトを指定秒数後に破壊する時間

    private List<Transform> childTransforms = new List<Transform>(); // 子要素のリスト

    void Start()
    {
        SetChildTransforms(); // 子要素の位置を等間隔で設定
        StartCoroutine(SpawnObjects()); // 一定間隔でオブジェクトを生成するコルーチンを開始
    }

    void SetChildTransforms()
    {
        childTransforms.Clear(); // 子要素のリストをクリア
        Vector3 currentPosition = firstChildPosition; // 一つ目の子要素の位置を設定

        for (int i = 0; i < numberOfChildren; i++)
        {
            GameObject child = new GameObject("Child" + i);
            child.transform.position = currentPosition;
            child.transform.parent = transform;
            childTransforms.Add(child.transform);

            currentPosition = firstChildPosition + new Vector3(objectsInterval * (i + 1), 0, 0); // 次の子要素の位置を計算
        }
    }

    IEnumerator SpawnObjects()
    {
        while (true)
        {
            for (int i = 0; i < childTransforms.Count; i++)
            {
                if (objectsToSpawn.Count > 0)
                {
                    Sprite selectedSprite;

                    if (useRandomSprites)
                    {
                        int randomIndex = Random.Range(0, objectsToSpawn.Count); // ランダムなインデックスを取得
                        selectedSprite = objectsToSpawn[randomIndex];
                    }
                    else
                    {
                        selectedSprite = objectsToSpawn[i % objectsToSpawn.Count]; // 順番にスプライトを取得
                    }

                    GameObject spawnedObject = new GameObject("SpawnedObject"); // 新しいGameObjectを作成
                    SpriteRenderer renderer = spawnedObject.AddComponent<SpriteRenderer>(); // SpriteRendererコンポーネントを追加
                    renderer.sprite = selectedSprite;
                    spawnedObject.transform.position = childTransforms[i].position; // 位置を設定
                    spawnedObject.transform.localScale = fixedSize; // サイズを固定

                    DiagonalMove moveScript = spawnedObject.AddComponent<DiagonalMove>(); // 生成したオブジェクトにDiagonalMoveスクリプトをアタッチ
                    moveScript.speed = moveSpeed;
                    moveScript.angle = moveAngle;

                    Destroy(spawnedObject, destroyAfterSeconds); // 指定した秒数後にオブジェクトを破壊
                }
            }
            yield return new WaitForSeconds(spawnInterval); // 指定した間隔で待機
        }
    }
}
