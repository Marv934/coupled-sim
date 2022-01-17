using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface for GSE_Collector
public interface GSE_Data
{
    float Speed { get; }
    float SteerAngle { get; }
    bool Reverse { get; }
    float Indicator { get; }
    bool Engine { get; }
}

// Requiered Components
[RequireComponent(typeof(VehicleBehaviour.WheelVehicle))]

public class GSE_Collector : MonoBehaviour, GSE_Data
{

    // Init Interface members

    // Speed
    float speed = 0.0f;
    public float Speed { get { return speed; } }

    // SteerAngle
    float steerAngle = 30.0f;
    public float SteerAngle { get { return steerAngle; } }

    // Reverse
    bool reverse = false;
    public bool Reverse { get { return reverse; } }

    // Blinker
    float indicator = 0.0f;
    public float Indicator { get { return indicator; } }

    // Engine
    bool engine = false;
    public bool Engine { get { return engine; } }

    // Init Interfaces from other Classes

    // Wheel Vehicle
    private IVehicle Vehicle;
    private GSEVehicle Vehicle_GSE;

    // Start is called before the first frame update
    void Start()
    {

        // Get Components from other Classes
        Vehicle = GetComponent<VehicleBehaviour.WheelVehicle>();
        Vehicle_GSE = GetComponent<VehicleBehaviour.WheelVehicle>();


    }

    // Update is called once per frame
    void Update()
    {

        // Update Interface members

        // Speed
        speed = Vehicle.Speed;

        // SteerAngle
        steerAngle = Vehicle_GSE.SteerAngle;

        // Reverse
        reverse = Vehicle_GSE.Reverse;

        // Blinker
        indicator = Vehicle_GSE.Indicator;

        // Engine
        engine = Vehicle_GSE.Engine;

    }
}
