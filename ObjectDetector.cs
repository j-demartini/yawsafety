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
            pipeline.Start();

            Console.WriteLine("Depth camera started.");


            while (true)
            {
                FrameSet set;
                pipeline.TryWaitForFrames(out set, 10000);

                // 848 480
                var depthFrame = set.DepthFrame.DisposeWith(set);
                var colorizer = new Colorizer();
                var colorizedDepth = colorizer.Process(depthFrame).DisposeWith(set);
                //Console.WriteLine(depthFrame.GetDistance(200, 200));
                Console.WriteLine(depthFrame.GetDistance(135, 135));

            }

        }

    }
}
