using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using TMPro;

public class MarqueeTitleHandler : MonoBehaviour
{

    int titleLength;
    // Use this for initialization
    void Start()
    {
        string title = "";
        title = SongsLoader.SongInfoStore.title;
        //this is for debug mode
        if (title == null)
        {
            title = "Hello World! This text is sample!";
        }

        Regex regulation = new Regex(@"(?:<size=\d+>)(?<body>.+)(?:<\/size>)", RegexOptions.IgnoreCase);
        if(regulation.IsMatch(title))
        {
            Match matchedText = regulation.Match(title);
            title = matchedText.Groups["body"].Value;
        }
        
        gameObject.GetComponent<TextMeshProUGUI>().text = title;
        titleLength = title.Length;
        LoopMarqueeing();
    }

    void LoopMarqueeing()
    {
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", 180f,
            "to", -18f * titleLength,
            "time", (180f + titleLength) * 0.1f,
            "easetype", iTween.EaseType.linear,
            "onupdate", "ChangeOffset",
            "oncomplete", "LoopMarqueeing"
            ));
    }

    void ChangeOffset(float _offset)
    {
        gameObject.GetComponent<TextMeshProUGUI>().margin = new Vector4(_offset, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
