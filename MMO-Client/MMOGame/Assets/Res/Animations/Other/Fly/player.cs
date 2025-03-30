using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour {

	public Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("1")) 
		{
			anim.Play("idle_ground", -1);
		}
		if (Input.GetMouseButtonDown(0)) 
		{
			anim.Play("attack1", -1,0f);
		}
	}
}
