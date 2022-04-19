/*
 * This code is part of Generative Sound Engine for Coupled Sim in Unity by Marv934 (2022)
 * Developped as part of the Sonic Interaction Design Seminar at Audiokomminikation Group, TU Berlin
 * 
 * This is distributed under the MIT Licence (see LICENSE.md for details)
 */

 /*
  * This Script can be added to a GameObject with a Collider to track Colliders Entering the Area
  * and calculate Distance and Angle
  *
  * GameObject Components Requiered:
  *     - Collider
  * GameObject Parent (Name: "CollisionDetection") Components Requiered:
  *     - Collider
  * GameObject Parents Component Requiered:
  *     - WheelVehicle
  *
  * Added public Method:
  *     - ParkingUpdate()
  */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealTimeAuralizationEngine
{
    [RequireComponent(typeof(Collider))]

    public class RAE_ParkingAssistant : MonoBehaviour
    {

        // Car HitBox
        Collider CarCollider;

        // Tracked Colliders
        List<Collider> Tracked = new List<Collider>();

        // Init WheelVehicle
        private RAEVehicle RAEVehicle;

        // Start is called before the first frame update
        void Start()
        {
            // Search for Car HitBox    
            foreach (Collider Element in GetComponentsInParent<Collider>())
            {
                if (Element.gameObject.name == "CollisionDetection")
                {
                    CarCollider = Element;
                }
            }

            RAEVehicle = GetComponentInParent<VehicleBehaviour.WheelVehicle>();
        }

        // Method Called for Parking Assistant to track nearest collider
        public (float DistClosest, float AngleClosest) ParkingUpdate()
        {
            // Get CarCenter
            Vector3 CarCenter = CarCollider.bounds.center;
            CarCenter.y = 0;

            // Get Direction
            Vector3 CarDirect;
            if ( !RAEVehicle.Reverse )
            {
                CarDirect = Vector3.forward;
            } else
            {
                CarDirect = Vector3.back;
            }
            CarDirect.y = 0;

            // tracked Angle:
            float trackedAngleStart;
            float trackedAngleStop;
            
            // Get Tracking Angle
            if (RAEVehicle.Steering < -5.0)
            { // steered to Left
                trackedAngleStart = -30.0f;
                trackedAngleStop = 150.0f;
            } else if (RAEVehicle.Steering > 5.0f)
            { // steered to right
                trackedAngleStart = -150.0f;
                trackedAngleStop = 30.0f;
            } else
            { // not steered
            trackedAngleStart = -30.0f;
            trackedAngleStop = 30.0f;
            }

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
                float Angle = Vector3.SignedAngle(transform.InverseTransformPoint(ColliderHitPoint), CarDirect, Vector3.up);

                if ( ( (Angle > trackedAngleStart) && (Angle < trackedAngleStop) )  && (Distance < DistClosest) )
                {
                    DistClosest = Distance;
                    AnglClosest = Vector3.SignedAngle(transform.InverseTransformPoint(ColliderHitPoint), Vector3.forward, Vector3.up);
                }
            }

            // Return Value
            return (DistClosest, AnglClosest);
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
