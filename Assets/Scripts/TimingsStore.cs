using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimingsStore : MonoBehaviour {
    
    public static float[][] timings = new float[4][];
    public int BPM;
    public TextMesh debugText;
    public GameObject MusicPlayer;

	// Use this for initialization
	void Start () {
        timings[0] = new float[1024];
        timings[1] = new float[1024];
        timings[2] = new float[1024];
        timings[3] = new float[1024];
    }

    // Update is called once per frame
    void Update () {
        if (MusicPlayer.GetComponent<AudioSource>().clip != null && MusicPlayer.GetComponent<AudioSource>().clip.loadState == AudioDataLoadState.Loaded)
        {
            debugText.text = ManageTime_Script.spentTime.ToString();
        }
	}
}
