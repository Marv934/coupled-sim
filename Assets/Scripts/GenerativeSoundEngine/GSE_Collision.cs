using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenerativeSoundEngine
{
    public class GSE_Collision : MonoBehaviour
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
        [SerializeField] static float CoolDownTimer = 5.0f;

        [Header("Every <SendRate> Fixed Update Sends OSC Message")]
        // update Counter
        private int updateCounter = 0;
        [SerializeField] static private int updateStep = 1;

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
        [SerializeField] bool ParkingAssistantState = false;
        [SerializeField] bool CollisionAssistantState = false;
        [SerializeField] bool BlindSpotAssistantState = false;

        // CoolDown
        int CoolDownSteps = Convert.ToInt32(Mathf.Round(CoolDownTimer * 50 / updateStep));
        int CoolDown = 0;

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
            //updateCounter++;

            // Call Parking Assistant
            //var Parking = ParkingAssistant.ParkingUpdate();

            // Call Collision Assistant
            var Collision = CollisionAssistant.CollisionUpdate();

            //if (updateCounter == updateStep)
            //{
                // Check for Parking Assistant
                //if ( Mathf.Abs(IVehicle.Speed) < ParkingAssistentMaxSpeed && Mathf.Abs(IVehicle.Speed) > 0.001f && !CollisionAssistantState && !BlindSpotAssistantState && Parking.Item1 < ParkingAssistentMaxDistance)
                //{
                    // trigger Parking Assistant
                //    UpdateCollisionDetection(1, Parking.Item1, Parking.Item2);
                //
                //    if (!ParkingAssistantState)
                //    {
                //      OSCtransmitter.ParkingTrigger(true);
                //    }
                //    ParkingAssistantState = true;
                //    CoolDown = CoolDownSteps;
                //
                //    Dashboard.DisplayParkingWarning(true);
                //}
                // Check Collision Assistant
                //else
                if ( Mathf.Abs(IVehicle.Speed) > CollisionAssistantMinSpeed && !ParkingAssistantState && !BlindSpotAssistantState && Collision.Item1 < GSEVehicle.MaxSpeed * CollisionAssistantMaxDistance )
                {
                    // trigger Collision Assistant

                    UpdateCollisionDetection(2, Collision.Item1, Collision.Item2);

                    if (!CollisionAssistantState)
                    {
                        OSCtransmitter.CollisionTriggerOn();
                    }

                    CollisionAssistantState = true;
                    CoolDown = CoolDownSteps;

                    Dashboard.DisplayCollisionWarning(true);
                }
                //else if ( ??? && !ParkingAssistantState && !CollisionAssistantState)
                //{
                //    // trigger BlindSpot Assistant
                //    BlindSpotAssistantState = true;
                //    CoolDown = CoolDownSteps;
                //}
                //else if ( CoolDown == 1 )
                //{
                //    if (ParkingAssistantState)
                //    {
                //        OSCtransmitter.ParkingTrigger(false);
                //        ParkingAssistantState = false;
                //    }
                //    if (CollisionAssistantState)
                //    {
                //        OSCtransmitter.CollisionTriggerOff();
                //        CollisionAssistantState = false;
                //    }
                //    CoolDown -= CoolDown;
                //}
                else if ( CoolDown == 0 )
                {
                    //UpdateCollisionDetection(0, 0.0f, 0.0f);
                    //
                    //if (ParkingAssistantState)
                    //{
                    //    OSCtransmitter.ParkingTrigger(false);
                    //    //ParkingAssistantState = false;
                    //}
                    if (CollisionAssistantState)
                    {
                        OSCtransmitter.CollisionTriggerOff();
                        CollisionAssistantState = false;
                        Dashboard.DisplayCollisionWarning(false);
                    }

                    //ParkingAssistantState = false;
                    //CollisionAssistantState = false;
                    //BlindSpotAssistantState = false;

                    //Dashboard.DisplayParkingWarning(false);
                    //Dashboard.DisplayCollisionWarning(false);
                    //Dashboard.DisplayBlindSpotWarning(false, 0.0f);
                }
                else
                {
                    CoolDown = CoolDown -1;

                    //if (ParkingAssistantState && Parking.Item1 < ParkingAssistentMaxDistance)
                    //{
                    //    UpdateCollisionDetection(1, Parking.Item1, Parking.Item2);
                    //}
                    //else if ( CollisionAssistantState && Collision.Item1 < GSEVehicle.MaxSpeed * CollisionAssistantMaxDistance )
                    //{
                    //    UpdateCollisionDetection(2, Collision.Item1, Collision.Item2);
                    //}
                    //else
                    //{
                    //    UpdateCollisionDetection(0, 0.0f, 0.0f);
                    //}
                }


                // Check Blind Spot Assistant, now commented out
                //if (GSEVehicle.Indicator != 0 || Mathf.Abs(IVehicle.Speed) < ExitMinimumSpeed) 
                //{
                //    var BlindSpot = BlindSpotAssistant.BlindSpotUpdate();
                //    if (BlindSpot.Item1 < BlindSpotAssistantMaxDistance)
                //    {
                //        UpdateCollisionDetection(3, BlindSpot.Item1, BlindSpot.Item2);
                //    }
                //    else
                //    {
                //        UpdateCollisionDetection(0, 0.0f, 0.0f);
                //    }
                //} else
                
                // Parking Assistant
                //if ( ParkingAssistant )
                //{
                //    // Check Parking Assistant
                //    var Parking = ParkingAssistant.ParkingUpdate();
                //
                //    if ( Parking.Item1 < ParkingAssistentMaxDistance )
                //    {
                //        UpdateCollisionDetection(1, Parking.Item1, Parking.Item2);
                //    }
                //    else
                //    {
                //        UpdateCollisionDetection(0, 0.0f, 0.0f);
                //    }
                //} 
                //else if ( CollisionAssistantState ) 
                //{
                //    // Check Collision Assistant
                //    var Collision = CollisionAssistant.CollisionUpdate();
                //
                //    if (Collision.Item1 < GSEVehicle.MaxSpeed * CollisionAssistantMaxDistance)
                //    {
                //        UpdateCollisionDetection(2, Collision.Item1, Collision.Item2);
                //    } 
                //    else
                //    {
                //        UpdateCollisionDetection(0, 0.0f, 0.0f);
                //    }
                //}

                //if ( CoolDown > 0 )
                //{
                //    CoolDown -= CoolDown;
                //}

                //updateCounter = 0;
            //}
        }

        public void UpdateCollisionDetection(int Type, float Distance, float Angle)
        {
            OSCtransmitter.CollisionType(Type);
            OSCtransmitter.CollisionDistance(Distance);
            OSCtransmitter.CollisionAngle(Angle);

            //if (Type == 1)
            //{
            //    OSCtransmitter.CollisionTrigger(false);
        //        Dashboard.DisplayParkingWarning(true);
        //        Dashboard.DisplayCollisionWarning(false);
        //        Dashboard.DisplayBlindSpotWarning(false, Angle);
            //}
            //else if (Type == 2)
            //{
            //    OSCtransmitter.CollisionTrigger(true);
        //        Dashboard.DisplayParkingWarning(false);
        //        Dashboard.DisplayCollisionWarning(true);
        //        Dashboard.DisplayBlindSpotWarning(false, Angle);
            //} 
            //else
            //{
            //    OSCtransmitter.CollisionTrigger(false);
            //}
        //    else if (Type == 3)
        //    {
        //        Dashboard.DisplayParkingWarning(false);
        //        Dashboard.DisplayCollisionWarning(false);
        //        Dashboard.DisplayBlindSpotWarning(true, Angle);
        //    }
        
        }

    }

}