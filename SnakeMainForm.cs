using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Text;
using System.Windows.Forms;

using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.FaceTracking;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

using LightBuzz.Vitruvius;
namespace SnakeGame
{
    public partial class SnakeMainForm : Form
    {
        Arena arena;
        bool updating;
        private KinectSensor sensor;
        GestureController _gestureController;
        private Skeleton[] skeletonData;
        private Skeleton[] skeletonData2;
        private int trackId1;
        private int trackId2;
        private Boolean isChoosen = false;
        Random random = new Random();
        public SnakeMainForm()
        {
            InitializeComponent();
            CreateArena();
        }

        private void PanelArena_Paint(object sender, PaintEventArgs e)
        {
            ArenaView.Render(e.Graphics, arena);
        }

        private void TimerUpdateWorld_Tick(object sender, EventArgs e)
        {
            if (!updating&&!arena.stop)
            {
                arena.stop = false;
                updating = true;
                arena.Update();
                PanelArena.Refresh();
                updating = false;
            }
            else
            {
                Console.WriteLine("!");
            }
        }

        private void PanelArena_Resize(object sender, EventArgs e)
        {
            CreateArena();
        }

        private void CreateArena()
        {
            arena = new Arena(PanelArena.Width /10, PanelArena.Height /10);
        }

        private void GestureController_GestureRecognized(object sender, GestureEventArgs e)
        {
            Console.WriteLine(e.Name);
            switch (e.Type)
            {
                case GestureType.SwipeDown:
                    arena.Snake.ChangeDirection("Down");
                    break;

                case GestureType.SwipeLeft:
                    arena.Snake.ChangeDirection("Left");
                    break;
                case GestureType.WaveLeft:
                    arena.Snake.ChangeDirection("Left");
                    break;
                case GestureType.SwipeRight:
                    arena.Snake.ChangeDirection("Right");
                    break;
                case GestureType.WaveRight:
                    arena.Snake.ChangeDirection("Right");
                    break;
                case GestureType.SwipeUp:
                    arena.Snake.ChangeDirection("Up");
                    break;
                default:
                    Console.Write("default");
                    break;
            }
        }
        
        private void SnakeMainForm_Load(object sender, EventArgs e)
        {
            //枚举Kinect设备，并将第一个连接成功的设备做为当前设备
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }
            if (null != this.sensor)
            {
                this.sensor.SkeletonStream.Enable();

                //启动设备
                try
                {
                    this.sensor.Start();
                }
                catch (System.IO.IOException)
                {
                    this.sensor = null;
                }
                //判断是否双人模式
                DialogResult result = MessageBox.Show("是否想进行双人对战？", "2P", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                {
                    //Thread.Sleep(2000);
                    //Reset every param
                    arena.Snake2 = new SnakeModel(arena, 2);
                    this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady2;
                    arena.isTwoP = true;
                }
                else
                {
                    arena.isTwoP = false;
                    this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;
                }
                _gestureController = new GestureController(this.sensor, GestureType.All);
                _gestureController.GestureRecognized += GestureController_GestureRecognized;


            }

            if (null == this.sensor)
            {
                MessageBox.Show("Kinect设备未准备好");
                Application.Exit();
            }
            else
            {
                this.Text = "Kinect连接成功";
            }
        }
       
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            bool received = false;

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                
                skeletonData = new Skeleton[sensor.SkeletonStream.FrameSkeletonArrayLength];
                
                if (skeletonFrame != null)
                {
                    skeletonFrame.CopySkeletonDataTo(this.skeletonData);
                    received = true;
                    
                }
            }

