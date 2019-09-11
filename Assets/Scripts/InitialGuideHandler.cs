using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialGuideHandler : MonoBehaviour {

    public GameObject MusicPlayer;
    bool wasPlaying;
    bool _isPausing;
    public bool IsPausing   //True: when music was already played, and now it's just paused. False: Exceptions for True.
    {
        get
        {
            _isPausing = wasPlaying && !MusicPlayer.GetComponent<AudioSource>().isPlaying;
            return _isPausing;
        }
    }
    bool _isReadyToPlay;    //Non Accessible. Use IsReadyToPlay instead.
    public bool IsReadyToPlay
    {
        get
        {
            _isReadyToPlay = MusicPlayer.GetComponent<AudioSource>().clip != null && MusicPlayer.GetComponent<AudioSource>().clip.loadState == AudioDataLoadState.Loaded;
            return _isReadyToPlay;
        }
    }
	// Use this for initialization
	void Start () {
        _isReadyToPlay = false;
        wasPlaying = false;
        _isPausing = true;
        gameObject.GetComponent<TextMesh>().text = "SPACE to\nstart editing";
	}
	
	// Update is called once per frame
	void Update () {
        if (IsReadyToPlay)
        {
            if (Input.GetKeyDown("space"))
            {
                //called when music is played for the first time
                if (!wasPlaying)
                {
                    MusicPlayer.GetComponent<AudioSource>().Play();
                    wasPlaying = true;
                }
                //called when music is already played and in being paused
                else if (IsPausing)
                {
                    MusicPlayer.GetComponent<AudioSource>().UnPause();
                }
                //called when music is already played and still in playing
                else if (!IsPausing)
                {
                    MusicPlayer.GetComponent<AudioSource>().Pause();
                }
            }
        }
        else if (MusicPlayer.GetComponent<AudioSource>().clip == null)
        {
            gameObject.GetComponent<TextMesh>().text = "<color=red>ERROR\nNo clip attached</color>";
        }
        else if (MusicPlayer.GetComponent<AudioSource>().clip.loadState != AudioDataLoadState.Loaded)
        {
            gameObject.GetComponent<TextMesh>().text = "<color=red>ERROR\nClip yet not loaded</color>";
        }

        if (IsPausing)
        {
            gameObject.GetComponent<TextMesh>().text = "SPACE to\nstop playing";
        }
        else if (!IsPausing && wasPlaying)
        {
            gameObject.GetComponent<TextMesh>().text = "SPACE to\n<size=47>resume playing</size>";
        }
    }
}
