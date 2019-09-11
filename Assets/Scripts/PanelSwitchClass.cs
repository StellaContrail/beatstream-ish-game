using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelSwitchClass : MonoBehaviour
{

    SongsLoader songsLoader;

    public GameObject rightButton;
    public GameObject leftButton;
    public GameObject targetPlane;
    void Start()
    {
        songsLoader = gameObject.GetComponent<SongsLoader>();
    }

    void LeftButtonClicked()
    {
        iTween.ColorFrom(leftButton, new Color32(0, 140, 255, 255), 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("right"))
        {
            iTween.ColorTo(rightButton, new Color32(0, 140, 255, 255), 0.05f);
            songsLoader.ShiftPlaneToNext();

        }
        if (Input.GetKeyUp("right"))
        {
            iTween.ColorTo(rightButton, new Color32(255, 255, 255, 255), 0.5f);
        }

        if (Input.GetKeyDown("left"))
        {
            iTween.ColorTo(leftButton, new Color32(0, 140, 255, 255), 0.05f);
            songsLoader.ShiftPlaneToPrevious();
        }
        if (Input.GetKeyUp("left"))
        {
            iTween.ColorTo(leftButton, new Color32(255, 255, 255, 255), 0.5f);
        }

    }
}
