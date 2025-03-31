using System.Diagnostics;
using System.Numerics;
using Intel.RealSense;

namespace YawSafety
{
    internal class ObjectDetector
    {

        private const int FRAME_WIDTH = 848;
        private const int FRAME_HEIGHT = 480;
        private List<CollisionPoint> points;

        private DateTime lastEmergencyStop;

        public ObjectDetector()
        {
         
            points = new List<CollisionPoint>()
            {

                new CollisionPoint(200, 120),
                new CollisionPoint(200, 180),
                new CollisionPoint(200, 240),
                new CollisionPoint(200, 300),
                new CollisionPoint(200, 360),
                
                new CollisionPoint(100, 120),
                new CollisionPoint(100, 180),
                new CollisionPoint(100, 240),
                new CollisionPoint(100, 300),
                new CollisionPoint(100, 360),


                new CollisionPoint(600, 120),
                new CollisionPoint(600, 180),
                new CollisionPoint(600, 240),
                new CollisionPoint(600, 300),
                new CollisionPoint(600, 360),

                new CollisionPoint(700, 120),
                new CollisionPoint(700, 180),
                new CollisionPoint(700, 240),
                new CollisionPoint(700, 300),
                new CollisionPoint(700, 360),

            };

            Console.WriteLine("Object detector initialized.");
            Thread t = new Thread(() => {
                StartDepthCamera();
            });
            t.Start();

        }

        public void StartDepthCamera()
        {

            Console.WriteLine("Depth camera starting...");

            var cameraContext = new Context();
            var list = cameraContext.QueryDevices(); // Get a snapshot of currently connected devices   
            if (list.Count == 0)
                throw new Exception("No device detected. Is it plugged in?");

            var pipeline = new Pipeline(cameraContext);
            Config cfg = new Config();
            cfg.EnableStream(Intel.RealSense.Stream.Depth);
            pipeline.Start(cfg);

            Console.WriteLine("Depth camera started.");


            while (true)
            {
                using (var frames = pipeline.WaitForFrames())
                {
                    // 848 480
                    var depthFrame = frames.DepthFrame.DisposeWith(frames);
                    var colorizer = new Colorizer();
                    var colorizedDepth = colorizer.Process(depthFrame).DisposeWith(frames);

                    foreach(CollisionPoint p in points)
                    {
                        Vector3 coordinates = p.GetActualCoordinates();
                        // Is this within the depth frame?
                        if(WithinBounds(coordinates))
                        {
                            try {
                                                            // Get distance in depth frame
                                float dist = depthFrame.GetDistance((int)coordinates.X, (int)coordinates.Y);
                                if(!Passes(dist))
                                {
                                    if(YawController.Instance.Activated)
                                    {
                                        Console.WriteLine("Chair emergency stopped at: " + coordinates.X + ", " + coordinates.Y + " with value: " + dist);
                                        YawController.Instance.StopChair();
                                        lastEmergencyStop = DateTime.Now;
                                    }
                                }

                            } catch (Exception e)
                            {
                                Console.WriteLine(e.StackTrace);
                            }

                        }
                    }
                }
            }

        }

        public void Tick()
        {
            if(DateTime.Now.Subtract(lastEmergencyStop).Seconds > 5 && !YawController.Instance.Activated)
            {
                Program.Instance.Reset();
            } 
        }

        private bool WithinBounds(Vector3 point)
        {
            return point.X > 1 && point.X < FRAME_WIDTH && point.Y > 1 && point.Y < FRAME_HEIGHT;
        }

        private bool Passes(float dist)
        {
            return dist > 1.75 || dist < .01;
        }

    }

}
