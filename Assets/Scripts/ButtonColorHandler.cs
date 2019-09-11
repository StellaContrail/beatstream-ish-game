using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ButtonColorHandler : MonoBehaviour {

    Color initialColor;
    public enum WhichButton
    {
        RIGHT = 0,
        LEFT,
        UP,
        DOWN
    }
    public WhichButton _whichButton;
    string buttonType;
	// Use this for initialization
	void Start () {
        initialColor = gameObject.GetComponent<Renderer>().material.GetColor("_EmissionColor");
        switch (_whichButton)
        {
            case WhichButton.RIGHT:
                buttonType = "right";
                break;
            case WhichButton.LEFT:
                buttonType = "left";
                break;
            case WhichButton.UP:
                buttonType = "up";
                break;
            case WhichButton.DOWN:
                buttonType = "down";
                break;
        }
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(buttonType))
        {
            iTween.ValueTo(gameObject, iTween.Hash(
                "from", initialColor,
                "to", new Color(0.5f, 0.75f, 1f),
                "time", 0.2f,
                "onupdate", "ChangeColor"
                ));
        }
        if (Input.GetKeyUp(buttonType))
        {
            iTween.ValueTo(gameObject, iTween.Hash(
                "to", initialColor,
                "from", new Color(0.5f, 0.75f, 1f),
                "time", 0.2f,
                "onupdate", "ChangeColor"
                ));
        }

    }

    void ChangeColor(Color _color)
    {
        gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", _color);
    }
}
