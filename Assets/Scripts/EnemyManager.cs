using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public Enemy[] m_enemyPrefabs;

    public float m_intervalFrom; // 出現間隔（秒）（ゲームの経過時間が 0 の時）
    public float m_intervalTo; // 出現間隔（秒）（ゲームの経過時間が m_elapsedTimeMax の時）
    public float m_elapsedTimeMax; // 経過時間の最大値
    public float m_elapsedTime; // 経過時間

    private float m_timer; //出現タイミングの管理

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
        // 経過時間を更新
        m_elapsedTime += Time.deltaTime;

        // 出現タイミングを管理するタイマーを更新
        m_timer += Time.deltaTime;

        // ゲームの経過時間から出現間隔（秒）を算出
        // ゲームの経過時間が長くなるほど、敵の出現間隔が短くなる
        var t = m_elapsedTime / m_elapsedTimeMax;
        var interval = Mathf.Lerp(m_intervalFrom, m_intervalTo, t);

        //まだ敵が出現するタイミングでない場合処理を中断
        if (m_timer < interval) return;

        //出現タイミングのタイマーをリセット
        m_timer = 0;

        //出現する敵をランダムに決定
        var enemyIndex = Random.Range(0, m_enemyPrefabs.Length);

        //出現する敵のプレハブを配列から取得
        var enemyPrefab = m_enemyPrefabs[enemyIndex];

        //敵のゲームオブジェクトを生成
        var enemy = Instantiate(enemyPrefab);

        // 敵を画面外のどの位置に出現させるかランダムに決定する
        var respawnType = 
            (RESPAWN_TYPE)Random.Range(0, (int)RESPAWN_TYPE.SIZEOF);

        // 敵を初期化する
        enemy.Init(respawnType);
    }
}
