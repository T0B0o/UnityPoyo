﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour {

    public Image m_hpGauge; 
    public Image m_expGauge;

    public Text m_levelText;  //レベルのテキスト
    public GameObject m_gameOverText; // ゲームオーバーのテキスト

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //プレイヤーを取得
        var player = Player.m_instance;

        //hpゲージの表示を更新
        var hp = player.m_hp;
        var hpMax = player.m_hpMax;
        m_hpGauge.fillAmount = (float)hp / hpMax;

        //経験値のゲージを更新
        var exp = player.m_exp;
        var prevNeedExp = player.m_prevNeedExp;
        var needExp = player.m_needExp;
        m_expGauge.fillAmount = (float)(exp - prevNeedExp) / (needExp - prevNeedExp); //いまのレベルアップ後の獲得EXP/次のレベルまでに必要なEXP

        m_levelText.text = player.m_level.ToString();

        //プレイヤーが非表示ならGAMEOVERと表示
        m_gameOverText.SetActive(!player.gameObject.activeSelf);
	}
}
