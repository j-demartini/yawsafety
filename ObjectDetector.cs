
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
                using (var frames = pipeline.WaitForFrames())
                {
                    // 848 480
                    var depthFrame = frames.DepthFrame.DisposeWith(frames);
                    var colorizer = new Colorizer();
                    var colorizedDepth = colorizer.Process(depthFrame).DisposeWith(frames);

                    for (int i = 0; i < 15; i++)
                    {
                        for (int j = 0; j < 15; j++)
                        {

                        }
                    }
                    Console.WriteLine(depthFrame.GetDistance(500, 5));

                }
            }

        }

    }
}
