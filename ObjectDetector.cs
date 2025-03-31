using System.Diagnostics;
using System.Numerics;
using Intel.RealSense;

namespace YawSafety
{
    internal class ObjectDetector
    {
        public ObjectDetector()
        {
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
                    //Console.WriteLine(depthFrame.GetDistance(200, 200));
                    Vector3 point = TransformPoint(new Vector3(60, 100, 0));

                    Console.WriteLine(point);

                    if(depthFrame.GetDistance((int)point.X, (int)point.Y) < 2)
                    {
                        YawController.Instance.StopChair();
                    }
                }
            }

        }

        public Vector3 TransformPoint(Vector3 input)
        {
            Vector3 axis = new Vector3(0, 1, 0);
            double angle = 180 * (Math.PI / 180f);
            float qx = axis.X * (float)Math.Sin(angle/2);
            float qy = axis.Y * (float)Math.Sin(angle/2);
            float qz = axis.Z * (float)Math.Sin(angle/2);
            float qw = (float)Math.Cos(angle/2);
    
            Quaternion q = new Quaternion(qx, qy, qz, qw);
            Quaternion newPos = q * new Quaternion(input.X, input.Y, input.Z, 0) * Quaternion.Inverse(q);
            return new Vector3(newPos.X, newPos.Y, newPos.Z);
        }

    }

}
