using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class DataHandler : MonoBehaviour
{
    [HideInInspector]
    public PlayerData playerData;
    [HideInInspector]
    public PlayerData.Regions regionData;
    private PlayerData.MeshEyeData eyeData;
    private PlayerData.HeadData _head;
    private PlayerData.TimePassed _totalTimePassed;
    private Stopwatch _stopwatch;
    private string _logFile;
    private string _logFilePath;
    private string _logDir;
    private string _previousRegion;
    public Dictionary<string, double> _regionTable;


    private float _interval = 1f;
    private float _currentTime = 0f;
    private bool _isRecording = true;

    private GameObject _target;


    private static DataHandler _instance;
    public static DataHandler instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DataHandler>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (PlayerPrefs.GetInt("Recording") == 1)
            _isRecording = true;

        UnityEngine.Debug.Log(_isRecording);

        _logDir = Path.Combine(Application.dataPath, "PlayerRecordings");

        if (!Directory.Exists(_logDir))
            Directory.CreateDirectory(_logDir);

        playerData = new PlayerData();
        playerData.headDataList = new List<PlayerData.HeadData>();
        playerData.regionList = new List<PlayerData.Regions>();
        playerData.meshEyeDataList = new List<PlayerData.MeshEyeData>();
        playerData.timeStampList = new List<string>();
        playerData.timePassedList = new List<PlayerData.TimePassed>();
        playerData.pNumber = PlayerPrefs.GetInt("Participant Number");
        playerData.condition = PlayerPrefs.GetString("Condition");

        _regionTable = new Dictionary<string, double>();

        _logFile = string.Format("log{0}-PNum{1}_Con_{2}.json",
            System.DateTime.Now.ToString("dd-MM-yyyy"),
            playerData.pNumber, playerData.condition);
        _logFilePath = Path.Combine(_logDir, _logFile);

        _stopwatch = new Stopwatch();
        _head = new PlayerData.HeadData();
        regionData = new PlayerData.Regions();
        eyeData = new PlayerData.MeshEyeData();

        UnityEngine.Debug.Log("Log files stored to: " + _logDir);

    }

    private void Update()
    {
        if (_isRecording)
        {
            _currentTime += Time.deltaTime;
            // record every second
            if (_currentTime >= _interval)
            {
                if (playerData.condition.Equals("HitAndRun"))
                {
                    _head = new PlayerData.HeadData();
                    _head.headPosition = Player.instance.hmdTransform.position;
                    _head.direction = Player.instance.hmdTransform.forward;
                    playerData.headDataList.Add(_head);
                    playerData.timeStampList.Add(string.Format("{0}:{1}:{2}:{3}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond));
                }
                _currentTime = _currentTime % _interval;
            }
        }
    }

    public void startTimer()
    {
        _stopwatch.Reset();
        _stopwatch.Start();
    }

    public void stopTimer()
    {
        _stopwatch.Stop();
        System.TimeSpan elapsed = _stopwatch.Elapsed;

        regionData.elapsedTime = string.Format("{0:00}:{1:00}:{2:00}", elapsed.Minutes, elapsed.Seconds, elapsed.Milliseconds);
       
        if (!_regionTable.ContainsKey(_previousRegion))
        {
            _regionTable.Add(_previousRegion, 0);
        } else
        {
            _regionTable[_previousRegion] += _stopwatch.Elapsed.TotalSeconds;
        }


    }

    public bool isWatchRunning()
    {
        return _stopwatch.IsRunning;
    }

    public void recordGazeTime(string name)
    {
        String currentRegion = name;
        if(!currentRegion.Equals(_previousRegion))
        {
            if(isWatchRunning())
                endRecordingEvidence();

            regionData = new PlayerData.Regions();
            regionData.name = name;
            regionData.startTime = string.Format("{0}:{1}:{2}:{3}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
            startTimer();

            _previousRegion = currentRegion;
        } 

    }

    public void endRecordingEvidence()
    {
        regionData.endTime = string.Format("{0}:{1}:{2}:{3}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
        playerData.regionList.Add(DataHandler.instance.regionData);
        stopTimer();
    }


    public void collectMeshTrackingData(Vector3 hitPoint, float distance, Vector3 rayDirection)
    {
        eyeData = new PlayerData.MeshEyeData();
        eyeData.hitPoint = hitPoint;
        eyeData.distanceToPlayer = distance;
        eyeData.direction = rayDirection;
        playerData.meshEyeDataList.Add(eyeData);
    }

    private void WriteToFile()
    {
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(_logFilePath, json);
    }

    private void OnApplicationQuit()
    {
        DataHandler.instance.endRecordingEvidence();
        foreach (KeyValuePair<string, double> entry in _regionTable)
        {
            _totalTimePassed = new PlayerData.TimePassed();
            _totalTimePassed.regionName = entry.Key;
            _totalTimePassed.totalTimePassed = entry.Value;
            playerData.timePassedList.Add(_totalTimePassed);
            UnityEngine.Debug.Log(entry.Key + " " + entry.Value);
        }


        if (_isRecording)
        {
            UnityEngine.Debug.Log("Exiting application and writing to file.");
            WriteToFile();
        }
    }
}
