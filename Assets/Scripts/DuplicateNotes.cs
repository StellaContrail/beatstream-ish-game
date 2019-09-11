using System.Collections.Generic;
using System.IO;
using UnityEngine;

// It does NOT seem to make any lag if we write inside or outside of this loop
// http://feather.cocolog-nifty.com/weblog/2010/05/post-a00a.html
// but I *DO* think that gameobject variable substitution or something actually affects performance lag,
// so little used I utmost.

// This class creates notes on the ground from the timings data
public class DuplicateNotes : MonoBehaviour {
    public GameObject IndicatorHandler;

    public GameObject ScoreStore;
    public float furthestDistance;
    public GameObject[] Buttons;
    public GameObject[] NotePrefab;
    public GameObject[] SimultaneousPrefab; // Prefabs used when there's another simultaneous notes on the other side
    private float velocity;

    // A jagged array to store the timings from csv file
    [System.NonSerialized] public float[][] timings = new float[4][];

    // A queue generic collection to handle notes on the screen
    [System.NonSerialized]
    public static Dictionary<int, GameObject> rightDictionary;
    public static Dictionary<int, GameObject> leftDictionary;
    public static Dictionary<int, GameObject> upDictionary;
    public static Dictionary<int, GameObject> downDictionary;
    public static int[] notesCount = new int[4];

    // How many note blocks has been processed + 1(next one) = noteNumber
    // So basically, this is just a index number for notes that's going to soon arrive.
    private int[] noteNumber;
    // Those are the representative letters for the numbers
    public static readonly int RIGHT_BUTTON = 0;
    public static readonly int LEFT_BUTTON = 1;
    public static readonly int UP_BUTTON = 2;
    public static readonly int DOWN_BUTTON = 3;
    ManageTime_Script TimeManagementClass;
    float[] initialOffset;

    public GameObject TimeManagement;

    [System.NonSerialized]
    public bool isDataAvailable;

    private void Start () {
        isDataAvailable = false;
    }

    public Dictionary<int, GameObject> FetchCollectionFromKeyIndex (int keyIndex) {
        switch (keyIndex) {
            case 0:
                return rightDictionary;
            case 1:
                return leftDictionary;
            case 2:
                return upDictionary;
            case 3:
                return downDictionary;
            default:
                return null;
        }
    }

    // Use this for initialization
    public bool LoadNotesData () {
        //class that manages startupTime, musicTime
        TimeManagementClass = TimeManagement.GetComponent<ManageTime_Script> ();

        //These offsets are set so that they don't keep going until they hit center point
        initialOffset = new float[4];
        initialOffset[RIGHT_BUTTON] = Buttons[RIGHT_BUTTON].transform.position.x + Buttons[RIGHT_BUTTON].transform.localScale.x / 2;
        initialOffset[LEFT_BUTTON] = Buttons[LEFT_BUTTON].transform.position.x - Buttons[LEFT_BUTTON].transform.localScale.x / 2;
        initialOffset[UP_BUTTON] = Buttons[UP_BUTTON].transform.position.z + Buttons[UP_BUTTON].transform.localScale.z / 2;
        initialOffset[DOWN_BUTTON] = Buttons[DOWN_BUTTON].transform.position.z - Buttons[DOWN_BUTTON].transform.localScale.z / 2;

        //initialize -> note index in each side
        noteNumber = new int[4];
        // all notes velocities should be the same due to accessibility
        velocity = gameObject.GetComponent<NotesSettingsStore> ().velocity;

        //these arrays are used as timing storage
        timings[RIGHT_BUTTON] = new float[1024];
        timings[LEFT_BUTTON] = new float[1024];
        timings[UP_BUTTON] = new float[1024];
        timings[DOWN_BUTTON] = new float[1024];

        rightDictionary = new Dictionary<int, GameObject> ();
        leftDictionary = new Dictionary<int, GameObject> ();
        upDictionary = new Dictionary<int, GameObject> ();
        downDictionary = new Dictionary<int, GameObject> ();

        //path to the sheet
        string notesPath = "";
        try {
            // Red CSV File and import data into those variables
            notesPath = SongsLoader.folderNames[SongsLoader.presentMainSongIndex] + "/sheet";
        } catch {
            //this is for debug mode
            notesPath = "Road_To_Heaven/sheet";
        }
        TextAsset sheetText = Resources.Load (notesPath) as TextAsset;
        StringReader sr = new StringReader (sheetText.text);
        //row is temporary variable for note index
        int row = 0;
        //Read until there is nothing to read
        while (sr.Peek () > -1) {
            //line_timings is analyzed results from sheet, whose one line is like [RIGHTSIDE],[LEFTSIDE],[UPSIDE].[DOWNSIDE]
            string lineString = sr.ReadLine ();
            string[] line_timings = lineString.Split (',');
            //column is for key index
            for (int column = 0; column < 4; column++) {
                //if a timing exits
                if (line_timings[column] != "") {
                    //substitute timing value
                    timings[column][row] = System.Convert.ToSingle (line_timings[column]);
                } else {
                    //timing of 0.0f is ignored which means nothing comes afterward in the side(column)
                    timings[column][row] = 0.0f;
                }
            }
            //after reading one line, increase temp index(row)
            row++;
        }
        //Tell Update that data is available
        isDataAvailable = true;

        return true;
    }

