using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayCommandController : MonoBehaviour {

    public GameObject buttonFront;
    public GameObject buttonBack;
    public GameObject playText;
    public GameObject playTextShadow;

    public AudioClip SEPlay;

    public GameObject fadeHandler;
    public AudioSource player;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("space"))
        {
            StartCoroutine("LoadPlayScene");
        }
    }

    IEnumerator LoadPlayScene()
    {
        Vector3 oldFrontPos = buttonFront.transform.position;
        Vector3 oldTextPos = playText.transform.position;
        iTween.MoveTo(buttonFront, buttonBack.transform.position, 0.5f);
        iTween.MoveTo(buttonFront, iTween.Hash(
            "position", oldFrontPos,
            "time", 0.5f,
            "delay", 0.5f
        ));
        iTween.MoveTo(playText, playTextShadow.transform.position, 0.5f);
        iTween.MoveTo(playText, iTween.Hash(
            "position", oldTextPos,
            "time", 0.5f,
            "delay", 0.5f
        ));
        iTween.Stab(gameObject, iTween.Hash(
            "audioclip", SEPlay,
            "volume", 0.1f
        ));

        // TODO: FADEOUT EFFECT
        yield return new WaitForSeconds(0.8f);
        fadeHandler.GetComponent<Fade>().FadeOut(1f);
        yield return new WaitForSeconds(1f);

        iTween.ValueTo(gameObject, iTween.Hash(
            "from", player.volume,
            "to", 0f,
            "time", 1f,
            "delay", 1f,
            "onupdate", "SoundFadeOut"
        ));
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("playing_scene");
    }

    void SoundFadeOut(float _volume)
    {
        player.volume = _volume;
    }
}
