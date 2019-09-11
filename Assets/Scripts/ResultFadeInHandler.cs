using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultFadeInHandler : MonoBehaviour {

    public GameObject fadeHandler;
    public GameObject particleOfWater;

	// Use this for initialization
	void Start () {
        fadeHandler.GetComponent<Fade>().FadeIn(1f);
        particleOfWater.GetComponent<ParticleSystem>().Play();
    }
}
