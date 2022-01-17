/*
 * This code is part of Generative Sound Engine
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenerativeSoundEngine
{

    // Interface for GSE_Collector
    public interface GSE_Data
    {
        float Speed { get; }
        float Steering { get; }
        float Reverse { get; }
        float Indicator { get; }
        float Engine { get; }
    }

    // Requiered Components
    [RequireComponent(typeof(VehicleBehaviour.WheelVehicle))]

    public class GSE_Collector : MonoBehaviour, GSE_Data
    {

        // Init Interface members

        // Speed
        float speed = 0.0f;
        public float Speed { get { return speed; } }

        // SteerAngle
        float steering = 0.0f;
        public float Steering { get { return steering; } }

        // Reverse
        float reverse = 0.0f;
        public float Reverse { get { return reverse; } }

        // Blinker
        float indicator = 0.5f;
        public float Indicator { get { return indicator; } }

        // Engine
        float engine = 0.0f;
        public float Engine { get { return engine; } }

        // Init Interfaces from other Classes

        // Wheel Vehicle
        private IVehicle Vehicle;
        private GSEVehicle Vehicle_GSE;

        // Start is called before the first frame update
        void Start()
        {

            // Get Components from other Classes
            Vehicle = GetComponent<VehicleBehaviour.WheelVehicle>();
            Vehicle_GSE = GetComponent<VehicleBehaviour.WheelVehicle>();


        }

        // Update is called once per frame
        void Update()
        {

            // Update Interface members

            // Speed - normiert
            speed = Math.Abs(Vehicle.Speed) / Vehicle_GSE.MaxSpeed;

            // SteerAngle - normiert
            steering = Vehicle_GSE.Steering/(2*Vehicle_GSE.SteerAngle) + 0.5f;

            // Reverse
            reverse = Vehicle_GSE.Reverse ? 1.0f : 0.0f;

            // Blinker
            indicator = Vehicle_GSE.Indicator;

            // Engine
            engine = Vehicle_GSE.Engine ? 1.0f : 0.0f;

        }
    }
}
