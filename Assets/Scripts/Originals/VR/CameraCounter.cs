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
        // Reset Camera changed from "R" to "M"
        if (Input.GetKeyDown(KeyCode.F1))
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
