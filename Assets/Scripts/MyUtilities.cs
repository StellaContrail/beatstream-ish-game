using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyUtilities : MonoBehaviour
{
    //Releasing iTween's animation and destroy gameobject
    //because iTween keeps animating even though it's destroyed
    public static void TweenDestroy(GameObject _target)
    {
        iTween.Stop(_target);
        Destroy(_target);
    }

    
    public class AudioTools
    {



    }
}


public static class Mathematics
{
    //Exceptional Inverse Number
    //if dividing by zero, it returns exOutput
    //if not, it returns the inversed number
    public static float ExInverse(this float _num, float exOutput)
    {
        return Mathf.Approximately(_num, 0) ? exOutput : 1 / _num;
    }
}
