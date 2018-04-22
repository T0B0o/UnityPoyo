using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour {

    private Vector3 m_velocity; //速度

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //移動する
        transform.localPosition += m_velocity;
	}

    //為を発射するときの初期化関数
    public void Init(float angle, float speed)
    {
        //弾の発射角度をベクトルに変換
        var direction = Utils.GetDirection(angle);

        //発射角度と速さから速度を求める
        m_velocity = direction * speed;

        //弾が進行方向を向くようにする
        var angles = transform.localEulerAngles;
        angles.z = angle - 90;
        transform.localEulerAngles = angles;

        //2秒後に削除
        Destroy(gameObject, 2);

    }
}
