using UnityEngine;
using UnityEngine.VR;
using System.Collections;

public class CameraCounter : MonoBehaviour {

    Transform
        childCamera;

	// Use this for initialization
	void Start () {
        childCamera = transform.GetChild(0);
	}

    private void Update()
    {
        // Added for RAE - START
        // Reset Camera changed from "R" to "F1"
        if (Input.GetKeyDown(KeyCode.F1))
        // Added for RAE - END
        {
            UnityEngine.XR.InputTracking.Recenter();
        }
    }
    // Update is called once per frame
    void LateUpdate () {
        Vector3 invertedPosition = -childCamera.localPosition;
        transform.localPosition = invertedPosition;
	}
}
