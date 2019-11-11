using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;

public class LookingAt : MonoBehaviour
{
    private FocusInfo FocusInfo;
    private readonly float MaxDistance = 20;
    private readonly GazeIndex[] GazePriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };


    void Start()
    {
        if (!SRanipal_Eye_Framework.Instance.EnableEye)
        {
            enabled = false;
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
            SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

        foreach (GazeIndex index in GazePriority)
        {
            Ray GazeRay;
            if (SRanipal_Eye.Focus(index, out GazeRay, out FocusInfo, MaxDistance))
            {
                transform.position = FocusInfo.point;
                Debug.Log(FocusInfo.point);
                break;
            }
        }

    }
}
