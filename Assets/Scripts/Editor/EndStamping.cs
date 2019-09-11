using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class EndStamping : MonoBehaviour
{
    public GameObject MusicPlayer;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EditorUtility.DisplayDialog("", "Are you sure to save and end?", "Ok", "Cancel"))
            {
                Debug.Log("Saving...");
                MusicPlayer.GetComponent<AudioSource>().Stop();
                // To append text, StringBuilder is faster than just using string variable.
                // https://www.dotnetperls.com/stringbuilder-performance
                // but Don't Use PLUS OPERATOR, it just slows down the performance
                // https://www.dotnetperls.com/stringbuilder-mistake
                StringBuilder bookContent = new StringBuilder();
                StringBuilder presentLine = new StringBuilder();
                for (int column = 0; column < TimingsStore.timings[0].Length; column++)
                {
                    for (int whichButton = 0; whichButton < 4; whichButton++)
                    {
                        // This is necessary because the initial value for float is 0.0f
                        if (Mathf.Approximately(TimingsStore.timings[whichButton][column], 0.0f))
                        {
                            bookContent.Append(",");
                        }
                        else
                        {
                            bookContent.Append(TimingsStore.timings[whichButton][column]);
                            bookContent.Append(",");
                            presentLine.Append(TimingsStore.timings[whichButton][column]);
                        }
                        //Debug.Log(presentLine.ToString() + " : " + tStore.timings[whichButton][column]);

                    }
                    if (presentLine.Length == 0)
                    {
                        bookContent.Remove(bookContent.Length - 4, 4);
                        break;
                    }
                    else
                    {
                        bookContent.Remove(bookContent.Length - 1, 1);
                        presentLine.Length = 0;
                    }
                    bookContent.AppendLine();
                }
                StreamWriter sw = new StreamWriter(@"C:\Users\skyco\Desktop\SecondBook.csv", false);
                sw.Write(bookContent);
                sw.Close();
                Debug.Log("Saved");
            }
        }
    }
}