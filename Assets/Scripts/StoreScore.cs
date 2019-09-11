using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreScore : MonoBehaviour
{

    public static int allPerfect_score;
    public static int perfect_score;
    public static int okay_score;
    public static int bad_score;
    public static int combo_score;
    public static int maxCombo_score;
    public static int totalScore;
    public static int accomplishedPercent;
    public static bool isComboLasting; //readonly from other classes
    public TextMesh scoreText;
    public TextMesh totalScoreText;
    [System.NonSerialized]
    public int notesCount;

    int scoreRate;

    // Use this for initialization
    void Start()
    {
        maxCombo_score = 0;
        accomplishedPercent = 0;
        totalScore = 0;
        allPerfect_score = 0;
        okay_score = 0;
        perfect_score = 0;
        bad_score = 0;
        combo_score = 0;
        isComboLasting = true;
        notesCount = SongsLoader.SongInfoStore.noteCount;
        //this is for debug mde
        if (notesCount == 0)
        {
            notesCount = 650;
        }
    }

    public void AddScore(int _scoreLevel)
    {
        if (_scoreLevel > -1)
        {
            scoreRate = 0;
            switch (_scoreLevel)
            {
                case 3:
                    scoreRate = 100;
                    allPerfect_score++;
                    isComboLasting = true;
                    combo_score++;
                    break;
                case 2:
                    scoreRate = 70;
                    perfect_score++;
                    isComboLasting = true;
                    combo_score++;
                    break;
                case 1:
                    scoreRate = 30;
                    okay_score++;
                    isComboLasting = true;
                    combo_score++;
                    break;
                case 0:
                    scoreRate = 0;
                    bad_score++;
                    isComboLasting = false;
                    if (maxCombo_score < combo_score)
                    {
                        maxCombo_score = combo_score;
                    }
                    combo_score = 0;
                    break;
            }
            //Adopted the scoresystem of Cytus. Maximum score is 1,000,000
            int basePoint = 0;
            if (scoreRate > 0)
            {
                //(900000 / AllNotesCount) * (scoreRate / 100)
                basePoint = Mathf.RoundToInt((900000 / notesCount) * (scoreRate / 100));
            }
            int comboBonus = 0;
            if (combo_score > 1)
            {
                //100000 * [(comboScore - 1) / {(1/2)N(N-1)}]
                comboBonus = (200000 * (combo_score - 1) / (notesCount * (notesCount - 1)));
            }
            totalScore += basePoint + comboBonus;
            Debug.Log(string.Format("[basePoint]{0} : [comboBonus]{1} : [totalScore]{2}", basePoint, comboBonus, totalScore));
            accomplishedPercent = Mathf.RoundToInt((totalScore / 1000000.0f) * 100);
            //we cannot use <br>, we have to use \n
            //See http://answers.unity3d.com/questions/138464/how-to-make-a-line-break-in-a-gui-label.html
            scoreText.text = string.Format("<size=50>{0}</size>\ncombo\n<size=50>{1} </size>\n% ", combo_score, accomplishedPercent);
            //put spaces between numbers
            //useless numbers should be grayed
            string totalScoreBody = "";
            bool NatNumShown = false;
            foreach (char num in string.Format("{0:0000000}", totalScore))
            {
                if (num != '0' && !NatNumShown)
                {
                    NatNumShown = true;
                    totalScoreBody += "<color=#FFFFFF>" + num;
                    //totalScoreBody += " ";
                }
                else
                {
                    totalScoreBody += num;
                    //totalScoreBody += " ";
                }
            }
            totalScoreBody.TrimEnd(' ');
            if (NatNumShown)
            {
                totalScoreBody += "</color>";
            }
            totalScoreText.text = totalScoreBody;
            /*
            scoreText.text = "All Perfect : " + allPerfect_score.ToString() + 
                "\nPerfect : " + perfect_score.ToString() + 
                "\nOkay : " + okay_score.ToString() + 
                "\nBad : " + bad_score.ToString() + 
                "\nCombo : " + combo_score.ToString();
                */
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
