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
                    Console.WriteLine(depthFrame.GetDistance(60, 100));
                }
            }

        }

    }
}
