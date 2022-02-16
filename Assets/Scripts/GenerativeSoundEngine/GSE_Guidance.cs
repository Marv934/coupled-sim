using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GenerativeSoundEngine
{
    public class GSE_Guidance : MonoBehaviour
    {
        [Header("Times in seconds")]
        [SerializeField]
        private float UpdateTime = 0.1f;
        [SerializeField]
        private float WaitTime = 1.0f;
        [SerializeField]
        private float DisplayTime = 5.0f;

        [Header("EngineStartMessage")]
        [SerializeField]
        private GameObject EngineStartObject;

        [Header("SWUpdateMessage")]
        [SerializeField]
        private GameObject SWUpdateObject;

        [Header("AusparkenMessage")]
        [SerializeField]
        private GameObject AusparkenObject;

        [Header("TextMessageMessage")]
        [SerializeField]
        private GameObject TextMessageObject;

        [Header("TextMessageConfirmationMessage")]
        [SerializeField]
        private GameObject TextMessageConfirmationObject;

        // Init Input Manager
        GSE_InputManager InputManager;

        // Init OSC Transmitter
        GSE_OSCtransmitter OSCtransmitter;

        // Init Dashboard
        GSE_Dashboard Dashboard;

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

            yield return new WaitForSeconds(WaitTime);

            SWUpdateObject.SetActive(true);

            bool state = true;
            while (state)
            {
                if (Input.GetKey(KeyCode.L))
                {
                    SWUpdateObject.SetActive(false);

                    state = false;
                }
                yield return new WaitForSeconds(UpdateTime);
            }

            OSCtransmitter.BoolTrigger("SWUpdateConfirm", true);

            Dashboard.DisplayServiceInfo(false);

            Waiting = false;
        }

        IEnumerator WaitForEngineStart()
        {
            Waiting = true;

            yield return new WaitForSeconds(WaitTime);

            EngineStartObject.SetActive(true);

            bool state = true;
            while (state)
            {
                if (Input.GetKey(KeyCode.L))
                {
                    EngineStartObject.SetActive(false);

                    InputManager.StartEngine();

                    state = false;
                }
                yield return new WaitForSeconds(UpdateTime);
            }

            Waiting = false;
        }

        IEnumerator WaitForParkingConfirmation()
        {

            yield return new WaitForSeconds(WaitTime);

            AusparkenObject.SetActive(true);

            bool state = true;
            while (state)
            {
                if (Input.GetKey(KeyCode.L))
                {
                    AusparkenObject.SetActive(false);

                    InputManager.StartParking();

                    state = false;
                }
                yield return new WaitForSeconds(UpdateTime);
            }

        }

        IEnumerator TextMessage()
        {
            OSCtransmitter.BoolTrigger("TextMessage", true);

            Dashboard.DisplaySMSInfo(true);

            yield return new WaitForSeconds(WaitTime);

            TextMessageObject.SetActive(true);

            yield return new WaitForSeconds(DisplayTime);

            TextMessageObject.SetActive(false);
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

            yield return new WaitForSeconds(WaitTime);

            OSCtransmitter.BoolTrigger("TextMessageConfirmation", true);
            Dashboard.DisplaySMSInfo(false);
            TextMessageConfirmationObject.SetActive(true);

            state = true;
            while (state)
            {
                if (Input.GetKey(KeyCode.L))
                {
                    TextMessageConfirmationObject.SetActive(false);

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
            foreach ( GameObject obj in root)
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

                    // Get Vehicle
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!Waiting)
            {
                if (Skript == 0)
                {   // Event Fahrtbeginn
                    Debug.Log("Skript 0");
                    StartCoroutine(WaitForEngineStart());
                    Skript = 1;
                }
                else if (Skript == 1)
                {   // Event SWUpdate
                    Debug.Log("Skript 1");
                    StartCoroutine(WaitForSWUpdateConfirmation());
                    Skript = 2;
                }
                else if (Skript == 2)
                {   // Event Ausparken
                    Debug.Log("Skript 2");
                    StartCoroutine(WaitForParkingConfirmation());
                    Skript = 3;
                }
                else if (Skript == 4)
                {   // Event End Ausparken
                    Debug.Log("Skript 4");
                    InputManager.StopParking();
                    Skript = 5;
                }
                else if (Skript == 6)
                {   // Event Text Message
                    Debug.Log("Skript 6");
                    StartCoroutine(TextMessage());
                    Skript = 7;
                }
                else if (Skript == 8)
                {   // Wait for Confirmation
                    Debug.Log("Skript 8");
                    StartCoroutine(WaitForTextMessageConfirmation());
                    Skript = 9;
                }
            }
        }
    }
}
