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

        // Get CarBlinkers
        [Header("Car Blinkers")]
        [SerializeField] CarBlinkers blinkers;
        bool BlinkerStateLeft = false;
        bool BlinkerStateRight = false;

        //private IEnumerator unsetBlinkerLeft;
        //private IEnumerator unsetBlinkerRight;

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

            //unsetBlinkerLeft = UnsetBlinkerLeft();
            //unsetBlinkerRight = UnsetBlinkerRight();

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
                else
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
                else
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
                else
                {
                    StopCollision();
                }
            }

            // Drive/Reverse
            if (Input.GetKeyDown(KeyCode.G))
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

            // Blinker Set Left/Right
            if (Input.GetKeyDown(KeyCode.Q))
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

            if (Input.GetKeyDown(KeyCode.E))
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
            OSCtransmitter.BoolTrigger("BlinkerOn", true);

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

                if (Vehicle.Steering > -0.1f)
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
            OSCtransmitter.BoolTrigger("BlinkerOn", true);
            
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

                if (Vehicle.Steering < 0.1f)
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
            OSCtransmitter.BoolTrigger("BlinkerOff", true);
        }
    }
}
