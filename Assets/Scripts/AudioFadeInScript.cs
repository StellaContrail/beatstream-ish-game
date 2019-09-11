using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFadeInScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        float defaltVolume = gameObject.GetComponent<AudioSource>().volume;
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", 0.0f,
            "to", defaltVolume,
            "time", 1.5f,
            "easetype", iTween.EaseType.linear, 
            "onupdate", "ChangeAudioVolume"));
	}
	
    void ChangeAudioVolume(float _volume)
    {
        gameObject.GetComponent<AudioSource>().volume = _volume;
    }
    
}
