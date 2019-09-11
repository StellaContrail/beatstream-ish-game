using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonChildClickedDetector : MonoBehaviour {

    public GameObject SongLoadHandler;
    public GameObject thisParent;
    SongsLoader songsLoader;

    public enum ButtonType
    {
        Right,
        Left
    }
    public ButtonType buttonType;
    private void Start()
    {
        songsLoader = SongLoadHandler.GetComponent<SongsLoader>();
    }

    private void OnMouseDown()
    {
        iTween.ColorFrom(thisParent, new Color32(0, 140, 255, 255), 0.5f);
        switch (buttonType)
        {
            case ButtonType.Right:
                songsLoader.ShiftPlaneToNext();
                break;
            case ButtonType.Left:
                songsLoader.ShiftPlaneToPrevious();
                break;
        }

    }
}
