using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Capture : MonoBehaviour
{
    int captureNumber = 0;

    AutoScreenCapture autoScreenCapture;
    public Camera captureCamera;
	public AudioSource alert;

    public 
    void Start()
    {
        autoScreenCapture = GetComponent<AutoScreenCapture>();

    }

    public void CaptureNow() {
        if (alert) {
            alert.Play();
        }
        string path = Application.dataPath + string.Format("/Captures/SavedScreen_{0}.png", captureNumber);
        autoScreenCapture.SaveScreenshot(CaptureMethod.RenderToTex_Asynch, path, captureCamera);
        captureNumber += 1;
        Debug.Log("Frame Captured");
    }
}
