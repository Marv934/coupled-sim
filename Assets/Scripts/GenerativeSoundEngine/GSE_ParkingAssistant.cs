using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenerativeSoundEngine
{
    public interface GSE_ParkingProximity
    {
        float Proximity { get; }
        float ProximityAngle { get; }
    }

    public class GSE_ParkingAssistant : MonoBehaviour, GSE_ParkingProximity
    {

        // Collider
        Collider ParkingAssistant;
        Collider CarCollider;

        // Tracked Colliders
        List<Collider> Tracked = new List<Collider>();

        [SerializeField] float proximity = 10.0f;
        public float Proximity { get { return proximity; } }

        [SerializeField] float proximityAngle = 0.0f;
        public float ProximityAngle { get { return proximityAngle; } }

        [SerializeField] Vector3 CarCenter;
        [SerializeField] Vector3 CarDirect;
        [SerializeField] Vector3 CollisionPoint;
        [SerializeField] Vector3 CarPoint;
        [SerializeField] Vector3 Diff;

        // GSE Collector
        private GSE_Data CarCollector;

        // Start is called before the first frame update
        void Start()
        {
            ParkingAssistant = GetComponent<Collider>();
            CarCollider = GetComponentsInParent<Collider>()[1];
            CarCollector = GetComponentInParent<GSE_Collector>();
        }

        // Update is called once per frame
        void Update()
        {

            // Set Collieders in Plane
            CarCenter = CarCollider.bounds.center;
            CarCenter.y = 0;
            if ( CarCollector.Reverse == 0)
            {
                CarDirect = transform.forward;
            } else
            {
                CarDirect = transform.forward * -1;
            }
            CarDirect.y = 0;

            // tracked Angle:
            float trackedAngleStart;
            float trackedAngleStop;
            //if ( CarCollector.Reverse == 0 )
            //{ // Forward direction
                if (CarCollector.Steering < 0.4f)
                { // steered to Left
                    trackedAngleStart = -30.0f;
                    trackedAngleStop = 150.0f;
                } else if (CarCollector.Steering > 0.6f)
                { // steered to right
                    trackedAngleStart = -150.0f;
                    trackedAngleStop = 30.0f;
                } else
                { // not steered
                    trackedAngleStart = -30.0f;
                    trackedAngleStop = 30.0f;
                }
            //} else
            //{
            //    if (CarCollector.Steering < 0.4f)
            //    { // steered to Left
            //        trackedAngleStart = 150.0f;
            //        trackedAngleStop = -30.0f;
            //    }
            //    else if (CarCollector.Steering > 0.6f)
            //    { // steered to right
            //        trackedAngleStart = 150.0f;
            //        trackedAngleStop = -30.0f;
            //    }
            //    else
            //    { // not steered
            //        trackedAngleStart = 150.0f;
            //        trackedAngleStop = -150.0f;
            //    }
            //}

            // Set Distance far away
            float DistClosest = 10.0f;
            float AnglClosest = 0.0f;

            // Check all tracked Colliders
            foreach (Collider Element in Tracked)
            {

                Vector3 closestColliderPoint = Element.ClosestPointOnBounds(CarCenter);
                closestColliderPoint.y = 0;
                Vector3 closestCarPoint = CarCollider.ClosestPointOnBounds(closestColliderPoint);
                closestCarPoint.y = 0;
                CollisionPoint = closestColliderPoint;
                CarPoint = closestCarPoint;
                Diff = closestColliderPoint - CarCenter;

                float Distance = Vector3.Distance(closestColliderPoint, closestCarPoint);
                float Angle = Vector3.SignedAngle(closestColliderPoint - CarCenter, CarDirect, Vector3.up);

                if ( ( (Angle > trackedAngleStart) && (Angle < trackedAngleStop) )  && (Distance < DistClosest) )
                {
                    DistClosest = Vector3.Distance(closestColliderPoint, closestCarPoint);
                    AnglClosest = Vector3.SignedAngle(closestColliderPoint - CarCenter, transform.forward, Vector3.up);
                }
            }
            proximity = DistClosest;
            proximityAngle = AnglClosest;
        }

        void OnTriggerEnter(Collider other)
        {
            // Begin tracking Colliders
            Tracked.Add(other);
        }

        void OnTriggerExit(Collider other)
        {
            // Begin tracking Colliders
            Tracked.Remove(other);
        }
    }
}
