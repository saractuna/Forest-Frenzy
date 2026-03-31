using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MuteAudio : MonoBehaviour
{
    [SerializeField] Toggle muteToogle;
    public void MuteToggle()
    {
        if (muteToogle.isOn)
        {
            AudioListener.volume = 0;
        }
        else
        {
            AudioListener.volume = 1;
        }
    }
}
