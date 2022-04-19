/*
 * This code is part of Generative Sound Engine for Coupled Sim in Unity by Marv934 (2022)
 * Developped as part of the Sonic Interaction Design Seminar at Audiokomminikation Group, TU Berlin
 * 
 * This is distributed under the MIT Licence (see LICENSE.md for details)
 */

/*
 * This Skript handles additional Inputs
 *
 * Components needed in GameObject:
 *      - WheelVehicle
 *      - RAE_OSCtranmitter
 *
 * Components needed in Childer:
 *      - RAE_Dashboard
 *      - RAE_Collision
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealTimeAuralizationEngine
{
    [RequireComponent(typeof(VehicleBehaviour.WheelVehicle))]
    [RequireComponent(typeof(RAE_OSCtransmitter))]

    public class RAE_InputManager : MonoBehaviour
    {
        // Init Vehivle
        VehicleBehaviour.WheelVehicle Vehicle;

        // Init OSCtransmitter;
        RAE_OSCtransmitter OSCtransmitter;

        // Init Dashboard
        RAE_Dashboard Dashboard;

        // Init Collision
        RAE_Collision Collision;

        // Get CarBlinkers
        [Header("Car Blinkers")]
        [SerializeField] CarBlinkers blinkers;
        bool BlinkerStateLeft = false;
        bool BlinkerStateRight = false;

        // Inputs
        [Header("Inputs")]
        [SerializeField] string EngineStartStop = "KeyCode.P";
        [SerializeField] string ParkingAssistantOnOff = "KeyCode.B";
        [SerializeField] string CollisionAssistantOnOff = "KeyCode.C";
        [SerializeField] string DriveReverse = "KeyCode.G";
        [SerializeField] string BlinkerRight = "KeyCode.Q";
        [SerializeField] string BlinkerLeft = "KeyCode.E";

        // Start is called before the first frame update
        void Start()
        {
            // Get Vehicle
            Vehicle = GetComponent<VehicleBehaviour.WheelVehicle>();

            // Get OSC Transmitter
            OSCtransmitter = GetComponent<RAE_OSCtransmitter>();
            
            // Get Dashboard
            Dashboard = GetComponentInChildren<RAE_Dashboard>();

            // Get Collision
            Collision = GetComponentInChildren<RAE_Collision>();
        }

        // Update is called once per frame
        void Update()
        {
            // Engine Start/Stop
            if (Input.GetKeyDown(EngineStartStop))
            {
                if (!Vehicle.Engine)
                {
                    StartEngine();
                }
                else
                {
                    StopEngine();
                }
            }

            // Parking Assistant On/Off
            if (Input.GetKeyDown(ParkingAssistantOnOff))
            {
                if(!Collision.ParkingAssistantState)
                {
                    StartParking();
                }
                else
                {
                    StopParking();
                }
            }

            // Collision Assistant Start/Stop
            if (Input.GetKeyDown(CollisionAssistantOnOff))
            {
                if (!Collision.CollisionAssistantState)
                {
                    StartCollision();
                }
                else
                {
                    StopCollision();
                }
            }

            // Drive/Reverse
            if (Input.GetKeyDown(DriveReverse))
            {
                if (!Vehicle.Reverse)
                {
                    SetReverse();
                    
                    if ( Collision.Collision > 0 )
                    {
                        Collision.Collision = 1;
                    }
                    if ( Collision.Parking > 0 )
                    {
                        Collision.Parking = 1;
                    }
                }
                else
                {
                    SetDrive();

                    if ( Collision.Collision > 0 )
                    {
                        Collision.Collision = 1;
                    }
                    if ( Collision.Parking > 0 )
                    {
                        Collision.Parking = 1;
                    }
                }
            }

            // Blinker Set Left
            if (Input.GetKeyDown(BlinkerLeft))
            {
                if (!BlinkerStateLeft)
                {
                    SetBlinkerLeft();
                }
                else
                {
                    SetBlinkerStop();
                }
            }

            // Blinker Set Right
            if (Input.GetKeyDown(BlinkerRight))
            {
                if (!BlinkerStateRight)
                {
                    SetBlinkerRight();
                }
                else
                {
                    SetBlinkerStop();
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
        }

        public void StopParking()
        {
            Collision.ParkingAssistantState = false;
        }

        public void StartCollision()
        {
            Collision.ParkingAssistantState = false;
            Collision.CollisionAssistantState = true;
        }

        public void StopCollision()
        {
            Collision.CollisionAssistantState = false;
        }

        private void SetReverse()
        {
            // Send OSCMessage
            OSCtransmitter.BoolTrigger("Reverse", true);

            // Set Dashboard
            Dashboard.DisplayReverse();

            // Set vehicle
            Vehicle.Reverse = true;
        }

        private void SetDrive()
        {
            // Send OSCMessage
            OSCtransmitter.BoolTrigger("Reverse", false);

            // Set Dashboard
            Dashboard.DisplayDrive();

            // Set vehicle
            Vehicle.Reverse = false;
        }

        private void SetBlinkerLeft()
        {
            // Set Car Blinkers
            blinkers.StartLeftBlinkers();

            // Set State
            BlinkerStateLeft = true;
            BlinkerStateRight = false;

            // Send OSC Message
            OSCtransmitter.BoolTrigger("IndicatorOn", true);

            StopAllCoroutines();
            StartCoroutine(UnsetBlinkerLeft());
        }

        IEnumerator UnsetBlinkerLeft()
        {
            // wait for Steering
            bool waiting = true;

            while (waiting)
            {
                yield return new WaitForSeconds(0.1f);
                
                if (Vehicle.Steering < -0.33f)
                {
                    waiting = false;
                }
            }

            // Wait for end Steering
            waiting = true;

            while (waiting)
            {
                yield return new WaitForSeconds(0.1f);

                if (Vehicle.Steering > -0.3f)
                {
                    waiting = false;
                }
            }

            SetBlinkerStop();
        }

        private void SetBlinkerRight()
        {
            // Set Car Blinker
            blinkers.StartRightBlinkers();

            // Set State
            BlinkerStateLeft = false;
            BlinkerStateRight = true;

            // Send OSC Message
            OSCtransmitter.BoolTrigger("IndicatorOn", true);
            
            StopAllCoroutines();
            StartCoroutine(UnsetBlinkerRight());
        }

        IEnumerator UnsetBlinkerRight()
        {
            // wait for Steering
            bool waiting = true;

            while (waiting)
            {
                yield return new WaitForSeconds(0.1f);

                if (Vehicle.Steering > 0.33f)
                {
                    waiting = false;
                }
            }

            // Wait for end Steering
            waiting = true;

            while (waiting)
            {
                yield return new WaitForSeconds(0.1f);

                if (Vehicle.Steering < 0.3f)
                {
                    waiting = false;
                }
            }

            SetBlinkerStop();
        }

        private void SetBlinkerStop()
        {
            // Set Car Blinker
            blinkers.Stop();

            // Set State
            BlinkerStateLeft = false;
            BlinkerStateRight = false;

            // Stop Coroutines
            StopAllCoroutines();

            // Senr OSC Message
            OSCtransmitter.BoolTrigger("IndicatorOff", true);
        }
    }
}
