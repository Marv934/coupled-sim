using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GenerativeSoundEngine
{
    public class GSE_PracticeGuidance : MonoBehaviour
    {
        [Header("Times in seconds")]
        [SerializeField]
        private float UpdateTime = 0.1f;
        [SerializeField]
        private float WaitTime = 1.0f;
        [SerializeField]
        private float DisplayTime = 5.0f;

        [Header("StartMessage")]
        [SerializeField]
        private GameObject StartObject;

        [Header("ParkMessage")]
        [SerializeField]
        private GameObject ParkObject;

        [Header("EndMessage")]
        [SerializeField]
        private GameObject EndObject;

        // Init Input Manager
        GSE_InputManager InputManager;

        // Init OSC Transmitter
        GSE_OSCtransmitter OSCtransmitter;

        // Init Dashboard
        GSE_Dashboard Dashboard;

        // Init Vehicle
        VehicleBehaviour.WheelVehicle Vehicle;

        // Init Skript
        public int Skript = 0;
        bool Waiting = false;

        // Init Canvas
        Canvas ScreenCanvas;

        IEnumerator WaitForStartConfirmation()
        {
            Waiting = true;

            yield return new WaitForSeconds(WaitTime);

            StartObject.SetActive(true);

            bool state = true;
            while (state)
            {
                if (Input.GetKey(KeyCode.L))
                {
                    StartObject.SetActive(false);

                    state = false;
                }
                yield return new WaitForSeconds(UpdateTime);
            }

            Vehicle.Engine = true;
            Dashboard.DisplayEngineStart();
            Vehicle.Handbrake = false;

            Waiting = false;
        }

        IEnumerator ParkMessage()
        {
            Waiting = true;

            ParkObject.SetActive(true);

            yield return new WaitForSeconds(DisplayTime);

            ParkObject.SetActive(false);

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

            yield return new WaitForSeconds(WaitTime);

            EndObject.SetActive(true);

            state = true;
            while (state)
            {
                if (Input.GetKey(KeyCode.L))
                {
                    EndObject.SetActive(false);

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
                if (obj.TryGetComponent(typeof(GSE_InputManager), out Component comp))
                {
                    // Get Input Manager
                    InputManager = comp.GetComponent<GSE_InputManager>();

                    // Get OSC Transmitter
                    OSCtransmitter = comp.GetComponent<GSE_OSCtransmitter>();

                    // Get Dashboard
                    Dashboard = comp.GetComponentInChildren<GSE_Dashboard>();

                    // Get Vehicle
                    Vehicle = comp.GetComponent<VehicleBehaviour.WheelVehicle>();
                }
            }

            ScreenCanvas = GetComponent<Canvas>();

            ScreenCanvas.worldCamera = Camera.main;
            ScreenCanvas.planeDistance = 1;
        }

        // Update is called once per frame
        void Update()
        {
            if (!Waiting)
            {
                if (Skript == 0)
                {   // Event Fahrtbeginn

                    OSCtransmitter.BoolTrigger("AmbientOn", true);
                    Debug.Log("Skript 0");
                    StartCoroutine(WaitForStartConfirmation());
                    Skript = 1;
                }
                else if (Skript == 2)
                {   // Event SWUpdate
                    Debug.Log("Skript 2");
                    StartCoroutine(ParkMessage());
                    Skript = 3;
                }
                else if (Skript == 4)
                {   // Event Ausparken
                    Debug.Log("Skript 4");
                    StartCoroutine(WaitForEndConfirmation());
                    Skript = 5;
                }
            }
        }
    }
}
