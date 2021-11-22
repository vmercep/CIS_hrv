﻿using _385_fisk.Exceptions;
using AutoUpdaterDotNET;
using System;
using System.Diagnostics;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows.Forms;

internal static class Program {




    [STAThread]
    private static void Main () {

        try
        {
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
                        //Console.WriteLine(o["Name"] + " [" + m.Groups[1] + "]");
                        Helper.Globals.Name = m.Groups[1].Value;
                    }
                }
            }

        }catch(Exception e)
        {
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
        Console.WriteLine(commandLineArgs.Length);

        if (commandLineArgs.Length > 1)
        {
            string[] array = commandLineArgs;
            foreach (string a in array)
            {
                if (a == "/config")
                {
                    Application.Run(new Config());
                }
            }
        }
        else
        {
            Application.Run(new MainForm());
        }
    }



}
