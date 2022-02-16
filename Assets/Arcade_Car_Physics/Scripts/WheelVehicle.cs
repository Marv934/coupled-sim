/*
 * This code is part of Arcade Car Physics for Unity by Saarg (2018)
 * 
 * This Version is Changed by Marv924 (2022) as part of Generative Sound Engine for Coupled Sim in Unity
 * Developped as part of the Sonic Interaction Design Seminar at Audiokomminikation Group, TU Berlin
 * 
 * This is distributed under the MIT Licence (see LICENSE.md for details)
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if MULTIOSCONTROLS
    using MOSC;
#endif

public interface IVehicle
{
    bool Handbrake { get; }
    float Speed { get; }
}

// Added for GSE
public interface GSEVehicle
{
    float Steering { get; }
    //float SteerAngle { get; }
    bool Reverse { get; }
    float Indicator { get; }
    bool Engine { get; }
    bool ParkAssistant { get; }
    float MaxSpeed { get; }
    float Throttle { get; }
}

namespace VehicleBehaviour {
    [RequireComponent(typeof(Rigidbody))]
    public class WheelVehicle : MonoBehaviour, IVehicle, GSEVehicle {
        
        [Header("Inputs")]
    #if MULTIOSCONTROLS
        [SerializeField] PlayerNumber playerId;
    #endif
        // If isPlayer is false inputs are ignored
        [SerializeField] bool isPlayer = true;
        public bool IsPlayer { get{ return isPlayer; } set{ isPlayer = value; } } 

        // Input names to read using GetAxis
        [SerializeField] string throttleInput = "Throttle";
        [SerializeField] string brakeInput = "Break";
        [SerializeField] string turnInput = "Horizontal";
        [SerializeField] string jumpInput = "Jump";
        [SerializeField] string driftInput = "Fire1";
	    [SerializeField] string boostInput = "Fire2";
        [SerializeField] string blinkersLeftInput = "blinker_left";
        [SerializeField] string blinkersRightInput = "blinker_right";
        [SerializeField] string blinkersClearInput = "blinker_clear";
        
        // Start/Stop Button added for GSE - Start
        [SerializeField] string EngineStartStop = "engine_start_stop";
        [SerializeField] string ParkAssistantStartStop = "park_assistant_start_stop";
        // Start/Stop Button added for GSE - Stop

        /* 
         *  Turn input curve: x real input, y value used
         *  My advice (-1, -1) tangent x, (0, 0) tangent 0 and (1, 1) tangent x
         */
        [SerializeField] AnimationCurve turnInputCurve = AnimationCurve.Linear(-1.0f, -1.0f, 1.0f, 1.0f);

        [Header("Wheels")]
        [SerializeField] WheelCollider[] driveWheel;
        public WheelCollider[] DriveWheel { get { return driveWheel; } }
        [SerializeField] WheelCollider[] turnWheel;

        public WheelCollider[] TurnWheel { get { return turnWheel; } }

        // This code checks if the car is grounded only when needed and the data is old enough
        bool isGrounded = false;
        int lastGroundCheck = 0;
        public bool IsGrounded { get {
            if (lastGroundCheck == Time.frameCount)
                return isGrounded;

            lastGroundCheck = Time.frameCount;
            isGrounded = true;
            foreach (WheelCollider wheel in wheels)
            {
                if (!wheel.gameObject.activeSelf || !wheel.isGrounded)
                    isGrounded = false;
            }
            return isGrounded;
        }}

        [Header("Behaviour")]
        /*
         *  Motor torque represent the torque sent to the wheels by the motor with x: speed in km/h and y: torque
         *  The curve should start at x=0 and y>0 and should end with x>topspeed and y<0
         *  The higher the torque the faster it accelerate
         *  the longer the curve the faster it gets
         */
        [SerializeField] AnimationCurve motorTorque = new AnimationCurve(new Keyframe(0, 200), new Keyframe(50, 300), new Keyframe(200, 0));
        [SerializeField] AnimationCurve deaccelerateMotorTorque = new AnimationCurve(new Keyframe(0, 400), new Keyframe(200, 600));

        // Differential gearing ratio
        [Range(2, 16)]
        [SerializeField] float diffGearing = 4.0f;
        public float DiffGearing { get { return diffGearing; } set { diffGearing = value; } }

        // Basicaly how hard it brakes
        [SerializeField] float brakeForce = 1250.0f;
        public float BrakeForce { get { return brakeForce; } set { brakeForce = value; } }

        // Max steering hangle, usualy higher for drift car
        [Range(0f, 50.0f)]
        [SerializeField] float steerAngle = 30.0f;
        public float SteerAngle { get { return steerAngle; } set { steerAngle = Mathf.Clamp(value, 0.0f, 50.0f); } }

        // The value used in the steering Lerp, 1 is instant (Strong power steering), and 0 is not turning at all
        [Range(0.001f, 1.0f)]
        [SerializeField] float steerSpeed = 0.2f;
        public float SteerSpeed { get { return steerSpeed; } set { steerSpeed = Mathf.Clamp(value, 0.001f, 1.0f); } }

        // How hight do you want to jump?
        [Range(1f, 1.5f)]
        [SerializeField] float jumpVel = 1.3f;
        public float JumpVel { get { return jumpVel; } set { jumpVel = Mathf.Clamp(value, 1.0f, 1.5f); } }

        // How hard do you want to drift?
        [Range(0.0f, 2f)]
        [SerializeField] float driftIntensity = 0.5f;
        public float DriftIntensity { get { return driftIntensity; } set { driftIntensity = Mathf.Clamp(value, 0.0f, 2.0f); }}

        // Reset Values
        Vector3 spawnPosition;
        Quaternion spawnRotation;

        /*
         *  The center of mass is set at the start and changes the car behavior A LOT
         *  I recomment having it between the center of the wheels and the bottom of the car's body
         *  Move it a bit to the from or bottom according to where the engine is
         */
        [SerializeField] Transform centerOfMass;

        // Force aplied downwards on the car, proportional to the car speed
        [Range(0.5f, 10f)]
        [SerializeField] float downforce = 1.0f;
        public float Downforce { get{ return downforce; } set{ downforce = Mathf.Clamp(value, 0, 5); } }     

        // When IsPlayer is false you can use this to control the steering
        float steering;
        public float Steering { get{ return steering; } set{ steering = Mathf.Clamp(value, -1f, 1f); } } 

        // When IsPlayer is false you can use this to control the throttle
        float throttle;
        public float Throttle { get{ return throttle; } set{ throttle = Mathf.Clamp(value, -1f, 1f); } } 

        // Like your own car handbrake, if it's true the car will not move
        [SerializeField] bool handbrake;
        public bool Handbrake { get{ return handbrake; } set{ handbrake = value; } } 
        
        // Use this to disable drifting
        [HideInInspector] public bool allowDrift = true;
        bool drift;
        public bool Drift { get{ return drift; } set{ drift = value; } }         

        // Use this to read the current car speed (you'll need this to make a speedometer)
        [SerializeField] float speed = 0.0f;
        public float Speed { get{ return speed; } }

        // maxSpeed added for GSE - Start

        [SerializeField] float maxSpeed = 60.0f;
        public float MaxSpeed { get { return maxSpeed; } }

        // maxSpeed added for GSE - End

        [Header("Particles")]
        // Exhaust fumes
        [SerializeField] ParticleSystem[] gasParticles;

        [Header("Boost")]
        // Disable boost
        [HideInInspector] public bool allowBoost = true;

        // Maximum boost available
        [SerializeField] float maxBoost = 10f;
        public float MaxBoost { get { return maxBoost; } set {maxBoost = value;} }

        // Current boost available
        [SerializeField] float boost = 10f;
        public float Boost { get { return boost; } set { boost = Mathf.Clamp(value, 0f, maxBoost); } }

        // Regen boostRegen per second until it's back to maxBoost
        [Range(0f, 1f)]
        [SerializeField] float boostRegen = 0.2f;
        public float BoostRegen { get { return boostRegen; } set { boostRegen = Mathf.Clamp01(value); } }

        /*
         *  The force applied to the car when boosting
         *  NOTE: the boost does not care if the car is grounded or not
         */
        [SerializeField] float boostForce = 5000;
        public float BoostForce { get { return boostForce; } set { boostForce = value; } }

        // Use this to boost when IsPlayer is set to false
        public bool boosting = false;

        public float breaking;
        // Use this to jump when IsPlayer is set to false
        public bool jumping = false;

        // Boost particles and sound
        [SerializeField] ParticleSystem[] boostParticles;
        [SerializeField] AudioClip boostClip;
        [SerializeField] AudioSource boostSource;
        [SerializeField] CarBlinkers blinkers;
        
        // Private variables set at the start
        Rigidbody _rb;
        WheelCollider[] wheels;

        // Added for GSE - Start
        // Blinkers
        float indicator = 0.0f;
        public float Indicator { get { return indicator; } }

        // Engine Start/Stop
        bool engine = false;
        public bool Engine { get { return engine; } set { engine = value; } }

        bool parkassistant = false;
        public bool ParkAssistant { get { return parkassistant; } }

        public GenerativeSoundEngine.GSE_AI_Test GSE_AI_Test = new GenerativeSoundEngine.GSE_AI_Test();
        public DateTime startDate;

        GenerativeSoundEngine.GSE_OSCtransmitter OSCtransmitter;
        GenerativeSoundEngine.GSE_Dashboard Dashboard;
        // Added for GSE - End

        // Init rigidbody, center of mass, wheels and more
        void Start() {
#if MULTIOSCONTROLS
            Debug.Log("[ACP] Using MultiOSControls");
#endif
            if (boostClip != null) {
                boostSource.clip = boostClip;
            }

		    boost = maxBoost;

            _rb = GetComponent<Rigidbody>();
            spawnPosition = transform.position;
            spawnRotation = transform.rotation;

            if (_rb != null && centerOfMass != null)
            {
                _rb.centerOfMass = centerOfMass.localPosition;
            }

            wheels = GetComponentsInChildren<WheelCollider>();

            // Set the motor torque to a non null value because 0 means the wheels won't turn no matter what
            foreach (WheelCollider wheel in wheels)
            {
                wheel.motorTorque = 0.0001f;
            }

            // Added for GSE - Start
            startDate = DateTime.Now;

            OSCtransmitter = GetComponent<GenerativeSoundEngine.GSE_OSCtransmitter>();
            Dashboard = GetComponentInChildren<GenerativeSoundEngine.GSE_Dashboard>();
            // Added for GSE - End

        }

        bool reverse = false;
        // Added for GSE - Start
        public bool Reverse { get { return reverse; } }

        // Added for GSE - End

        // Visual feedbacks and boost regen
        void Update()
        {
            foreach (ParticleSystem gasParticle in gasParticles)
            {
                gasParticle.Play();
                ParticleSystem.EmissionModule em = gasParticle.emission;
                em.rateOverTime = handbrake ? 0 : Mathf.Lerp(em.rateOverTime.constant, Mathf.Clamp(150.0f * throttle, 30.0f, 100.0f), 0.1f);
            }

            if (isPlayer && allowBoost) {
                boost += Time.deltaTime * boostRegen;
                if (boost > maxBoost) { boost = maxBoost; }
            }

            // Get all the inputs!
            if (isPlayer)
            {
                // Added for GSE - Start
                
                // Park Assistant Start/Stop
                if (Input.GetButtonDown("park_assistant_start_stop"))
                {
                    parkassistant = parkassistant != true;
                }

                // Added for GSE - End

                if (Input.GetButtonDown("forward"))
                {
                    reverse = false;
                    OSCtransmitter.Forward();
                } else if (Input.GetButtonDown("reverse"))
                {
                    reverse = true;
                    OSCtransmitter.Reverse();
                }

                if (Input.GetButtonDown("blinker_left"))
                {
                    if (blinkers.State != BlinkerState.Left)
                    {
                        blinkers.StartLeftBlinkers();
                        // Added for GSE - Start
                        /// = -1.0f;
                        OSCtransmitter.BoolTrigger("BlinkerOn", true);
                        // Added for GSE - End
                    }
                    else
                    {
                        blinkers.Stop();
                        // Added for GSE - Start
                        indicator = 0.0f;
                        OSCtransmitter.BoolTrigger("BlinkerOff", true);
                        // Added for GSE - End
                    }
                }
                else if (Input.GetButtonDown("blinker_right"))
                {
                    if (blinkers.State != BlinkerState.Right)
                    {
                        blinkers.StartRightBlinkers();
                        // Added for GSE - Start
                        indicator = 1.0f;
                        OSCtransmitter.BoolTrigger("BlinkerOn", true);
                        // Added for GSE - End
                    }
                    else
                    {
                        blinkers.Stop();
                        // Added for GSE - Start
                        indicator = 0.0f;
                        OSCtransmitter.BoolTrigger("BlinkerOff", true);
                        // Added for GSE - End
                    }
                }
                else if (Input.GetButtonDown("blinker_clear"))
                {
                    blinkers.Stop();
                    // Added for GSE - Start
                    indicator = 0.0f;
                    OSCtransmitter.BoolTrigger("BlinkerOff", true);
                    // Added for GSE - End
                }
            // Added for GSE - AI-Test - Start
            }
            else if (!isPlayer)
            {

                GSE_AI_Test.Timer(startDate, DateTime.Now);

                // Engine Start/Stop
                GSE_AI_Test.CheckEngine(out engine, out handbrake);

                // Blinker Left
                GSE_AI_Test.CheckBlinkers(blinkers, out indicator);

                // Reverse
                GSE_AI_Test.CheckReverse(out reverse);

                // Added for GSE - AI-Test - End
            }
        }

        float steeringWheelAngle = 0;
        public Transform steeringWheel;
        public float steeringWheelMul = -2;
        // Update everything
        void FixedUpdate () {
            // Added for GSE - Start

            // max out velocity
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, maxSpeed / 3.6f);

            // Added for GSE - End
            // Mesure current speed

            speed = transform.InverseTransformDirection(_rb.velocity).z * 3.6f;

            // Get all the inputs!
            if (isPlayer) {
                // Accelerate & brake
                if (throttleInput != "" && throttleInput != null)
                {
                    throttle = GetInput(throttleInput) * (reverse ? -1f : 1);
                }
                //breaking = Mathf.Clamp01(GetInput(brakeInput));
                
                // Added for GSE - Start
                if (engine)
                {
                    breaking = Mathf.Clamp01(GetInput(brakeInput));
                } else if (!engine)
                {
                    breaking = 1.0f;
                }
                // Added for GSE - End

                // Turn
                Debug.Log("turnInput: " + GetInput(turnInput));

                steering = turnInputCurve.Evaluate(GetInput(turnInput)) * steerAngle;

                Debug.Log("Steering: " + steering);

            // Added for GSE - AI-Test - Start
            } else if (!isPlayer)
            {
                // Accelerate & brake
                // - throttle
                GSE_AI_Test.CheckThrottle(out throttle);

                // - breaking
                GSE_AI_Test.CheckBreaking(out breaking);

                // Steering
                GSE_AI_Test.CheckSteering(out steering);

                // Speed
                GSE_AI_Test.CheckSpeed(out speed, _rb);

                // Added for GSE - AI-Test - End
            }

            steeringWheelAngle = Mathf.Lerp(steeringWheelAngle, steering * steeringWheelMul, steerSpeed);
            if (steeringWheel != null) {
                steeringWheel.localRotation = Quaternion.AngleAxis(steeringWheelAngle, Vector3.forward);
            }

            // Direction
            foreach (WheelCollider wheel in turnWheel)
            {
                wheel.steerAngle = Mathf.Lerp(wheel.steerAngle, steering, steerSpeed);
            }

            foreach (WheelCollider wheel in wheels)
            {
                wheel.brakeTorque = 0;
            }

            // Handbrake
            if (Mathf.Abs(speed) < 2 && GetInput(throttleInput) < 0.1f)
            {
                foreach (WheelCollider wheel in wheels)
                {
                    // Don't zero out this value or the wheel completly lock up
                    wheel.motorTorque = 0.0001f;
                    wheel.brakeTorque = brakeForce;
                }
            }
            else
            {
                foreach (WheelCollider wheel in driveWheel)
                {
                    if ((speed <= 0 && !reverse && throttle < 0) || (speed >= 0 && reverse && throttle > 0))
                    {
                        wheel.motorTorque = 0;
                    } else
                    {
                        if (throttle > 0) {
                            wheel.motorTorque = throttle * motorTorque.Evaluate(speed) * diffGearing / driveWheel.Length;
                        } else
                        {
                            wheel.motorTorque = throttle * deaccelerateMotorTorque.Evaluate(speed) * diffGearing / driveWheel.Length;
                        }
                    }
                }
                foreach (WheelCollider wheel in wheels)
                {
                    wheel.brakeTorque = Mathf.Abs(breaking) * brakeForce;
                }
            }

            // Jump
            if (jumping && isPlayer) {
                if (!IsGrounded)
                    return;
                
                _rb.velocity += transform.up * jumpVel;
            }

            // Boost
            if (boosting && allowBoost && boost > 0.1f) {
                _rb.AddForce(transform.forward * boostForce);

                boost -= Time.fixedDeltaTime;
                if (boost < 0f) { boost = 0f; }

                if (boostParticles.Length > 0 && !boostParticles[0].isPlaying) {
                    foreach (ParticleSystem boostParticle in boostParticles) {
                        boostParticle.Play();
                    }
                }

                if (boostSource != null && !boostSource.isPlaying) {
                    boostSource.Play();
                }
            } else {
                if (boostParticles.Length > 0 && boostParticles[0].isPlaying) {
                    foreach (ParticleSystem boostParticle in boostParticles) {
                        boostParticle.Stop();
                    }
                }

                if (boostSource != null && boostSource.isPlaying) {
                    boostSource.Stop();
                }
            }

            // Drift
            if (drift && allowDrift) {
                Vector3 driftForce = -transform.right;
                driftForce.y = 0.0f;
                driftForce.Normalize();

                if (steering != 0)
                    driftForce *= _rb.mass * speed/7f * throttle * steering/steerAngle;
                Vector3 driftTorque = transform.up * 0.1f * steering/steerAngle;


                _rb.AddForce(driftForce * driftIntensity, ForceMode.Force);
                _rb.AddTorque(driftTorque * driftIntensity, ForceMode.VelocityChange);             
            }
            
            // Downforce
            _rb.AddForce(-transform.up * speed * downforce);
        }

        // Reposition the car to the start position
        public void ResetPos() {
            transform.position = spawnPosition;
            transform.rotation = spawnRotation;

            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        public void toogleHandbrake(bool h)
        {
            handbrake = h;
        }

        // MULTIOSCONTROLS is another package I'm working on ignore it I don't know if it will get a release.
#if MULTIOSCONTROLS
        private static MultiOSControls _controls;
#endif

        // Use this method if you want to use your own input manager
        private float GetInput(string input) {
#if MULTIOSCONTROLS
        return MultiOSControls.GetValue(input, playerId);
#else
        return Input.GetAxis(input);
#endif
        }
    }
}
