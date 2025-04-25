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
                ObjectDetector = new ObjectDetector();
            } catch (Exception e)
            {
                Console.WriteLine("Detector Error: " + e.Message);  
            }

            try {
                Controller = new GpioController();
                Controller.OpenPin(ORANGE_PIN, PinMode.Output);
                Controller.OpenPin(GREEN_PIN, PinMode.Output);
            } catch (Exception)
            {
                Console.WriteLine("Error opening pins.");
            }

            Controller.Write(GREEN_PIN, PinValue.High);

            while (Active)
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
            Controller.Write(GREEN_PIN, PinValue.Low);
            Controller.Write(ORANGE_PIN, PinValue.Low);
            Controller.ClosePin(GREEN_PIN);
            Controller.ClosePin(ORANGE_PIN);
            Thread.Sleep(2000);
            new Program();
        }

        public void OrangeLightOn(bool b)
        {
            if(Controller != null)
            {
                if(Controller.IsPinOpen(ORANGE_PIN))
                    Controller.Write(ORANGE_PIN, b ? PinValue.High : PinValue.Low);
            }
        }
 
    }

}