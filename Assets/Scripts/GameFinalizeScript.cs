using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;

public class GameFinalizeScript : MonoBehaviour {

    // wasPlaying becomes true when music finishes
    bool wasPlaying;
    public GameObject fadeHandler;
    public float fadeOutDuration = 1f;

    public GameObject mainCamera;

    // didLaunch becomes true when scene is shifting
    bool didLaunch;
    AudioSource music;

    // Use this for initialization
    void Start () {
        didLaunch = false;
        wasPlaying = false;
        music = gameObject.GetComponent<AudioSource>();
    }

    void ChangeBlurDensity (float _aperture) {
        mainCamera.GetComponent<DepthOfField> ().aperture = _aperture;
    }

    IEnumerator ShiftToResult () {
        InitialSettings_Script.isBorderChangingColor = false;

        //increase density of blur
        /*iTween.ValueTo(gameObject, iTween.Hash(
            "from", 0f,
            "to", 0.5f,
            "time", fadeOutDuration,
            "onupdate", "ChangeBlurDensity"
        ));
        */

        fadeHandler.GetComponent<Fade>().FadeOut(fadeOutDuration);
        yield return new WaitForSeconds (fadeOutDuration + 1f);

        //Load result scene after playing
        SceneManager.LoadScene ("result_scene");
        yield return null;
    }

    // Update is called once per frame
    void Update () {
        // 画面からフォーカスがズレるとPlayingが止まる現象を解決したい
        if (!wasPlaying && music.time >= music.clip.length) { wasPlaying = true; }

        if (!didLaunch && wasPlaying) {
            didLaunch = true;
            if (StoreScore.maxCombo_score < StoreScore.combo_score) {
                StoreScore.maxCombo_score = StoreScore.combo_score;
            }
            StartCoroutine (ShiftToResult ());
        }

        //This is for debug mode
        if (Input.GetKeyDown ("f8")) {
            if (StoreScore.maxCombo_score < StoreScore.combo_score) {
                StoreScore.maxCombo_score = StoreScore.combo_score;
            }
            StartCoroutine (ShiftToResult ());
        }
    }

}