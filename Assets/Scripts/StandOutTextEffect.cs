using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StandOutTextEffect : MonoBehaviour {

    string[] templeteTexts;
    TextMesh targetTextMesh;
    public GameObject fadeHandler;

	// Use this for initialization
	void Start () {
        targetTextMesh = gameObject.GetComponent<TextMesh>();
        templeteTexts = new string[4] {
            "Press Space to proceed",
            "Press Space to proceed.",
            "Press Space to proceed..",
            "Press Space to proceed..."
        };
        LoopChangingTextBody();
        iTween.ColorTo(gameObject, iTween.Hash(
            "a", 0.5f,
            "time", 0.2f,
            "delay", 0.5f,
            "easetype", iTween.EaseType.easeInExpo,
            "looptype", iTween.LoopType.pingPong
            ));
	}
	
    void LoopChangingTextBody()
    {
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", 0.0f,
            "to", 3.9f,
            "time", 2.0f,
            "easetype", iTween.EaseType.linear,
            "onupdate", "ChangeTextBody",
            "oncomplete", "LoopChangingTextBody"
            ));
    }

    void ChangeTextBody(float count)
    {
        targetTextMesh.text = templeteTexts[Mathf.FloorToInt(count)];
    }

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("space"))
        {
            StartCoroutine(NextScene());
        }
	}
    
    IEnumerator NextScene() {
        fadeHandler.GetComponent<Fade>().FadeOut(1f);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("menu_scene");
    }
}
