using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StampTimings : MonoBehaviour {

    public string whichButton;
    private int buttonNum;
    private int[] count = new int[4];
    private bool didPressAway;

	// Use this for initialization
	void Start () {
        didPressAway = true;
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(whichButton))
        {
            if (didPressAway)
            {
                switch (whichButton)
                {
                    case "right":
                        buttonNum = 0;
                        break;
                    case "left":
                        buttonNum = 1;
                        break;
                    case "up":
                        buttonNum = 2;
                        break;
                    case "down":
                        buttonNum = 3;
                        break;
                }
                // timings[0][0] -> First timing for the right button
                // timings[1][1] -> Second timing for the left button
                // timings[2][3] -> Fourth timing for the up button
                TimingsStore.timings[buttonNum][count[buttonNum]++] =
                    ManageTime_Script.spentTime;
                didPressAway = false;
            }
        }
        else
        {
            didPressAway = true;
        }
    }
}
