using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenerativeSoundEngine
{
    public class GSE_ServiceWaypoint : MonoBehaviour
    {
        // Set Warning Prio
        [SerializeField] float Priority = 1.0f;

        // Set Destroyed
        [Header("Destroy on Trigger - true, Not destroy on Trigger - false")]
        [SerializeField] bool destroy = true;

        // Init OSC Transmitter
        GSE_OSCtransmitter OSCtransmitter;

        // Init Display
        GSE_Dashboard Dashboard;

        // Start is called before the first frame update
        void Start()
        {

        }

        void OnTriggerEnter(Collider other)
        {
            // Get OSC Transmitter
            OSCtransmitter = other.gameObject.GetComponent<GSE_OSCtransmitter>();

            // Get Dashboard
            Dashboard = other.gameObject.GetComponentInChildren<GSE_Dashboard>();

            if ( OSCtransmitter != null )
            {
                OSCtransmitter.Info(Priority);

                Dashboard.DisplayServiceInfo(true);

                if (destroy)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
