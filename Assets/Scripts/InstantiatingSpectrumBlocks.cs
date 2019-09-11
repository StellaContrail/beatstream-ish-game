using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatingSpectrumBlocks : MonoBehaviour
{
    [SerializeField, Tooltip("Used for volume levels")]
    public GameObject AudioObject;
    public GameObject bandPrefab;
    public int bandCount = 3;
    [SerializeField, Tooltip("Margin width ratio to volume level's")]
    public float multiply = 0.5f;
    [HideInInspector]
    //instantiated cubes are stored in this array from left to right
    public GameObject[] bandBars;

    // Use this for initialization
    void Start()
    {
        bandBars = new GameObject[bandCount];
        Vector3 middlePosition = gameObject.transform.position;
        Vector3 leftPosition = middlePosition - new Vector3(gameObject.transform.lossyScale.x / 2, 0, 0);
        Vector3 rightPosition = middlePosition + new Vector3(gameObject.transform.lossyScale.x / 2, 0, 0);
        float width = rightPosition.x - leftPosition.x;
        float thickness = width / (multiply * (bandCount - 1) + bandCount);

        GameObject spectrumBars = new GameObject("spectrumBars");
        spectrumBars.transform.localPosition = middlePosition - new Vector3(0, 0, gameObject.transform.localScale.z / 2);
        for (int i = 0; i < bandCount; i++)
        {
            //leftPosition += new Vector3((thickness + thickness * multiply) * i, 0, 0);
            GameObject bar = Instantiate(bandPrefab, leftPosition, Quaternion.identity, spectrumBars.transform);
            leftPosition += new Vector3(thickness * (1f + multiply), 0, 0);
            bar.transform.localScale = new Vector3(thickness, 1, 1);
            bandBars[i] = bar;
        }
    }
}
