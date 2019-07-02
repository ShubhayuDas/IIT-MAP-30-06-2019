using UnityEngine;
using System.Collections;
using System.Text;

using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class LGTest : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use


        LogitechGSDK.LogiControllerPropertiesData properties;

        public float xAxis = 0, GasInput = 0, BreakInput = 0;
        public int Gear; 
        public float timestamp;
        public int buttonStatus, shifterMode;
        public float currentSpeed, currentSteeringAngle;

        private string speed = "0";

        private void Start()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();

            Debug.Log("SteeringInit:" + LogitechGSDK.LogiSteeringInitialize(false));
        }


        void OnApplicationQuit()
        {
            Debug.Log("SteeringShutdown:" + LogitechGSDK.LogiSteeringShutdown());
        }

        private void Update()
        {
            //All the test functions are called on the first device plugged in(index = 0)
            if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
            {

                //CONTROLLER PROPERTIES
                StringBuilder deviceName = new StringBuilder(256);
                LogitechGSDK.LogiGetFriendlyProductName(0, deviceName, 256);
                LogitechGSDK.LogiControllerPropertiesData actualProperties = new LogitechGSDK.LogiControllerPropertiesData();
                LogitechGSDK.LogiGetCurrentControllerProperties(0, ref actualProperties);

                //CONTROLLER STATE
                LogitechGSDK.DIJOYSTATE2ENGINES rec;
                rec = LogitechGSDK.LogiGetStateUnity(0);

                xAxis = rec.lX / 32768f; // -1 to 1 -- 0 is center

                if (rec.lY == 0)
                {
                    GasInput = 0;
                }
                else
                {
                    GasInput = (32767 - rec.lY) / (32768f + 32767f); // -1 to 0
                }


                if (rec.lRz == 0)
                {
                    BreakInput = 0;
                }
                else
                {
                    BreakInput = (32767 - rec.lRz) / (32768f + 32767f); //0 to 1
                }

                timestamp = Time.realtimeSinceStartup;
                currentSpeed = m_Car.CurrentSpeed;
                currentSteeringAngle = m_Car.CurrentSteerAngle;
                speed += currentSpeed; // for printing in GUI

                //Button status :
                for (int i = 0; i < 128; i++)
                {
                    if (rec.rgbButtons[i] == 128)
                    {
                        buttonStatus = i;

                    }
                }
                if (buttonStatus == 4)
                {
                    Gear = 1;
                }
                else if (buttonStatus == 5)
                {
                    Gear = -1;
                }
                /* THIS AXIS ARE NEVER REPORTED BY LOGITECH CONTROLLERS 
                 * 
                 * actualState += "x-axis velocity :" + rec.lVX + "\n";
                 * actualState += "y-axis velocity :" + rec.lVY + "\n";
                 * actualState += "z-axis velocity :" + rec.lVZ + "\n";
                 * actualState += "x-axis angular velocity :" + rec.lVRx + "\n";
                 * actualState += "y-axis angular velocity :" + rec.lVRy + "\n";
                 * actualState += "z-axis angular velocity :" + rec.lVRz + "\n";
                 * actualState += "extra axes velocities 1 :" + rec.rglVSlider[0] + "\n";
                 * actualState += "extra axes velocities 2 :" + rec.rglVSlider[1] + "\n";
                 * actualState += "x-axis acceleration :" + rec.lAX + "\n";
                 * actualState += "y-axis acceleration :" + rec.lAY + "\n";
                 * actualState += "z-axis acceleration :" + rec.lAZ + "\n";
                 * actualState += "x-axis angular acceleration :" + rec.lARx + "\n";
                 * actualState += "y-axis angular acceleration :" + rec.lARy + "\n";
                 * actualState += "z-axis angular acceleration :" + rec.lARz + "\n";
                 * actualState += "extra axes accelerations 1 :" + rec.rglASlider[0] + "\n";
                 * actualState += "extra axes accelerations 2 :" + rec.rglASlider[1] + "\n";
                 * actualState += "x-axis force :" + rec.lFX + "\n";
                 * actualState += "y-axis force :" + rec.lFY + "\n";
                 * actualState += "z-axis force :" + rec.lFZ + "\n";
                 * actualState += "x-axis torque :" + rec.lFRx + "\n";
                 * actualState += "y-axis torque :" + rec.lFRy + "\n";
                 * actualState += "z-axis torque :" + rec.lFRz + "\n";
                 * actualState += "extra axes forces 1 :" + rec.rglFSlider[0] + "\n";
                 * actualState += "extra axes forces 2 :" + rec.rglFSlider[1] + "\n";
                 */

                //shifterMode = LogitechGSDK.LogiGetShifterMode(0);


                //Spring Force -> S
                //Debug.Log(LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_SPRING));
                LogitechGSDK.LogiPlaySpringForce(0, 0, 30, 25);

                /*if (Input.GetKeyUp(KeyCode.S))
                {
                    
                    if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_SPRING))
                    {
                        LogitechGSDK.LogiStopSpringForce(0);
                    }
                    else
                    {
                        Debug.Log("SpringForce activated");
                        LogitechGSDK.LogiPlaySpringForce(0, 0, 80, 25);
                    }
                }*/

                //Constant Force -> C
                if (Input.GetKeyUp(KeyCode.C))
                {
                    
                    if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_CONSTANT))
                    {
                        LogitechGSDK.LogiStopConstantForce(0);
                    }
                    else
                    {   Debug.Log("ConstantForce activated");
                        LogitechGSDK.LogiPlayConstantForce(0, 50);
                    }
                }

                //Damper Force -> D
                if (Input.GetKeyUp(KeyCode.D))
                {
                    Debug.Log("DamperForce activated");
                    if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_DAMPER))
                    {
                        LogitechGSDK.LogiStopDamperForce(0);
                    }
                    else
                    {
                        LogitechGSDK.LogiPlayDamperForce(0, 50);
                    }
                }

                //Side Collision Force -> left or right arrow
                if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
                {
                    LogitechGSDK.LogiPlaySideCollisionForce(0, 60);
                }

                //Front Collision Force -> up arrow

                if (Input.GetKeyUp(KeyCode.UpArrow))
                {
                    LogitechGSDK.LogiPlayFrontalCollisionForce(0, 60);
                }

                //Dirt Road Effect-> I
                if (Input.GetKeyUp(KeyCode.I))
                {
                    Debug.Log("DirtRoadEffect activated");
                    if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_DIRT_ROAD))
                    {
                        LogitechGSDK.LogiStopDirtRoadEffect(0);
                    }
                    else
                    {
                        LogitechGSDK.LogiPlayDirtRoadEffect(0, 50);
                    }

                }

                //Bumpy Road Effect-> B
                if (Input.GetKeyUp(KeyCode.B))
                {
                    Debug.Log("BumpyRoadEffect activated");
                    if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_BUMPY_ROAD))
                    {
                        LogitechGSDK.LogiStopBumpyRoadEffect(0);
                    }
                    else
                    {
                        LogitechGSDK.LogiPlayBumpyRoadEffect(0, 50);
                    }

                }

                //Slippery Road Effect-> L
                if (Input.GetKeyUp(KeyCode.L))
                {
                    Debug.Log("SlipperyRoadEffect activated");
                    if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_SLIPPERY_ROAD))
                    {
                        LogitechGSDK.LogiStopSlipperyRoadEffect(0);
                    }
                    else
                    {
                        LogitechGSDK.LogiPlaySlipperyRoadEffect(0, 50);
                    }
                }

                //Surface Effect-> U
                if (Input.GetKeyUp(KeyCode.U))
                {
                    Debug.Log("SurfaceEffect activated");
                    if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_SURFACE_EFFECT))
                    {
                        LogitechGSDK.LogiStopSurfaceEffect(0);
                    }
                    else
                    {
                        LogitechGSDK.LogiPlaySurfaceEffect(0, LogitechGSDK.LOGI_PERIODICTYPE_SQUARE, 50, 1000);
                    }
                }

                //Car Airborne -> A
                if (Input.GetKeyUp(KeyCode.A))
                {
                    Debug.Log("AirborneEffect activated");
                    if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_CAR_AIRBORNE))
                    {
                        LogitechGSDK.LogiStopCarAirborne(0);
                    }
                    else
                    {
                        LogitechGSDK.LogiPlayCarAirborne(0);
                    }
                }

                //Soft Stop Force -> O
                if (Input.GetKeyUp(KeyCode.O))
                {
                    Debug.Log("SoftStopForce activated");
                    if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_SOFTSTOP))
                    {
                        LogitechGSDK.LogiStopSoftstopForce(0);
                    }
                    else
                    {
                        LogitechGSDK.LogiPlaySoftstopForce(0, 20);
                    }
                }

                //Set preferred controller properties -> PageUp
                if (Input.GetKeyUp(KeyCode.PageUp))
                {
                    //Setting example values
                    properties.wheelRange = 90;
                    properties.forceEnable = true;
                    properties.overallGain = 80;
                    properties.springGain = 80;
                    properties.damperGain = 80;
                    properties.allowGameSettings = true;
                    properties.combinePedals = false;
                    properties.defaultSpringEnabled = true;
                    properties.defaultSpringGain = 80;
                    LogitechGSDK.LogiSetPreferredControllerProperties(properties);

                }

                //Play leds -> P
                if (Input.GetKeyUp(KeyCode.P))
                {
                    LogitechGSDK.LogiPlayLeds(0, 20, 20, 20);
                }



                // pass the input to the car!
                float h = CrossPlatformInputManager.GetAxis("Steering Angle");
                float v = CrossPlatformInputManager.GetAxis("Accelerator Input");
                float b = CrossPlatformInputManager.GetAxis("Break Input");



                v = GasInput;
                h = xAxis;
                b = BreakInput;

                m_Car.Move(h, v, b, 0f, Gear);
            }
        }

        void OnCollisionEnter(Collision col)
        {

            Debug.Log("Collision : " + col.gameObject.name); //find out why collision name is not comming as any of the car's colliders
            LogitechGSDK.LogiPlayFrontalCollisionForce(0, 60);
        }

        void OnGUI()
        {
            Debug.Log("In GUI!!");
            speed = GUI.TextField(new Rect(10, 10, 200, 20), speed, 100);
        }
    }
}