/*
 * This code is part of Generative Sound Engine for Coupled Sim in Unity by Marv934 (2022)
 * Developped as part of the Sonic Interaction Design Seminar at Audiokomminikation Group, TU Berlin
 * 
 * This is distributed under the MIT Licence (see LICENSE.md for details)
 */

/*
 * This script controlls the Dashboard via callable Methods
 *
 * Added public Methods:
 *      - DisplaySMSInfo(bool state)
 *      - DisplaySWUpdateInfo(bool state)
 *      - DisplayServiceInfo(bool state)
 *      - DisplayBatteryWarning(bool state)
 *      - DisplayTirePressureWarning(bool state)
 *      - DisplayEngineStart()
 *      - DisplayEngineStop()
 *      - DisplayCollisionWarning(bool state)
 *      - DisplayParkingWarning(bool state)
 *      - DisplayDrive()
 *      - DisplayReverse()
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealTimeAuralizationEngine
{
    public class RAE_Dashboard : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody carBody = null;

        // Init Speed and Power Indicator Transforms
        [Header("Speed")]
        [SerializeField]
        private Transform pivotSpeed = null; //arrow pivot
        private float pivotMinSpeedAngle = 120f; //arrow inclination when speed = 0
        private float pivotMaxSpeedAngle = -120f; //arrow inclination when speed = maxSpeed 
        private float maxSpeed = 160f;

        [Header("Power")]
        [SerializeField]
        private Transform pivotPower = null; //arrow pivot
        private float pivotMinPowerAngle = 120f; //arrow inclination when Power = 0
        private float pivotMaxPowerAngle = -120f; //arrow inclination when sPower = maxPower
        private float maxPower = 1.0f;

        // Init Display GameObjects
        private bool DriveState = true;
        [Header("Drive")]
        [SerializeField]
        private GameObject Drive = null;

        private bool ReverseState = false;
        [Header("Reverse")]
        [SerializeField]
        private GameObject Reverse = null;

        private bool CollisionWarningState = false;
        [Header("Collision Warning")]
        [SerializeField]
        private GameObject CollisionWarning = null;

        private bool ParkingWarningState = false;
        [Header("Parking Warning")]
        [SerializeField]
        private GameObject ParkingWarning = null;

        private bool BatteryWarningState = false;
        [Header("Battery Warning")]
        [SerializeField]
        private GameObject BatteryWarning = null;
        [SerializeField]
        private GameObject BatteryWarningIcon = null;

        private bool TirePressureWarningState = false;
        [Header("Tire Pressure Warning")]
        [SerializeField]
        private GameObject TirePressureWarning = null;
        [SerializeField]
        private GameObject TirePressureWarningIcon = null;

        [Header("SMS Info")]
        [SerializeField]
        private GameObject SMSInfo = null;
        [SerializeField]
        private GameObject SMSInfoIcon = null;

        [Header("SW Update Info")]
        [SerializeField]
        private GameObject SWUpdateInfo = null;
        [SerializeField]
        private GameObject SWUpdateInfoIcon = null;

        [Header("ServiceInfo")]
        [SerializeField]
        private GameObject ServiceInfo = null;
        [SerializeField]
        private GameObject ServiceInfoIcon = null;

        private bool ShutdownInfoState = true;
        [Header("Shutdown Info")]
        [SerializeField]
        private GameObject ShutdownInfo = null;

        // Init Display Priority states
        private bool CollisionState = false;
        private bool WarningState = false;

        // Init Vehicle
        RAEVehicle Vehicle;

        // Start is called before the first frame update
        void Start()
        {
            Vehicle = GetComponentInParent<VehicleBehaviour.WheelVehicle>();

        }

        void Update()
        {
            // Speed
            float v = carBody.velocity.magnitude * SpeedConvertion.Mps2Kmph;

            var angleSpeed = Mathf.Lerp(pivotMinSpeedAngle, pivotMaxSpeedAngle, v / maxSpeed);
            if (pivotSpeed != null)
            {
                pivotSpeed.localRotation = Quaternion.AngleAxis(angleSpeed, Vector3.forward);
            }   

            // Power
            var anglePower = Mathf.Lerp(pivotMinPowerAngle, pivotMaxPowerAngle, Vehicle.Throttle / maxPower);
            if (pivotPower != null)
            {
                pivotPower.localRotation = Quaternion.AngleAxis(anglePower, Vector3.forward);
            }

            // Set States
            if ( CollisionWarningState || ParkingWarningState )
            {
                CollisionState = true;
            } else
            { 
                CollisionState = false;
            }

            if ( BatteryWarningState || TirePressureWarningState )
            {
                WarningState = true;
            }
            else
            {
                WarningState = false;
            }

        }

        IEnumerator SetSMSActiveFor(float Seconds)
        {
            // Set Active
            SMSInfo.SetActive(true);
            // Wait for Seconds
            yield return new WaitForSeconds(Seconds);
            // Unset Active
            SMSInfo.SetActive(false);
        }

        public void DisplaySMSInfo(bool State)
        {
            // Set Icon
            SMSInfoIcon.SetActive(State);

            // Overwrite Info
            OverwriteInfo();

            // Set Display
            if (State && !CollisionState && !WarningState && !ShutdownInfoState)
            {
                StartCoroutine(SetSMSActiveFor(5.0f));
            }
        }

        IEnumerator SetSWUpdateActiveFor(float Seconds)
        {
            // Set Active
            SWUpdateInfo.SetActive(true);
            // Wait for Seconds
            yield return new WaitForSeconds(Seconds);
            // Unset Active
            SWUpdateInfo.SetActive(false);
        }

        public void DisplaySWUpdateInfo(bool State)
        {
            // Set Icon
            SWUpdateInfoIcon.SetActive(State);

            // Overwrite Info
            OverwriteInfo();

            // Set Display
            if (State && !CollisionState && !WarningState && !ShutdownInfoState)
            {
                StartCoroutine(SetSWUpdateActiveFor(5.0f));
            }
        }

        IEnumerator SetServiceActiveFor(float Seconds)
        {
            // Set Active
            ServiceInfo.SetActive(true);
            // Wait for Seconds
            yield return new WaitForSeconds(Seconds);
            // Unset Active
            ServiceInfo.SetActive(false);
        }

        public void DisplayServiceInfo(bool State)
        {
            // Set Icon
            ServiceInfoIcon.SetActive(State);

            // Overwrite Info
            OverwriteInfo();

            // Set Display
            if (State && !CollisionState && !WarningState && !ShutdownInfoState)
            {
                StartCoroutine(SetServiceActiveFor(5.0f));
            }
        }

        private void OverwriteInfo()
        {
            // Called by Collisions and Warnings

            // Overwrite SMS Info
            SMSInfo.SetActive(false);

            // Overwrite SW Update
            SWUpdateInfo.SetActive(false);

            // Overwrite Service
            ServiceInfo.SetActive(false);
        }

        IEnumerator SetBatteryActiveFor(float Seconds)
        {
            // Set Active
            BatteryWarning.SetActive(true);
            // Set State
            BatteryWarningState = true;
            // Wait for Seconds
            yield return new WaitForSeconds(Seconds);
            // Unset Active
            BatteryWarning.SetActive(false);
            // Unset State
            BatteryWarningState = false;
        }

        public void DisplayBatteryWarning( bool State)
        {
            // Set Icon
            BatteryWarningIcon.SetActive(State);

            // Overwrite Info
            OverwriteInfo();
            OverwriteWarning();

            // Set Display
            if (State && !CollisionState && !ShutdownInfoState)
            {
                StartCoroutine(SetBatteryActiveFor(5.0f));
            }
        }

        IEnumerator SetTirePressureActiveFor(float Seconds)
        {
            // Set Active
            TirePressureWarning.SetActive(true);
            // Set State
            TirePressureWarningState = true;
            // Wait for Seconds
            yield return new WaitForSeconds(Seconds);
            // Unset Active
            TirePressureWarning.SetActive(false);
            // Unset State
            TirePressureWarningState = false;
        }

        public void DisplayTirePressureWarning ( bool State )
        {
            // Set Icon
            TirePressureWarningIcon.SetActive(State);

            // Overwrite Info
            OverwriteInfo();
            OverwriteWarning();

            // Set Display
            if (State && !CollisionState && !ShutdownInfoState)
            {
                StartCoroutine(SetTirePressureActiveFor(5.0f));
            }
        }

        private void OverwriteWarning()
        {
            // Called by Collisions

            // Overwrite Battery Warning
            BatteryWarning.SetActive(false);

            // Overwrite Tire Pressure Warning
            TirePressureWarning.SetActive(false);
        }

        private void OverwriteShutdown()
        {
            // Overwrite Shutdown
            ShutdownInfo.SetActive(false);
        }

        private void ResetShutdown()
        {
            // Reset Shutdown
            if (ShutdownInfoState)
            {
                ShutdownInfo.SetActive(true);
            }
        }

        public void DisplayEngineStart()
        {
            // Startup Engine
            ShutdownInfoState = false;
            ShutdownInfo.SetActive(false);

            if (DriveState)
            {
                Drive.SetActive(true);
            }
            else if (ReverseState)
            {
                Reverse.SetActive(true);
            }

            OverwriteWarning();
            OverwriteInfo();
        }

        public void DisplayEngineStop()
        {
            // Shutdown Engine
            ShutdownInfoState = true;
            ShutdownInfo.SetActive(true);

            // Set Drive false
            Drive.SetActive(false);
            Reverse.SetActive(false);
        }

        public void DisplayCollisionWarning(bool State)
        {
            if ( State != CollisionWarningState && !ShutdownInfoState)
            {
                if (State)
                {
                    // Turn on CollisionWarning
                    CollisionWarningState = State;
                    CollisionWarning.SetActive(true);

                    // Ovewrite Other Displays
                    OverwriteWarning();
                    OverwriteInfo();
                } else
                {
                    // Turn off CollisionWarning
                    CollisionWarningState = State;
                    CollisionWarning.SetActive(false);
                }
            }
        }

        public void DisplayParkingWarning(bool State)
        {
            if (State != ParkingWarningState && !ShutdownInfoState)
            {
                if (State)
                {
                    // Turn on CollisionWarning
                    ParkingWarningState = State;
                    ParkingWarning.SetActive(true);

                    // Ovewrite Other Displays
                    OverwriteWarning();
                    OverwriteInfo();
                }
                else
                {
                    // Turn off CollisionWarning
                    ParkingWarningState = State;
                    ParkingWarning.SetActive(false);
                }
            }
        }

        public void DisplayDrive()
        {
            // Set Reverse false
            ReverseState = false;
            Reverse.SetActive(false);

            // Set Drive true
            DriveState = true;
            Drive.SetActive(true);
        }

        public void DisplayReverse()
        {
            // Set Reverse true
            ReverseState = true;
            Reverse.SetActive(true);

            // Set Drive false
            DriveState = false;
            Drive.SetActive(false);
        }
    }
}