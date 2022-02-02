using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenerativeSoundEngine
{
    public interface GSE_BlindSpotProximity
    {
        float Proximity { get; }
        float ProximityAngle { get; }
    }

    public class GSE_BlindSpotAssistant : MonoBehaviour, GSE_BlindSpotProximity
    {

        // Collider
        // Collider BlindSpotAssistant;
        Collider CarCollider;

        // Tracked Colliders
        List<Collider> Tracked = new List<Collider>();

        [SerializeField] float proximity = 10.0f;
        public float Proximity { get { return proximity; } }

        [SerializeField] float proximityAngle = 0.0f;
        public float ProximityAngle { get { return proximityAngle; } }

        // GSE Collector
        private GSE_Data CarCollector;

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

            CarCollector = GetComponentInParent<GSE_Collector>();
        }

        // Update is called once per frame
        void Update()
        {

            // Get CarCenter
            Vector3 CarCenter = CarCollider.bounds.center;
            CarCenter.y = 0;

            // Set Distance far away
            float DistClosest = 10.0f;
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

                if ( ( ( ( CarCollector.Indicator < 0.4 ) && ( Angle > 0 ) ) || ( ( CarCollector.Indicator > 0.6 ) && (Angle < 0 ) ) ) & (Distance < DistClosest) )              
                {
                    // Assign Values
                    DistClosest = Distance;
                    AnglClosest = Angle;
                }
            }
            // Assign Values
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
