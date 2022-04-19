/*
 * This code is part of Generative Sound Engine for Coupled Sim in Unity by Marv934 and zeyuyang42 (2022)
 * Developped as part of the Sonic Interaction Design Seminar at Audiokomminikation Group, TU Berlin
 * 
 * This is distributed under the MIT Licence (see LICENSE.md for details)
 */
 
/*
 * This script handles the OSC transmitting
 * It opens the Transmitter with the given Adress and Port
 * It Sends on the given OSC Adress
 *
 * Added Methods:
 *      - BoolTrigger(Name, state)
 *      - FloatTrigger(Name, value)
 */

using System;
using UnityEngine;
using System.Globalization;
using extOSC;
using System.Collections;
using System.Collections.Generic;

namespace RealTimeAuralizationEngine
{
    public class RAE_OSCtransmitter : MonoBehaviour
    {
        // Set OSC Transmitting
        public OSCTransmitter Transmitter;
        [SerializeField] public string RemoteHost = "127.0.0.1";
        [SerializeField] public int RemotePort = 7711;
        [SerializeField] public string RootAddress = "/RAE";

        // Start is called before the first frame update
        protected virtual void Start()
        {
            // create transmitter
            Transmitter = gameObject.AddComponent<OSCTransmitter>();
            
            // Set remote host and port
            Transmitter.RemoteHost = RemoteHost;
            Transmitter.RemotePort = RemotePort;
        }

        // Send OSC Bool Trigger Message
        public void BoolTrigger(string Name, bool state)
        {
            // Create Message
            var Message = new OSCMessage(RootAddress + "/" + Name, OSCValue.Bool(state));
            Transmitter.Send(Message);
        }

        // Send OSC Float Trigger Message
        public void FloatTrigger(string Name, float value)
        {
            // Create Message
            var Message = new OSCMessage(RootAddress + "/" + Name, OSCValue.Float(value));
            Transmitter.Send(Message);
        }
    }
}
