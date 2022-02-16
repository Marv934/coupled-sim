using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenerativeSoundEngine
{
    public class GSE_InputManager : MonoBehaviour
    {
        // Init Vehivle
        VehicleBehaviour.WheelVehicle Vehicle;

        // Init OSCtransmitter;
        GSE_OSCtransmitter OSCtransmitter;

        // Init Dashboard
        GSE_Dashboard Dashboard;

        // Init Collision
        GSE_Collision Collision;

        // Start is called before the first frame update
        void Start()
        {
            // Get Vehicle
            Vehicle = GetComponentInParent<VehicleBehaviour.WheelVehicle>();

            // Get OSC Transmitter
            OSCtransmitter = GetComponent<GSE_OSCtransmitter>();
            
            // Get Dashboard
            Dashboard = GetComponentInChildren<GSE_Dashboard>();

            // Get Collision
            Collision = GetComponentInChildren<GSE_Collision>();

        }

        // Update is called once per frame
        void Update()
        {
            // Engine Start/Stop
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (!Vehicle.Engine)
                {
                    StartEngine();
                }
                else if (Vehicle.Engine)
                {
                    StopEngine();
                }
            }

            // Parking Assistant Start/Stop
            if (Input.GetKeyDown(KeyCode.B))
            {
                if(!Collision.ParkingAssistantState)
                {
                    StartParking();
                }
                else if (Collision.ParkingAssistantState)
                {
                    StopParking();
                }
            }

            // Collision Assistant Start/Stop
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (!Collision.CollisionAssistantState)
                {
                    StartCollision();
                }
                else if (Collision.CollisionAssistantState)
                {
                    StopCollision();
                }
            }
        }

        public void StartEngine()
        {
            Vehicle.Engine = true;

            // Send OSCMessage
            OSCtransmitter.BoolTrigger("StartUp", true);

            // Set Dashboard
            Dashboard.DisplayEngineStart();

            // set Handbrake to true
            Vehicle.Handbrake = false;
        }

        public void StopEngine()
        {
            Vehicle.Engine = false;

            // Send OSCMessage
            OSCtransmitter.BoolTrigger("ShutDown", true);

            // Set Dashboard
            Dashboard.DisplayEngineStop();

            // set Handbrake to false
            Vehicle.Handbrake = true;
        }

        public void StartParking()
        {
            Collision.ParkingAssistantState = true;
            Collision.CollisionAssistantState = false;
            Collision.BlindSpotAssistantState = false;
        }

        public void StopParking()
        {
            Collision.ParkingAssistantState = false;
        }

        public void StartCollision()
        {
            Collision.ParkingAssistantState = false;
            Collision.CollisionAssistantState = true;
            Collision.BlindSpotAssistantState = false;
        }

        public void StopCollision()
        {
            Collision.CollisionAssistantState = false;
        }
    }
}
