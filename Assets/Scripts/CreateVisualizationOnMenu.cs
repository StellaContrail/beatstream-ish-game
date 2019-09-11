using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CreateVisualizationOnMenu : MonoBehaviour
{
    public TextMesh debugText;

    public float limitHeight;
    public float enhancementBaseNum = 1.5f;
    public GameObject InstantiateHandler;
    AudioSource _audioSource;
    int audioResolution = 1024;
    float[] leftSamples;
    float[] rightSamples;
    float baseHeight;
    int bandCount;
    InstantiatingSpectrumBlocks InstantiateSystem;
    float[] highestBandLevels;
    float[] newBandLevelBuffers;
    float[] oldBandLevelBuffers;
    float[] bufferedBandLevels;
    public float initialHighestLevel;
    public float initialBufferLevel;

    private void Start()
    {
        InstantiateSystem = InstantiateHandler.GetComponent<InstantiatingSpectrumBlocks>();

        leftSamples = new float[audioResolution];
        rightSamples = new float[audioResolution];
        bandCount = InstantiateSystem.bandBars.Length;
        highestBandLevels = new float[bandCount];
        highestBandLevels = highestBandLevels.Select(x => x = initialHighestLevel).ToArray<float>();
        newBandLevelBuffers = new float[bandCount];
        oldBandLevelBuffers = new float[bandCount];
        oldBandLevelBuffers = oldBandLevelBuffers.Select(x => x = initialBufferLevel).ToArray<float>();
        bufferedBandLevels = new float[bandCount];
        _audioSource = gameObject.GetComponent<AudioSource>();
        baseHeight = InstantiateSystem.transform.localPosition.z;
        Destroy(InstantiateHandler);
    }

    // Update is called once per frame
    void Update()
    {
        //initialize values for new song
        if (SongsLoader.IsSongChanged)
        {
            highestBandLevels = new float[bandCount];
            highestBandLevels = highestBandLevels.Select(x => x = initialHighestLevel).ToArray<float>();
            newBandLevelBuffers = new float[bandCount];
            oldBandLevelBuffers = new float[bandCount];
            oldBandLevelBuffers = oldBandLevelBuffers.Select(x => x = initialBufferLevel).ToArray<float>();
            bufferedBandLevels = new float[bandCount];
        }

        //About FFTWindow, read here -> http://www.ni.com/white-paper/4844/ja/
        _audioSource.GetSpectrumData(leftSamples, 0, FFTWindow.BlackmanHarris);
        _audioSource.GetSpectrumData(rightSamples, 1, FFTWindow.BlackmanHarris);
        /*
         * all degital data converted from analog data were stored in _samples one by one.
         * they are sorted by how big each datum's frequency is
         * Well, now we want three blocks to show how audio volume varies,
         * so we will divide all frequency level by three.
         */

        float deltaFreq = AudioSettings.outputSampleRate / audioResolution;
        float deltaThresholdFreq = AudioSettings.outputSampleRate / bandCount;
        float[] bandLevels = new float[bandCount];
        for (int i = 0; i < audioResolution; i++)
        {
            for (int j = 0; j < bandCount; j++)
            {
                if (deltaThresholdFreq * j < deltaFreq * i && deltaFreq * i < deltaThresholdFreq * (j + 1))
                {
                    bandLevels[j] += leftSamples[i];
                    bandLevels[j] += rightSamples[i];
                }
            }
        }

        //range outputs in 0 - 1
        for (int i = 0; i < bandCount; i++)
        {
            if (Mathf.Approximately(highestBandLevels[i], 0))
            {
                highestBandLevels[i] = bandLevels[i];
                bandLevels[i] = 0f;
            }
            else
            {
                if (bandLevels[i] > highestBandLevels[i])
                {
                    highestBandLevels[i] = bandLevels[i];
                }
                bandLevels[i] /= highestBandLevels[i];
                if (bandLevels[i] > 0.9f)
                {
                    highestBandLevels[i] += 0.1f;
                    bandLevels[i] -= 0.1f;
                }
            }
        }

        //buffer volume levels
        for (int i = 0; i < bandCount; i++)
        {
            newBandLevelBuffers[i] = bandLevels[i];
            if (newBandLevelBuffers[i] > oldBandLevelBuffers[i])
            {
                bufferedBandLevels[i] = newBandLevelBuffers[i];
            }
            if (newBandLevelBuffers[i] < oldBandLevelBuffers[i])
            {
                float decreasingLevel = 0.03f;
                bufferedBandLevels[i] -= decreasingLevel;
                if (bufferedBandLevels[i] < 0f)
                {
                    bufferedBandLevels[i] = 0f;
                }
            }
            oldBandLevelBuffers[i] = bufferedBandLevels[i];

        }


        for (int i = 0; i < bandCount; i++)
        {
            Vector3 barScale = InstantiateSystem.bandBars[i].transform.localScale;
            //barScale.z = volumeLevels[i] * Mathf.Pow(enhancementBaseNum, (i + 1));
            barScale.z = bufferedBandLevels[i] * (enhancementBaseNum + i);
            if (barScale.z > limitHeight)
            {
                barScale.z = limitHeight;
            }
            InstantiateSystem.bandBars[i].transform.localScale = barScale;

            Vector3 barPos = InstantiateSystem.bandBars[i].transform.localPosition;
            barPos.z = barScale.z / 2;
            InstantiateSystem.bandBars[i].transform.localPosition = barPos;
        }
    }
}
