using LightBuzz.Vitruvius.Gestures;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.FaceTracking;

namespace LightBuzz.Vitruvius
{
    /// <summary>
    /// 一个姿势中的某个的角度
    /// </summary>
    public class PoseAngle
    {
        KinectSensor kinectDevice;
        public JointType CenterJoint;
        public JointType AngleJoint;
        public double Angle;
        public double Threshold;
        public PoseAngle(KinectSensor kinectDevice, JointType J1, JointType J2, int Angle,int Threshold)
        {
            this.kinectDevice = kinectDevice;
            this.CenterJoint = J1;
            this.AngleJoint = J2;
            this.Angle = Angle;
            this.Threshold = Threshold;
        }
    }
    /// <summary>
    /// 姿势类 每一个对象代表一个自定义姿势
    /// </summary>
    public class Pose
    {
        public String Name;
        public PoseAngle[] Angles;

        public Pose()
        {

        }
    }

    /// <summary>
    /// Represents a gesture controller.
    /// </summary>
    public class GestureController
    {
        #region Members

        /// <summary>
        /// A list of all the gestures the controller is searching for.
        /// </summary>
        private List<Gesture> _gestures = new List<Gesture>();
        KinectSensor kinectDevice;
        public Pose[] PoseLibrary;
        public Skeleton skeleton;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="GestureController"/>.
        /// </summary>
        public GestureController()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="GestureController"/>.
        /// </summary>
        /// <param name="type">The gesture type to recognize. Set to GesureType.All for instantly adding all of the predefined gestures.</param>
        public GestureController(KinectSensor kinectDevice,GestureType type)
        {
            this.kinectDevice = kinectDevice;
            if (type == GestureType.All)
            {
                foreach (GestureType t in Enum.GetValues(typeof(GestureType)))
                {
                    if (t != GestureType.All)
                    {
                        AddGesture(t);
                    }
                }
            }
            else
            {
                AddGesture(type);
            }
            PopulatePoseLibrary();
        }
        /// <summary>  
        /// 自定义姿势 
        /// </summary>  
        private void PopulatePoseLibrary()
        {
            this.PoseLibrary = new Pose[5];

            //Pose 0 - 伸开双臂 
            this.PoseLibrary[0] = new Pose();
            this.PoseLibrary[0].Name = "伸开双臂(Arms Extended)";
            this.PoseLibrary[0].Angles = new PoseAngle[4];
            this.PoseLibrary[0].Angles[0] = new PoseAngle(this.kinectDevice, JointType.ShoulderLeft, JointType.ElbowLeft, 180, 8);
            this.PoseLibrary[0].Angles[1] = new PoseAngle(this.kinectDevice, JointType.ElbowLeft, JointType.WristLeft, 180, 8);
            this.PoseLibrary[0].Angles[2] = new PoseAngle(this.kinectDevice, JointType.ShoulderRight, JointType.ElbowRight, 0, 8);
            this.PoseLibrary[0].Angles[3] = new PoseAngle(this.kinectDevice, JointType.ElbowRight, JointType.WristRight, 0, 8);

            //Pose 1 - Down
            this.PoseLibrary[1] = new Pose();
            this.PoseLibrary[1].Angles = new PoseAngle[1];
            this.PoseLibrary[1].Angles[0] = new PoseAngle(this.kinectDevice, JointType.ElbowRight, JointType.WristRight, 270, 20);

            //Pose 2 - "右";
            this.PoseLibrary[2] = new Pose();
            this.PoseLibrary[2].Angles = new PoseAngle[1];
            this.PoseLibrary[2].Angles[0] = new PoseAngle(this.kinectDevice, JointType.ElbowRight, JointType.WristRight, 0, 20);

            //Pose 3 - "左";
            this.PoseLibrary[3] = new Pose();
            this.PoseLibrary[3].Angles = new PoseAngle[1];
            this.PoseLibrary[3].Angles[0] = new PoseAngle(this.kinectDevice, JointType.ElbowRight, JointType.WristRight, 180, 20);
          
            //Pose 4 - "右手上";
            this.PoseLibrary[4] = new Pose();
            this.PoseLibrary[4].Angles = new PoseAngle[1];
            this.PoseLibrary[4].Angles[0] = new PoseAngle(this.kinectDevice, JointType.ElbowRight, JointType.WristRight, 90, 20);
            


        }
        #endregion

        #region Events

