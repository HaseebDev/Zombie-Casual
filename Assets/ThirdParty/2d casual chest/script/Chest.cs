using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {

	public GameObject StartScreen;
	public GameObject Back;
	public GameObject Wooden;
	public GameObject Silver;
	public GameObject Golden;
	public GameObject Magical;
	// Use this for initialization
	void Start () {
		Back.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Cr_w () {
		Instantiate(Wooden);
		StartScreen.SetActive(false);
		Back.SetActive(true);
	}
	public void Cr_s () {
		Instantiate(Silver);
		StartScreen.SetActive(false);
		Back.SetActive(true);
	}
	public void Cr_g () {
		Instantiate(Golden);
		StartScreen.SetActive(false);
		Back.SetActive(true);
	}
	public void Cr_m () {
		Instantiate(Magical);
		StartScreen.SetActive(false);
		Back.SetActive(true);
	}
	public void rest () {
		 Application.LoadLevel("Demo_visual");
	}
}
