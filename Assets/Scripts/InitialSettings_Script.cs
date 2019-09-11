using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class InitialSettings_Script : MonoBehaviour
{
    public Text centerText;
    public GameObject countdownParticle;
    public GameObject MainCamera;
    public GameObject MusicPlayer;
    public GameObject NoteLoader;

    public TextMesh debugText;

    public static bool isBorderChangingColor;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(InitialAwakenSetUp());
    }

    IEnumerator InitialAwakenSetUp()
    {
        AudioClip song;
        try
        {
            song = Resources.Load(SongsLoader.folderNames[SongsLoader.presentMainSongIndex] + "/music") as AudioClip;
        }
        catch
        {
            //this is for debug mode
            song = Resources.Load("Road_To_Heaven/music") as AudioClip;
        }
        MusicPlayer.GetComponent<AudioSource>().clip = song;
        yield return null;
        
        if (NoteLoader.GetComponent<DuplicateNotes>().LoadNotesData())
        {
            iTween.ValueTo(gameObject, iTween.Hash(
                "from", 0.05f,
                "to", 0f,
                "time", 3f,
                "onupdate", "DepthOfCameraBlur",
                "easetype", iTween.EaseType.easeInExpo,
                "oncomplete", "PlaySound"
                ));
            InvokeRepeating("LoopAndCountdown", 0f, 1f);
            InvokeRepeating("InvokeChangeBorderColor", 0f, 4f);
            isBorderChangingColor = true;
            yield return null;
        }
    }
    

    int i = 3;
    void LoopAndCountdown()
    {
        centerText.text = "<size=70>" + i-- + "</size>";
        //if time is integer
        if (i >= 0)
        {
            Instantiate(countdownParticle, new Vector3(0, 10, 0), Quaternion.Euler(90, 0, 0));
        }
        if (i < 0)
        {
            CancelInvoke("LoopAndCountdown");
            centerText.text = "";
        }
    }

    void PlaySound()
    {
        if (MusicPlayer.GetComponent<AudioSource>().clip.loadState == AudioDataLoadState.Loaded)
        {
            MusicPlayer.GetComponent<AudioSource>().Play();
        }
        else
        {
            Debug.Log("Clip has not been loaded.");
        }
    }

    void DepthOfCameraBlur(float _aperture)
    {
        MainCamera.GetComponent<DepthOfField>().aperture = _aperture;
    }

    void InvokeChangeBorderColor()
    {
        if (isBorderChangingColor)
        {
            StartCoroutine(ChangeBorderColorRepeatedly());
        }
        else
        {
            CancelInvoke("ChangeBorderColorRepeatedly");
        }
    }

    IEnumerator ChangeBorderColorRepeatedly()
    {
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", -1f,
            "to", 1f,
            "time", 2f,
            "onupdate", "ChangeBrightnessOfDisplayBorders"
            ));
        yield return new WaitForSeconds(2f);

        iTween.ValueTo(gameObject, iTween.Hash(
            "from", 1f,
            "to", -1f,
            "time", 2f,
            "onupdate", "ChangeBrightnessOfDisplayBorders"
            ));
        
        yield return null;
    }

    void ChangeBrightnessOfDisplayBorders(float _intensity)
    {
        debugText.text = _intensity.ToString();
        MainCamera.GetComponent<CreaseShading>().intensity = _intensity;
    }
}
