using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour {

    public int m_exp; //獲得できる経験値
    public float m_brake = 0.9f; //散らばるときの減速量

    private Vector3 m_direction; //散らばるときの進行方向
    private float m_speed; //散らばるときの速さ

    public float m_followAccel = 0.02f; //ぷれいやーを追尾するときの加速度

    private bool m_isFollow;  //プレイヤーを追尾する⇒true

    private float m_followSpeed; //プレイヤー追尾の速さ

    public AudioClip m_goldClip;   //宝石を取得したときのSE



	// Use this for initialization
	void Start () {
      
    }

    // Update is called once per frame
    void Update () {
        //プレイヤーの現在地を取得
        var playerPos = Player.m_instance.transform.localPosition;

        //プレイヤーと宝石の距離
        var distance = Vector3.Distance(playerPos, transform.localPosition);

        //プレイヤーと宝石が接近した場合
        if (distance < Player.m_instance.m_magnetDistance)
        {
            m_isFollow = true; //追尾する
        }

        //追尾モード中にプレイヤーが生きている
        if (m_isFollow && Player.m_instance.gameObject.activeSelf)
        {
            //プレイヤーの位置へ向かうベクトル
            var direction = playerPos - transform.localPosition;
            direction.Normalize(); //ベクトルの正規化(長さを１にする)

            //宝石がプレイヤーの方へ向かう
            transform.localPosition += direction * m_followSpeed; //追尾特有の速さ

            //加速接近
            m_followSpeed += m_followAccel;
            return;
        }

        //散らばる速さの計算
        var velocity = m_direction * m_speed;
        // 散らばる移動
        transform.localPosition += velocity;

        // だんだん減速する
        m_speed *= m_brake;

        // 宝石が画面外に出ないように位置を制限する
        transform.localPosition =Utils.ClampPosition(transform.localPosition);
    }

    //宝石が出現するときに初期化する関数
    public void Init(int score,float speedMin,float speedMax)
    {
        // 宝石がどの方向に散らばるかランダムに決定する
        var angle = Random.Range(0, 360);

        //進行方向をラジアンに変換
        var f = angle * Mathf.Deg2Rad;

        //進行方向のベクトル
        m_direction = new Vector3(Mathf.Cos(f), Mathf.Sin(f), 0);

        // 宝石の散らばる速さをランダムに決定
        m_speed = Mathf.Lerp(speedMin, speedMax, Random.value);

        // 8 秒後に宝石を削除
        Destroy(gameObject, 5);

    }

    //他のオブジェクトに衝突したとき呼び出される
    private void OnTriggerEnter2D(Collider2D collision)  //引数に2Dを付け忘れないように
    {
        if (!collision.name.Contains("Player")) return;

        //宝石を削除
        Destroy(gameObject);

        //プレイヤーの経験値を増やす
        var player = collision.GetComponent<Player>();
        player.AddExp(m_exp);

        //宝石を取得したときにSEを流す
        var audioSource = FindObjectOfType<AudioSource>();
        audioSource.PlayOneShot(m_goldClip);

    }
}
