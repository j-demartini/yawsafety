using Intel.RealSense;

namespace YawSafety
{
    class Program
    {

        public static Program Instance;

        //public static Application WinApp { get; private set; }
        //public static Window MainWindow { get; private set; }
        public static YawController YawController { get; private set; }
        public static ObjectDetector ObjectDetector { get; private set; }
        public bool Active { get; private set; }
        
        static void Main(string[] args)
        {
            new Program();
        }

        public Program()
        {

            Active = true;
            Instance = this;

            try {
                YawController = new YawController();            
            } catch (Exception e)
            {
                Console.WriteLine("Network Error: " + e.StackTrace);
            }
            
            try {
                ObjectDetector = new ObjectDetector();
            } catch (Exception e)
            {
                Console.WriteLine("Detector Error: " + e.Message);
            }

            while (true)
            {
                if(YawController != null)
                {
                    YawController.Tick();
                }
                if(ObjectDetector != null)
                {
                    ObjectDetector.Tick();
                }
            }
        }

        public void Reset()
        {
            Active = false;
            YawController = null;
            ObjectDetector = null;
            Thread.Sleep(1000);
            new Program();
        }
 
    }

}