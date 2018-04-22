using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour {

    //移動可能範囲
    public static Vector2 m_moveLimit = new Vector2(4.7f, 3.4f);


	// Use this for initialization
	void Start () {
		
	}
	
	//指定位置を移動可能範囲に収める
    public static Vector3 ClampPosition(Vector3 position)
    {
        return new Vector3
        (
            Mathf.Clamp(position.x, -m_moveLimit.x,m_moveLimit.x), //x
            Mathf.Clamp(position.y, -m_moveLimit.y, m_moveLimit.y),  //y
            0  //z
            );
    }

    //指定された２つの位置から角度を求めて返す
    public static float GetAngle(Vector2 from,Vector2 to)
    {
        var dx = to.x - from.x;
        var dy = to.y - from.y;
        var rad = Mathf.Atan2(dy,dx); //Atanは360度網羅していない。Atan2はベクトル計算なので360度いける
        return rad * Mathf.Rad2Deg;
    }

    //指定された角度(0～360)をベクトル変換して返す
    public static Vector3 GetDirection(float angle)
    {
        return new Vector3
        (
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad),
            0
        );
    }

}
