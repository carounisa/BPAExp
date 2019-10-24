using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloudHighlightController : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    [SerializeField] float _hilightOpacity = 0.25f;

    [Space(5)]
    [SerializeField] Transform _highlight1Transform;
    [SerializeField] Transform _highlight2Transform;


    Material _sharedMaterial = null;

    void FindSharedMaterial()
    {
        foreach (var obj in FindObjectsOfType<MeshRenderer>())
        {
            if (obj.name.Contains("bpa"))
            {
                _sharedMaterial = obj.sharedMaterial;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_sharedMaterial == null)
        {
            FindSharedMaterial();
        }

        if (_sharedMaterial != null)
        {
            _sharedMaterial.SetVector("_highlight1Position", _highlight1Transform.position);
            _sharedMaterial.SetVector("_highlight1Size", _highlight1Transform.localScale);
            _sharedMaterial.SetFloat("_hilightOpacity", _hilightOpacity);

            _sharedMaterial.SetVector("_highlight2Position", _highlight2Transform.position);
            _sharedMaterial.SetVector("_highlight2Size", _highlight2Transform.localScale);
        }

    }
}
