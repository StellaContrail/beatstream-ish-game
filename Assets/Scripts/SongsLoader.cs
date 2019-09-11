using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//TODO: Press Esc Key to show menu(Exit, BackToTitle, Resume)
public class SongsLoader : MonoBehaviour
{
    public GameObject fadeHandler;

    public TextMesh SongTitleObject;
    public TextMesh SongtitleShadowObject;
    public TextMesh SongCreaterObject;
    public TextMesh SongBPMObject;
    public TextMesh SongDifficultyStarsObject;
    public GameObject MainSongAlbum;
    public GameObject PrevSongAlbum;
    public GameObject NextSongAlbum;
    public AudioSource MainSongSource;

    public TextMesh debugText;

    int folderCount;
    static public int presentMainSongIndex;
    bool isChanging;
    bool doPlay;

    float defaultVolume;

    static bool _isSongChanged;
    public static bool IsSongChanged    //once lifespan
    {
        get
        {
            if (_isSongChanged)
            {
                _isSongChanged = false;
                return true;
            }
            else
            {
                return false;
            }
        }
        set
        {
            _isSongChanged = value;
        }
    }

    [System.Serializable]
    public class SongInfo
    {
        public string title;
        public string creater;
        public int bpm;
        public int noteCount;
        public int stars;
    }
    
    public static class SongInfoStore
    {
        public static string title;
        public static string creater;
        public static int bpm;
        public static int noteCount;
    }
    
    public static string[] folderNames;

    void FadeIn(float _density)
    {
        // Fade in effect
        //fadeHandler.GetComponent<FadeImage>().Range = _density;
    }

    // <summary>Used for initializing the menu album graphics</summary>
    void Start()
    {
        _isSongChanged = false;
        presentMainSongIndex = 0;
        SongInfoStore.title = "";
        SongInfoStore.creater = "";
        SongInfoStore.bpm = 0;
        SongInfoStore.noteCount = 0;

        iTween.ValueTo(gameObject, iTween.Hash(
                "from", 1f,
                "to", 0f,
                "time", 1.5f,
                "onupdate", "FadeIn"));

        doPlay = false;
        defaultVolume = MainSongSource.volume;
        isChanging = false;
        List<string> sortedFolderNames = new List<string>();

        TextAsset listText = Resources.Load("list") as TextAsset;
        StringReader sr = new StringReader(listText.text);
        while (sr.Peek() > -1)
        {
            sortedFolderNames.Add(sr.ReadLine());
        }

        folderCount = sortedFolderNames.Count;
        Debug.Log(folderCount);
        folderNames = new string[folderCount];
        for (int i = 0; i < folderCount; i++)
        {
            folderNames[i] = sortedFolderNames[i];
        }

        StartCoroutine(ChangeSongInfos(folderCount - 1, 0, 1));
    }

    // <summary>Shifts main song to the next</summary>
    public void ShiftPlaneToNext()
    {
        #region sorting_example
        // if there are four elements in total...
        // Rotate forward
        // [ 0 1 2 ]
        // [ 1 2 3 ]
        // [ 2 3 0 ]
        // [ 3 0 1 ]
        // [ 0 1 2 ]
        #endregion

        if (!isChanging)
        {
            //when rotating forward, next main song index will be the next song's index or 0
            int mainSongIndex = presentMainSongIndex < folderCount - 1 ? presentMainSongIndex + 1 : 0;
            //if there is no next song, the index cycle start over from zero
            int nextSongIndex = mainSongIndex < folderCount - 1 ? mainSongIndex + 1 : 0;
            //if there is no prev song, the endpoint will come toward your ass
            int prevSongIndex = mainSongIndex != 0 ? mainSongIndex - 1 : folderCount - 1;

            StartCoroutine(ChangeSongInfos(prevSongIndex, mainSongIndex, nextSongIndex));
        }
    }


    // <summary>Shifts main song to the next</summary>
    public void ShiftPlaneToPrevious()
    {
        #region sorting_example
        // if there are four elements in total...

        // Rotate backward
        // [ 0 1 2 ]
        // [ 3 0 1 ]
        // [ 2 3 0 ]
        // [ 1 2 3 ]
        // [ 0 1 2 ]
        #endregion

        if (!isChanging)
        {
            //when rotating forward, next main song index will be the next song's index or 0
            int mainSongIndex = presentMainSongIndex > 0 ? presentMainSongIndex - 1 : folderCount - 1;
            //if there is no next song, the index cycle start over from zero
            int nextSongIndex = mainSongIndex < folderCount - 1 ? mainSongIndex + 1 : 0;
            //if there is no prev song, the endpoint will come toward your ass
            int prevSongIndex = mainSongIndex != 0 ? mainSongIndex - 1 : folderCount - 1;
            
            StartCoroutine(ChangeSongInfos(prevSongIndex, mainSongIndex, nextSongIndex));
        }
    }

