using System.Device.Gpio;
using Intel.RealSense;

namespace YawSafety
{
    class Program
    {

        public const int ORANGE_PIN = 14;
        public const int GREEN_PIN = 15;


        public static Program Instance;

        //public static Application WinApp { get; private set; }
        //public static Window MainWindow { get; private set; }
        public static YawController YawController { get; private set; }
        public static ObjectDetector ObjectDetector { get; private set; }
        public static GpioController Controller { get; private set; }
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
                //ObjectDetector = new ObjectDetector();
            } catch (Exception e)
            {
                Console.WriteLine("Detector Error: " + e.Message);  
            }

            Controller = new GpioController();
            Controller.OpenPin(ORANGE_PIN, PinMode.Output);
            Controller.OpenPin(GREEN_PIN, PinMode.Output);

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


                Controller.Write(ORANGE_PIN, PinValue.High);
                Controller.Write(GREEN_PIN, PinValue.High);
                Thread.Sleep(1000);
                Controller.Write(ORANGE_PIN, PinValue.Low);
                Controller.Write(GREEN_PIN, PinValue.Low);
                Thread.Sleep(1000);

            }
        }

        public void Reset()
        {
            Active = false;
            YawController = null;
            ObjectDetector = null;
            Thread.Sleep(5000);
            new Program();
        }
 
    }

}