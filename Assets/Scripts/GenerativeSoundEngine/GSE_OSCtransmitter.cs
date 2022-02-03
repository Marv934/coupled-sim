/*
 * This code is part of Generative Sound Engine for Coupled Sim in Unity by Marv934 and zeyuyang42 (2022)
 * Developped as part of the Sonic Interaction Design Seminar at Audiokomminikation Group, TU Berlin
 * 
 * This is distributed under the MIT Licence (see LICENSE.md for details)
 */
using System;
using UnityEngine;
using System.Globalization;
using extOSC;

namespace GenerativeSoundEngine
{
    [RequireComponent(typeof(VehicleBehaviour.WheelVehicle))]

    public class GSE_OSCtransmitter : MonoBehaviour
    {
        #region Public Vars

        // Set OSC Transmitting
        public OSCTransmitter Transmitter;
        [SerializeField] public string RemoteHost = "127.0.0.1";
        [SerializeField] public int RemotePort = 7711;
        [SerializeField] public string RootAddress = "/GSE";

        #endregion

        // Init WheelVehicle
        private GSEVehicle GSEVehicle;
        private IVehicle IVehicle;

        #region Unity Methods

        // Start is called before the first frame update
        protected virtual void Start()
        {
            // create transmitter
            Transmitter = gameObject.AddComponent<OSCTransmitter>();
            
            // Set remote host and port
            Transmitter.RemoteHost = RemoteHost;
            Transmitter.RemotePort = RemotePort;
            
            // send initial OSC Message
            var message = new OSCMessage(RootAddress);
            message.AddValue(OSCValue.String("Start transmitting parameters!"));
            Transmitter.Send(message);

            // Get WheelVehicle Interface
            GSEVehicle = GetComponent<VehicleBehaviour.WheelVehicle>();
            IVehicle = GetComponent<VehicleBehaviour.WheelVehicle>();
        }

        // Update is called once per frame
        void Update()
        {
            // Continusly send Values

            // Speed
            var Speed = new OSCMessage(RootAddress + "/Speed", OSCValue.Float(IVehicle.Speed));
            Transmitter.Send(Speed);

            // Steering
            var Steering = new OSCMessage(RootAddress + "/Steering", OSCValue.Float(GSEVehicle.Steering));
            Transmitter.Send(Steering);
        }

        // Method to Send Engine Start
        public void EngineStart()
        {
            // Create Message
            var Engine = new OSCMessage(RootAddress + "/Engine", OSCValue.Bool(true));
            Transmitter.Send(Engine);
        }

        // Method to Send Engine Stop
        public void EngineStop()
        {
            // Create Message
            var Engine = new OSCMessage(RootAddress + "/Engine", OSCValue.Bool(false));
            Transmitter.Send(Engine);
        }

        // Method to Send Indicator Start Left
        public void IndicatorStartLeft()
        {
            // Create Message
            var Indicator = new OSCMessage(RootAddress + "/Indicator", OSCValue.Bool(true), OSCValue.Float(-1.0f));
            Transmitter.Send(Indicator);
        }

        // Method to Send Indicator Start Right
        public void IndicatorStartRight()
        {
            // Create Message
            var Indicator = new OSCMessage(RootAddress + "/Indicator", OSCValue.Bool(true), OSCValue.Float(1.0f));
            Transmitter.Send(Indicator);
        }

        // Method to Send Indicator Stop
        public void IndicatorStop()
        {
            // Create Message
            var Indicator = new OSCMessage(RootAddress + "/Indicator", OSCValue.Bool(false), OSCValue.Float(0.0f));
            Transmitter.Send(Indicator);
        }

        // Method to Send Reverse 
        public void Reverse()
        {
            // Create Message
            var Reverse = new OSCMessage(RootAddress + "/Reverse", OSCValue.Bool(true));
            Transmitter.Send(Reverse);
        }

        // Method to Send Forward 
        public void Forward()
        {
            // Create Message
            var Reverse = new OSCMessage(RootAddress + "/Reverse", OSCValue.Bool(false));
            Transmitter.Send(Reverse);
        }

        // Method to Send Collision Warning
        public void Collision(string type, float Distance, float Angle)
        {
            // Create Message
            var Collision = new OSCMessage(RootAddress + "/Collision", OSCValue.String(type), OSCValue.Float(Distance), OSCValue.Float(Angle));
            Transmitter.Send(Collision);
        }

        // Method to Send Warning
        public void Warning(float prio)
        {
            // Create Message
            var Warning = new OSCMessage(RootAddress + "/Warning", OSCValue.Float(prio));
            Transmitter.Send(Warning);
        }

        // Method to Send Info
        public void Info(float prio)
        {
            // Create Message
            var Info = new OSCMessage(RootAddress + "/Info", OSCValue.Float(prio));
            Transmitter.Send(Info);
        }
        #endregion
    }
}
