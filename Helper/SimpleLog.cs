using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
/*
public static class SimpleLog {

  public enum Severity {
    Info,
    Warning,
    Error,
    Exception
  }

  private static DirectoryInfo _logDir;

  private static string _prefix;

  private static string _dateFormat;

  private static string _suffix;

  private static string _extension;

  private static Severity _logLevel;

  private static readonly Queue<XElement> _logEntryQueue;

  private static Task _backgroundTask;

  private static readonly object _backgroundTaskSyncRoot;

  private static readonly object _logFileSyncRoot;

  private static string _textSeparator;

  public static string LogDir => _logDir.FullName;

  public static string Prefix {
    get {
      return _prefix ?? string.Empty;
    }
    set {
      _prefix = value;
    }
  }

  public static string Suffix {
    get {
      return _suffix ?? string.Empty;
    }
    set {
      _suffix = value;
    }
  }

  public static string Extension {
    get {
      return _extension ?? "log";
    }
    set {
      _extension = value;
    }
  }

  public static string DateFormat {
    get {
      return _dateFormat ?? "yyyy_MM_dd";
    }
    set {
      _dateFormat = value;
    }
  }

  public static Severity LogLevel {
    get {
      return _logLevel;
    }
    set {
      _logLevel = value;
    }
  }

  public static bool StartExplicitly {
    get;
    set;
  }

  public static bool WriteText {
    get;
    set;
  }

  public static string TextSeparator {
    get {
      return _textSeparator;
    }
    set {
      _textSeparator = (value ?? string.Empty);
    }
  }

  public static string FileName => GetFileName(DateTime.Now);

  public static bool StopEnqueingNewEntries {
    get;
    private set;
  }

  public static bool StopLoggingRequested {
    get;
    private set;
  }

  public static Exception LastExceptionInBackgroundTask {
    get;
    private set;
  }

  public static int NumberOfLogEntriesWaitingToBeWrittenToFile => _logEntryQueue.Count;

  public static bool LoggingStarted => _backgroundTask != null;

  static SimpleLog () {
    _logDir = new DirectoryInfo(Directory.GetCurrentDirectory());
    _logLevel = Severity.Info;
    _logEntryQueue = new Queue<XElement>();
    _backgroundTaskSyncRoot = new object();
    _logFileSyncRoot = new object();
    _textSeparator = " | ";
    AppDomain.CurrentDomain.ProcessExit += CurrentDomainProcessExit;
  }

  private static void CurrentDomainProcessExit (object sender, EventArgs e) {
    StopLogging();
  }

  public static Exception SetLogFile (string logDir = null, string prefix = null, string suffix = null, string extension = null, string dateFormat = null, Severity? logLevel = default(Severity?), bool? startExplicitly = default(bool?), bool check = true, bool? writeText = default(bool?), string textSeparator = null) {
    Exception ex = null;
    try {
      if (writeText.HasValue) {
        WriteText = writeText.Value;
      }
      if (textSeparator != null) {
        TextSeparator = textSeparator;
      }
      if (logLevel.HasValue) {
        LogLevel = logLevel.Value;
      }
      if (extension != null) {
        Extension = extension;
      }
      if (suffix != null) {
        Suffix = suffix;
      }
      if (dateFormat != null) {
        DateFormat = dateFormat;
      }
      if (prefix != null) {
        Prefix = prefix;
      }
      if (startExplicitly.HasValue) {
        StartExplicitly = startExplicitly.Value;
      }
      if (logDir != null) {
        ex = SetLogDir(logDir, createIfNotExisting: true);
      }
      if (ex == null && check) {
        ex = Check();
      }
    } catch (Exception ex2) {
      ex = ex2;
    }
    return ex;
  }

  public static Exception SetLogDir (string logDir, bool createIfNotExisting = false) {
    if (string.IsNullOrEmpty(logDir)) {
      logDir = Directory.GetCurrentDirectory();
    }
    try {
      _logDir = new DirectoryInfo(logDir);
      if (!_logDir.Exists) {
        if (!createIfNotExisting) {
          throw new DirectoryNotFoundException($"Directory '{_logDir.FullName}' does not exist!");
        }
        _logDir.Create();
      }
    } catch (Exception result) {
      return result;
    }
    return null;
  }

  public static Exception Check (string message = "Test entry to see if logging works.") {
    return Log(message, Severity.Info, useBackgroundTask: false);
  }

  public static Exception Info (string message, bool useBackgroundTask = true) {
    return Log(message);
  }

  public static Exception Warning (string message, bool useBackgroundTask = true) {
    return Log(message, Severity.Warning);
  }

  public static Exception Error (string message, bool useBackgroundTask = true) {
    return Log(message, Severity.Error);
  }

  public static Exception Log (Exception ex, bool useBackgroundTask = true, int framesToSkip = 0) {
    return (ex == null) ? null : Log(GetExceptionXElement(ex), Severity.Exception, useBackgroundTask, framesToSkip);
  }

  public static string GetExceptionAsXmlString (Exception ex) {
    XElement exceptionXElement = GetExceptionXElement(ex);
    return (exceptionXElement == null) ? string.Empty : exceptionXElement.ToString();
  }

  public static XElement GetExceptionXElement (Exception ex) {
    if (ex == null) {
      return null;
    }
    XElement xElement = new XElement("Exception");
    xElement.Add(new XAttribute("Type", ex.GetType().FullName));
    xElement.Add(new XAttribute("Source", (ex.TargetSite == null || ex.TargetSite.DeclaringType == null) ? ex.Source : $"{ex.TargetSite.DeclaringType.FullName}.{ex.TargetSite.Name}"));
    xElement.Add(new XElement("Message", ex.Message));
    if (ex.Data.Count > 0) {
      XElement xElement2 = new XElement("Data");
      IDictionaryEnumerator enumerator = ex.Data.GetEnumerator();
      try {
        while (enumerator.MoveNext()) {
          DictionaryEntry dictionaryEntry = (DictionaryEntry) enumerator.Current;
          xElement2.Add(new XElement("Entry", new XAttribute("Key", dictionaryEntry.Key), new XAttribute("Value", dictionaryEntry.Value ?? string.Empty)));
        }
      } finally {
        (enumerator as IDisposable)?.Dispose();
      }
      xElement.Add(xElement2);
    }
    if (ex is SqlException) {
      SqlException ex2 = (SqlException) ex;
      XElement xElement3 = new XElement("SqlException");
      xElement3.Add(new XAttribute("ErrorNumber", ex2.Number));
      if (!string.IsNullOrEmpty(ex2.Server)) {
        xElement3.Add(new XAttribute("ServerName", ex2.Server));
      }
      if (!string.IsNullOrEmpty(ex2.Procedure)) {
        xElement3.Add(new XAttribute("Procedure", ex2.Procedure));
      }
      xElement.Add(xElement3);
    }
    if (ex is COMException) {
      COMException ex3 = (COMException) ex;
      XElement xElement4 = new XElement("ComException");
      xElement4.Add(new XAttribute("ErrorCode", $"0x{(uint) ex3.ErrorCode:X8}"));
      xElement.Add(xElement4);
    }
    if (ex is AggregateException) {
      XElement xElement5 = new XElement("AggregateException");
      foreach (Exception innerException in ((AggregateException) ex).InnerExceptions) {
        xElement5.Add(GetExceptionXElement(innerException));
      }
      xElement.Add(xElement5);
    }
    xElement.Add((ex.InnerException == null) ? new XElement("StackTrace", ex.StackTrace) : GetExceptionXElement(ex.InnerException));
    return xElement;
  }

  public static Exception Log (string message, Severity severity = Severity.Info, bool useBackgroundTask = true, int framesToSkip = 0) {
    return string.IsNullOrEmpty(message) ? null : Log(new XElement("Message", message), severity, useBackgroundTask, framesToSkip);
  }

  public static Exception Log (XElement xElement, Severity severity = Severity.Info, bool useBackgroundTask = true, int framesToSkip = 0) {
    if (xElement == null || severity < LogLevel) {
      return null;
    }
    try {
      XElement xElement2 = new XElement("LogEntry");
      xElement2.Add(new XAttribute("Date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
      xElement2.Add(new XAttribute("Severity", severity));
      xElement2.Add(new XAttribute("Source", GetCaller(framesToSkip)));
      xElement2.Add(new XAttribute("ThreadId", Thread.CurrentThread.ManagedThreadId));
      xElement2.Add(xElement);
      if (!useBackgroundTask) {
        return WriteLogEntryToFile(xElement2);
      }
      Enqueue(xElement2);
    } catch (Exception result) {
      return result;
    }
    return null;
  }

  public static string GetFileName (DateTime dateTime) {
    return $"{LogDir}\\{Prefix}{dateTime.ToString(DateFormat)}{Suffix}.{Extension}";
  }

  public static bool LogFileExists (DateTime dateTime) {
    return File.Exists(GetFileName(dateTime));
  }

  public static XDocument GetLogFileAsXml () {
    return GetLogFileAsXml(DateTime.Now);
  }

  public static XDocument GetLogFileAsXml (DateTime dateTime) {
    string fileName = GetFileName(dateTime);
    if (!File.Exists(fileName)) {
      return null;
    }
    Flush();
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
    stringBuilder.AppendLine("<LogEntries>");
    stringBuilder.AppendLine(File.ReadAllText(fileName));
    stringBuilder.AppendLine("</LogEntries>");
    return XDocument.Parse(stringBuilder.ToString());
  }

  public static string GetLogFileAsText () {
    return GetLogFileAsText(DateTime.Now);
  }

  public static string GetLogFileAsText (DateTime dateTime) {
    string fileName = GetFileName(dateTime);
    if (!File.Exists(fileName)) {
      return null;
    }
    Flush();
    return File.ReadAllText(fileName);
  }

  public static void ShowLogFile () {
    ShowLogFile(DateTime.Now);
  }

  public static void ShowLogFile (DateTime dateTime) {
    string text;
    if (WriteText) {
      Flush();
      text = GetFileName(dateTime);
    } else {
      text = string.Format("{0}Log_{1}.xml", Path.GetTempPath(), DateTime.Now.ToString("yyyyMMddHHmmssffff"));
      GetLogFileAsXml(dateTime)?.Save(text);
    }
    if (File.Exists(text)) {
      Process.Start(text);
      Thread.Sleep(2000);
    }
  }

  public static void StartLogging () {
    if (_backgroundTask == null && !StopEnqueingNewEntries && !StopLoggingRequested) {
      StopEnqueingNewEntries = false;
      StopLoggingRequested = false;
      lock (_backgroundTaskSyncRoot) {
        if (_backgroundTask == null) {
          LastExceptionInBackgroundTask = null;
          _backgroundTask = new Task(WriteLogEntriesToFile, TaskCreationOptions.LongRunning);
          _backgroundTask.Start();
        }
      }
    }
  }

  public static void StopLogging (bool flush = true) {
    StopEnqueingNewEntries = true;
    if (_backgroundTask != null) {
      if (flush) {
        Flush();
      }
      StopLoggingRequested = true;
      lock (_backgroundTaskSyncRoot) {
        if (_backgroundTask != null) {
          _backgroundTask.Wait(1000);
          _backgroundTask = null;
        }
      }
    }
  }

  public static void Flush () {
    if (LoggingStarted) {
      while (NumberOfLogEntriesWaitingToBeWrittenToFile > 0) {
        int numberOfLogEntriesWaitingToBeWrittenToFile = NumberOfLogEntriesWaitingToBeWrittenToFile;
        Thread.Sleep(222);
        if (numberOfLogEntriesWaitingToBeWrittenToFile == NumberOfLogEntriesWaitingToBeWrittenToFile) {
          break;
        }
      }
    }
  }

  public static void ClearQueue () {
    lock (_logEntryQueue) {
      _logEntryQueue.Clear();
    }
  }

  private static void Enqueue (XElement logEntry) {
    if (!StopEnqueingNewEntries) {
      if (!StartExplicitly) {
        StartLogging();
      }
      lock (_logEntryQueue) {
        if (_logEntryQueue.Count < 10000) {
          _logEntryQueue.Enqueue(logEntry);
        }
      }
    }
  }

  private static XElement Peek () {
    lock (_logEntryQueue) {
      return (_logEntryQueue.Count == 0) ? null : _logEntryQueue.Peek();
    }
  }

  private static void Dequeue () {
    lock (_logEntryQueue) {
      if (_logEntryQueue.Count > 0) {
        _logEntryQueue.Dequeue();
      }
    }
  }

  private static void WriteLogEntriesToFile () {
    while (!StopLoggingRequested) {
      XElement xElement = Peek();
      if (xElement == null) {
        Thread.Sleep(100);
      } else {
        for (int i = 0; i < 10; i++) {
          Exception ex = WriteLogEntryToFile(xElement);
          WriteOwnExceptionToEventLog(ex);
          LastExceptionInBackgroundTask = ex;
          if (LastExceptionInBackgroundTask == null || NumberOfLogEntriesWaitingToBeWrittenToFile > 1000) {
            break;
          }
          Thread.Sleep(100);
        }
        Dequeue();
      }
    }
  }

  private static void WriteOwnExceptionToEventLog (Exception ex) {
    if (ex != null && (LastExceptionInBackgroundTask == null || !(ex.Message == LastExceptionInBackgroundTask.Message))) {
      try {
        string message;
        try {
          XElement exceptionXElement = GetExceptionXElement(ex);
          message = exceptionXElement.ToString();
        } catch {
          message = ex.Message;
        }
        if (!EventLog.SourceExists("SimpleLog")) {
          EventLog.CreateEventSource("SimpleLog", "Application");
        }
        EventLog.WriteEntry("SimpleLog", message, EventLogEntryType.Error, 0);
      } catch {
      }
    }
  }

  private static Exception WriteLogEntryToFile (XElement xmlEntry) {
    if (xmlEntry != null) {
      if (!Monitor.TryEnter(_logFileSyncRoot, new TimeSpan(0, 0, 0, 5))) {
        try {
          return new Exception($"Could not write to file '{FileName}', because it was blocked by another thread for more than {5} seconds.");
        } catch (Exception result) {
          return result;
        }
      }
      try {
        using (FileStream stream = new FileStream(FileName, FileMode.Append, FileAccess.Write, FileShare.None)) {
          using (StreamWriter streamWriter = new StreamWriter(stream)) {
            if (WriteText) {
              streamWriter.WriteLine(ConvertXmlToPlainText(xmlEntry));
            } else {
              streamWriter.WriteLine(xmlEntry);
            }
          }
        }
        return null;
      } catch (Exception ex) {
        try {
          ex.Data["Filename"] = FileName;
        } catch {
        }
        try {
          WindowsIdentity current = WindowsIdentity.GetCurrent();
          ex.Data["Username"] = ((current == null) ? "unknown" : current.Name);
        } catch {
        }
        return ex;
      } finally {
        Monitor.Exit(_logFileSyncRoot);
      }
    }
    return null;
  }

  private static string ConvertXmlToPlainText (XElement xmlEntry) {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (XElement item in xmlEntry.DescendantsAndSelf()) {
      if (item.HasAttributes) {
        foreach (XAttribute item2 in item.Attributes()) {
          if (stringBuilder.Length > 0) {
            stringBuilder.Append(TextSeparator);
          }
          stringBuilder.Append(item2.Name).Append(" = ").Append(item2.Value);
        }
      } else {
        if (stringBuilder.Length > 0) {
          stringBuilder.Append(TextSeparator);
        }
        string value = item.Value.Replace("\r\n", " ");
        stringBuilder.Append(item.Name).Append(" = ").Append(value);
      }
    }
    return stringBuilder.ToString();
  }

  private static string GetCaller (int framesToSkip = 0) {
    string result = string.Empty;
    int num = 1;
    Type declaringType;
    do {
      StackFrame stackFrame = new StackFrame(num++);
      MethodBase method = stackFrame.GetMethod();
      if (method == null) {
        break;
      }
      declaringType = method.DeclaringType;
      if (declaringType == null) {
        break;
      }
      result = $"{declaringType.FullName}.{method.Name}";
    }
    while (!(declaringType != typeof(SimpleLog)) || --framesToSkip >= 0);
    return result;
  }
}
*/