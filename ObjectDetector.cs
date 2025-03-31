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

        public ObjectDetector()
        {
         
            points = new List<CollisionPoint>()
            {

                new CollisionPoint(240, 120),
                new CollisionPoint(240, 180),
                new CollisionPoint(240, 240),
                new CollisionPoint(240, 300),
                new CollisionPoint(240, 360),
                new CollisionPoint(540, 120),
                new CollisionPoint(540, 180),
                new CollisionPoint(540, 240),
                new CollisionPoint(540, 300),
                new CollisionPoint(540, 360),

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
                            // Get distance in depth frame
                            float dist = depthFrame.GetDistance((int)coordinates.X, (int)coordinates.Y);
                            if(!Passes(dist))
                            {
                                Console.WriteLine("Chair emergency stopped at: " + coordinates.X + ", " + coordinates.Y + " with value: " + dist);
                                YawController.Instance.StopChair();
                                return;
                            }
                        }
                    }
                }
            }

        }

        private bool WithinBounds(Vector3 point)
        {
            return point.X > 1 && point.X < FRAME_WIDTH && point.Y > 1 && point.Y < FRAME_HEIGHT;
        }

        private bool Passes(float dist)
        {
            return dist > 2 || dist < .01;
        }

    }

}
