/*
 * This code is part of Generative Sound Engine for Coupled Sim in Unity by Marv934 (2022)
 * Developped as part of the Sonic Interaction Design Seminar at Audiokomminikation Group, TU Berlin
 * 
 * This is distributed under the MIT Licence (see LICENSE.md for details)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenerativeSoundEngine
{

//    public interface GSE_CollisionProximity
//    {
//        float Proximity { get; }
//        float ProximityAngle { get; }
//    }

    public class GSE_CollisionAssistant : MonoBehaviour //, GSE_CollisionProximity
    {

        // Collider
        Collider CarCollider;

        // Tracked Colliders
        List<Collider> Tracked = new List<Collider>();

        [SerializeField] float proximity = float.MaxValue;
        //public float Proximity { get { return proximity; } }

        [SerializeField] float proximityAngle = 0.0f;
        //public float ProximityAngle { get { return proximityAngle; } }

        // GSE Collector
        private IVehicle IVehicle;
        private GSEVehicle GSEVehicle;

        // Start is called before the first frame update
        void Start()
        {
            // BlindSpotAssistant = GetComponents<Collider>();
            foreach (Collider Element in GetComponentsInParent<Collider>())
            {
                if (Element.gameObject.name == "CollisionDetection")
                {
                    CarCollider = Element;
                }
            }

            IVehicle = GetComponentInParent<VehicleBehaviour.WheelVehicle>();
            GSEVehicle = GetComponentInParent<VehicleBehaviour.WheelVehicle>();
        }

        // Method Called for Collision Assistant
        public (float DistClosest, float AngleClosest) CollisionUpdate()
        {

            // Get CarCenter
            Vector3 CarCenter = CarCollider.bounds.center;
            CarCenter.y = 0;

            // Set Distance far away
            float DistClosest = float.MaxValue;
            float AnglClosest = 0.0f;

            // Check all tracked Colliders
            foreach (Collider Element in Tracked)
            {
                // Calculate Closest Point on Element to CarCenter
                Vector3 ColliderHitPoint = Element.ClosestPointOnBounds(CarCenter);
                ColliderHitPoint.y = 0;

                // Calculate Clostest Point on Car to ColliderHitPoint
                Vector3 CarHitPoint = CarCollider.ClosestPointOnBounds(ColliderHitPoint);
                CarHitPoint.y = 0;

                // Calculate Distance and Angle
                float Distance = Vector3.Distance(ColliderHitPoint, CarHitPoint);
                float Angle = Vector3.SignedAngle(transform.InverseTransformPoint(ColliderHitPoint), Vector3.forward, Vector3.up);

                if ( (!GSEVehicle.Reverse) && (Distance < (IVehicle.Speed) ) && (Distance < DistClosest) )              
                {
                    // Assign Values
                    DistClosest = Distance;
                    AnglClosest = Angle;
                }
            }
            // Return Value
            return (DistClosest, AnglClosest);
            proximity = DistClosest;
            proximityAngle = AnglClosest;
        }

        void OnTriggerEnter(Collider other)
        {
            // Begin tracking Colliders
            if (!other.isTrigger)
            {
                Tracked.Add(other);
            }
        }

        void OnTriggerExit(Collider other)
        {
            // End tracking Colliders
            Tracked.Remove(other);
        }
    }
}
