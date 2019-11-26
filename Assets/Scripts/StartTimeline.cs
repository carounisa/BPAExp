using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StartTimeline : MonoBehaviour
{
    [SerializeField] PlayableDirector _timeline;
    [SerializeField] AudioSource _audioSource;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Timeline and Audio start playing now");
            _audioSource.Play();
            _timeline.Play();
        }
    }


}
