using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.Extras
{
    public class Highlighter : MonoBehaviour
    {

        [Tooltip("Set whether or not you want this interactible to highlight when hovering over it")]
        public bool highlightOnHover = true;
        protected MeshRenderer[] highlightRenderers;
        protected MeshRenderer[] existingRenderers;
        protected GameObject highlightHolder;
        protected static Material highlightMat;

        private bool _isHighlighted;
        private float _duration = 2f;



        //public bool isDestroying { get; protected set; }
       // public bool isHovering { get; protected set; }
       // public bool wasHovering { get; protected set; }



        protected virtual void Start()
        {

            highlightMat = (Material)Resources.Load("SteamVR_HoverHighlight", typeof(Material));

            if (highlightMat == null)
                UnityEngine.Debug.LogError("<b>[SteamVR Interaction]</b> Hover Highlight Material is missing. Please create a material named 'SteamVR_HoverHighlight' and place it in a Resources folder");

            CreateHighlightRenderers();
        }

        public virtual void CreateHighlightRenderers()
        {
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

                StopAllCoroutines();
                Debug.Log("enter coroutine");
                StartCoroutine(GrowHighlight());

            }
        }

        protected virtual void UpdateHighlightRenderers()
        {
            if (highlightHolder == null)
                return;

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

                    //highlightRenderer.enabled = isHovering && existingRenderer.enabled && existingRenderer.gameObject.activeInHierarchy;
                }
                else if (highlightRenderer != null)
                    highlightRenderer.enabled = false;
            }
        }

        /// <summary>
        /// Called when the laser pointer points at this object
        /// </summary>
        protected void OnPoint(object sender, PointerEventArgs e)
        {
           // wasHovering = isHovering;
           // isHovering = true;

            if (highlightOnHover == true && e.target == transform)
            {
                
                //MarkUI.UpdateUI(heading, image1, image2, transform);
                //MarkUI.ShowUI();
                CreateHighlightRenderers();
                UpdateHighlightRenderers();
            }
        }

        /// <summary>
        /// Called when the laser pointer pulls the trigger on this object
        /// </summary>
        protected void OnPointClick(object sender, PointerEventArgs e)
        {



        }


        /// <summary>
        /// Called when a the laser pointer stops pointing at this object
        /// </summary>
        private void OnPointOut(object sender, PointerEventArgs e)
        {
           // wasHovering = isHovering;
           // isHovering = false;

            //if (highlightOnHover && highlightHolder != null)
            //    MarkUI.HideUI();
            Destroy(highlightHolder);
        }

        protected virtual void Update()
        {
            //  if (highlightOnHover)
            //   {
                UpdateHighlightRenderers();

         //       if (isHovering == false && highlightHolder != null)
         //           Destroy(highlightHolder);
         //   }
        }

        public virtual void OnDestroy()
        {
           // isDestroying = true;

            if (highlightHolder != null)
                Destroy(highlightHolder);

        }


        protected virtual void OnDisable()
        {
           // isDestroying = true;

            if (highlightHolder != null)
                Destroy(highlightHolder);
        }

        private IEnumerator GrowHighlight()
        {
            float width = highlightMat.GetFloat("g_flOutlineWidth");
            float startValue = width;
            float endValue = _isHighlighted ?  0.000f : 0.003f;
            float counter = 0f;

            while (counter < _duration )
            {
                counter += Time.deltaTime / _duration;
                width = Mathf.Lerp(startValue, endValue, counter);
                highlightMat.SetFloat("g_flOutlineWidth", width);
                yield return null;
            }

            if (width >= 0.002f) _isHighlighted = true;


        }
    }
}



