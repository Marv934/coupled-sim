using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GenerativeSoundEngine
{
    public class GSE_AI_Test
    {

        public bool stateEngine = false;
        public float stateBlinker = 0.5f;
        public bool stateReverse = false;
        public float stateThrottle = 0.0f;
        public float stateBreaking = 0.0f;
        public float stateSteering = 0.0f;
        public float stateSpeed = 0.0f;

        public TimeSpan lastTimePassed = new TimeSpan(0, 0, 0);

        public void Timer(DateTime startDate, DateTime nowDate)
        {
            TimeSpan timePassed = nowDate - startDate;

            if ((lastTimePassed.TotalSeconds < 2) && (2 <= timePassed.TotalSeconds))
            {
                stateEngine = true;
            } else if ((lastTimePassed.TotalSeconds < 3) && (3 <= timePassed.TotalSeconds))
            {
                stateBlinker = 0.0f;
            } else if ((lastTimePassed.TotalMilliseconds < 6600) && (6000 <= timePassed.TotalMilliseconds))
            {
                stateSpeed = 5.0f;
                stateThrottle = 0.33f;
                stateSteering = (float)(timePassed.TotalMilliseconds - 6000) * -0.05f;
            } else if ((lastTimePassed.TotalMilliseconds < 8300) && (6600 <= timePassed.TotalMilliseconds))
            {
                stateSteering = -30.0f;
            } else if ((lastTimePassed.TotalMilliseconds < 9300) && (8300 <= timePassed.TotalMilliseconds))
            {
                stateSteering = (float)(timePassed.TotalMilliseconds - 9300) * 0.03f;
            } else if ((lastTimePassed.TotalMilliseconds < 9400) && (9300 <= timePassed.TotalMilliseconds))
            {
                stateSteering = 0.0f;
            } else if ((lastTimePassed.TotalMilliseconds < 10390) && (9400 <= timePassed.TotalMilliseconds))
            {
                stateSteering = (float)(timePassed.TotalMilliseconds - 9400) * 0.03f;
            } else if ((lastTimePassed.TotalMilliseconds < 11380) && (10390 <= timePassed.TotalMilliseconds))
            {
                stateSteering = (float)(timePassed.TotalMilliseconds - 11380) * -0.03f;
            } else if ((lastTimePassed.TotalSeconds < 12) && (11380 <= timePassed.TotalMilliseconds))
            {
                stateSteering = 0.0f;
                stateSpeed = 50.0f;
                stateThrottle = 0.66f;
            }



                //else if ((lastTimePassed < new TimeSpan(0, 0, 13)) && (new TimeSpan(0, 0, 13) <= timePassed))
                //{
                //    stateSteering = -30f;
                //}
                //else if ((lastTimePassed < new TimeSpan(0, 0, 15)) && (new TimeSpan(0, 0, 15) <= timePassed))
                //{
                //    stateSpeed = 5.0f;
                //    stateThrottle = 0.33f;
                //}
                //else if ((lastTimePassed < new TimeSpan(0, 0, 17)) && (new TimeSpan(0, 0, 17) <= timePassed))
                //{
                //    stateSteering = 30f;
                //}
                //else if ((lastTimePassed < new TimeSpan(0, 0, 19)) && (new TimeSpan(0, 0, 19) <= timePassed))
                //{
                //    stateSteering = 0.0f;
                //}

                lastTimePassed = timePassed;
        }

        public void CheckEngine(out bool engine, out bool handbrake)
        {
            if (stateEngine)
            {
                engine = true;
                handbrake = false;
            }
            else
            {
                engine = false;
                handbrake = true;
            }
        }

        public void CheckBlinkers(CarBlinkers blinkers, out float indicator)
        {
            indicator = stateBlinker;
            if (stateBlinker == 0.0f)
            {
                if (blinkers.State != BlinkerState.Left)
                {
                    blinkers.StartLeftBlinkers();
                }
            }
            else if (stateBlinker == 1.0f)
            {
                if (blinkers.State != BlinkerState.Right)
                {
                    blinkers.StartRightBlinkers();
                }
            }
            else if (stateBlinker == 0.5f)
            {
                blinkers.Stop();
            }
        }

        public void CheckReverse( out bool reverse)
        {
            if (stateReverse)
            {
                reverse = true;
            }
            else
            {
                reverse = false;
            }
        }

        public void CheckThrottle( out float throttle)
        {
            throttle = stateThrottle * (stateReverse ? -1f : 1);
        }

        public void CheckBreaking( out float breaking)
        {
            if (stateEngine)
            {
                breaking = stateBreaking;
            }
            else
            {
                breaking = 1.0f;
            }
        }

        public void CheckSteering (out float steering)
        {
            steering = stateSteering;
        }

        public void CheckSpeed (out float Speed, Rigidbody _rb)
        {
            Speed = stateSpeed; //* (stateReverse ? -1f : 1);
            //_rb.velocity = new Vector3(0.0f, stateSpeed, 0.0f); ;
            //_rb.AddForce(transform.forward * Speed);
        }
    }
}