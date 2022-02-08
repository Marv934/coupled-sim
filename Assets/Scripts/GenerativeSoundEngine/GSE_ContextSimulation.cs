using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GenerativeSoundEngine
{
    [RequireComponent(typeof(VehicleBehaviour.WheelVehicle))]

    public class GSE_ContextSimulation : MonoBehaviour
    {
        // Driver statues or Driving
        [Header("Driver statues")]
        [SerializeField] float TirenessLevel = 0.2f;
        [SerializeField] float StressLevel = 0.2f;
        [Header("Atomsphore")]
        [SerializeField] bool TelephoneActive = false;
        [SerializeField] bool MusicActive = false;
        [SerializeField] bool ConversationActive = false;
        [SerializeField] bool AutoDrive = false;

        // Init WheelVehicle
        private GSEVehicle GSEVehicle;
        private IVehicle IVehicle;

        // update Counter
        private int updateCounter = 0;
        private int updateStep = 5;

        // Init OSC Transmitter
        GSE_OSCtransmitter OSCtransmitter;

        // Start is called before the first frame update
        void Start()
        {
            // Get WheelVehicle Interface
            GSEVehicle = GetComponent<VehicleBehaviour.WheelVehicle>();
            IVehicle = GetComponent<VehicleBehaviour.WheelVehicle>();
            // Get OSC Transmitter
            OSCtransmitter = GetComponent<GSE_OSCtransmitter>();
            // set initial contexts
            OSCtransmitter.Telephone(TelephoneActive);
            OSCtransmitter.Music(MusicActive);
            OSCtransmitter.Conversation(ConversationActive);
            OSCtransmitter.AutoDrive(AutoDrive);
        }

        // Update is called once per frame
        void FixedUpdate()
        {   // Continusly send Values
            updateCounter++;
            if (updateCounter == updateStep)
            {
                // Vehicle values
                OSCtransmitter.Speed(IVehicle.Speed);
                OSCtransmitter.Steering(GSEVehicle.Steering);

                // Driver values
                UpdateTireness();
                UpdateStressLevel();
                OSCtransmitter.TirenessLevel(TirenessLevel);
                OSCtransmitter.StressLevel(StressLevel);

                updateCounter = 0;
            }
        }

        public void UpdateTireness()
        {
            // fake the Tireness by random walking for testing
            float maxStep = 0.01f;
            float change  = Random.Range(-maxStep, maxStep);
            float tmpTirenessLevel = change + TirenessLevel;
            if (tmpTirenessLevel > 1.0f)
            {
                TirenessLevel = 1.0f;
            }else if (tmpTirenessLevel < 0.0f)
            {
                TirenessLevel = 0.0f;
            }else
            {
                TirenessLevel = tmpTirenessLevel;
            }
        }

        public void UpdateStressLevel()
        {
            // fake the StressLevel by random walking for testing
            float maxStep = 0.01f;
            float change = Random.Range(-maxStep, maxStep);
            float tmpStressLevel = change + StressLevel;
            if (tmpStressLevel > 1.0f)
            {
                StressLevel = 1.0f;
            }
            else if (tmpStressLevel < 0.0f)
            {
                StressLevel = 0.0f;
            }
            else
            {
                StressLevel = tmpStressLevel;
            }
        }

        public void UpdateTelephoneActive(bool active)
        {
            TelephoneActive = active;
            OSCtransmitter.Telephone(TelephoneActive);
        }

        public void UpdateMusicActive(bool active)
        {
            MusicActive = active;
            OSCtransmitter.Music(MusicActive);

        }

        public void UpdateConversationActive(bool active)
        {
            ConversationActive = active;
            OSCtransmitter.Conversation(ConversationActive);
        }

        public void UpdateAutoDrive(bool active)
        {
            AutoDrive = active;
            OSCtransmitter.AutoDrive(AutoDrive);
        }
    }
}

