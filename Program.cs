using System;
using YawSafety;


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
            YawController = new YawController();
            ObjectDetector = new ObjectDetector();

            while (true) ;
        }

    }

}