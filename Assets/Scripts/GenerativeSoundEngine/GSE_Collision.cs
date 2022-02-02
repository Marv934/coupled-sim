using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GenerativeSoundEngine
{
    public interface GSE_Proximity
    {
        float Proximity { get; }
        float ProximityAngle { get; }
    }
    
    public class GSE_Collision : MonoBehaviour, GSE_Proximity
    {

        // Init Interface to GSE_ParkingAssistant
        private GSE_ParkingAssistant ParkingProximity;

        // Proximity
        float proximity = 1.0f;
        public float Proximity { get { return proximity; } }

        float proximityAngle = 1.0f;
        public float ProximityAngle { get { return proximityAngle; } }

        // Start is called before the first frame update
        void Start()
        {
            // Get ParkingAssistant Components
            ParkingProximity = GetComponentInChildren<GSE_ParkingAssistant>();
        }

        // Update is called once per frame
        void Update()
        {
            proximity = ParkingProximity.Proximity;
            proximityAngle = ParkingProximity.ProximityAngle;

        }
    }
}
