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
        float Proximity { get; }
        float ProximityAngle { get; }
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

        // Proximity
        float proximity = 1.1f;
        public float Proximity { get { return proximity; } }

        float proximityAngle = 0.0f;
        public float ProximityAngle { get { return proximityAngle; } }

        // Init Interfaces from other Classes

        // Wheel Vehicle
        private IVehicle Vehicle;
        private GSEVehicle Vehicle_GSE;

        // Rigidbody 
        public Vector3 center;
        public Rigidbody rb;

        // Collidision Calculation
        void CollisionCalculator(out float proximity, out float proximityAngle)
        {
            // get rigidbody center
            rb = GetComponent<Rigidbody>();
            center = rb.position;

            proximity = 0.0f;
            proximityAngle = 0.0f;

            // Collect Colliders
            Collider[] Colliders = Physics.OverlapSphere(center, 10);

            // get nearest Collider
            float dist_clostest = 11.0f;
            foreach (var Collider in Colliders)
            {
                proximityAngle = proximityAngle + 1.0f;
                Vector3 closestPoint = Collider.ClosestPoint(center);
                float dist_current = Vector3.Distance(center, closestPoint);
                if ((dist_current < dist_clostest) && (dist_current > 0.0f))
                {
                    dist_clostest = dist_current;
                }
            }
            //
            proximity = dist_clostest;
            //proximityAngle = 10.0f;
        }

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

            // Proximity
            CollisionCalculator(out proximity, out proximityAngle);

        }
    }
}
