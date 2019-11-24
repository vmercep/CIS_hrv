using AutoUpdaterDotNET;
using System;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows.Forms;

internal static class Program {




    [STAThread]
    private static void Main () {

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

        CentralniInformacijskiSustav.CreateDirectories(); //CreateDirectories();
        Helper.LogFile.InitConfig();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        string[] commandLineArgs = Environment.GetCommandLineArgs();
        Console.WriteLine(commandLineArgs.Length);
    
        if (commandLineArgs.Length > 1) {
      string[] array = commandLineArgs;
      foreach (string a in array) {
        if (a == "/config") {
          Application.Run(new Config());
        }
        if (a == "/validity") {
          Application.Run(new ExpirationDate());
        }
      }
    } else {
      Application.Run(new MainForm());
    }
  }



}
