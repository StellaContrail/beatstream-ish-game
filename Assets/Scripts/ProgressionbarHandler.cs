using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionbarHandler : MonoBehaviour {
    
    Vector3 fullProgressionPos;
    Vector3 fullProgressScale;
    float musicLength;

    GameObject ProgressbarFront;
    public GameObject ProgressbarBack;
    public GameObject MusicPlayer;

    bool didTakeLength;
    bool didSetProgressAnimation;

	// Use this for initialization
	void Start () {
        didTakeLength = false;
        didSetProgressAnimation = false;
        ProgressbarFront = gameObject;
        fullProgressionPos = ProgressbarBack.transform.position;
        fullProgressScale = ProgressbarBack.transform.lossyScale;
    }
	
	// Update is called once per frame
	void Update () {
        if (!didTakeLength && MusicPlayer.GetComponent<AudioSource>().clip.loadState == AudioDataLoadState.Loaded)
        {
            musicLength = MusicPlayer.GetComponent<AudioSource>().clip.length;
            didTakeLength = true;
        }
        if (!didSetProgressAnimation && MusicPlayer.GetComponent<AudioSource>().isPlaying)
        {
            iTween.ScaleTo(ProgressbarFront, iTween.Hash(
                "x", fullProgressScale.x,
                "time", musicLength,
                "easetype", iTween.EaseType.linear
                ));
            iTween.MoveAdd(ProgressbarFront, iTween.Hash(
                "x", fullProgressScale.x / 2,
                "time", musicLength,
                "easetype", iTween.EaseType.linear
                ));
            didSetProgressAnimation = true;
        }
    }
}
