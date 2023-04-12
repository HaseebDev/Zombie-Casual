using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationChest : MonoBehaviour {
	public GameObject[] card;
	Animator anima;

	void Start () {
		anima = GetComponent<Animator>();
	}
	
	void Update () {

	}

	void OnMouseDown() {
		Destroy(GameObject.FindWithTag("card"));
        anima.Play ("wooden_reopen");
        anima.Play ("silver_reopen");
        anima.Play ("golden_reopen");
        anima.Play ("magical_reopen");
        Instantiate(card[Random.Range(1, 4)]);
    }
}
