using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float m_speed; //移動速度
    public Shot m_shotPrefab; //弾のプレハブ
    public float m_shotAngleRange; //複数の弾を発射するとき
    public float m_shotSpeed; //弾の移動速度
    public float m_shotTimer; //弾の発射タイミングを管理
    public int m_shotCount; //弾の発射数
    public float m_shotInterval; //弾の発射間隔(秒)
    public int m_hpMax; //hpの最大値
    public int m_hp; //現hp
    public static Player m_instance; //プレイヤーのインスタンスを管理
    public float m_magnetDistance; //宝石を引き付ける距離

    public int m_nextExpBase;  //次のレベルに必要な経験値の基本値
    public int m_nextExpInterval; //次のレベルに必要な経験値の増分
    public int m_level; //レベル
    public int m_exp; //トータル経験値
    public int m_prevNeedExp; //前のレベルで必要だった経験値
    public int m_needExp; //次のレベルに必要な経験値

    public AudioClip m_levelUpClip; //レベルアップするときのSE
    public AudioClip m_damageClip;  //ダメージを受けたときのSE

    public int m_levelMax; // レベルの最大値
    public int m_shotCountFrom; // 弾の発射数（レベルが最小値の時）
    public int m_shotCountTo; // 弾の発射数（レベルが最大値の時）
    public float m_shotIntervalFrom; // 弾の発射間隔（秒）（レベルが最小値の時）
    public float m_shotIntervalTo; // 弾の発射間隔（秒）（レベルが最大値の時）
    public float m_magnetDistanceFrom; // 宝石を引きつける距離（レベルが最小値の時）
    public float m_magnetDistanceTo; // 宝石を引きつける距離（レベルが最大値の時）



    // Use this for initialization
    void Start()
    {

    }

    private void Awake()
    {
        //他のクラスからプレイヤーを参照できるように
        //static変数にインスタンス情報を格納
        m_instance = this; 

        m_hp = m_hpMax;

        m_level = 1;
        m_needExp = GetNeedExp(1); //最初のレベルアップ（レベル２）に必要な経験値（1はレベルの１。）

        m_shotCount = m_shotCountFrom; // 弾の発射数
        m_shotInterval = m_shotIntervalFrom; // 弾の発射間隔（秒）
        m_magnetDistance = m_magnetDistanceFrom; // 宝石を引きつける距離

    }

    // Update is called once per frame
    void Update()
    {
        //矢印キーの入力情報を取得
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        //矢印キーが押されている方向にプレイヤーを移動
        var velocity = new Vector3(h, v) * m_speed;
        transform.localPosition += velocity;  //なんかVector2だとここが使えない

        //プレイヤーが画面外に出ないように制御
        transform.localPosition = Utils.ClampPosition(transform.localPosition);

        //プレイヤーの座標を計算
        var screenPos = Camera.main.WorldToScreenPoint(transform.position);

        //プレイヤーから見たマウスカーソルの方向を計算する
        var direction = Input.mousePosition - screenPos;

        //マウスカーソルが存在する方向の角度計算
        var angle = Utils.GetAngle(Vector3.zero, direction); //原点から向いたマウスカーソル座標の角度

        //プレイヤーがマウスカーソルの方向を見るように設定
        var angles = transform.localEulerAngles;
        angles.z = angle - 90; //???
        transform.localEulerAngles = angles;

        //弾の発射タイミングを管理するタイマーを更新する
        m_shotTimer += Time.deltaTime;

        //まだ発射タイミングでない場合処理を中断
        if (m_shotTimer < m_shotInterval) return;

        //弾の発射タイミングを管理するタイマーのリセット
        m_shotTimer = 0;

        //弾を発射
        ShootNWay(angle, m_shotAngleRange, m_shotSpeed, m_shotCount);

    }

    //弾を発射する関数
    private void ShootNWay(
        float angleBase, float angleRange, float speed, int count)
    {
        var pos = transform.localPosition; //プレイヤーの位置
        var rot = transform.localRotation; //プレイヤーの向き

        //弾を複数発射する場合
        if (1 < count)
        {
            //発射回数分ループ
            for (int i = 0; i < count; ++i)
            {
                //弾の発射角度を計算
                var angle = angleBase + angleRange * ((float)i / (count - 1) - 0.5f);

                //発射する弾の生成
                var shot = Instantiate(m_shotPrefab, pos, rot);

                //弾の発射方向と速さの設定
                shot.Init(angle, speed);
            }
        }
        //弾を一つだけ発射する場合
        else if (count == 1)
        {
            //発射する弾を生成
            var shot = Instantiate(m_shotPrefab, pos, rot);

            //弾の発射方向と速さの設定
            shot.Init(angleBase, speed);
        }
    }

    //ダメージを受ける関数
    //敵と衝突時に呼び出される
    public void Damage(int damage)
    {
        //Hpを減らす
        m_hp -= damage;

        //hpがまだあればここで処理を終了
        if (0 < m_hp) return;

        //プレイヤーがヤラレチャタ...(非表示)
        //なぜDestroyを使わないのだろう
        //→Transformが破壊後も参照しようとするから。
        //(おそらく、Backgroundが参照している?)
        gameObject.SetActive(false);

        //ダメージを受けたときにSEを流す
        var audioSource = FindObjectOfType<AudioSource>();
        audioSource.PlayOneShot(m_damageClip);
    }

    public void AddExp(int exp)
    {
        //経験値を増やす
        m_exp += exp;

        //まだレベルアップに必要な経験値に足りてなければ中断
        if (m_exp < m_needExp) return;

        //レベルアップする
        m_level++;

        //今回のレベルアップに必要だった経験値を記憶
        //(経験値ゲージに使用するから)
        m_prevNeedExp = m_needExp;

        //次のレベルアップに必要な経験値を計算
        m_needExp = GetNeedExp(m_level);

        //レベルアップしたときボムを打つ
        var angleBase = 0;
        var angleRange = 360;
        var count = 28;
        ShootNWay(angleBase, angleRange, 0.2f, count);
        ShootNWay(angleBase, angleRange, 0.25f, count);
        ShootNWay(angleBase, angleRange, 0.3f, count);

        //レベルアップしたときにSEを流す
        var audioSource = FindObjectOfType<AudioSource>();
        audioSource.PlayOneShot(m_levelUpClip);

        // レベルアップしたので、各種パラメータを更新する
        var t = (float)(m_level - 1) / (m_levelMax - 1);  //レベル1でt=0,レベルMAXでt=1 ⇒From~Toを上手くとるLerp
        m_shotCount = Mathf.RoundToInt(
            Mathf.Lerp(m_shotCountFrom, m_shotCountTo, t)); // 弾の発射数
        m_shotInterval = Mathf.Lerp(
            m_shotIntervalFrom, m_shotIntervalTo, t); // 弾の発射間隔（秒）
        m_magnetDistance = Mathf.Lerp(
            m_magnetDistanceFrom, m_magnetDistanceTo, t); // 宝石を引きつける距離
    }

    //指定されたレベルに必要な経験値を計算
    private int GetNeedExp(int level)
    {
        /*経験値の増加
         * 
         * 例えば 
         * level1 : 10+12*0=10
         * level2 : 10+12*1=22
         * level3 : 10+12*4=58
         * 
         * */
        return m_nextExpBase + m_nextExpInterval * (level - 1) * (level - 1);
    }
}