using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

namespace GenerativeSoundEngine
{
    public class GSE_PedestrianTriggerStart : MonoBehaviour
    {
        bool destroy = true;

        WaypointProgressTracker _AIPedestrian;

        GSE_Collision Collision;

        // Start is called before the first frame update
        void Start()
        {
            _AIPedestrian = GetComponentInParent<WaypointProgressTracker>();
        }

        void OnTriggerEnter(Collider other)
        {
            Collision = other.gameObject.GetComponent<GSE_Collision>();

            if ( Collision != null)
            {
                //_AIPedestrian.Trigger();

                if (destroy)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
