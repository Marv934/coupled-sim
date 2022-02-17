using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenerativeSoundEngine
{
    public class GSE_GuidanceTrigger : MonoBehaviour
    {
        // Set Next
        [SerializeField] int NextSkript;

        // Set Destroyed
        [Header("Destroy on Trigger - true, Not destroy on Trigger - false")]
        [SerializeField] bool destroy = true;

        // Init Guidance
        GSE_Guidance Guidance;

        // Start is called before the first frame update
        void Start()
        {
            // Get Scene
            GameObject[] root = gameObject.scene.GetRootGameObjects();

            // Get GameObject
            foreach (GameObject obj in root)
            {
                if (obj.TryGetComponent(typeof(ExperimentDefinition), out Component comp))
                {
                    // Get Guidance
                    Guidance = comp.GetComponentInChildren<GSE_Guidance>();
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            // Get InputManager
            GSE_InputManager InputManager = other.gameObject.GetComponent<GSE_InputManager>();

            if (InputManager != null)
            {
                Guidance.Skript = NextSkript;
            }
        }

        void OnTriggerExit(Collider other)
        {
            // Get InputManager
            GSE_InputManager InputManager = other.gameObject.GetComponent<GSE_InputManager>();

            if (InputManager != null)
            { 
                if (destroy)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
