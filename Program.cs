using Intel.RealSense;

namespace YawSafety
{
    class Program
    {

        //public static Application WinApp { get; private set; }
        //public static Window MainWindow { get; private set; }
        public static YawController YawController { get; private set; }
        public static ObjectDetector ObjectDetector { get; private set; }
 
        public bool visualization = true;
        
        static void Main(string[] args)
        {
            new Program();
        }

        public Program()
        {
            // try {
            //     YawController = new YawController();            
            // } catch (Exception e)
            // {
            //     Console.WriteLine("Network Error: " + e.StackTrace);
            // }
            
            try {
                ObjectDetector = new ObjectDetector();
            } catch (Exception e)
            {
                Console.WriteLine("Detector Error: " + e.StackTrace);
            }

            while (true) ;
        }
 
    }

}