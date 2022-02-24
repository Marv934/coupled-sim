using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CollisionCoolDown
{
    int Collision { get; }
    int Parking { get; }
}

namespace GenerativeSoundEngine
{
    public class GSE_Collision : MonoBehaviour, CollisionCoolDown
    {
        // Set Collision type Variants
        [Header("Parking Assistant, static")]
        [SerializeField] float ParkingAssistentMaxSpeed = 5.0f;
        [SerializeField] float ParkingAssistentMaxDistance = 2.5f;

        [Header("Collision Assistant, times maxSpeed")]
        [SerializeField] float CollisionAssistantMaxDistance = 0.5f;
        [SerializeField] float CollisionAssistantMinSpeed = 5.0f;

        [Header("Blind Spot Assistant, static")]
        [SerializeField] float BlindSpotAssistantMaxDistance = 10.0f;
        [SerializeField] float ExitMinimumSpeed = 0.2f;

        [Header("CoolDownTimer in Seconds")]
        [SerializeField] static float CoolDownTimer = 10.0f;

        [Header("Every <SendRate> Fixed Update Sends OSC Message")]
        // update Counter
        private int updateCounter = 0;
        [SerializeField] static private int updateStep = 6;

        // Init WheelVehicle
        private IVehicle IVehicle;
        private GSEVehicle GSEVehicle;

        // Init Parking Assistant
        GSE_ParkingAssistant ParkingAssistant;
        // Init Collision Assistant
        GSE_CollisionAssistant CollisionAssistant;
        // Init Blind Spot Assistant
        GSE_BlindSpotAssistant BlindSpotAssistant;

        // Init OSC Transmitter
        GSE_OSCtransmitter OSCtransmitter;
        // Init Dashboard
        GSE_Dashboard Dashboard;

        // States
        [SerializeField] public bool ParkingAssistantState = false;
        [SerializeField] public bool CollisionAssistantState = false;
        [SerializeField] public bool BlindSpotAssistantState = false;

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
            GSEVehicle = GetComponent<VehicleBehaviour.WheelVehicle>();

            // Get Parking Assistant
            ParkingAssistant = GetComponentInChildren<GSE_ParkingAssistant>();
            // Get Collision Assistant
            CollisionAssistant = GetComponentInChildren<GSE_CollisionAssistant>();
            // Get Blind Spot Assistant
            BlindSpotAssistant = GetComponentInChildren<GSE_BlindSpotAssistant>();

            // Get OSC Transmitter
            OSCtransmitter = GetComponent<GSE_OSCtransmitter>();

            // Get Dashboard
            Dashboard = GetComponentInChildren<GSE_Dashboard>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {

            if (CollisionAssistantState)
            {
                // Collision Assistant
                var Collision = CollisionAssistant.CollisionUpdate();

                if (Mathf.Abs(IVehicle.Speed) > CollisionAssistantMinSpeed && Collision.Item1 < GSEVehicle.MaxSpeed * CollisionAssistantMaxDistance)
                {
                    if (CollisionCoolDown == 0)
                    {
                        // trigger Collision Assistant On

                        OSCtransmitter.BoolTrigger("CollisionTriggerOn", true);

                        CollisionCoolDown = CoolDownSteps;

                        Dashboard.DisplayCollisionWarning(true);
                    }

                    CollisionCoolDown = CoolDownSteps;
                }
                else if (CollisionCoolDown == 1)
                {
                    // trigger Collision Assistant off

                    OSCtransmitter.BoolTrigger("CollisionTriggerOff", true);

                    Dashboard.DisplayCollisionWarning(false);
                }
                if (CollisionCoolDown > 0)
                {
                    CollisionCoolDown = CollisionCoolDown - 1;
                }

                if (updateCounter == updateStep)
                {
                    OSCtransmitter.FloatTrigger("CollisionDistance", Collision.Item1 / CollisionAssistantMaxDistance);
                    float Angle = ((Collision.Item2) + 180) / 360;
                    OSCtransmitter.FloatTrigger("CollisionAngle", Angle);
                }
            }
            else if (CollisionCoolDown > 0)
            {
                OSCtransmitter.BoolTrigger("CollisionTriggerOff", true);

                Dashboard.DisplayCollisionWarning(false);

                CollisionCoolDown = 0;
            }

            if (ParkingAssistantState)
            {
                // Parking Assistant
                var Parking = ParkingAssistant.ParkingUpdate();

                if (Mathf.Abs(IVehicle.Speed) < ParkingAssistentMaxSpeed && Mathf.Abs(IVehicle.Speed) > 0.01f && Parking.Item1 < ParkingAssistentMaxDistance)
                {
                    if (ParkingCoolDown == 0)
                    {
                        // trigger Parking Assistant On

                        OSCtransmitter.BoolTrigger("ParkingTriggerOn", true);

                        ParkingCoolDown = CoolDownSteps;

                        Dashboard.DisplayParkingWarning(true);
                    }

                    ParkingCoolDown = CoolDownSteps;
                }
                else if (ParkingCoolDown == 1)
                {
                    // trigger Parkin Assistant off

                    OSCtransmitter.BoolTrigger("ParkingTriggerOff", true);

                    Dashboard.DisplayParkingWarning(false);
                }
                if (ParkingCoolDown > 0)
                {
                    ParkingCoolDown = ParkingCoolDown - 1;
                }

                if (updateCounter == updateStep)
                {
                    OSCtransmitter.FloatTrigger("CollisionDistance", Parking.Item1 / ParkingAssistentMaxDistance);
                    float Angle = ((Parking.Item2) + 180) / 360;
                    OSCtransmitter.FloatTrigger("CollisionAngle", Angle);
                }
            }
            else if (ParkingCoolDown > 0)
            {
                OSCtransmitter.BoolTrigger("ParkingTriggerOff", true);

                Dashboard.DisplayParkingWarning(false);

                ParkingCoolDown = 0;
            }
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