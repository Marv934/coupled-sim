using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

namespace GenerativeSoundEngine
{
    public class GSE_PedestrianTriggerStart : MonoBehaviour
    {

        bool triggered = false;

        Animator _AIPedestrian;

        GSE_Collision Collision;

        // Start is called before the first frame update
        void Start()
        {
            _AIPedestrian = GetComponentInParent<Animator>();         
        }

        void Update()
        {
            _AIPedestrian.enabled = triggered;
        }

        void OnTriggerEnter(Collider other)
        {
            Collision = other.gameObject.GetComponent<GSE_Collision>();

            if ( Collision != null)
            {
                triggered = true;
            }
        }
    }
}
