using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {

    public Transform m_player; //プレイヤー
    public Vector2 m_limit; //背景の移動範囲

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //プレイヤーの現在位置の取得
        var pos = m_player.localPosition;

        //画面端の位置を取得
        var limit = Utils.m_moveLimit;

        //プレイヤーが画面のどの位置にいるか0～1で表現
        //Lerp関数= 0～1に値を圧縮
        var tx = 1 - Mathf.InverseLerp(-limit.x, limit.x, pos.x);
        var ty = 1 - Mathf.InverseLerp(-limit.y, limit.y, pos.y);

        //プレイヤーの現在地から背景の表示位置を算出
        var x = Mathf.Lerp(-m_limit.x, m_limit.x, tx); //-m_limit=0,m_limit=1
        var y = Mathf.Lerp(-m_limit.y, m_limit.y, ty);

        //背景の表示位置を更新
        transform.localPosition = new Vector3(x, y, 0);
	}
}
