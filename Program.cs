using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Threading;

namespace HimawariNoFormAutostart
{
    static class Program
    {
        static HimawariAppContext app;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            app = new HimawariAppContext();
            Application.Run(app);
        }
    }

    /// <summary>
    /// MADE BY Ivan Koroteev ,  koroteeww@yandex.ru
    /// inspired by http://tproger.ru/tools/himawari-8-downloader/
    /// </summary>
    public class HimawariAppContext : ApplicationContext
    {
        //MAIN HIMAWARI URL
        private const string HIMAWARI_url = @"http://himawari8-dl.nict.go.jp/himawari8/img/D531106/";
        bool useLog = true;//set to false if no log needed
        bool useAutoStart = true;//set to false if no autostart needed

        private string appExeDir = "";//application dir
        private string logFileName = "log.txt";

        private string logPath = "";//path to log file

        DateTime dtNow;//universal datetime

        int width = 550;//width & height of one image block
        string level = "4d";//#Level can be 4d, 8d, 16d, 20d 
        int numblocks = 4;// #this apparently corresponds directly with the level, keep this exactly the same as level without the 'd'

        string time = "";//HHmmss

        string year = "";//year - yyyy

        string month = "";//MM

        string day = "";//dd

        WebRequest request;// web request for image update

        string resDir = "";// directory for saving

        string outFile = "latest_dotnet.jpg";//file name for image

        string him_url = "";//Himawari URL with datetime and level and width
        
        
        Bitmap img;//bitmap for saving
        
        Graphics graph;//drawing surface
        
        System.Drawing.Imaging.Encoder QualityEncoder;//Quality encoder
        
        EncoderParameters Encparams;//encoder params
       
        bool defaultProxy = true;//using default proxy, for Corporative internet
        
        ImageCodecInfo jpegEncoder;//jpec endcoder
        
        string last_path = "";//path for saved image
        System.Threading.Timer ThreadingTimer;//main timer
        TimerCallback tcb;//timer callback 
        int counter = 1;//thread counter
        int minutesInterval = 10;//interval for update

        //constructor
        public HimawariAppContext()
        {
            appExeDir = Path.GetDirectoryName(Application.ExecutablePath);

            logPath = Path.Combine(appExeDir, logFileName);
            //AUTORUN enabling
            if (useAutoStart)
            {
                AutoStart.EnableAutoStart(Application.ExecutablePath);
            }
            //Create the folder My Pictures\Himawari\ if it doesnt exist
            createFolder();
            //initital image set
            imageSet();
            //set encoder
            Encparams = new System.Drawing.Imaging.EncoderParameters(1);
            QualityEncoder = System.Drawing.Imaging.Encoder.Quality;
            //$encoderParams.Param[0] = New-Object System.Drawing.Imaging.EncoderParameter($qualityEncoder, 90)
            EncoderParameter quality90 = new EncoderParameter(QualityEncoder, 90L);
            Encparams.Param[0] = quality90;
            ImageCodecInfo[] allEncoders = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
            //next line is LINQ synononym to search in FOR cycle: for(j = 0; j < allEncoders.Length; ++j)
            //{                if (allEncoders[j].MimeType == "image/jpeg")
            //        return allEncoders[j];
            //}
            jpegEncoder = allEncoders.Where(imgEncoder => imgEncoder.MimeType == "image/jpeg").First();
            //INITIAL URL FILL
            FormNewUrl();
            addToLog("Program launch sucess");
            //starting thread
            start();
        }
        //setting new image in memory
        void imageSet()
        {
            img = new Bitmap(width * numblocks, width * numblocks);
            graph = Graphics.FromImage(img);
            //set black background
            graph.Clear(System.Drawing.Color.Black);
        }
        //URL filling from datetime
        void FormNewUrl()
        {
            dtNow = DateTime.Now.ToUniversalTime();          
            
            int minRemove = (dtNow.Minute % 10);
            int secondsRemove = dtNow.Second;
            //remove half an hour back is NECESSSARY
            dtNow = dtNow.AddMinutes(-30);
            //remove minutes
            dtNow = dtNow.AddMinutes((-1) * minRemove);
            //remove seconds
            dtNow = dtNow.AddSeconds((-1) * secondsRemove);
            //set time
            time = dtNow.ToString("HHmmss");
            year = dtNow.ToString("yyyy");
            month = dtNow.ToString("MM");
            day = dtNow.ToString("dd");
            
            //fill url
            him_url = HIMAWARI_url + level + "/" + width + "/" + year + "/" + month + "/" + day + "/" + time;
            
        }
        //saving image from url
        void SaveImage()
        {
            for (int y = 0; y < numblocks; y++)
            {
                for (int x = 0; x < numblocks; x++)
                {
                    try
                    {
                        string curUrl = him_url + "_" + x.ToString() + "_" + y.ToString() + ".png";
                        addToLog("Downloading " + curUrl);
                        request = WebRequest.Create(curUrl);
                        if (defaultProxy)
                        {
                            request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                        }
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        //HttpWebResponse httpResp = response.
                        HttpStatusCode status = response.StatusCode;
                        if (status == HttpStatusCode.OK)
                        {
                            //get image from http stream
                            Image curBlock = Image.FromStream(response.GetResponseStream());
                            //draw block in position
                            graph.DrawImage(curBlock, x * width, y * width, width, width);
                            curBlock.Dispose();
                            response.Close();

                        }


                    }
                    catch (Exception e)
                    {
                    addToLog("SaveImage "+e.Message+e.StackTrace);
                    }
                }
            }
            last_path = Path.Combine(resDir, outFile);
            //after this, all changes are in graph and in img
            img.Save(last_path, jpegEncoder, Encparams);
            img.Dispose();

            //new image set
            imageSet();
        }
        void SetWallpaper()
        {
            Wallpaper.Set(last_path, Wallpaper.Style.FIT);

        }

        void createFolder()
        {
            //#Create the folder My Pictures\Himawari\ if it doesnt exist
            string myPicPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string result = Path.Combine(myPicPath, @"Himawari\");
            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
                addToLog("Dir " + result + " created!");
            }
            resDir = result;
            
        }
        private void addToLog(string text)
        {
            if (useLog)
            {
                StreamWriter sw = File.AppendText(logPath);
                sw.WriteLine(text);
                sw.Close();
                sw.Dispose();
            }
            
        }
        //main thread target
        void thread_target(object param)
        {
            //tick
            addToLog("timer tick #" + counter + " started, next update at " + DateTime.Now.AddMinutes(minutesInterval).ToString());
            FormNewUrl();
            SaveImage();
            SetWallpaper();
            addToLog("Wallpaper SET OK");
            counter++;
        }
        //timer start
        void start()
        {
            tcb = new TimerCallback(thread_target);
            //timer start immediately with interval of 600000 miliseconds
            ThreadingTimer = new System.Threading.Timer(tcb, null, 0, minutesInterval * 60 * 1000);
                        
        }
        
    }

    /// <summary>
    /// from
    /// http://stackoverflow.com/questions/1061678/change-desktop-wallpaper-using-code-in-net
    /// </summary>
    public class Wallpaper
    {


        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style : int
        {
            Tiled,
            Centered,
            Stretched,
            FIT
        }

        public static void Set(string Path, Style style)
        {


            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == Style.Stretched)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Centered)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Tiled)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }
            if (style == Style.FIT)
            {
                //FIT
                key.SetValue(@"WallpaperStyle", 6.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
                //key.SetValue(@"WallpaperStyle", "6");
                //key.SetValue(@"TileWallpaper", "0");
            }
            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                Path,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
    //autorun class
    public static class AutoStart
    {
        private static string RUN_LOCATION = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
        private static string VALUE_NAME = "HimawariBackground";
        //set autostart 
        public static void EnableAutoStart(string pathToExeFile)
        {
           
                Registry.CurrentUser.CreateSubKey(RUN_LOCATION).SetValue(VALUE_NAME, (object)(pathToExeFile));
            
        }

        public static void DisableAutoStart()
        {
            Registry.CurrentUser.CreateSubKey(RUN_LOCATION).DeleteValue(VALUE_NAME);
        }
    }
}
