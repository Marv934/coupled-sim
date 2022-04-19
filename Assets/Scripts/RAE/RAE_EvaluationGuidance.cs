/*
 * This code is part of Generative Sound Engine for Coupled Sim in Unity by Marv934 (2022)
 * Developped as part of the Sonic Interaction Design Seminar at Audiokomminikation Group, TU Berlin
 * 
 * This is distributed under the MIT Licence (see LICENSE.md for details)
 */

 /*
  * Script to controll the Evaluation Scenario. It is hard coded!
  *
  * public variable:
  *     - Skript
  *
  * Components needed in Scene:
  *     - RAE_InputManager
  *     - RAE_OSCtransmitter
  *     - RAE_Dashboard
  *     - WheelVehicle
  */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RealTimeAuralizationEngine
{
    public class RAE_EvaluationGuidance : MonoBehaviour
    {
        [Header("Times in seconds")]
        [SerializeField]
        private float UpdateTime = 0.1f;
        [SerializeField]
        private float WaitTime = 1.0f;

        // Init Pedestrian
        [Header("CollisionPedestrian")]
        [SerializeField]
        private GameObject CollisionPedestrian = null;
        Animator _AIPedestrian;

        // Init Parkluecke
        [Header("Parkluecke")]
        [SerializeField]
        private GameObject ParklueckeObject = null;

        [Header("Input")]
        // Confirm Key
        [SerializeField]
        private string ConfirmKeyCode = "KeyCode.L";

        // Init Input Manager
        RAE_InputManager InputManager;

        // Init OSC Transmitter
        RAE_OSCtransmitter OSCtransmitter;

        // Init Dashboard
        RAE_Dashboard Dashboard;

        // Init Vehicle
        IVehicle Vehicle;

        // Init Skript
        public int Skript = 0;
        bool Waiting = false;

        IEnumerator WaitForSWUpdateConfirmation()
        {
            Waiting = true;

            yield return new WaitForSeconds(WaitTime);

            OSCtransmitter.FloatTrigger("SWUpdate", 1.0f);

            Dashboard.DisplayServiceInfo(true);

            bool state = true;
            while (state)
            {
                if (Input.GetKey(ConfirmKeyCode))
                {
                    state = false;
                }
                yield return new WaitForSeconds(UpdateTime);
            }

            OSCtransmitter.BoolTrigger("Confirm", true);

            Dashboard.DisplayServiceInfo(false);

            Waiting = false;
        }

        IEnumerator WaitForEngineStart()
        {
            Waiting = true;

            bool state = true;
            while (state)
            {
                if (Input.GetKey(ConfirmKeyCode))
                {
                    InputManager.StartEngine();

                    state = false;
                }
                yield return new WaitForSeconds(UpdateTime);
            }

            Waiting = false;
        }

        IEnumerator WaitForTextMessageConfirmation()
        {
            Waiting = true;

            bool state = true;
            while (state)
            {
                if (Vehicle.Speed < 0.01f)
                {
                    state = false;
                }
                yield return new WaitForSeconds(UpdateTime);
            }

            state = true;
            while (state)
            {
                if (Input.GetKey(ConfirmKeyCode))
                {
                    state = false;

                }
                yield return new WaitForSeconds(UpdateTime);
            }

            Dashboard.DisplaySMSInfo(false);

            OSCtransmitter.BoolTrigger("Confirm", true);

            Waiting = false;
        }

        IEnumerator WaitForBatteryWarningConfirmation()
        {
            Waiting = true;

            bool state = true;
            while (state)
            {
                if (Vehicle.Speed < 0.01f)
                {
                    state = false;
                }
                yield return new WaitForSeconds(UpdateTime);
            }

            state = true;
            while (state)
            {
                if (Input.GetKey(ConfirmKeyCode))
                {
                    state = false;
                }
                yield return new WaitForSeconds(UpdateTime);
            }

            Dashboard.DisplayBatteryWarning(false);

            OSCtransmitter.BoolTrigger("Confirm", true);

            Waiting = false;
        }

        IEnumerator WaitForServiceInfoConfirmation()
        {
            Waiting = true;

            bool state = true;
            while (state)
            {
                if (Vehicle.Speed < 0.01f)
                {
                    state = false;
                }
                yield return new WaitForSeconds(UpdateTime);
            }

            state = true;
            while (state)
            {
                if (Input.GetKey(ConfirmKeyCode))
                {
                    state = false;
                }
                yield return new WaitForSeconds(UpdateTime);
            }
            Dashboard.DisplayServiceInfo(false);

            OSCtransmitter.BoolTrigger("Confirm", true);

            Waiting = false;
        }

        IEnumerator WaitForTirePressureWarningConfirmation()
        {
            Waiting = true;

            bool state = true;
            while (state)
            {
                if (Vehicle.Speed < 0.01f)
                {
                    state = false;
                }
                yield return new WaitForSeconds(UpdateTime);
            }

            state = true;
            while (state)
            {
                if (Input.GetKey(ConfirmKeyCode))
                {
                    state = false;
                }
                yield return new WaitForSeconds(UpdateTime);
            }
            Dashboard.DisplayTirePressureWarning(false);

            OSCtransmitter.BoolTrigger("Confirm", true);

            Waiting = false;
        }

        IEnumerator Einparken()
        {
            Waiting = true;

            bool state = true;
            while (state)
            {
                if (Input.GetKey(ConfirmKeyCode))
                { 
                    state = false;
                }
                yield return new WaitForSeconds(UpdateTime);
            }

            Waiting = false;
        }

        IEnumerator WaitForEngineStop()
        {
            Waiting = true;

            bool state = true;
            while (state)
            {
                if (Input.GetKey(ConfirmKeyCode))
                {

                    InputManager.StopEngine();

                    state = false;
                }
                yield return new WaitForSeconds(UpdateTime);
            }

            Waiting = false;
        }

        // Start is called before the first frame update
        void Start()
        {
            // Get Scene
            GameObject[] root = gameObject.scene.GetRootGameObjects();

            // Get GameObject
            foreach (GameObject obj in root)
            {
                if (obj.TryGetComponent(typeof(RAE_InputManager), out Component comp))
                {
                    // Get Input Manager
                    InputManager = comp.GetComponent<RAE_InputManager>();

                    // Get OSC Transmitter
                    OSCtransmitter = comp.GetComponent<RAE_OSCtransmitter>();

                    // Get Dashboard
                    Dashboard = comp.GetComponentInChildren<RAE_Dashboard>();

                    // Get Vehicle
                    Vehicle = comp.GetComponent<VehicleBehaviour.WheelVehicle>();
                }
            }

            CollisionPedestrian.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (!Waiting)
            {
                if (Skript == 0)
                {   // Event Fahrtbeginn

                    OSCtransmitter.BoolTrigger("AmbientOn", true);
                    StartCoroutine(WaitForEngineStart());
                    Skript = 1;
                }
                else if (Skript == 1)
                {   // Event SWUpdate
                    StartCoroutine(WaitForSWUpdateConfirmation());
                    Skript = 2;
                }
                else if (Skript == 2)
                {   // Event Ausparken
                    InputManager.StartParking();
                    Skript = 3;
                }
                else if (Skript == 4)
                {   // Event End Ausparken
                    InputManager.StopParking();
                    ParklueckeObject.SetActive(true);
                    Skript = 5;
                }
                else if (Skript == 6)
                {   // Event Text Message
                    OSCtransmitter.BoolTrigger("TextMessage", true);
                    Dashboard.DisplaySMSInfo(true);
                    Skript = 7;
                }
                else if (Skript == 8)
                {   // Wait for Confirmation
                    StartCoroutine(WaitForTextMessageConfirmation());
                    Skript = 9;
                }
                else if (Skript == 11)
                {   // Start Collision
                    CollisionPedestrian.SetActive(true);
                    InputManager.StartCollision();
                    Skript = 12;
                }
                else if (Skript == 13)
                {   // End Collision
                    CollisionPedestrian.SetActive(false);
                    InputManager.StopCollision();
                    Skript = 14;
                }
                else if (Skript == 15)
                {   // Event Battery Warning
                    OSCtransmitter.BoolTrigger("Battery", true);
                    Dashboard.DisplayBatteryWarning(true);
                    Skript = 16;
                }
                else if (Skript == 17)
                {   // Wait for Confirmation
                    StartCoroutine(WaitForBatteryWarningConfirmation());
                    Skript = 18;
                }
                else if (Skript == 20)
                {   // Event Battery Warning
                    OSCtransmitter.BoolTrigger("Service", true);
                    Dashboard.DisplayServiceInfo(true);
                    Skript = 21;
                }
                else if (Skript == 22)
                {   // Wait for Confirmation
                    StartCoroutine(WaitForServiceInfoConfirmation());
                    Skript = 23;
                }
                else if (Skript == 25)
                {   // Event Battery Warning
                    OSCtransmitter.BoolTrigger("TirePressure", true);
                    Dashboard.DisplayTirePressureWarning(true);
                    Skript = 26;
                }
                else if (Skript == 27)
                {   // Wait for Confirmation
                    StartCoroutine(WaitForTirePressureWarningConfirmation());
                    Skript = 28;
                }
                else if (Skript == 30)
                {   // Start Einparken
                    StartCoroutine(Einparken());
                    InputManager.StartParking();
                    Skript = 31;
                }
                else if (Skript == 31)
                {   // Motor Aus
                    StartCoroutine(WaitForEngineStop());
                    Skript = 32;
                }
            }
        }
    }
}
