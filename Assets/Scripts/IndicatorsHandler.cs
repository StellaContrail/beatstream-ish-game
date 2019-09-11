using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorsHandler : MonoBehaviour {
    
    public GameObject rightIndicator;
    public GameObject leftIndicator;
    public GameObject upIndicator;
    public GameObject downIndicator;

    public void SetActive(int keyIndex, bool _setting)
    {
        GameObject _target = null;
        switch(keyIndex)
        {
            case 0:
                _target = rightIndicator;
                break;
            case 1:
                _target = leftIndicator;
                break;
            case 2:
                _target = upIndicator;
                break;
            case 3:
                _target = downIndicator;
                break;
        }
        _target.SetActive(_setting);
    }

}