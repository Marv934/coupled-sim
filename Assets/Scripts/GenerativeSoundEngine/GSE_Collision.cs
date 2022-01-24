using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GenerativeSoundEngine
{
    public interface GSE_Proximity
    {
        float Proximity { get; }
        float ProximityAngle { get; }
    }
    
    public class GSE_Collision : MonoBehaviour
    {
        struct CollidersListEntry
        {
            public Collider collider;
            public float proximity;
            public float proximityAngle;
        }

        // Proximity
        float proximity = 1.0f;
        public float Proximity { get { return proximity; } }

        float proximityAngle = 1.0f;
        public float ProximityAngle { get { return proximityAngle; } }

        // Collider
        Collider ProximityCollider;

        // Collided Objects
        List<CollidersListEntry> Colliders = new List<CollidersListEntry>();

        // Start is called before the first frame update
        void Start()
        {
            ProximityCollider = GetComponent<Collider>();

        }

        // Update is called once per frame
        void Update()
        {
            // Proximity
            //proximityAngle = (float)Colliders.Count;

            List<CollidersListEntry> Colliders_sorted = Colliders.OrderBy(Entry => Entry.proximity).ToList();
            if (Colliders_sorted.Count != 0)
            {
                proximity = Colliders_sorted[0].proximity;
                proximityAngle = Colliders_sorted[0].proximityAngle;
            }
            else
            {
                proximity = 100.0f;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            CollidersListEntry ListEntry;
            ListEntry.collider = other;
            Vector3 closestPoint = other.ClosestPoint(ProximityCollider.bounds.center);
            ListEntry.proximity = Vector3.Distance(closestPoint, ProximityCollider.bounds.center);
            ListEntry.proximityAngle = Vector3.Angle(closestPoint, transform.forward); ;

            Colliders.Add(ListEntry);
        }

        void OnTriggerExit(Collider other)
        {
            Colliders.RemoveAll(Entry => Entry.collider == other);
        }

        void OnTriggerStay(Collider other)
        {
            int index = Colliders.FindIndex(Entry => Entry.collider == other);

            CollidersListEntry ListEntry;
            ListEntry.collider = other;
            Vector3 closestPoint = other.ClosestPoint(ProximityCollider.bounds.center);
            ListEntry.proximity = Vector3.Distance(closestPoint, ProximityCollider.bounds.center);
            ListEntry.proximityAngle = Vector3.Angle(closestPoint, transform.forward);

            Colliders[index] = ListEntry;
        }
    }
}
