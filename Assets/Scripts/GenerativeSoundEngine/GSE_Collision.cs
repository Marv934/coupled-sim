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
        [SerializeField] float ExitMinimumSpeed = 0.2f;

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
        private int updateStep = 5;

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
            updateCounter++;
            if (updateCounter == updateStep)
            {
                // Check Blind Spot Assistant
                if (GSEVehicle.Indicator != 0 || Mathf.Abs(IVehicle.Speed) < ExitMinimumSpeed) 
                {
                    var BlindSpot = BlindSpotAssistant.BlindSpotUpdate();
                    if (BlindSpot.Item1 < BlindSpotAssistantMaxDistance)
                    {
                        UpdateCollisionDetection(3, BlindSpot.Item1, BlindSpot.Item2);
                    }
                    else
                    {
                        UpdateCollisionDetection(0, 0.0f, 0.0f);
                    }
                } else if (GSEVehicle.ParkAssistant)
                {
                    // Check Parking Assistant
                    var Parking = ParkingAssistant.ParkingUpdate();
                    UpdateCollisionDetection(1, Parking.Item1, Parking.Item2);
                } else 
                {
                    // Check Collision Assistant
                    var Collision = CollisionAssistant.CollisionUpdate();

                    if (Collision.Item1 < GSEVehicle.MaxSpeed * CollisionAssistantMaxDistance)
                    {
                        UpdateCollisionDetection(2, Collision.Item1, Collision.Item2);
                    } else
                    {
                        UpdateCollisionDetection(0, 0.0f, 0.0f);
                    }
                }
                updateCounter = 0;
            }
        }

        public void UpdateCollisionDetection(int Type, float Distance, float Angle)
        {
            OSCtransmitter.CollisionType(Type);
            OSCtransmitter.CollisionDistance(Distance);
            OSCtransmitter.CollisionAngle(Angle);

            if (Type == 1)
            {
                Dashboard.DisplayParkingWarning(true);
                Dashboard.DisplayCollisionWarning(false);
                Dashboard.DisplayBlindSpotWarning(false, Angle);
            }
            else if (Type == 2)
            {
                Dashboard.DisplayParkingWarning(false);
                Dashboard.DisplayCollisionWarning(true);
                Dashboard.DisplayBlindSpotWarning(false, Angle);
            }
            else if (Type == 3)
            {
                Dashboard.DisplayParkingWarning(false);
                Dashboard.DisplayCollisionWarning(false);
                Dashboard.DisplayBlindSpotWarning(true, Angle);
            }
            else
            {
                Dashboard.DisplayParkingWarning(false);
                Dashboard.DisplayCollisionWarning(false);
                Dashboard.DisplayBlindSpotWarning(false, Angle);
            }
        }

    }

}