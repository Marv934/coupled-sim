﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenerativeSoundEngine
{
    public class GSE_CameraAdjustmentVR : MonoBehaviour
    {

        //[Header("Vertical Position")]

        //[SerializeField] [Range(-0.293f, 0.0f)] float PositionY;

        // Start is called before the first frame update
        void Start()
        {
            // transform.position = new Vector3(-0.3000003f, 1.22f, -0.2659975f);
        }

        // Update is called once per frame
        void Update()
        {
            //transform.position = transform.position + new Vector3(0, PositionY, 0);
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

