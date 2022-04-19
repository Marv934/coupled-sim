/*
 * This code is part of Generative Sound Engine for Coupled Sim in Unity by Marv934 (2022)
 * Developped as part of the Sonic Interaction Design Seminar at Audiokomminikation Group, TU Berlin
 * 
 * This is distributed under the MIT Licence (see LICENSE.md for details)
 */

 /* 
  * This script collects and gernerates context information and sends it via OSCtransmitter
  * 
  * Context infromation from the Simulation:
  *     - Speed
  *     - Steering
  * Context information faked by the Script:
  *     - TirenessLevel
  *     - StessLevel
  * Context information triggered via Method:
  *     - TelephoneActive
  *     - MusicActive
  *     - ConversationActive
  *     - AutoDrive
  *
  * Added public Methods:
  *     - UpdateTelephoneActive(bool)
  *     - UpdateMusicActive(bool)
  *     - UpdateConversationActive(bool)
  *     - UpdateAutoDrive(bool)
  */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealTimeAuralizationEngine
{
    [RequireComponent(typeof(VehicleBehaviour.WheelVehicle))]
    [RequireComponent(typeof(RAE_OSCtransmitter))]

    public class RAE_ContextSimulation : MonoBehaviour
    {
        // Driver status
        [Header("Driver statues")]
        [SerializeField] float TirenessLevel = 0.2f;
        [SerializeField] float StressLevel = 0.2f;

        // Atmosphere status
        [Header("Atomsphere")]
        [SerializeField] bool TelephoneActive = false;
        [SerializeField] bool MusicActive = false;
        [SerializeField] bool ConversationActive = false;
        [SerializeField] bool AutoDrive = false;

        // Init WheelVehicle
        private RAEVehicle RAEVehicle;
        private IVehicle IVehicle;

        // update Counter
        private int updateCounter = 0;
        private int updateStep = 5;

        // Init OSC Transmitter
        RAE_OSCtransmitter OSCtransmitter;

        // Start is called before the first frame update
        void Start()
        {
            // Get WheelVehicle Interface
            RAEVehicle = GetComponent<VehicleBehaviour.WheelVehicle>();
            IVehicle = GetComponent<VehicleBehaviour.WheelVehicle>();

            // Get OSC Transmitter
            OSCtransmitter = GetComponent<RAE_OSCtransmitter>();

            // set initial contexts
            OSCtransmitter.BoolTrigger("Telephone", TelephoneActive);
            OSCtransmitter.BoolTrigger("Music",  MusicActive);
            OSCtransmitter.BoolTrigger("Conversation", ConversationActive);
            OSCtransmitter.BoolTrigger("AutoDrive", AutoDrive);
        }

        // Update is called once per frame
        void FixedUpdate()
        {   // Continusly send Values
            updateCounter++;
            if (updateCounter == updateStep)
            {
                // Vehicle values
                OSCtransmitter.FloatTrigger("Speed", Math.Abs(IVehicle.Speed) / RAEVehicle.MaxSpeed);
                OSCtransmitter.FloatTrigger("Steering", RAEVehicle.Steering);

                // Driver values
                UpdateTireness();
                UpdateStressLevel();
                OSCtransmitter.FloatTrigger("TirenessLevel", TirenessLevel);
                OSCtransmitter.FloatTrigger("StressLevel", StressLevel);

                updateCounter = 0;
            }
        }

        public void UpdateTireness()
        {
            // fake the Tireness by random for testing
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
            // fake the StressLevel by random for testing
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

