using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class DataHandler : MonoBehaviour
{

    public AudioSource audioSource;
    [HideInInspector]
    public PlayerData playerData;
    [HideInInspector]
    private PlayerData.Regions _regionData;
    private PlayerData.MeshEyeData _eyeData;
    private PlayerData.HeadData _head;
    private PlayerData.TimePassed _totalTimePassed;
    private PlayerData.AudioTime _audioTime;

    private Stopwatch _stopwatch;
    private string _logFile;
    private string _logFilePath;
    private string _logDir;
    private string _previousRegion;
    private Dictionary<string, double> _regionTable;
    private int _pNumber;
    private string _condition;


    private float _interval = 1f;
    private float _currentTime = 0f;
    private bool _isRecording;
    private bool _isRecordingGaze;


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
        playerData.audioTimeList = new List<PlayerData.AudioTime>();

        _pNumber = PlayerPrefs.GetInt("Participant Number");
        UnityEngine.Debug.Log(_pNumber);
        _condition = PlayerPrefs.GetString("Condition");

        _regionTable = new Dictionary<string, double>();

        _logFile = string.Format("log{0}-PNum{1}_Con_{2}.json",
            System.DateTime.Now.ToString("dd-MM-yyyy"),
            _pNumber, _condition);
        _logFilePath = Path.Combine(_logDir, _logFile);

        _stopwatch = new Stopwatch();
        _head = new PlayerData.HeadData();
        _regionData = new PlayerData.Regions();
        _eyeData = new PlayerData.MeshEyeData();

         _isRecordingGaze = false;


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
            //    if (playerData.condition.Equals("BPAExperimentVR"))
             //   {
                    _head = new PlayerData.HeadData();
                    _head.headPosition = Player.instance.hmdTransform.position;
                    _head.direction = Player.instance.hmdTransform.forward;
                    playerData.headDataList.Add(_head);
                    playerData.timeStampList.Add(string.Format("{0}:{1}:{2}:{3}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond));

                    if (audioSource.isPlaying)
                    {
                        _audioTime = new PlayerData.AudioTime();
                        _audioTime.audioTime = audioSource.time;
                        playerData.audioTimeList.Add(_audioTime);
                    }
              //  }
              //  }
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

        _regionData.elapsedTime = string.Format("{0:00}:{1:00}:{2:00}", elapsed.Minutes, elapsed.Seconds, elapsed.Milliseconds);

        if ((_previousRegion != null) && (!_regionTable.ContainsKey(_previousRegion)))
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

            _regionData = new PlayerData.Regions();
            _regionData.name = name;
            _regionData.startTime = string.Format("{0}:{1}:{2}:{3}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
            startTimer();

            _previousRegion = currentRegion;
        }

        _isRecordingGaze = true;
    }

    public void endRecordingEvidence()
    {
        if (_isRecordingGaze)
        {
            _regionData.endTime = string.Format("{0}:{1}:{2}:{3}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
            playerData.regionList.Add(DataHandler.instance._regionData);
            stopTimer();
        }
    }


    public void collectMeshTrackingData(Vector3 hitPoint, float distance, Vector3 rayDirection)
    {
        _eyeData = new PlayerData.MeshEyeData();
        _eyeData.hitPoint = hitPoint;
        _eyeData.distanceToPlayer = distance;
        _eyeData.direction = rayDirection;
        playerData.meshEyeDataList.Add(_eyeData);
    }

    private void WriteToFile()
    {
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(_logFilePath, json);
    }

    private void OnApplicationQuit()
    {

        DataHandler.instance.endRecordingEvidence();

        if (_isRecording)
        {
            foreach (KeyValuePair<string, double> entry in _regionTable)
            {
                _totalTimePassed = new PlayerData.TimePassed();
                _totalTimePassed.regionName = entry.Key;
                _totalTimePassed.totalTimePassed = entry.Value;
                playerData.timePassedList.Add(_totalTimePassed);
            }

            UnityEngine.Debug.Log("Exiting application and writing to file.");
            WriteToFile();
        }
    }
}
