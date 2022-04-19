/*
 * This code is part of REAL-TIME AURALIZATION ENGINE for Coupled Sim in Unity by Marv934 (2022)
 * Developped as part of the Sonic Interaction Design Seminar at Audiokomminikation Group, TU Berlin
 * 
 * This is distributed under the MIT Licence (see LICENSE.md for details)
 */

/*
 * With this Script added to an GameObject, the Y-Coordinate of the Transform changes:
 * While Keypad2 is pressed - move in negative Y
 * While Keypad8 is pressed - move in positive Y
 *
 * It is used to adjust the Camera Position in Game While wearing VR.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealTimeAuralizationEngine
{
    public class RAE_CameraAdjustmentVR : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.Keypad2))
            {
                transform.position = transform.position - new Vector3(0.0f, 0.001f, 0.0f);
            } 
            if (Input.GetKey(KeyCode.Keypad8))
            {
                transform.position = transform.position + new Vector3(0.0f, 0.001f, 0.0f);
            }
        }
    }
}

