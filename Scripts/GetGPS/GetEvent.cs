using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetEvent
{
    class Program
    {
        static String DataSource;
        static String tmpStr;
        static long number1 = 0;    

        static void Main(string[] args)
        {
            try
            {
                string ourLoc = getArgs(args, "-l");
                if (ourLoc.Length == 0)
                {
                    ourLoc = "C:\\dev\\ELOG\\scripts\\EV.txt";
                }


                if (args.Length == 0)
                {
                    System.Console.WriteLine(" Usage: getEvent <ER> or <EW -e # [-l <file location and name>] Example: GETEVENT_v2 ER -l c:\\dev\\EventNum.txt");
                    return;
                }
                else
                {


                    DataSource = getArgs(args).Trim();

                    if ((DataSource != "ER") && (DataSource != "EW"))
                    {
                        System.Console.WriteLine(" Usage: getEvent <ER> or <EW -e #>");
                        return;
                    }
                        if (DataSource == "EW")
                        {
                            //Write new event number to file.
                            
                            tmpStr = getArgs(args, "-e");
                            bool canConvert = long.TryParse(tmpStr, out number1);
                            if (canConvert == true)
                            {
                                System.IO.StreamWriter file = new System.IO.StreamWriter(ourLoc);
                                file.WriteLine(tmpStr);
                                //System.IO.File.WriteAllText(@"C:\dev\ELOG\scripts\EV.txt", tmpStr);
                                file.Close();
                            }

                        }
                        else if (DataSource == "ER")
                        {
                            //Read event number from file and increment by 1
                            string text;
                            try
                            {
                                System.IO.StreamReader file = new System.IO.StreamReader(ourLoc);
                                text = file.ReadToEnd();
                                file.Close();
                            }
                            catch
                            {
                                text = "";
                            }
                            //string text = System.IO.File.ReadAllText(@"C:\dev\ELOG\scripts\EV.txt");
                            if (text.Length == 0)
                            {
                                text = "0";
                            }
                            int newCon = Convert.ToInt32(text);
                            newCon++;
                            //Pad the number with 0 to a total length of 3
                            string outVal = newCon.ToString();
                            if (outVal.Length == 1)
                            {
                                outVal = "00" + outVal;
                            }
                            else if (outVal.Length == 2)
                            {
                                outVal = "0" + outVal;
                            }
                            Console.WriteLine(outVal);
                           
                        }   
                }
                //Readline used for testing...
                //Console.WriteLine("Press any key to exit.");
                //System.Console.ReadKey();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Could not find file"))
                {
                    Console.WriteLine(ex.Message);
                }
                else
                {
                    Console.WriteLine("Arg. Err.");
                }
            }
        }
        static string getArgs(string[] inArgs)
        {
            string argVal = "";
            argVal = inArgs[0];

            return argVal;
        }
        static string getArgs(string[] inArgs, string opt)
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
    }
}
