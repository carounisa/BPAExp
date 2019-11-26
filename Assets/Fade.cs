using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{

    private float _currentAlpha = 0f;
    private string material = "_Alpha";

    public void Start()
    {
        gameObject.GetComponent<Renderer>().material.SetFloat(material, _currentAlpha);
    }

    public void FadeInAndOut()
    {
        StopAllCoroutines();
        StartCoroutine(Fader());
    }

    private IEnumerator Fader()
    {
        float _duration = 2f;
        float _fadeSpeed = 1f / _duration;
        float _targetAlpha = _currentAlpha < 1f ? 1f : 0f;
        float _counter = 0f;

        while (_counter < _duration)
        {
            _counter += Time.deltaTime / _duration;
            _currentAlpha = Mathf.Lerp(_currentAlpha, _targetAlpha, _counter);
            gameObject.GetComponent<Renderer>().material.SetFloat(material, _currentAlpha);
            yield return null;
        } 
    }
}
