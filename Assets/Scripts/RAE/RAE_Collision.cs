/*
 * This code is part of REAL-TIME AURALIZATION ENGINE for Coupled Sim in Unity by Marv934 (2022)
 * Developped as part of the Sonic Interaction Design Seminar at Audiokomminikation Group, TU Berlin
 * 
 * This is distributed under the MIT Licence (see LICENSE.md for details)
 */

 /*
  * With this Script added to an GameObject the Collision Assistant Functionallities are added.
  *
  * GameObject Components Requiered:
  *      - WheelVehicle
  *      - RAE_OSCtransmitter
  * GameObjects Childrens Components Requiered:
  *      - RAE_ParkingAssistant
  *      - RAE_CollisionAssistant
  *      - RAE_Dashboard
  *
  * This Script Cchecks if a Collision is imminent and triggers the OSC Signal and Dashboard Display,
  * as long as the Collision is still imminent and the cooldown time is not over.
  */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealTimeAuralizationEngine
{
    [RequireComponent(typeof(VehicleBehaviour.WheelVehicle))]
    [RequireComponent(typeof(RAE_OSCtransmitter))]


    public class RAE_Collision : MonoBehaviour
    {
        [Header("Parking Assistant")]
        // While Speed < this, Parking Assistant is On
        [SerializeField] float ParkingAssistentMaxSpeed = 5.0f;
        // While Distance < this, Parking Assistant is triggered
        [SerializeField] float ParkingAssistentMaxDistance = 2.5f;

        [Header("Collision Assistant")]
        // While Speed > this, Collision Assistant is On
        [SerializeField] float CollisionAssistantMinSpeed = 5.0f;
        // While Distance < this, Collision Assistant is triggered
        [SerializeField] float CollisionAssistantMaxDistance = 0.5f;

        [Header("CoolDownTimer in Seconds")]
        // Cool Down for Assistants, they turn off after this seconds
        [SerializeField] static float CoolDownTimer = 10.0f;

        [Header("Every <SendRate> Fixed Update Sends OSC Message")]
        // update Counter
        private int updateCounter = 0;
        [SerializeField] static private int updateStep = 6;

        // Init WheelVehicle
        private IVehicle IVehicle;
        private RAEVehicle RAEVehicle;

        // Init Parking Assistant
        RAE_ParkingAssistant ParkingAssistant;
        // Init Collision Assistant
        RAE_CollisionAssistant CollisionAssistant;

        // Init OSC Transmitter
        RAE_OSCtransmitter OSCtransmitter;
        // Init Dashboard
        RAE_Dashboard Dashboard;

        // States
        [SerializeField] public bool ParkingAssistantState = false;
        [SerializeField] public bool CollisionAssistantState = false;

        // CoolDown
        int CoolDownSteps = Convert.ToInt32(Mathf.Round(CoolDownTimer * 50 / updateStep));
        int CollisionCoolDown = 0;
        public int Collision { get { return CollisionCoolDown; } set { CollisionCoolDown = value; } }
        int ParkingCoolDown = 0;
        public int Parking { get { return ParkingCoolDown; } set { ParkingCoolDown = value; } }

        // Start is called before the first frame update
        void Start()
        {
            // Get WheelVehicle Interface
            IVehicle = GetComponent<VehicleBehaviour.WheelVehicle>();
            RAEVehicle = GetComponent<VehicleBehaviour.WheelVehicle>();

            // Get Parking Assistant
            ParkingAssistant = GetComponentInChildren<RAE_ParkingAssistant>();
            // Get Collision Assistant
            CollisionAssistant = GetComponentInChildren<RAE_CollisionAssistant>();

            // Get OSC Transmitter
            OSCtransmitter = GetComponent<RAE_OSCtransmitter>();

            // Get Dashboard
            Dashboard = GetComponentInChildren<RAE_Dashboard>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            // Collision Assistant
            if (CollisionAssistantState)
            {
                // Update CollisionAssistant
                var Collision = CollisionAssistant.CollisionUpdate();

                // Check if Collision is imminent
                if (Mathf.Abs(IVehicle.Speed) > CollisionAssistantMinSpeed && Collision.Item1 < RAEVehicle.MaxSpeed * CollisionAssistantMaxDistance)
                {
                    // Check if first Update to trigger Assistant
                    if (CollisionCoolDown == 0)
                    {
                        // trigger Collision Assistant on
                        OSCtransmitter.BoolTrigger("CollisionTriggerOn", true);
                        Dashboard.DisplayCollisionWarning(true);
                    }
                    // Set CollDown
                    CollisionCoolDown = CoolDownSteps;
                }
                // Check if CoolDown is over
                else if (CollisionCoolDown == 1)
                {
                    // trigger Collision Assistant off
                    OSCtransmitter.BoolTrigger("CollisionTriggerOff", true);
                    Dashboard.DisplayCollisionWarning(false);
                }
                // Reduce CoolDown
                if (CollisionCoolDown > 0)
                {
                    CollisionCoolDown = CollisionCoolDown - 1;
                }
                // Check if time to Send Angle and Distance
                if (updateCounter == updateStep)
                {
                    OSCtransmitter.FloatTrigger("CollisionDistance", Collision.Item1 / CollisionAssistantMaxDistance);
                    float Angle = ((Collision.Item2) + 180) / 360;
                    OSCtransmitter.FloatTrigger("CollisionAngle", Angle);
                }
            }
            // If Collision Assistant is turned off but still triggered
            else if (CollisionCoolDown > 0)
            {
                // Reset
                OSCtransmitter.BoolTrigger("CollisionTriggerOff", true);
                Dashboard.DisplayCollisionWarning(false);
                CollisionCoolDown = 0;
            }

            // Parking Assistant
            if (ParkingAssistantState)
            {
                // Update ParkingAssistant
                var Parking = ParkingAssistant.ParkingUpdate();

                // Check if Collision is imminent
                if (Mathf.Abs(IVehicle.Speed) < ParkingAssistentMaxSpeed && Mathf.Abs(IVehicle.Speed) > 0.01f && Parking.Item1 < ParkingAssistentMaxDistance)
                {
                    // Check if first update to trigger Assistant
                    if (ParkingCoolDown == 0)
                    {
                        // trigger Parking Assistant On
                        OSCtransmitter.BoolTrigger("ParkingTriggerOn", true);
                        Dashboard.DisplayParkingWarning(true);
                    }
                    // Set CoolDown
                    ParkingCoolDown = CoolDownSteps;
                }
                // Check if CoolDown is over
                else if (ParkingCoolDown == 1)
                {
                    // trigger Parkin Assistant off
                    OSCtransmitter.BoolTrigger("ParkingTriggerOff", true);
                    Dashboard.DisplayParkingWarning(false);
                }
                // Reduce CoolDown
                if (ParkingCoolDown > 0)
                {
                    ParkingCoolDown = ParkingCoolDown - 1;
                }
                // Check if time to send Angle and Distance
                if (updateCounter == updateStep)
                {
                    OSCtransmitter.FloatTrigger("CollisionDistance", Parking.Item1 / ParkingAssistentMaxDistance);
                    float Angle = ((Parking.Item2) + 180) / 360;
                    OSCtransmitter.FloatTrigger("CollisionAngle", Angle);
                }
            }
            // if Parking Assistant is turned off but still triggered
            else if (ParkingCoolDown > 0)
            {
                // Reset
                OSCtransmitter.BoolTrigger("ParkingTriggerOff", true);
                Dashboard.DisplayParkingWarning(false);
                ParkingCoolDown = 0;
            }
            // Set and Reset UpdateCounter
            if (updateCounter == updateStep)
            {
                updateCounter = 0;
            }
            else
            {
                updateCounter = updateCounter +1;
            }
        }
    }
}