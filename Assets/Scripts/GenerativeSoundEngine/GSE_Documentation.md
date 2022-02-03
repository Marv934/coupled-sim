# Generative Sound Engine

Documentation for all Classes Added for the Generative Sound Engine

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
When Indicator is set in Collider direction it stores Distance and Angle of nearest Collider in Interface

#### Interface GSE_BlindSpotProximity

Property | Description
-|-
Proximity | Distance of Collider
Angle | Angle of Collider

### <a id="GSE_BlindSpotAssistant">GSE_CollisionAssistant</a>

#### Description
Tracks all non Trigger Colliders in Colliders.
When Distance is less than Speed it stores Distance and Angle of nearest Collider in Interface

#### Interface GSE_BlindSpotProximity

Property | Description
-|-
Proximity | Distance of Collider
Angle | Angle of Collider

### <a id="GSE_BlindSpotAssistant">GSE_ParkingAssistant</a>

#### Description
Tracks all non Trigger Colliders in Colliders.
When Steering Angle and Direction fits the Collider Angle it stores Distance and Angle of nearest Collider in Interface

#### Interface GSE_BlindSpotProximity

Property | Description
-|-
Proximity | Distance of Collider
Angle | Angle of Collider

### <a id="GSE_OSCtransmitter">GSE_OSCtransmitter</a>

#### Descrption

Class to Transmitt OSC Messages to the Sound Engine.

**On Start:** Create Transmitter and send start Message "Start transmitting parameters!"

**On Update:** Send Data from [WheelVehicle](#WheelVehicle) to transmitter

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

Method | Description
- | -
EngineStart | Sends *True* to OSC Addresss *\<RootAddress\>/Engine*
EngineStop | Sends *False* to OSC Addresss *\<RootAddress\>/Engine*
IndicatorStartLeft | Sends *True*, *-1* to OSC Address *\<RootAddress\>/Indicator*
IndicatorStartRight | Sends *True*, *1* to OSC Address *\<RootAddress\>/Indicator*
IndicatorStop | Sends *False*, *0* to OSC Address *\<RootAddress\>/Indicator*
Reverse | Sends *True* to OSC Address *\<RootAddress\>/Reverse*
Forward | Sends *False* to OSC Address *\<RootAddress\>/Reverse*

### <a id="GSE_AI_Test">GSE_AI_Test</a>