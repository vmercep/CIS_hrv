using _385_fisk.Exceptions;
using AutoUpdaterDotNET;
using CisDal;
using System;
using System.Diagnostics;
using System.Management;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

internal static class Program {

    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


    [STAThread]
    private static void Main () {

        try
        {
            log.Debug("Starting CIS application");
            ManagementClass mngmtClass = new ManagementClass("Win32_Process");
            foreach (ManagementObject o in mngmtClass.GetInstances())
            {
                if (o["Name"].Equals("MerlinX2.exe"))
                {
                    String commandLine = (String)o["CommandLine"];
                    Regex envRE = new Regex(@"/cis (.*)");
                    Match m = envRE.Match(commandLine);
                    if (m.Success)
                    {
                        Helper.Globals.Name = m.Groups[1].Value;
                        log.Debug("Setting global name to "+ Helper.Globals.Name);
                    }
                }
            }

        }catch(Exception e)
        {
            log.Error("Error ocured in trying to find merlin exe ",e);
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry("Error ocured in trying to find merlin exe "+e.Message, EventLogEntryType.Error);
            }
        }
        
        Helper.LogFile.InitConfig();
        CentralniInformacijskiSustav.CreateDirectories();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        string[] commandLineArgs = Environment.GetCommandLineArgs();
        //Console.WriteLine(commandLineArgs.Length);
        log.Debug("Command line arguments "+commandLineArgs.Length);

        log.Debug("Altering table");
        IMerlinData dalMerlin = new MerlinData();
        dalMerlin.AlterMerlinTable();
        log.Debug("Table altered");
        string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        log.Info("Starting CIS application version "+version);


        if (commandLineArgs.Length > 1)
        {
            string[] array = commandLineArgs;
            foreach (string a in array)
            {
                if (a == "/config")
                {
                    log.Debug("Starting config form-------");
                    Application.Run(new Config());
                }
            }
        }
        else
        {
            log.Debug("Starting main form------");
            Application.Run(new MainForm());
        }
    }



}
