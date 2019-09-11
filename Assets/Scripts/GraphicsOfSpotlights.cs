using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GraphicsOfSpotlights : MonoBehaviour
{
    private float[] waveData = new float[1024];
    public GameObject[] spotLights = new GameObject[4];
    private float[] _angle = new float[4];
    public float enhancement;

    public int resolution = 1024;
    public float lowFreqThreshold = 14700, midFreqThreshold = 29400, highFreqThreshold = 44100;
    public float lowEnhance = 1f, midEnhance = 10f, highEnhance = 100f;

    public GameObject MusicPlayer;


    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            _angle[i] = spotLights[i].GetComponent<Light>().spotAngle;
        }
    }

    // Update is called once per frame
    void Update()
    {
        AudioListener.GetOutputData(waveData, 1);
        float volume = waveData.Select(x => x * x).Sum() / waveData.Length;
        for (int i = 0; i < 4; i++)
        {
            if (1 < enhancement * volume)
            {
                spotLights[i].GetComponent<Light>().spotAngle = _angle[i] * enhancement * volume;
            }
        }
        
        float[] spectrum = new float[resolution];
        MusicPlayer.GetComponent<AudioSource>().GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        

        var deltaFreq = AudioSettings.outputSampleRate / resolution;
        float low = 0f, mid = 0f, high = 0f;

        for (var i = 0; i < resolution; ++i)
        {
            var freq = deltaFreq * i;
            if (freq <= lowFreqThreshold) low += spectrum[i];
            else if (freq <= midFreqThreshold) mid += spectrum[i];
            else if (freq <= highFreqThreshold) high += spectrum[i];
        }

        low *= lowEnhance;
        mid *= midEnhance;
        high *= highEnhance;
        /*
        spotLights[0].GetComponent<Light>().color = new Color32((byte)(255 * low), (byte)(255 * low), 0, 0);
        spotLights[1].GetComponent<Light>().color = new Color32(0, 0, (byte)(255 * mid), 0);
        spotLights[2].GetComponent<Light>().color = new Color32((byte)(255 * high), 0, 0, 0);
        spotLights[3].GetComponent<Light>().color = new Color32(0, (byte)(255 * (high + mid + low) / 3), 0, 0);
        */
        for (int i = 0; i < 4; i++)
        {
            //spotLights[i].GetComponent<Light>().color = new Color32((byte)(255 * high), (byte)(255 * mid), (byte)(255 * low), 0);
            byte _color = (byte)(255 * (high + mid + low) / 3);
            spotLights[i].GetComponent<Light>().color = new Color32(_color, _color, _color, 0);
        }

    }
}
