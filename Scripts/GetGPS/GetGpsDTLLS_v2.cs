using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
namespace GetGpsTLL
{
    class Program
    {
        //User input stuff
        static string DataSource;
        static string tmpStr;
        static bool uPortOK = false;
        static string uRequest = ""; //user requested parameter
        static bool _listen = true;
        static public string outStr;
        static string _Usage;
        static int oneSec = 1000;
        static double uTimeOut = 1.0;
        //Serial Port Stuff
        static SerialPortManager _spManager;

        //Timer
        static Timer _timer = new Timer(); // From System.Timers


        static void Main(string[] args)
        {
            _Usage = " Usage: getGpsTll <S> (Serial) or <T> (TCP/IP) <-p # (port)> <-o (Output Variable) all, lat, lon, time, sddbk, sddbt> [-b #### = baud ]  [-d #.# = draft ] [-t # (Timeout sec 1 = 1 second)];  <ER> or <EW -e #>";
            try
            {
                outStr = "";
                
                int uPort = 0;  //  Port number to listen on for data
                //setupWorker(bw_Listen);
                int ubaud = 9600; // Default baud rate

                if (args.Length == 0)
                {
                    System.Console.WriteLine(_Usage);
                    return;
                }
                else
                {
                    DataSource = getArgs(args).Trim();
                    if ((DataSource != "ER") && (DataSource != "S") && (DataSource != "s") && (DataSource != "T") && (DataSource != "t") && (DataSource != "EW"))
                    {
                        System.Console.WriteLine(_Usage);
                        return;
                    }
                    tmpStr = getArgs(args, "-t");
                    bool ok = double.TryParse(tmpStr, out uTimeOut);
                    if (ok == false)
                    {
                        uTimeOut = 2;
                    }
                    _timer.Interval = oneSec * uTimeOut; // Set up the timer 
                    _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed); //Add event handler for time elapsed

                    _timer.Enabled = true;
                    _timer.Start();
                    if ((DataSource != "ER") && (DataSource != "EW"))
                    {
                        tmpStr = getArgs(args, "-o");
                        if (tmpStr == "lat" || tmpStr == "lon" || tmpStr == "time" || tmpStr == "all" || tmpStr == "sddbk" || tmpStr == "sddbt")
                        {
                            uRequest = tmpStr;
                        }
                        else
                        {
                            System.Console.WriteLine(_Usage);
                        }
                        tmpStr = getArgs(args, "-p");
                        ok = int.TryParse(tmpStr, out uPort);
                        if (ok == false)
                        {
                            System.Console.WriteLine(_Usage);
                            return;
                        }
                        tmpStr = getArgs(args, "-b");
                        if (tmpStr != "")
                        {
                            ok = int.TryParse(tmpStr, out ubaud);
                            if (ok == false)
                            {
                                System.Console.WriteLine(_Usage);
                                return;
                            }
                        }
                        if (DataSource == "S")
                        {
                            //User is requesting data from Serial Port
                            //uPort is an integer that holds the user's port number
                            //Check that the port number exists.
                            
                            UserInitialization("COM" + uPort.ToString(), ref uPortOK, ubaud); // Com port
                            _spManager = new SerialPortManager("COM" + uPort.ToString(), ref uPortOK, uRequest, ubaud);
                            if (!uPortOK)
                            {
                                Console.WriteLine("Port {0} Not available!", "Com " + uPort.ToString());
                                //Console.ReadLine();
                                return;
                            }
                            SerialSettings mySerialSettings = _spManager.CurrentSerialSettings;
                            _spManager.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved);
                            _spManager.StartListening();
                            string[] gpsData;
                            
                            while (_listen)
                            {
                                if (_spManager.LastReceived.Length > 31)
                                {
                                    
                                    gpsData = _spManager.LastReceived.Split(',');
                                    //gpsData = "$GPGGA,215306,4515.4550,N,06416.1939,W,2,7,0.8,12.8,M,,,,*01".Split(',');
                                    if (uRequest == "time" || uRequest == "lat" || uRequest == "lon" || uRequest == "all")
                                    {
                                        double dLatDeg = Convert.ToDouble(gpsData[2].Substring(0, 2));
                                       // double dLatMin = Convert.ToDouble(gpsData[2].Substring(3, gpsData[2].Length - 3)) / 60;
                                        double dLatMin = Convert.ToDouble(gpsData[2].Substring(2, gpsData[2].Length - 2)) / 60 ;
                                        double dLat = Math.Round(dLatDeg + dLatMin, 6);
                                        if (gpsData[3] == "S")
                                        {
                                            dLat = dLat * -1;
                                        }

                                        gpsData = _spManager.LastReceived.Split(',');
                                        double dLonDeg = Convert.ToDouble(gpsData[4].Substring(0, 3));
                                        double dLonMin = Convert.ToDouble(gpsData[4].Substring(3, gpsData[4].Length - 3)) / 60;
                                        double dLon = Math.Round(dLonDeg + dLonMin, 6);
                                        if (gpsData[5] == "W")
                                        {
                                            dLon = dLon * -1;
                                        }

                                        if (uRequest == "time")
                                        {
                                            outStr = gpsData[1];
                                        }
                                        else if (uRequest == "lat")
                                        {
                                            outStr = dLat.ToString();
                                        }
                                        else if (uRequest == "lon")
                                        {
                                            outStr = dLon.ToString();
                                        }
                                        else if (uRequest == "all")
                                        {
                                            outStr = gpsData[1] + " | " + dLat.ToString() + " | " + dLon.ToString();
                                        }
                                    }
                                    else
                                    {
                                        if (uRequest == "sddbk" || uRequest == "sddbt")
                                        {
                                            double uDraft = 0.0;
                                            tmpStr = getArgs(args, "-d");
                                            if (tmpStr != "")
                                            {
                                                ok = double.TryParse(tmpStr, out uDraft);
                                                if (ok == false)
                                                {
                                                    uDraft = 0.0;
                                                }
                                            }

                                            double sddb = Convert.ToDouble(gpsData[3]);
                                            sddb = sddb + uDraft;
                                            outStr = sddb.ToString();
                                        }
                                    }
                                    _timer.Enabled = false;
                                    _listen = false;
                                }
                                else
                                {
                                    outStr = "NA";
                                }
                                
                            }
                            _spManager.StopListening();
                        }
                    }
                    else
                    {


                        if (DataSource == "EW")
                        {
                            //Write new event number to file.
                            tmpStr = getArgs(args, "-e");
                            System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\dev\\ELOG\\scripts\\EV.txt");
                            file.WriteLine(tmpStr);
                            //System.IO.File.WriteAllText(@"C:\dev\ELOG\scripts\EV.txt", tmpStr);
                            file.Close();
                            outStr = "";
                        }
                        else if (DataSource == "ER")
                        {
                            //Read event number from file and increment by 1
                            System.IO.StreamReader file = new System.IO.StreamReader("C:\\dev\\ELOG\\scripts\\EV.txt");
                            string text = file.ReadToEnd();
                            file.Close();
                            //string text = System.IO.File.ReadAllText(@"C:\dev\ELOG\scripts\EV.txt");
                            if (text.Length == 0)
                            {
                                text = "0";
                            }
                            int newCon = Convert.ToInt32(text);
                            newCon++;
                            outStr = newCon.ToString();

                        }
                    }

                }                
                CloseAll();
            }
            catch 
            {
                Console.WriteLine("Invalid Arg.");
                //System.Console.ReadKey();
                CloseAll();
            }    

        }// Main  


        static public void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //throw new NotImplementedException();

            // Time is up. Close program. 
            if (DataSource != "ER" && DataSource != "EW")
            {
                _spManager.StopListening();
            }
            _timer.Enabled = false;
            _listen = false;
        }    
            
        static public void CloseAll()
        {
            try
            {
                Console.WriteLine(outStr);

                //Readline used for testing...
                //Console.WriteLine("Press any key to exit.");
                //System.Console.ReadKey();
                
                //_timer.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
            }
        }
        static public string getArgs(string[] inArgs)
        {
            string argVal = "";
            argVal = inArgs[0];

            return argVal;
        }

        static public string getArgs(string[] inArgs, string opt)
        {
            string argVal = "";
            for (int i = 0; i < inArgs.Length; i++)
            {
                if (inArgs[i] == opt)
                {
                    argVal = inArgs[i + 1];
                }
            }
            return argVal;
        }
        
        static public void UserInitialization(string uPort, ref bool uPortOK, int ubaud)
        {
            try
            {
                SerialPortManager _spManager = new SerialPortManager(uPort, ref uPortOK, uRequest,ubaud);
                SerialSettings mySerialSettings = _spManager.CurrentSerialSettings;
                //serialSettingsBindingSource.DataSource = mySerialSettings;
                _spManager.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved);
            }
            catch
            {
                Console.WriteLine("UserInitialization");
            }

        }
        
        static public void _spManager_NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {
            
            //This application is connected to a GPS sending ASCCI characters, so data is converted to text
            string str = Encoding.ASCII.GetString(e.Data);
            _spManager.StopListening();

        }
    }
}
