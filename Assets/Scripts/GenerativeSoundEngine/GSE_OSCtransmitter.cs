/*
 * This code is part of Generative Sound Engine
 */
using System;
using UnityEngine;
using System.Globalization;
using extOSC;

namespace GenerativeSoundEngine
{
    [RequireComponent(typeof(GSE_Collector))]

    public class GSE_OSCtransmitter : MonoBehaviour
    {
        #region Public Vars
        public OSCTransmitter Transmitter;

        [Header("OSC Settings")]
        [SerializeField] public string RemoteHost = "127.0.0.1";
        [SerializeField] public int RemotePort = 7711;
        [SerializeField] public string RootAddress = "/GSE";

        #endregion

        private GSE_Data collector;

        #region Unity Methods

        protected virtual void Start()
        {
            // create transmitter
            Transmitter = gameObject.AddComponent<OSCTransmitter>();
            // init collector interface
            collector = GetComponent<GSE_Collector>();
            // Set remote host and port
            Transmitter.RemoteHost = RemoteHost;
            Transmitter.RemotePort = RemotePort;
            // send initial OSC Message
            var message = new OSCMessage(RootAddress);
            message.AddValue(OSCValue.String("Start transmitting parameters!"));
            Transmitter.Send(message);

        }


        void Update()
        {
            var message = new OSCMessage(RootAddress);
            message.AddValue(OSCValue.Float(collector.Speed));
            message.AddValue(OSCValue.Float(collector.SteerAngle));
            message.AddValue(OSCValue.Bool(collector.Reverse));
            message.AddValue(OSCValue.Float(collector.Indicator));
            message.AddValue(OSCValue.Bool(collector.Engine));
            Transmitter.Send(message);
        }

        #endregion
    }
}
