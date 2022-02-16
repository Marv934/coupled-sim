using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenerativeSoundEngine
{
    public class GSE_Dashboard : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody carBody;

        [Header("Analog")]

        [Header("Speed")]
        [SerializeField]
        private Transform pivotSpeed; //arrow pivot
        [SerializeField]
        private float pivotMinSpeedAngle = 120f; //arrow inclination when speed = 0
        [SerializeField]
        private float pivotMaxSpeedAngle = -120f; //arrow inclination when speed = maxSpeed
        [SerializeField]
        private float maxSpeed = 160f;

        [Header("Power")]
        [SerializeField]
        private Transform pivotPower; //arrow pivot
        [SerializeField]
        private float pivotMinPowerAngle = 120f; //arrow inclination when speed = 0
        [SerializeField]
        private float pivotMaxPowerAngle = -120f; //arrow inclination when speed = maxSpeed
        [SerializeField]
        private float maxPower = 1.0f;

        [Header("Collision Warning")]
        private bool CollisionWarningState = false;
        [SerializeField]
        private GameObject CollisionWarning;

        [Header("Blind Spot Warning")]
        private bool BlindSpotWarningState = false;
        [SerializeField]
        private GameObject BlindSpotWarningLeft;
        [SerializeField]
        private GameObject BlindSpotWarningRight;

        [Header("Battery Warning")]
        private bool BatteryWarningState = false;
        [SerializeField]
        private GameObject BatteryWarning;
        [SerializeField]
        private GameObject BatteryWarningIcon;

        [Header("Tire Pressure Warning")]
        private bool TirePressureWarningState = false;
        [SerializeField]
        private GameObject TirePressureWarning;
        [SerializeField]
        private GameObject TirePressureWarningIcon;

        [Header("SMS Info")]
        private bool SMSInfoState = false;
        [SerializeField]
        private GameObject SMSInfo;
        [SerializeField]
        private GameObject SMSInfoIcon;

        [Header("SW Update Info")]
        private bool SWUpdateInfoState = false;
        [SerializeField]
        private GameObject SWUpdateInfo;
        [SerializeField]
        private GameObject SWUpdateInfoIcon;

        [Header("ServiceInfo")]
        private bool ServiceInfoState = false;
        [SerializeField]
        private GameObject ServiceInfo;
        [SerializeField]
        private GameObject ServiceInfoIcon;

        [Header("Shutdown Info")]
        private bool ShutdownInfoState = true;
        [SerializeField]
        private GameObject ShutdownInfo;

        [Header("Parking Warning")]
        private bool ParkingWarningState = false;
        [SerializeField]
        private GameObject ParkingWarning;

        private bool CollisionState = false;
        private bool WarningState = false;
        private bool InfoState = false;

        GSEVehicle Vehicle;

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
            if ( CollisionWarningState || BlindSpotWarningState || ParkingWarningState )
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

            if ( SMSInfoState || SWUpdateInfoState || ServiceInfoState )
            {
                InfoState = true;
            }
            else
            {
                InfoState = false;
            }

        }

        IEnumerator SetSMSActiveFor(float Seconds)
        {
            // Set Active
            SMSInfo.SetActive(true);
            // Set State
            SMSInfoState = true;
            // Wait for Seconds
            yield return new WaitForSeconds(Seconds);
            // Unset Active
            SMSInfo.SetActive(false);
            // Unset State
            SMSInfoState = false;
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
            // Set State
            SWUpdateInfoState = true;
            // Wait for Seconds
            yield return new WaitForSeconds(Seconds);
            // Unset Active
            SWUpdateInfo.SetActive(false);
            // Unset State
            SWUpdateInfoState = false;
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
            // Set State
            ServiceInfoState = true;
            // Wait for Seconds
            yield return new WaitForSeconds(Seconds);
            // Unset Active
            ServiceInfo.SetActive(false);
            // Unset State
            ServiceInfoState = false;
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
            // Called by BlindSpot

            // Overwrite Shutdown
            ShutdownInfo.SetActive(false);
        }

        private void ResetShutdown()
        {
            // Called by BlindSpot

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

            OverwriteWarning();
            OverwriteInfo();
        }

        public void DisplayEngineStop()
        {
            // Shutdown Engine
            ShutdownInfoState = true;
            ShutdownInfo.SetActive(true);
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

        public void DisplayBlindSpotWarning(bool State, float Direction)
        {
            if (State != BlindSpotWarningState)
            {
                if (State)
                {
                    // Turn on CollisionWarning
                    BlindSpotWarningState = State;

                    if (Direction > 0)
                    {
                        BlindSpotWarningLeft.SetActive(true);
                        BlindSpotWarningRight.SetActive(false);
                    }
                    else
                    {
                        BlindSpotWarningLeft.SetActive(false);
                        BlindSpotWarningRight.SetActive(true);
                    }

                    // Ovewrite Other Displays
                    OverwriteWarning();
                    OverwriteInfo();
                    OverwriteShutdown();
                }
                else
                {
                    // Turn off CollisionWarning
                    BlindSpotWarningState = State;
                    BlindSpotWarningLeft.SetActive(false);
                    BlindSpotWarningRight.SetActive(false);

                    // Reset other Displays
                    ResetShutdown();
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
    }
}