        /// <summary>
        /// Occurs when a gesture is recognized.
        /// </summary>
        public event EventHandler<GestureEventArgs> GestureRecognized;

        #endregion

        #region Methods
        /// <summary>  
        /// 获取每一个节点在主 UI 布局空间中的坐标的方法  
        /// </summary>  
        /// <param name="kinectDevice"></param>  
        /// <param name="joint"></param>  
        /// <param name="containerSize"></param>  
        /// <param name="offset"></param>  
        /// <returns></returns>  
        private Point GetJointPoint(Joint joint, Point offset)
        {
            //得到节点在主 UI 布局空间中的坐标  
            //DepthImagePoint point = kinectDevice.MapSkeletonPointToDepth(joint.Position, kinectDevice.DepthStream.Format);  
            DepthImagePoint point = this.kinectDevice.CoordinateMapper.MapSkeletonPointToDepthPoint(joint.Position, this.kinectDevice.DepthStream.Format);
            point.X = (int)(point.X - offset.X);
            point.Y = (int)(point.Y - offset.Y);

            return new Point(point.X, point.Y);
        }

        /// <summary>  
        /// 计算2关节点之间的夹角  
        /// </summary>  
        /// <param name="centerJoint"></param>  
        /// <param name="angleJoint"></param>  
        /// <returns></returns>  
        private double GetJointAngle( Joint centerJoint, Joint angleJoint)
        {

            Point primaryPoint = GetJointPoint(centerJoint, new Point());
            Point anglePoint = GetJointPoint(angleJoint, new Point());
            Point x = new Point(primaryPoint.X + anglePoint.X, primaryPoint.Y);

            double a;
            double b;
            double c;

            a = Math.Sqrt(Math.Pow(primaryPoint.X - anglePoint.X, 2) + Math.Pow(primaryPoint.Y - anglePoint.Y, 2));
            b = anglePoint.X;
            c = Math.Sqrt(Math.Pow(anglePoint.X - x.X, 2) + Math.Pow(anglePoint.Y - x.Y, 2));

            double angleRad = Math.Acos((a * a + b * b - c * c) / (2 * a * b));
            double angleDeg = angleRad * 180 / Math.PI;

            //如果计算角度大于180度，将其转换到0-180度  
            if (primaryPoint.Y < anglePoint.Y)
            {
                angleDeg = 360 - angleDeg;
            }

            return angleDeg;
        }
        /// <summary>  
        /// 判断与指定姿势是否匹配的方法  
        /// </summary>  
        /// <param name="skeleton"></param>  
        /// <param name="pose"></param>  
        /// <returns></returns>  
        public bool IsPose(Skeleton skeleton, Pose pose)
        {
            bool isPose = true;
            double angle;
            double poseAngle;
            double poseThreshold;
            double loAngle;
            double hiAngle;

            //遍历一个姿势中所有poseAngle，判断是否符合相应的条件  
            for (int i = 0; i < pose.Angles.Length && isPose; i++)
            {
                poseAngle = pose.Angles[i].Angle;
                poseThreshold = pose.Angles[i].Threshold;
                //调用 GetJointAngle 方法来计算两个关节点之间的角度  
                angle = GetJointAngle(skeleton.Joints[pose.Angles[i].CenterJoint], skeleton.Joints[pose.Angles[i].AngleJoint]);

                hiAngle = poseAngle + poseThreshold;
                loAngle = poseAngle - poseThreshold;

                //判断角度是否在360范围内，如果不在，则转换到该范围内  
                if (hiAngle >= 360 || loAngle < 0)
                {
                    loAngle = (loAngle < 0) ? 360 + loAngle : loAngle;
                    hiAngle = hiAngle % 360;

                    isPose = !(loAngle > angle && angle > hiAngle);
                }
                else
                {
                    isPose = (loAngle <= angle && hiAngle >= angle);
                }
            }
            //如果判断角度一致，则返回true  
            return isPose;
        }  
        /// <summary>
        /// Updates all gestures.
        /// </summary>
        /// <param name="skeleton">The skeleton data.</param>
        public void Update(Skeleton skeleton)
        {
            this.skeleton = skeleton;
            foreach (Gesture gesture in _gestures)
            {
                gesture.Update(skeleton);
            }
        }

