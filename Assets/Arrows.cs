using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Arrows : MonoBehaviour
{

    [SerializeField]
    private float _fadeDuration = 0.5f;
    [SerializeField]
    private float _showAngle = 0.6f;
    [SerializeField]
    private Transform directionToLook;
    [SerializeField]
    private Transform hmdTransform;
    [SerializeField]
    private Renderer[] arrowRenderer;

    private float _currentAlpha;
    private float _targetAlpha;
    private float _fadeSpeed;
    private string material = "_Alpha";
    private float _dotProduct;

    // Start is called before the first frame update
    void Start()
    {
        _fadeSpeed = 1f / _fadeDuration;
        hmdTransform = Player.instance.hmdTransform;
    }

    // Update is called once per frame
    void Update()
    {
        _dotProduct = Vector3.Dot(hmdTransform.forward, (directionToLook.position - transform.position).normalized);

        _targetAlpha = _dotProduct < _showAngle ? 1f : 0f;

        _currentAlpha = Mathf.MoveTowards(_currentAlpha, _targetAlpha, _fadeSpeed * Time.deltaTime);

        for(int i = 0; i < arrowRenderer.Length; i++)
        {
            arrowRenderer[i].material.SetFloat(material, _currentAlpha);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show(Transform lookThisWay)
    {
        directionToLook = lookThisWay;
        transform.LookAt(lookThisWay);
        gameObject.SetActive(true);
    }
}
