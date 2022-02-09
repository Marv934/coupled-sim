using UnityEngine;
using UnityStandardAssets.Utility;

public class AIPedestrian : MonoBehaviour
{
    WaypointProgressTracker _tracker;
    float _startYPosition;
    WaypointCircuit _circuit;

    bool triggered = false;

    public void Init(WaypointCircuit circuit)
    {
        enabled = true;
        _tracker = GetComponent<WaypointProgressTracker>();
        _circuit = circuit;
        _tracker.enabled = true;
        _tracker.Init(circuit);
        _startYPosition = transform.position.y;
    }

    private void Update()
    {
        //if (triggered)
        //{ 
            var steer = Quaternion.LookRotation(_tracker.target.position - transform.position, Vector3.up).eulerAngles;
            var rot = transform.eulerAngles;
            rot.y = steer.y;
            transform.eulerAngles = rot;
            var pos = transform.position;
            pos.y = _startYPosition;
            transform.position = pos;
        //}
    }

//    public void Trigger()
//    {
//        triggered = true;
//
//        _tracker.enabled = true;
//        _tracker.Init(_circuit);
//        _startYPosition = transform.position.y;
//    }
}