        /// <summary>
        /// Adds the specified gesture for recognition.
        /// </summary>
        /// <param name="type">The predefined <see cref="GestureType" />.</param>
        public void AddGesture(GestureType type)
        {
            IGestureSegment[] segments = null;

            // DEVELOPERS: If you add a new predefined gesture with a new GestureType,
            // simply add the proper segments to the switch statement here.
            switch (type)
            {
                case GestureType.JoinedHands:
                    segments = new IGestureSegment[20];

                    JoinedHandsSegment1 joinedhandsSegment = new JoinedHandsSegment1();
                    for (int i = 0; i < 20; i++)
                    {
                        segments[i] = joinedhandsSegment;
                    }
                    break;
                case GestureType.Menu:
                    segments = new IGestureSegment[20];

                    MenuSegment1 menuSegment = new MenuSegment1();
                    for (int i = 0; i < 20; i++)
                    {
                        segments[i] = menuSegment;
                    }
                    break;
                case GestureType.SwipeDown:
                    segments = new IGestureSegment[3];

                    segments[0] = new SwipeDownSegment1();
                    segments[1] = new SwipeDownSegment2();
                    segments[2] = new SwipeDownSegment3();
                    break;
                case GestureType.SwipeLeft:
                    segments = new IGestureSegment[3];

                    segments[0] = new SwipeLeftSegment1();
                    segments[1] = new SwipeLeftSegment2();
                    segments[2] = new SwipeLeftSegment3();
                    break;
                case GestureType.SwipeRight:
                    segments = new IGestureSegment[3];

                    segments[0] = new SwipeRightSegment1();
                    segments[1] = new SwipeRightSegment2();
                    segments[2] = new SwipeRightSegment3();
                    break;
                case GestureType.SwipeUp:
                    segments = new IGestureSegment[3];

                    segments[0] = new SwipeUpSegment1();
                    segments[1] = new SwipeUpSegment2();
                    segments[2] = new SwipeUpSegment3();
                    break;
                case GestureType.WaveLeft:
                    segments = new IGestureSegment[6];

                    WaveLeftSegment1 waveLeftSegment1 = new WaveLeftSegment1();
                    WaveLeftSegment2 waveLeftSegment2 = new WaveLeftSegment2();

                    segments[0] = waveLeftSegment1;
                    segments[1] = waveLeftSegment2;
                    segments[2] = waveLeftSegment1;
                    segments[3] = waveLeftSegment2;
                    segments[4] = waveLeftSegment1;
                    segments[5] = waveLeftSegment2;
                    break;
                case GestureType.WaveRight:
                    segments = new IGestureSegment[6];

                    WaveRightSegment1 waveRightSegment1 = new WaveRightSegment1();
                    WaveRightSegment2 waveRightSegment2 = new WaveRightSegment2();

                    segments[0] = waveRightSegment1;
                    segments[1] = waveRightSegment2;
                    segments[2] = waveRightSegment1;
                    segments[3] = waveRightSegment2;
                    segments[4] = waveRightSegment1;
                    segments[5] = waveRightSegment2;
                    break;
                case GestureType.ZoomIn:
                    segments = new IGestureSegment[3];

                    segments[0] = new ZoomSegment1();
                    segments[1] = new ZoomSegment2();
                    segments[2] = new ZoomSegment3();
                    break;
                case GestureType.ZoomOut:
                    segments = new IGestureSegment[3];

                    segments[0] = new ZoomSegment3();
                    segments[1] = new ZoomSegment2();
                    segments[2] = new ZoomSegment1();
                    break;
                case GestureType.All:
                case GestureType.None:
                default:
                    break;
            }

            if (type != GestureType.None)
            {
                Gesture gesture = new Gesture(type, segments);
                gesture.GestureRecognized += OnGestureRecognized;

                _gestures.Add(gesture);
            }
        }

        /// <summary>
        /// Adds the specified gesture for recognition.
        /// </summary>
        /// <param name="name">The gesture name.</param>
        /// <param name="segments">The gesture segments.</param>
        public void AddGesture(string name, IGestureSegment[] segments)
        {
            Gesture gesture = new Gesture(name, segments);
            gesture.GestureRecognized += OnGestureRecognized;

            _gestures.Add(gesture);
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Handles the GestureRecognized event of the g control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KinectSkeltonTracker.GestureEventArgs"/> instance containing the event data.</param>
        private void OnGestureRecognized(object sender, GestureEventArgs e)
        {
            if (GestureRecognized != null)
            {
                GestureRecognized(this, e);
            }

            foreach (Gesture gesture in _gestures)
            {
                gesture.Reset();
            }
        }

        #endregion
    }
}
