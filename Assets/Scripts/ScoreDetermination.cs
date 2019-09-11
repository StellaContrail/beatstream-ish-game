using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDetermination : MonoBehaviour
{
    public GameObject ScoreStore;
    public GameObject TimeManagement;

    public GameObject MusicPlayer;

    public GameObject particleWhenBroken;
    public GameObject allPerfectWhenBroken;
    public GameObject perfectWhenBroken;
    public GameObject okayWhenBroken;
    public GameObject badWhenBroken;

    int[] nextNoteIndex;
    public float maxmumRange = 0.783f;
    public float allPerfectRange = 0.033f;
    public float perfectRange = 0.350f;
    public float okayRange = 0.400f;
    public float deleteRange = 3.0f;

    // Use this for initialization
    void Start()
    {
        //initialize with zero
        nextNoteIndex = new int[4];
    }

    // Update is called once per frame
    void Update()
    {
        if (MusicPlayer.GetComponent<AudioSource>().isPlaying)
        {
            int keyIndex = -1;
            //duplicates are intended to understand more than two inputs
            if (Input.GetKeyDown("right"))
            {
                keyIndex = DuplicateNotes.RIGHT_BUTTON;
                ScoreNote(keyIndex);
            }
            if (Input.GetKeyDown("left"))
            {
                keyIndex = DuplicateNotes.LEFT_BUTTON;
                ScoreNote(keyIndex);

            }
            if (Input.GetKeyDown("up"))
            {
                keyIndex = DuplicateNotes.UP_BUTTON;
                ScoreNote(keyIndex);

            }
            if (Input.GetKeyDown("down"))
            {
                keyIndex = DuplicateNotes.DOWN_BUTTON;
                ScoreNote(keyIndex);

            }
        }
    }

    void ScoreNote(int keyIndex)
    {
        //finds out which timing the next note of this line is
        float nextNoteTiming = gameObject.GetComponent<DuplicateNotes>().timings[keyIndex][nextNoteIndex[keyIndex]];
        //get the score
        int score = GetScore(nextNoteTiming, keyIndex);
        //add score into the storage
        ScoreStore.GetComponent<StoreScore>().AddScore(score);
        Debug.Log((score > -1) ? score.ToString() : "Out of range");
    }

    void FixedUpdate()
    {
        DeletePassedNotes();
    }

    void DestroyAfterCounted(float _nextNoteTiming, int keyIndex)
    {
        //destroy the scored note
        MyUtilities.TweenDestroy(gameObject.GetComponent<DuplicateNotes>().FetchCollectionFromKeyIndex(keyIndex)[nextNoteIndex[keyIndex]]);
        gameObject.GetComponent<DuplicateNotes>().FetchCollectionFromKeyIndex(keyIndex).Remove(nextNoteIndex[keyIndex]);
        DuplicateNotes.notesCount[keyIndex]--;
        //prepare for the next note
        nextNoteIndex[keyIndex]++;
    }

    int GetScore(float _nextNoteTiming, int keyIndex)
    {
        int _score = -1;
        float delay = TimeManagement.GetComponent<ManageTime_Script>().judgeDelay;
        float deltaTimingAbs;   //How different timing is it for its correct timing
        deltaTimingAbs = Mathf.Abs((_nextNoteTiming + delay) - ManageTime_Script.spentTime);
        //if the note is within the range of detection
        if (deltaTimingAbs < maxmumRange)
        {
            Vector3 targetNotePos = gameObject.GetComponent<DuplicateNotes>().FetchCollectionFromKeyIndex(keyIndex)[nextNoteIndex[keyIndex]].transform.position;
            targetNotePos.y += 3f;
            GameObject textWhenBroken = null;
            //if the note is pressed precisely
            if (deltaTimingAbs < allPerfectRange)
            {
                //Unique effect when pressed precisely
                Instantiate(particleWhenBroken, targetNotePos, Quaternion.Euler(90, 0, 0));

                textWhenBroken = allPerfectWhenBroken;
                _score = 3;
            }
            //if the note is pressed perfectly
            else if (deltaTimingAbs < perfectRange)
            {
                textWhenBroken = perfectWhenBroken;
                _score = 2;
            }
            //if the note is pressed okay
            else if (deltaTimingAbs < okayRange)
            {
                textWhenBroken = okayWhenBroken;
                _score = 1;
            }
            //if the note is pressed badly
            else
            {
                textWhenBroken = badWhenBroken;
                _score = 0;
            }

            //Text effect when broken
            if (textWhenBroken != null)
            {
                StartCoroutine(EffectWithScoreText(textWhenBroken, targetNotePos));
            }

            //counted note should be destroyed
            DestroyAfterCounted(_nextNoteTiming, keyIndex);
        }
        //if the note wasn't in the detecton field
        else
        {
            _score = -1;
        }

        return _score;
    }

    IEnumerator EffectWithScoreText(GameObject textWhenBroken, Vector3 targetNotePos)
    {
        GameObject performText = Instantiate(textWhenBroken, targetNotePos, Quaternion.Euler(90, 0, 0));
        iTween.ColorTo(performText, iTween.Hash(
            "a", 0f,
            "time", 0.2f,
            "delay", 0.5f
            ));
        yield return new WaitForSeconds(0.7f);
        MyUtilities.TweenDestroy(performText);
        yield return null;
    }
    //I've found that rigidbody eats performance so much, so adopted this distance-deleting system
    void DeletePassedNotes()
    {
        if (gameObject.GetComponent<DuplicateNotes>().isDataAvailable)
        {
            GameObject[] targetNote = new GameObject[4];
            for (int i = 0; i < 4; i++)
            {
                float nextNoteTiming = gameObject.GetComponent<DuplicateNotes>().timings[i][nextNoteIndex[i]];
                if (gameObject.GetComponent<DuplicateNotes>().FetchCollectionFromKeyIndex(i).ContainsKey(nextNoteIndex[i]))
                {
                    targetNote[i] = gameObject.GetComponent<DuplicateNotes>().FetchCollectionFromKeyIndex(i)[nextNoteIndex[i]];
                }
                else
                {
                    targetNote[i] = null;
                }
                if (targetNote[i] != null && Vector3.Distance(targetNote[i].transform.position, new Vector3(0, 0, 0)) < deleteRange)
                {

                    Debug.Log("DELETED -> DISTANCE : " + Vector3.Distance(targetNote[i].transform.position, new Vector3(0, 0, 0)));
                    MyUtilities.TweenDestroy(targetNote[i]);
                    DuplicateNotes.notesCount[i]--;
                    gameObject.GetComponent<DuplicateNotes>().FetchCollectionFromKeyIndex(i).Remove(nextNoteIndex[i]);
                    Debug.Log("DELETE -> TARGET : " + targetNote[i]);
                    ScoreStore.GetComponent<StoreScore>().AddScore(0);
                    nextNoteIndex[i]++;
                }
            }
        }
    }
}
