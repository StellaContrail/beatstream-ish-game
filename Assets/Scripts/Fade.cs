using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour {

    UnityEngine.UI.Image fadeImage;
    Color _color;
    void Start() {
        fadeImage = gameObject.GetComponent<UnityEngine.UI.Image>();
        _color = fadeImage.color;
    }

	public void FadeIn (float duration) {
        iTween.ValueTo(gameObject, iTween.Hash(
			"from", 1,
			"to", 0,
			"time", duration,
			"easetype", iTween.EaseType.linear,
			"onupdate", "ChangeAlpha"
		));
    }

	public void FadeOut (float duration) {
        iTween.ValueTo(gameObject, iTween.Hash(
			"from", 0,
			"to", 1,
			"time", duration,
			"easetype", iTween.EaseType.linear,
			"onupdate", "ChangeAlpha"
		));
	}

	void ChangeAlpha (float alpha) {
        Debug.Log(alpha);
        gameObject.GetComponent<UnityEngine.UI.Image>().color = new Color(_color.r, _color.g, _color.b, alpha);
	}
}
