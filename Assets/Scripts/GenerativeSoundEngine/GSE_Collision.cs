using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenerativeSoundEngine
{
    public class GSE_Collision : MonoBehaviour
    {
        // Set Collision type Variants
        [Header("Parking Assistant, static")]
        [SerializeField] float ParkingAssistentMaxSpeed = 10.0f;
        [SerializeField] float ParkingAssistentMaxDistance = 1.0f;

        [Header("Collision Assistant, times maxSpeed")]
        [SerializeField] float CollisionAssistantMaxDistance = 0.5f;
        [SerializeField] float CollisionAssistantMinSpeed = 10.0f;

        [Header("Blind Spot Assistant, static")]
        [SerializeField] float BlindSpotAssistantMaxDistance = 10.0f;

        [Header("Every <SendRate> Fixed Update Sends OSC Message")]
        [SerializeField] public int SendRate = 5;

        // Init WheelVehicle
        private IVehicle IVehicle;
        private GSEVehicle GSEVehicle;

        // Init Parking Assistant
        GSE_ParkingAssistant ParkingAssistant;
        // Init Collision Assistant
        GSE_CollisionAssistant CollisionAssistant;
        // Init Blind Spot Assistant
        GSE_BlindSpotAssistant BlindSpotAssistant;

        // Init OSC Transmitter
        GSE_OSCtransmitter OSCtransmitter;

        // Init Dashboard
        GSE_Dashboard Dashboard;

        // update Counter
        private int updateCounter = 0;

        // Start is called before the first frame update
        void Start()
        {
            // Get WheelVehicle Interface
            IVehicle = GetComponent<VehicleBehaviour.WheelVehicle>();
            GSEVehicle = GetComponent<VehicleBehaviour.WheelVehicle>();

            // Get Parking Assistant
            ParkingAssistant = GetComponentInChildren<GSE_ParkingAssistant>();
            // Get Collision Assistant
            CollisionAssistant = GetComponentInChildren<GSE_CollisionAssistant>();
            // Get Blind Spot Assistant
            BlindSpotAssistant = GetComponentInChildren<GSE_BlindSpotAssistant>();

            // Get OSC Transmitter
            OSCtransmitter = GetComponent<GSE_OSCtransmitter>();

            // Get Dashboard
            Dashboard = GetComponentInChildren<GSE_Dashboard>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            updateCounter = updateCounter + 1;
            if (updateCounter == SendRate)
            {
                var BlindSpot = BlindSpotAssistant.BlindSpotUpdate();
                var Parking = ParkingAssistant.ParkingUpdate();
                var Collision = CollisionAssistant.CollisionUpdate();

                // Check Blind Spot Assistant
                if (GSEVehicle.Indicator != 0 && BlindSpot.Item1 < BlindSpotAssistantMaxDistance)
                {
                    OSCtransmitter.Collision(3, BlindSpot.Item1, BlindSpot.Item2);

                    Dashboard.DisplayCollisionWarning(false);
                    Dashboard.DisplayParkingWarning(false);
                    Dashboard.DisplayBlindSpotWarning(true, BlindSpot.Item2);

                } // Check Parking Assistant
                else if (IVehicle.Speed < ParkingAssistentMaxSpeed && Parking.Item1 < ParkingAssistentMaxDistance)
                {
                    OSCtransmitter.Collision(1, Parking.Item1, Parking.Item2);

                    Dashboard.DisplayBlindSpotWarning(false, 0.0f);
                    Dashboard.DisplayCollisionWarning(false);
                    Dashboard.DisplayParkingWarning(true);
                } // Check Collision Assistant
                else if (IVehicle.Speed > CollisionAssistantMinSpeed && Collision.Item1 < GSEVehicle.MaxSpeed * CollisionAssistantMaxDistance)
                {
                    OSCtransmitter.Collision(2, Collision.Item1, Collision.Item2);

                    Dashboard.DisplayBlindSpotWarning(false, 0.0f);
                    Dashboard.DisplayParkingWarning(false);
                    Dashboard.DisplayCollisionWarning(true);
                } 
                else
                {
                    OSCtransmitter.Collision(0, 0.0f, 0.0f);

                    Dashboard.DisplayCollisionWarning(false);
                    Dashboard.DisplayParkingWarning(false);
                    Dashboard.DisplayBlindSpotWarning(false, 0.0f);
                }
                updateCounter = 0;
            }
        }
    }

}