using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StartTimeline : MonoBehaviour
{
    [SerializeField] PlayableDirector _timeline;
    [SerializeField] AudioSource _audioSource;

    private bool _hasPlayed = false;


    void Update()
    {
        if (!(_audioSource.isPlaying) && _hasPlayed == false)
        {
            _timeline.Play();
          StartCoroutine(startAudio());
        }
    }

    public IEnumerator startAudio()
    {
        Debug.Log("Timeline and Audio start playing now");
        _hasPlayed = true;
        _audioSource.Play();
        yield return new WaitForSeconds(_audioSource.clip.length);
        Debug.Log("Quitting application");
        Application.Quit();
    }



}
