using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public　enum RESPAWN_TYPE
{
    UP,
    RIGHT,
    DAWN,
    LEFT,
    SIZEOF, //敵の出現位置の数 (なんだこの "," ??)
}

public class Enemy : MonoBehaviour {

    //敵の出現位置（内側）
    public Vector2 m_respawnPosInside;

    //敵の出現位置(外側)
    public Vector2 m_respawnPosOutside;

    //移動速度
    public float m_speed;
    //HPの最大値
    public int m_hpMax;
    //討伐時の経験値
    public int m_exp;
    //この敵がプレイヤーに与えるダメージ
    public int m_damage;
    //爆発エフェクトのプレハブ
    public Explosion m_explosionPrefab;
    //プレイヤーを追尾する場合 true
    public bool m_isFollow;
    //プレーイヤーを追尾する距離
    public float m_followDistance;

    private int m_hp;
    private Vector3 m_direction;

    public Gem[] m_gemPrefabs; //宝石のプレハブを管理する配列
    public float m_gemSpeedMin; //宝石の移動速度最小値
    public float m_gemSpeedMax; //宝石の移動速度最大値

    public AudioClip m_deathClip; //敵を倒したときのSE

    // Use this for initialization
    void Start () {
        //hpを初期化
        m_hp = m_hpMax;
	}
	
	// Update is called once per frame
	void Update () {
        if (m_isFollow)
        {
            var distance = Vector3.Distance(transform.localPosition, Player.m_instance.transform.localPosition);

            if(Player.m_instance.gameObject.activeSelf&&distance<m_followDistance) {
                //プレイヤーの現在地へ向かうベクトルを作成
                var angle = Utils.GetAngle(
                    transform.localPosition,
                    Player.m_instance.transform.localPosition);
                var direction = Utils.GetDirection(angle);

                //プレイヤー方向に移動
                transform.localPosition += direction * m_speed;

                //プレイヤーが存在する方向を向く
                var angles = transform.localEulerAngles;
                angles.z = angle - 90; //是の意味はいまだ分からん
                transform.localEulerAngles = angles;
                return;
            }
        }
        //まっすぐ移動する
        transform.localPosition += m_direction * m_speed;
	}


    //敵が出現する場所を初期化する関数
    public void Init(RESPAWN_TYPE respawnType)
    {
        //原点
        var pos = Vector3.zero;

        //指定された出現位置の種類に応じて出現位置と進行方向を決定
        switch (respawnType)
        {
            case RESPAWN_TYPE.UP:
                pos.x = Random.Range(
                    -m_respawnPosInside.x, m_respawnPosInside.x);
                pos.y = m_respawnPosOutside.y;
                m_direction = Vector2.down;
                break;
            case RESPAWN_TYPE.RIGHT:
                pos.x = m_respawnPosOutside.x;
                pos.y = Random.Range(
                    -m_respawnPosInside.y, m_respawnPosInside.y);
                m_direction = Vector2.left;
                break;
            case RESPAWN_TYPE.DAWN:
                pos.x = Random.Range(
                   -m_respawnPosInside.x, m_respawnPosInside.x);
                pos.y = -m_respawnPosOutside.y;
                m_direction = Vector2.up;
                break;
            case RESPAWN_TYPE.LEFT:
                pos.x = -m_respawnPosOutside.x;
                pos.y = Random.Range(
                    -m_respawnPosInside.y, m_respawnPosInside.y);
                m_direction = Vector2.left;
                break;
        }
        //位置を反映する
        transform.localPosition = pos;
    }

    //他のオブジェクトと衝突したときに呼び出される関数
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //弾と衝突した場合
        if (collision.name.Contains("Shot"))
        {
            //弾が当たったばっしょに爆発エフェクトを生成
            Instantiate(
                m_explosionPrefab,
                collision.transform.localPosition,
                Quaternion.identity //identity=単位回転(=回転していない)
                );

            //弾を削除
            Destroy(collision.gameObject);

            //敵のhpを減らす
            m_hp--;

            //敵のHPがまだ残っている場合処理を中断
            if (0 < m_hp) return;

            //敵の削除
            Destroy(gameObject);


            /*
             * 敵を倒したときに宝石を散らばらせる
             * 
             * 敵を倒したときに得られる経験値と和が一致するような
             * 宝石の組み合わせを散らばらせる。
             * 
             */
            var exp = m_exp;

            while (0 < exp)
            {
                // 生成可能な宝石を配列で取得
                var gemPrefabs = m_gemPrefabs.Where(c => c.m_exp <= exp).ToArray();

                // 生成可能な宝石の配列から、生成する宝石をランダムに決定
                var gemPrefab = gemPrefabs[Random.Range(0, gemPrefabs.Length)];

                // 敵の位置に宝石を生成
                var gem = Instantiate(
                    gemPrefab, transform.localPosition, Quaternion.identity);

                // 宝石を初期化
                gem.Init(m_exp, m_gemSpeedMin, m_gemSpeedMax);

                // まだ宝石を生成できるか計算
                exp -= gem.m_exp;
            }

            //敵を倒したときにSEを流す
            var audioSource = FindObjectOfType<AudioSource>();
            audioSource.PlayOneShot(m_deathClip);

        }

        //プレイヤーと衝突した場合
        if (collision.name.Contains("Player"))
        {
            //プレイヤーにダメージ
            var player = collision.GetComponent<Player>();
            player.Damage(m_damage);

            //隕石大爆発
            if (this.name.Contains("Enemy5"))
            {
                //爆発エフェクトを生成
                Instantiate(
                    m_explosionPrefab,
                    collision.transform.localPosition,
                    Quaternion.identity //identity=単位回転(=回転していない)
                    );

                //敵を倒したときにSEを流す
                var audioSource = FindObjectOfType<AudioSource>();
                audioSource.PlayOneShot(m_deathClip);
                
                //敵を消去
                Destroy(gameObject);
            }

            return;
        }
    }

}
