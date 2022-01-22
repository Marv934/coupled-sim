/*
 * This code is part of Generative Sound Engine
 */
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using System;
using UnityEngine;

namespace GenerativeSoundEngine
{

    // Requiered Components
    [RequireComponent(typeof(GSE_Collector))]

    public class GSE_Logger : MonoBehaviour
    {

        // Init <logfile>.csv
        [Header("PATH")]
        [SerializeField] string logFilePath = @"Logs";
        DateTime startDate = DateTime.Now; 
        string logFile;
        string logSeperator = ",";
        string stringWrite;

        // Init Interface to GSE_WheelVehicle
        private GSE_Data collector;

        // Start is called before the first frame update
        void Start()
        {

            // Get Collector Components
            collector = GetComponent<GSE_Collector>();

            // Create logfile
            logFile = logFilePath + "/GSE_LOG_" + startDate.ToString().Replace(" ", "-").Replace(".", "-").Replace(":", "-") + ".csv";
            Console.WriteLine(logFile);

            // Column name String
            string[] headerArray = { "Timestamp", "Speed", "Steering", "Reverse", "Indicator", "Engine" };
            stringWrite = string.Join(logSeperator, headerArray) + ";" + System.Environment.NewLine;

            // Create and write logfile
            File.WriteAllText(logFile, stringWrite);

        }

        // Update is called once per frame
        void Update()
        {

            TimeSpan timePassed = DateTime.Now - startDate;
            // Value String Array
            string value =
                timePassed.ToString() + logSeperator +
                collector.Speed.ToString("0.0000", CultureInfo.GetCultureInfo("en-US")) + logSeperator +
                collector.Steering.ToString("0.0000", CultureInfo.GetCultureInfo("en-US")) + logSeperator +
                collector.Reverse.ToString("0.0000", CultureInfo.GetCultureInfo("en-US")) + logSeperator +
                collector.Indicator.ToString("0.0000", CultureInfo.GetCultureInfo("en-US")) + logSeperator +
                collector.Engine.ToString("0.0000", CultureInfo.GetCultureInfo("en-US")) + logSeperator +
                ";" + System.Environment.NewLine;

            // Write to logfile
            File.AppendAllText(logFile, value);

        }
    }
}
