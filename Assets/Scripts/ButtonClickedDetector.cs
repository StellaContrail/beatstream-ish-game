using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClickedDetector : MonoBehaviour {

    public GameObject SongLoadHandler;
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
        iTween.ColorFrom(gameObject, new Color32(0, 140, 255, 255), 0.5f);
        switch(buttonType)
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