    // <summary>Setting up album appearance</summary>
    IEnumerator ChangeSongInfos(int prevSongIndex, int mainSongIndex, int nextSongIndex)
    {
        float waitTime = 0.5f;

        Debug.Log("START");
        isChanging = true;
        TextAsset jsonFile = Resources.Load(folderNames[mainSongIndex] + "/property") as TextAsset;
        string jsonData = jsonFile.text;
        SongInfo infos = JsonUtility.FromJson<SongInfo>(jsonData);
        SongInfoStore.title = infos.title;
        SongInfoStore.creater = infos.creater;
        SongInfoStore.bpm = infos.bpm;
        SongInfoStore.noteCount = infos.noteCount;
        yield return null;

        //when playing music
        if (MainSongSource.clip != null)
        {
            iTween.ValueTo(gameObject, iTween.Hash(
                "from", defaultVolume,
                "to", 0f,
                "time", 1f,
                "onupdate", "ChangeAudioVolume"
                ));
            yield return new WaitForSeconds(1f);
            MainSongSource.Stop();  //stop playing
            MainSongSource.clip = null; //unload the clip
        }
        
        SongTitleObject.text = infos.title;
        SongtitleShadowObject.text = infos.title;
        SongCreaterObject.text = "by " + infos.creater;
        SongBPMObject.text = string.Format("{0:000} BPM", infos.bpm);
        if (MainSongAlbum.GetComponent<Renderer>().material.mainTexture != null)
        {
            StartCoroutine(DelayedChangeImage(waitTime, MainSongAlbum, folderNames[mainSongIndex]));
        }
        else
        {
            MainSongAlbum.GetComponent<Renderer>().material.mainTexture = LoadPic(folderNames[mainSongIndex]);
        }

        //Only allows wav format
        //Seems like WWW.audioClip has a HUGE issue with releasing its memory space...
        //https://forum.unity3d.com/threads/the-ins-and-outs-of-audio-memory-in-unity.97690/
        //http://answers.unity3d.com/questions/692459/unload-audio-from-external-source.html
        MainSongSource.clip = Resources.Load((folderNames[mainSongIndex]) + "/music") as AudioClip;
        doPlay = true;
        
        SongDifficultyStarsObject.text = string.Format("{0}", new string('★', infos.stars).PadRight(7, '☆'));

        iTween.ValueTo(gameObject, iTween.Hash(
            "to", defaultVolume,
            "from", 0f,
            "time", 1f,
            "onupdate", "ChangeAudioVolume"
            ));

        //if image is attached already
        if (PrevSongAlbum.GetComponent<Renderer>().material.mainTexture != null)
        {
            StartCoroutine(DelayedChangeImage(waitTime, PrevSongAlbum, folderNames[prevSongIndex]));
            yield return StartCoroutine(DelayedChangeImage(waitTime, NextSongAlbum, folderNames[nextSongIndex]));
        }
        else
        {
            PrevSongAlbum.GetComponent<Renderer>().material.mainTexture = LoadPic(folderNames[prevSongIndex]);
            //Debug.Log();
            NextSongAlbum.GetComponent<Renderer>().material.mainTexture = LoadPic(folderNames[nextSongIndex]);
        }

        presentMainSongIndex = mainSongIndex;
        
        isChanging = false;
        Debug.Log("END");
    }

    void ChangeAudioVolume(float _volume)
    {
        MainSongSource.volume = _volume;
    }

    IEnumerator DelayedChangeImage(float waitTime, GameObject album, string folderPath)
    {
        iTween.ColorTo(album, iTween.Hash(
            "a", 0f,
            "includeChildren", false,
            "time", waitTime));
        yield return new WaitForSeconds(waitTime);

        album.GetComponent<Renderer>().material.mainTexture = LoadPic(folderPath);
        yield return null;

        iTween.ColorTo(album, iTween.Hash(
            "includeChildren", false,
            "a", 1f,
            "time", waitTime));
        yield return new WaitForSeconds(waitTime);
    }
    
    Texture2D LoadPic(string folderPath)
    {
        return Resources.Load(folderPath + "/image") as Texture2D;
    }

    // Update is called once per frame
    void Update()
    {
        if (doPlay && MainSongSource.clip != null && MainSongSource.clip.loadState == AudioDataLoadState.Loaded && !MainSongSource.isPlaying)
        {
            doPlay = false;
            IsSongChanged = true;
            Debug.Log("PLAY");
            MainSongSource.Play();
        }
    }
}
