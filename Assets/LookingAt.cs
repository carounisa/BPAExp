using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;

public class LookingAt : MonoBehaviour
{
    private FocusInfo FocusInfo;
    private readonly float MaxDistance = 20;
    private readonly GazeIndex[] GazePriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };
    private LineRenderer lineRenderer;
    private LayerMask mesh;
    private int region;
    public bool enableLineRenderer;

    void Start()
    {
        if (!SRanipal_Eye_Framework.Instance.EnableEye)
        {
            enabled = false;
            return;
        }

        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;

        mesh = LayerMask.GetMask("Mesh");
        region =~ mesh;
    }

    // Update is called once per frame
    void Update()
    {

        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
            SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

        foreach (GazeIndex index in GazePriority)
        {
            Ray GazeRay;
            RaycastHit hit;
            bool valid = SRanipal_Eye.GetGazeRay(index, out GazeRay);
            Ray rayGlobal = new Ray(Camera.main.transform.position, Camera.main.transform.TransformDirection(GazeRay.direction));

            if (valid)
            {
                if (Physics.Raycast(rayGlobal, out hit, MaxDistance, region))
                {
                   DataHandler.instance.recordGazeTime(hit.collider.tag);
                } else if (DataHandler.instance.isWatchRunning())
                {
                    DataHandler.instance.endRecordingEvidence();
                }

                if (Physics.Raycast(rayGlobal, out hit, MaxDistance, mesh))
                {
                    if(enableLineRenderer)
                    {
                    transform.position = hit.point;
                    lineRenderer.SetPosition(0, Camera.main.transform.position - Camera.main.transform.up * 0.05f);
                    lineRenderer.SetPosition(1, hit.point);
                    }

                    DataHandler.instance.collectMeshTrackingData(hit.point, hit.distance, rayGlobal.direction);
                }
            }
        }
    }
}
