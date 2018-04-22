using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	// Use this for initialization(爆発effect生成時に呼び出される)
	void Start () {
        //演出が終了したら削除
        var particleSystem = GetComponent<ParticleSystem>();
        Destroy(gameObject, particleSystem.main.duration);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
