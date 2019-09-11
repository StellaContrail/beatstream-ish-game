using UnityEngine;
using System.Text.RegularExpressions;

public class ScoreHandleScript : MonoBehaviour {

    public TextMesh allPerfectText, perfectText, okayText, badText, maxComboText, totalScoreText, percentText;
    public TextMesh SongTitleLabel, SongCreaterLabel, SongBPMLabel;
    public TextMesh SongTitleShadow, SongCreaterShadow, SongBPMShadow;
    public GameObject Album;

    // Use this for initialization
    void Start() {
        Texture2D albumImage;
        try
        {
            albumImage = Resources.Load(SongsLoader.folderNames[SongsLoader.presentMainSongIndex] + "/image") as Texture2D;
        }
        catch
        {
            //this is for debug mode
            albumImage = Resources.Load("Road_To_Heaven/image") as Texture2D;
        }
        Album.GetComponent<Renderer>().material.mainTexture = albumImage;

        allPerfectText.text = "x " + PutSpaceBetweenEachWord(string.Format("{0:000}", StoreScore.allPerfect_score));
        perfectText.text = "x " + PutSpaceBetweenEachWord(string.Format("{0:000}", StoreScore.perfect_score));
        okayText.text = "x " + PutSpaceBetweenEachWord(string.Format("{0:000}", StoreScore.okay_score));
        badText.text = "x " + PutSpaceBetweenEachWord(string.Format("{0:000}", StoreScore.bad_score));
        maxComboText.text = "x " + PutSpaceBetweenEachWord(string.Format("{0:000}", StoreScore.maxCombo_score));
        totalScoreText.text = PutSpaceBetweenEachWord(string.Format("{0}", StoreScore.totalScore));
        percentText.text = string.Format("{0, 3}", Mathf.RoundToInt((StoreScore.totalScore / 1000000.0f) * 100)) + "%";

        
        if (SongsLoader.SongInfoStore.title != null)
        {
            string title = SongsLoader.SongInfoStore.title;
            Regex regulation = new Regex(@"(?:<size=\d+>)(?<body>.+)(?:<\/size>)", RegexOptions.IgnoreCase);
            if (regulation.IsMatch(title))
            {
                Match matchedText = regulation.Match(title);
                title = matchedText.Groups["body"].Value;
            }
            SongTitleLabel.text = title;
            SongCreaterLabel.text = string.Format("by {0}", SongsLoader.SongInfoStore.creater);
            SongBPMLabel.text = string.Format("{0, 3} BPM", SongsLoader.SongInfoStore.bpm);
        }
        else
        {
            //this is for debug mode
            SongTitleLabel.text = "Debug Mode";
            SongCreaterLabel.text = "by Contrail";
            SongBPMLabel.text = "999 BPM";
        }


        SongTitleShadow.text = SongTitleLabel.text;
        SongCreaterShadow.text = SongCreaterLabel.text;
        SongBPMShadow.text = SongBPMLabel.text;

    }

    string PutSpaceBetweenEachWord(string original)
    {
        string result = "";
        foreach (char _original in original)
        {
            result += _original + " ";
        }
        result.TrimEnd(' ');
        return result;
    }
}