    float oldDifferentialTime = 0f;
    // Update is called once per frame
    void Update () {
        if (isDataAvailable) {
            for (int i = 0; i < 4; i++) {
                float differentialTime = timings[i][noteNumber[i]] - ManageTime_Script.spentTime;
                if (0f < ManageTime_Script.spentTime && (0f < differentialTime && differentialTime < 5f || notesCount[i] > 0)) {
                    IndicatorHandler.GetComponent<IndicatorsHandler> ().SetActive (i, true);
                } else {
                    IndicatorHandler.GetComponent<IndicatorsHandler> ().SetActive (i, false);
                }
            }
        }

        //if data is fully loaded
        if (isDataAvailable) {
            //check for all sides
            for (int i = 0; i < 4; i++) {
                // Adopted *RenderDistance*
                //Interval of musicTime and timing
                float differentialTime = timings[i][noteNumber[i]] - ManageTime_Script.spentTime; //timings[i] => 
                // if there're more notes coming in the side && the render distance of the note is within the furthestDistance 
                if (noteNumber[i] < timings[i].Length && (differentialTime * velocity) <= furthestDistance) {
                    // Whether the timing data isn't to be ignored
                    if (!Mathf.Approximately (timings[i][noteNumber[i]], 0.0f)) {
                        GameObject[] prefabs = new GameObject[4];

                        // When there's another note with same timing on the other lines,
                        if (Mathf.Approximately (oldDifferentialTime, differentialTime)) {
                            // Set prefabs as SimultaneousPrefab which is prepared for notes prefab to express simultaneousness
                            prefabs = SimultaneousPrefab;
                        } else {
                            // If there's only one note or first note even there's another note with same timing, 
                            // Set prefabs as NotePrefab
                            prefabs = NotePrefab;
                        }
                        oldDifferentialTime = differentialTime;

                        //declare here so we can tickle this variable afterward
                        GameObject gameObj = null;
                        switch (i) {
                            case 0: // RIGHT
                                //Instantiate note
                                gameObj = Instantiate (prefabs[RIGHT_BUTTON], new Vector3 ((differentialTime * velocity) + initialOffset[RIGHT_BUTTON], 0.5f, 0), Quaternion.identity);
                                //Move the note toward the center
                                iTween.MoveTo (gameObj, iTween.Hash (
                                    "position", new Vector3 (0, 0, 0),
                                    "easetype", "linear",
                                    "time", differentialTime + initialOffset[RIGHT_BUTTON] / velocity
                                ));
                                rightDictionary.Add (noteNumber[i], gameObj);
                                notesCount[0]++;
                                break;
                            case 1: // LEFT
                                gameObj = Instantiate (prefabs[LEFT_BUTTON], new Vector3 (-(differentialTime * velocity) + initialOffset[LEFT_BUTTON], 0.5f, 0), Quaternion.identity);
                                iTween.MoveTo (gameObj, iTween.Hash (
                                    "position", new Vector3 (0, 0, 0),
                                    "easetype", "linear",
                                    "time", differentialTime - initialOffset[LEFT_BUTTON] / velocity
                                ));
                                leftDictionary.Add (noteNumber[i], gameObj);
                                notesCount[1]++;
                                break;
                            case 2: // UP
                                gameObj = Instantiate (prefabs[UP_BUTTON], new Vector3 (0, 0.5f, (differentialTime * velocity) + initialOffset[UP_BUTTON]), Quaternion.identity);
                                iTween.MoveTo (gameObj, iTween.Hash (
                                    "position", new Vector3 (0, 0, 0),
                                    "easetype", "linear",
                                    "time", differentialTime + initialOffset[UP_BUTTON] / velocity
                                ));
                                upDictionary.Add (noteNumber[i], gameObj);
                                notesCount[2]++;
                                break;
                            case 3: // DOWN
                                gameObj = Instantiate (prefabs[DOWN_BUTTON], new Vector3 (0, 0.5f, -(differentialTime * velocity) + initialOffset[DOWN_BUTTON]), Quaternion.identity);
                                iTween.MoveTo (gameObj, iTween.Hash (
                                    "position", new Vector3 (0, 0, 0),
                                    "easetype", "linear",
                                    "time", differentialTime - initialOffset[DOWN_BUTTON] / velocity
                                ));
                                downDictionary.Add (noteNumber[i], gameObj);
                                notesCount[3]++;
                                break;
                        }

                        //Set the note the child of this object to tidy up
                        gameObj.transform.parent = transform;
                        // if successfully instantiated note, increase noteNumber[i] so that next note of the button is to be set.
                        noteNumber[i]++;
                    }

                }
            }
        }
    }
}