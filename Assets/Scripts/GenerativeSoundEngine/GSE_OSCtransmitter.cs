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
    //[RequireComponent(typeof(VehicleBehaviour.WheelVehicle))]

    public class GSE_OSCtransmitter : MonoBehaviour
    {
        #region Public Vars

        // Set OSC Transmitting
        public OSCTransmitter Transmitter;
        [SerializeField] public string RemoteHost = "127.0.0.1";
        [SerializeField] public int RemotePort = 7711;
        [SerializeField] public string RootAddress = "/GSE";

        #endregion
        #region Unity Methods

        // Start is called before the first frame update
        protected virtual void Start()
        {
            // create transmitter
            Transmitter = gameObject.AddComponent<OSCTransmitter>();
            
            // Set remote host and port
            Transmitter.RemoteHost = RemoteHost;
            Transmitter.RemotePort = RemotePort;
            
        }

        #endregion
        #region OSC-transmitting Methods

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

        public void Speed(float speed)
        {
            // Create Message
            var Speed = new OSCMessage(RootAddress + "/Speed", OSCValue.Float(speed));
            Transmitter.Send(Speed);
        }

        public void Steering(float steering)
        {
            // Create Message
            var Steering = new OSCMessage(RootAddress + "/Steering", OSCValue.Float(steering));
            Transmitter.Send(Steering);
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

        // Method to Send Collision Type
        public void CollisionType(int type)
        {
            // Create Message
            var Collision = new OSCMessage(RootAddress + "/CollisionType", OSCValue.Int(type));
            Transmitter.Send(Collision);
        }

        // Method to Send Collision Distance
        public void CollisionDistance(float Distance)
        {
            // Create Message
            var Collision = new OSCMessage(RootAddress + "/CollisionDistance", OSCValue.Float(Distance));
            Transmitter.Send(Collision);
        }

        // Method to Send Collision Angle
        public void CollisionAngle(float Angle)
        {
            // Create Message
            var Collision = new OSCMessage(RootAddress + "/CollisionAngle", OSCValue.Float(Angle));
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

        public void TirenessLevel(float level)
        {
            // Create Message
            var TirenessLevel = new OSCMessage(RootAddress + "/TirenessLevel", OSCValue.Float(level));
            Transmitter.Send(TirenessLevel);
        }

        public void StressLevel(float level)
        {
            // Create Message
            var StressLevel = new OSCMessage(RootAddress + "/StressLevel", OSCValue.Float(level));
            Transmitter.Send(StressLevel);
        }

        public void Telephone(bool statue)
        {
            // Create Message
            var Telephone = new OSCMessage(RootAddress + "/Telephone", OSCValue.Bool(statue));
            Transmitter.Send(Telephone);
        }

        public void Music(bool statue)
        {
            // Create Message
            var Music = new OSCMessage(RootAddress + "/Music", OSCValue.Bool(statue));
            Transmitter.Send(Music);
        }

        public void Conversation(bool statue)
        {
            // Create Message
            var Conversation = new OSCMessage(RootAddress + "/Conversation", OSCValue.Bool(statue));
            Transmitter.Send(Conversation);
        }

        public void AutoDrive(bool statue)
        {
            // Create Message
            var AutoDrive = new OSCMessage(RootAddress + "/AutoDrive", OSCValue.Bool(statue));
            Transmitter.Send(AutoDrive);
        }
        #endregion
    }
}
