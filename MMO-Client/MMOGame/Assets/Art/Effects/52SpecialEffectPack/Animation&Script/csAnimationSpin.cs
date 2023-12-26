using UnityEngine;
using System.Collections;

public class csAnimationSpin : MonoBehaviour {

    Animation an;

	void Update () {
        an = gameObject.GetComponent<Animation>();
        an.Play();
	}
}
