using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenerativeSoundEngine
{
    public class GSE_WaypointTrigger : MonoBehaviour
    {
        // Set Name
        [SerializeField] string Name;

        // Set Prio
        [SerializeField] bool sendPrio = false;
        [SerializeField] float Priority = 1.0f;

        // Set Bool
        [SerializeField] bool sendBool = false;
        [SerializeField] bool Bool = true;

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

                if (sendBool)
                {
                    OSCtransmitter.BoolTrigger(Name, Bool);
                }
                if (sendPrio)
                {
                    OSCtransmitter.FloatTrigger(Name, Priority);
                }

                if (destroy)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
