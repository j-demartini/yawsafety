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

                    Vector3 centerPoint = new Vector3(depthFrame.Width, depthFrame.Height, 0);
                    Vector3 startingPoint = new Vector3(120, 240, 0);

                    // Get point relative to center
                    Vector3 relativePoint = startingPoint - centerPoint;

                    // Transform with quaternion
                    relativePoint = TransformPoint(relativePoint);

                    // Convert back to other coordinate frame
                    Vector3 finalPoint = relativePoint + centerPoint;

                    if(finalPoint.X > 1 && finalPoint.X < depthFrame.Width && finalPoint.Y > 1 && finalPoint.Y < depthFrame.Height)
                    {
                        float dist = depthFrame.GetDistance((int)finalPoint.X, (int)finalPoint.Y);
                        if(dist < 2 && dist > 0.01)
                        {
                            Console.WriteLine("Chair emergency stopped at: " + finalPoint.X + ", " + finalPoint.Y);
                            YawController.Instance.StopChair();
                        }
                    }
                }
            }

        }

        public Vector3 TransformPoint(Vector3 input)
        {
            Vector3 axis = new Vector3(0, 0, 1);
            double angle = YawController.Instance.ChairYaw * (Math.PI / 180f);
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
