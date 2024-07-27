using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiagonalMove : MonoBehaviour
{
    // 移動速度を定義
    public float speed = 1.0f;
    // 回転角度を定義
    public float angle = 45.0f;

    // Update is called once per frame
    void Update() 
    {
        Move();
    }

    void Move()
    {
        // 角度をラジアンに変換
        float radians = angle * Mathf.Deg2Rad;
        // 毎フレーム少しずつ指定した角度で移動させる
        transform.position += new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0) * speed * Time.deltaTime;
    }
}
