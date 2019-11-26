using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StartTimeline : MonoBehaviour
{
    [SerializeField] PlayableDirector _timeline;
    [SerializeField] AudioSource _audioSource;
    private const int finish = 3;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Timeline and Audio start playing now");
            _timeline.Play();
            StartCoroutine(startAudio());
        }
    }

    public IEnumerator startAudio()
    {
        _audioSource.Play();
        yield return new WaitForSeconds(_audioSource.clip.length + finish);
        Application.Quit();
    }


}
