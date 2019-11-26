using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.Extras
{
    public class Highlighter : MonoBehaviour
    {
        protected MeshRenderer[] highlightRenderers;
        protected MeshRenderer[] existingRenderers;
        protected GameObject highlightHolder;
        protected static Material highlightMat;

        private bool _isHighlighted;
        private float _duration = 2f;
        private float _highlightWidth;

        protected virtual void Start()
        {

            highlightMat = (Material)Resources.Load("SteamVR_HoverHighlight", typeof(Material));
            _highlightWidth = 0f;

            if (highlightMat == null)
                UnityEngine.Debug.LogError("<b>[SteamVR Interaction]</b> Hover Highlight Material is missing. Please create a material named 'SteamVR_HoverHighlight' and place it in a Resources folder");
        }

        public virtual void CreateHighlightRenderers()
        {
            if(highlightHolder == null)
                highlightHolder = new GameObject("Highlighter");

            MeshFilter[] existingFilters = this.GetComponentsInChildren<MeshFilter>(true);
            existingRenderers = new MeshRenderer[existingFilters.Length];
            highlightRenderers = new MeshRenderer[existingFilters.Length];

            for (int filterIndex = 0; filterIndex < existingFilters.Length; filterIndex++)
            {
                MeshFilter existingFilter = existingFilters[filterIndex];
                MeshRenderer existingRenderer = existingFilter.GetComponent<MeshRenderer>();

                if (existingFilter == null || existingRenderer == null)
                    continue;

                GameObject newFilterHolder = new GameObject("FilterHolder");
                newFilterHolder.transform.parent = highlightHolder.transform;
                MeshFilter newFilter = newFilterHolder.AddComponent<MeshFilter>();
                newFilter.sharedMesh = existingFilter.sharedMesh;
                MeshRenderer newRenderer = newFilterHolder.AddComponent<MeshRenderer>();

                Material[] materials = new Material[existingRenderer.sharedMaterials.Length];
                for (int materialIndex = 0; materialIndex < materials.Length; materialIndex++)
                {
                    materials[materialIndex] = highlightMat;
                }

                newRenderer.sharedMaterials = materials;

                highlightRenderers[filterIndex] = newRenderer;
                existingRenderers[filterIndex] = existingRenderer;

                RenderHighlight();

                StopAllCoroutines();
                StartCoroutine(GrowHighlight());

            }
        }

        private void RenderHighlight()
        {
            for (int rendererIndex = 0; rendererIndex < highlightRenderers.Length; rendererIndex++)
            {
                MeshRenderer existingRenderer = existingRenderers[rendererIndex];
                MeshRenderer highlightRenderer = highlightRenderers[rendererIndex];

                if (existingRenderer != null && highlightRenderer != null)
                {
                    highlightRenderer.transform.position = existingRenderer.transform.position;
                    highlightRenderer.transform.rotation = existingRenderer.transform.rotation;
                    highlightRenderer.transform.localScale = existingRenderer.transform.lossyScale;
                    highlightRenderer.enabled = existingRenderer.enabled && existingRenderer.gameObject.activeInHierarchy;

                }
                else if (highlightRenderer != null)
                    highlightRenderer.enabled = false;
            }
        }

        public virtual void OnDestroy()
        {
            if (highlightHolder != null) {
                Destroy(highlightHolder);
            }
        }

        private IEnumerator GrowHighlight()
        {
            if (_highlightWidth >= 0.0005f) _isHighlighted = true;

            float startValue = _highlightWidth;
            float endValue = _isHighlighted ?  0.000f : 0.001f;
            float counter = 0f;

            while (counter < _duration )
            {
                counter += Time.deltaTime / _duration;
                _highlightWidth = Mathf.Lerp(startValue, endValue, counter);
                highlightMat.SetFloat("g_flOutlineWidth", _highlightWidth);
                yield return null;
            }
        }
    }
}



