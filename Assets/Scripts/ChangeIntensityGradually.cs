using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class will change its emission intensity
public class ChangeIntensityGradually : MonoBehaviour
{
    //Material attached to GameObject already
    Material m;

    //Color will pingpong between these values
    public Color start_color;
    public Color end_color;

    //this shows how value changes at present
    float rate = 0.0f;      //0f -> 1f:start -> end
    bool changing = false;  //if intensity is changing
    float duration;         //how long does it take to change from start to end


    // Use this for initialization
    void Start()
    {
        m = gameObject.GetComponent<Renderer>().material;
    }

    public void ChangeChange()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
