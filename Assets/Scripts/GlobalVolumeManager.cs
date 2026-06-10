using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GlobalVolumeManager : MonoBehaviour
{
    public static GlobalVolumeManager Instance { get; private set; }
    public Volume globalVolume;

    private void Awake()
    {
        Instance = this;
    }

    private void Dosomething()
    {
        //globalVolume.profile.TryGet(out )
    }
}