            if (received)
            {
                foreach (Skeleton skeleton in this.skeletonData)
                {
                    if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        _gestureController.Update(skeleton);
                        if (_gestureController.IsPose(skeleton, _gestureController.PoseLibrary[1]))//    down  
                        {
                            Console.WriteLine("Lib 1");
                            arena.Snake.ChangeDirection("Down");
                        }
                        else if(_gestureController.IsPose(skeleton, _gestureController.PoseLibrary[2]))
                        {
                            Console.WriteLine("Lib 2");
                            arena.Snake.ChangeDirection("Right");
                        }
                        else if (_gestureController.IsPose(skeleton, _gestureController.PoseLibrary[3]))
                        {
                            Console.WriteLine("Lib 3");
                            arena.Snake.ChangeDirection("Left");
                        }
                        else if (_gestureController.IsPose(skeleton, _gestureController.PoseLibrary[4]))
                        {
                            Console.WriteLine("Lib 4");
                            arena.Snake.ChangeDirection("Up");
                        }

                    }
                }

            }

        }

        private void SensorSkeletonFrameReady2(object sender, SkeletonFrameReadyEventArgs e)
        {
            bool received = false;

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                skeletonData = new Skeleton[sensor.SkeletonStream.FrameSkeletonArrayLength];
                if (skeletonFrame != null)
                {
                    skeletonFrame.CopySkeletonDataTo(this.skeletonData);
                    received = true;
                    if (!isChoosen)
                    {
                        //选择距离kinect最近的两个人追踪
                        Dictionary<int, float> dict = new Dictionary<int, float>();
                        foreach (Skeleton skeleton in this.skeletonData)
                        {
                            if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                            {
                                if (!dict.ContainsKey(skeleton.TrackingId) && skeleton.TrackingId != 0)
                                    dict.Add(skeleton.TrackingId, 10);
                                Console.WriteLine(skeleton.Position.Z + "" + skeleton.TrackingId + "z");
                            }
                        }
                        var dicSort = from objDic in dict orderby objDic.Value ascending select objDic;
                        int count = 0;
                        foreach (var item in dict)
                        {
                            if (count == 0)
                                trackId1 = item.Key;
                            else if (count == 1)
                                trackId2 = item.Key;
                           
                        }

                        if (trackId1 != 0 && trackId2 != 0 && trackId2 != trackId1)
                        {
                            //保证真真正正地识别到两个不同的人
                            isChoosen = true;
                            this.sensor.SkeletonStream.AppChoosesSkeletons = true;
                            this.sensor.SkeletonStream.ChooseSkeletons(trackId1, trackId2);
                            Console.WriteLine("tracking " + trackId1 + " " + trackId2);
                            MessageBox.Show("Tracking People Confirmed");
                        }
                        
                    }//if isChoosen
                }
            }

            if (received)
            {
                foreach (Skeleton skeleton in this.skeletonData)
                {
                    if (skeleton.TrackingState == SkeletonTrackingState.Tracked && skeleton.TrackingId == trackId1)//分析第一个人的骨架
                    {
                        _gestureController.Update(skeleton);
                        if (_gestureController.IsPose(skeleton, _gestureController.PoseLibrary[1]))//    down  
                        {
                            Console.WriteLine("Lib 1");
                            arena.Snake.ChangeDirection("Down");
                        }
                        else if (_gestureController.IsPose(skeleton, _gestureController.PoseLibrary[2]))
                        {
                            Console.WriteLine("Lib 2");
                            arena.Snake.ChangeDirection("Right");
                        }
                        else if (_gestureController.IsPose(skeleton, _gestureController.PoseLibrary[3]))
                        {
                            Console.WriteLine("Lib 3");
                            arena.Snake.ChangeDirection("Left");
                        }
                        else if (_gestureController.IsPose(skeleton, _gestureController.PoseLibrary[4]))
                        {
                            Console.WriteLine("Lib 4");
                            arena.Snake.ChangeDirection("Up");
                        }
                    }
                    else if(skeleton.TrackingId == trackId2)//分析第二个人的骨架
                    {
                        if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            _gestureController.Update(skeleton);
                            if (_gestureController.IsPose(skeleton, _gestureController.PoseLibrary[1]))//    down  
                            {
                                Console.WriteLine("2pLib 1");
                                arena.Snake2.ChangeDirection("Down");
                            }
                            else if (_gestureController.IsPose(skeleton, _gestureController.PoseLibrary[2]))
                            {
                                Console.WriteLine("2pLib 2");
                                arena.Snake2.ChangeDirection("Right");
                            }
                            else if (_gestureController.IsPose(skeleton, _gestureController.PoseLibrary[3]))
                            {
                                Console.WriteLine("2pLib 3");
                                arena.Snake2.ChangeDirection("Left");
                            }
                            else if (_gestureController.IsPose(skeleton, _gestureController.PoseLibrary[4]))
                            {
                                Console.WriteLine("2pLib 4");
                                arena.Snake2.ChangeDirection("Up");
                            }
                        }

                    }

                }

            }

        }

    }
}
