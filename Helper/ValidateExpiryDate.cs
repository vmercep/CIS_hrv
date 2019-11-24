using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public static class ValidateExpiryDate {
  [DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
  public static extern bool ZeroMemory (ref string Destination, int Length);

  public static string GenerateKey () {
    DESCryptoServiceProvider dESCryptoServiceProvider = (DESCryptoServiceProvider) DES.Create();
    return Encoding.ASCII.GetString(dESCryptoServiceProvider.Key);
  }

  public static void Save (string dateTime) {
    FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
    string directoryName = fileInfo.DirectoryName;
    FileStream stream = new FileStream(Path.Combine(directoryName, "95d6c3f32d0508ebce35724496382eb3"), FileMode.Create, FileAccess.Write);
    DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
    dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes("?E??>b?T");
    dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes("?E??>b?T");
    ICryptoTransform transform = dESCryptoServiceProvider.CreateEncryptor();
    CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Write);
    byte[] bytes = Encoding.ASCII.GetBytes(dateTime);
    cryptoStream.Write(bytes, 0, bytes.Length);
    cryptoStream.Flush();
    cryptoStream.Close();
  }

  public static DateTime Load () {
    DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
    dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes("?E??>b?T");
    dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes("?E??>b?T");
    FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
    string directoryName = fileInfo.DirectoryName;
    FileStream stream = new FileStream(Path.Combine(directoryName, "95d6c3f32d0508ebce35724496382eb3"), FileMode.Open, FileAccess.Read);
    ICryptoTransform transform = dESCryptoServiceProvider.CreateDecryptor();
    CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
    string text = new StreamReader(cryptoStream).ReadToEnd();
    DateTime dateTime = default(DateTime);
    if (!Regex.IsMatch(text, "[0-9]{4}.[0-9]{2}.[0-9]{2}")) {
      dateTime = new DateTime(9999, 1, 1);
    } else {
      string[] array = text.Split('.');
      int year = int.Parse(array[0]);
      int month = int.Parse(array[1]);
      int day = int.Parse(array[2]);
      dateTime = new DateTime(year, month, day);
    }
    cryptoStream.Flush();
    cryptoStream.Close();
    return dateTime;
  }

  public static bool CheckIfFileExists () {
    FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
    string directoryName = fileInfo.DirectoryName;
    if (File.Exists(Path.Combine(directoryName, "95d6c3f32d0508ebce35724496382eb3"))) {
      return true;
    }
    return false;
  }
}
