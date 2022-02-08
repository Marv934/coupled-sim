# Generative Sound Engine

Documentation for all Classes Added for the Generative Sound Engine

## Issues
I Had to remove *_driverBuffer.AddRange(_playerSystem.Cars);* in WorldLogger.cs

## Classes

### <a id="WheelVehicle">WheelVehicle</a>

#### Description

Inheritated from Coupled Sim and changed to fit the Generative Sound Engine

*Added*:
- public interface GSEVehicle
- Start/Stop Button
- Max Speed
- [Testing Scenario](#GSE_AI_Test)
- [OSC Transmitter](#GSE_OSCtransmitter)

#### Interface GSE_Vehicle

Property | Description
-|-
float Steering | Steering Wheel Angle; Default between -30° and 30° or according to *[SerializeField] steerAngle*
bool Reverse | Reverse Gear [True, False]
float Indicator | Indicator [-1 &rarr; Left, 0 &rarr; off, 1 &rarr; Right]
bool Engine | Engine [True &rarr; On, False &rarr; Off]
float MaxSpeed | Maximum Speed; Default 60 or according to *[SerializeField] maxSpeed*

### <a id="GSE_BlindSpotAssistant">GSE_BlindSpotAssistant</a>

#### Description
Tracks all non Trigger Colliders in Colliders.

#### Public Methods

**public (float DistClosest, float AngleClosest) BlindSpotUpdate()**:
Returns Angle and Distance of nearest Object in Direction of Indicator, if set.


### <a id="GSE_BlindSpotAssistant">GSE_CollisionAssistant</a>

#### Description
Tracks all non Trigger Colliders in Colliders.

#### Public Methods

**public (float DistClosest, float AngleClosest) CollisionUpdate()**:
Returns Angle and Distance of nearest Object when Distance is less than Speed

### <a id="GSE_BlindSpotAssistant">GSE_ParkingAssistant</a>

#### Description
Tracks all non Trigger Colliders in Colliders.
When Steering Angle and Direction fits the Collider Angle it stores Distance and Angle of nearest Collider in Interface

#### Public Methods

**public (float DistClosest, float AngleClosest) ParkingUpdate()**: Returns Angle and Distance of nearest Object in Steering Direction. Tracked Angles are:
- Steered to Left (SteeringAngle < -5°): Tracked between -30° and 150°
- Steered to Right (SteeringAngle > 5°): Tracked between -150° and 30°
- Not Steered (-5° <= SteeringAngle <= 5°): Tracked between -30° and 30°

### <a id="GSE_Collision">GSE_Collision</a>

#### Description
Collects Angles and Distances from [GSE_ParkingAssistant](#GSE_BlindSpotAssistant)[GSE_BlindSpotAssistant](#GSE_CollisionAssistant)[GSE_CollisionAssistant](#GSE_ParkingAssistant) and Calls [GSE_OSCtransmitter](#GSE_OSCRtransmitter).

**On Fixed Update**: Every 5th Fixed Update step (10 per second) collects Angles and Distances from Assistant scripts and Calls [OSCtransmitter.Collision](#GSE_OSCRtransmitter)

Collision Type | OSC Message Values
-|-
None | 0, 0, 0
Parking | 1, Distance, Angle
Collision | 2, Distance, Angle
BlindSpot | 3, Distance, Angle

#### Properties

Property | Description
-|-
float ParkingAssistentMaxSpeed | Below this Speed the Parking Assistant is active, above the Collision Assistant is active
float ParkingAssistentMaxDistance | Max Distance transmittet by Parking Assistant
float CollisionAssistantMaxDistance | Max Distance transmittet by Collision Assistant. This Value is multyplied by Speed
float BlindSpotAssistantMaxDistance | Max Distance transmittet by BlindSpot Assistant

### <a id="GSE_OSCtransmitter">GSE_OSCtransmitter</a>

#### Descrption

Class to Transmitt OSC Messages to the Sound Engine.

**On Start:** Create Transmitter and send start Message "Start transmitting parameters!"

**On FixedUpdate:** Every 5th Fixed Update step (10 per Second) Send Data from [WheelVehicle](#WheelVehicle) to transmitter

Variable | OSC Adsress
-|-
IVehicle.Speed | \<RootAdress\>/Speed
GSEVehicle.Steering | \<RootAddress\>/Steering


#### Properties

Property    | Description
------------|--
RemoteHost  | IP Adress for OSC Reciving
RemotePort  | Port for OSC Reciving
RootAddress | OSC Root Address

#### Public Methods

**public void EngineStart()**: Sends *True* to OSC Addresss *\<RootAddress\>/Engine*

**public void EngineStop()**: Sends *False* to OSC Addresss *\<RootAddress\>/Engine*

**public void IndicatorStartLeft()**: Sends *True*, *-1* to OSC Address *\<RootAddress\>/Indicator*

**public void IndicatorStartRight()** Sends *True*, *1* to OSC Address *\<RootAddress\>/Indicator*

**public void IndicatorStop()**: Sends *False*, *0* to OSC Address *\<RootAddress\>/Indicator*

**public void Reverse()**: Sends *True* to OSC Address *\<RootAddress\>/Reverse*

**public void Forward()**: Sends *False* to OSC Address *\<RootAddress\>/Reverse*

**public void Collision(int type, float Distance, float Angle)**: Sends [*type*, *Distance*, *Angle*](#GSE_Collision) to OSC Address *\<RootAddress\>/Reverse*

**public void Warning(float prio)**: Sends *Prio* to OSC Adress *\RootAddress\>Warning*

**public void Info(float prio)**: Sends *Prio* to OSC Adress *\RootAddress\>Info*

### <a id="GSE_InfoWaypoint">GSE_InfoWaypoint</a>

### <a id="GSE_WarningWaypoint">GSE_WarningWaypoint</a>

### <a id="GSE_AI_Test">GSE_AI_Test</a>