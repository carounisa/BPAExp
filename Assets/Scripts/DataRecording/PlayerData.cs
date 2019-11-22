using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int pNumber;
    public string condition;
    public List<AudioTime> audioTimeList;
    public List<string> timeStampList;
    public List<HeadData> headDataList;
    public List<Regions> regionList;
    public List<MeshEyeData> meshEyeDataList;
    public List<TimePassed> timePassedList;

    [System.Serializable]
    public class HeadData
    {
        public Vector3 headPosition;
        public Vector3 direction;
    }

    [System.Serializable]
    public class Regions
    {
        public string name;
        public string startTime;
        public string endTime;
        public string elapsedTime;
    }

    [System.Serializable]
    public class MeshEyeData
    {
        public Vector3 hitPoint;
        public Vector3 direction;
        public float distanceToPlayer;
    }

    [System.Serializable]
    public class TimePassed
    {
        public string regionName;
        public double totalTimePassed;
    }

    [System.Serializable]
    public class AudioTime
    {
        public float audioTime;
        public enum WhereToLook {
            WINDOWWALL,
            SPATTERWALL,
            SPATTER_PINK,
            SPATTER_BLUE,
            CHAIR, WINDOW,
            ORIGIN_PINK,
            ORIGIN_BLUE }
    }



}
