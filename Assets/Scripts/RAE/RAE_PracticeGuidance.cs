/*
 * This code is part of Generative Sound Engine for Coupled Sim in Unity by Marv934 and zeyuyang42 (2022)
 * Developped as part of the Sonic Interaction Design Seminar at Audiokomminikation Group, TU Berlin
 * 
 * This is distributed under the MIT Licence (see LICENSE.md for details)
 */

 /*
  * Script to controll the Practice Scenario. It is hard coded!
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
    public class RAE_PracticeGuidance : MonoBehaviour
    {
        [Header("Times in seconds")]
        [SerializeField]
        private float UpdateTime = 0.02f;

        // Init Input Manager
        RAE_InputManager InputManager;

        // Init OSC Transmitter
        RAE_OSCtransmitter OSCtransmitter;

        // Init Dashboard
        RAE_Dashboard Dashboard;

        // Init Vehicle
        VehicleBehaviour.WheelVehicle Vehicle;

        // Init Skript
        public int Skript = 0;
        bool Waiting = false;

        [Header("Input")]
        // Confirm Key
        [SerializeField]
        private string ConfirmKeyCode = "Submit";

        IEnumerator WaitForStartConfirmation()
        {
            Waiting = true;

            bool state = true;
            while (state)
            {
                if (Input.GetButtonDown(ConfirmKeyCode))
                {
                    InputManager.StartEngine();

                    state = false;
                }
                yield return new WaitForSeconds(UpdateTime);
            }

            Waiting = false;
        }

        IEnumerator WaitForEndConfirmation()
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
                if (Input.GetButtonDown(ConfirmKeyCode))
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
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!Waiting)
            {
                if (Skript == 0)
                {   // Event Fahrtbeginn

                    OSCtransmitter.BoolTrigger("AmbientOn", true);
                    StartCoroutine(WaitForStartConfirmation());
                    Skript = 1;
                }
                else if (Skript == 4)
                {   // Event Ausparken
                    StartCoroutine(WaitForEndConfirmation());
                    Skript = 5;
                }
            }
        }
    }
}
