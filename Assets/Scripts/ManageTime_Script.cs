using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageTime_Script : MonoBehaviour {

    //TODO: musicDelay and judgeDelay should be automatically measured before the music starts playing
    public float musicDelay = 0f;
    public float judgeDelay = 0f;
    public static float spentTime;
    public GameObject MusicPlayer;

    // Use this for initialization
    void Start ()
    {
        spentTime = 0f;
    }
	
	// Update is called once per frame
	void Update ()
    {

        //spentTime: note will appear depending on this time
        spentTime = MusicPlayer.GetComponent<AudioSource>().time + musicDelay;
    }
}
