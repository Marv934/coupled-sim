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

        [Header("Blind Spot Assistant, static")]
        [SerializeField] float BlindSpotAssistantMaxDistance = 10.0f;

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
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            updateCounter = updateCounter + 1;
            if (updateCounter == 5)
            {
                // Check Blind Spot Assistant
                if (GSEVehicle.Indicator != 0)
            {
                var BlindSpot = BlindSpotAssistant.BlindSpotUpdate();

                if (BlindSpot.Item1 < BlindSpotAssistantMaxDistance)
                {
                    OSCtransmitter.Collision(3, BlindSpot.Item1, BlindSpot.Item2);
                }
                else
                {
                    OSCtransmitter.Collision(0, 0.0f, 0.0f);
                }
                // Check if Collision or Parking Assistant
            } else if (IVehicle.Speed < ParkingAssistentMaxSpeed)
            {
                // Check Parking Assistant
                var Parking = ParkingAssistant.ParkingUpdate();

                if (Parking.Item1 < ParkingAssistentMaxDistance)
                {
                    OSCtransmitter.Collision(1, Parking.Item1, Parking.Item2);
                }
                else
                {
                    OSCtransmitter.Collision(0, 0.0f, 0.0f);
                }
            } else
            {
                // Check Collision Assistant
                var Collision = CollisionAssistant.CollisionUpdate();

                if (Collision.Item1 < GSEVehicle.MaxSpeed * CollisionAssistantMaxDistance)
                {
                    OSCtransmitter.Collision(2, Collision.Item1, Collision.Item2);
                } else
                {
                    OSCtransmitter.Collision(0, 0.0f, 0.0f);
                }
            }
                updateCounter = 0;
            }
        }
    }

}