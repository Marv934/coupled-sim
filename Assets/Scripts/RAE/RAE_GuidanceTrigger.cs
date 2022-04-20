/*
 * This code is part of Generative Sound Engine for Coupled Sim in Unity by Marv934 (2022)
 * Developped as part of the Sonic Interaction Design Seminar at Audiokomminikation Group, TU Berlin
 * 
 * This is distributed under the MIT Licence (see LICENSE.md for details)
 */

 /*
  * This script adds trigger for the guidance Script.
  * The trigger is activated by enter of a gameobject with RAE_InputManager Component
  * It will trigger the Specified NextSkript Element in it
  * 
  * Component needed in Scene:
  *     - ExperimentDefinition
  *
  * Component needed in ExperimentDefinition's gameObject:
  *     - RAE_EvaluationGuidance
  *     or
  *     - RAE_PracticeGuidance
  */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealTimeAuralizationEngine
{
    public class RAE_GuidanceTrigger : MonoBehaviour
    {

        // Set ExperimentDefinition
        [Header("Eyperiment")]
        [SerializeField]
        Component ExperimentDefinition = null;

        // Set Next
        [SerializeField] int NextSkript = 0;

        // Set Destroyed
        [Header("Destroy on Trigger - true, Not destroy on Trigger - false")]
        [SerializeField] bool destroy = true;

        // Start is called before the first frame update
        void Start()
        {

        }

        void OnTriggerEnter(Collider other)
        {
            // Get InputManager
            RAE_InputManager InputManager = other.gameObject.GetComponent<RAE_InputManager>();

            if (InputManager != null)
            {
                // Trigger next Skript Step
                if (ExperimentDefinition.TryGetComponent(out RAE_EvaluationGuidance EvaluationGuidance))
                {
                    EvaluationGuidance.Skript = NextSkript;
                }
                else if (ExperimentDefinition.TryGetComponent(out RAE_PracticeGuidance PracticeGuidance))
                {
                    PracticeGuidance.Skript = NextSkript;
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            // Get InputManager
            RAE_InputManager InputManager = other.gameObject.GetComponent<RAE_InputManager>();

            if (InputManager != null)
            { 
                if (destroy)
                {
                    // Destroy trigger GameObject
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
