using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

public class EntityBase<T> {
  private static XmlSerializer serializer;

  private static XmlSerializer Serializer {
    get {
      if (serializer == null) {
        serializer = new XmlSerializerFactory().CreateSerializer(typeof(T));
      }
      return serializer;
    }
  }

  public virtual string Serialize (Encoding encoding) {
    StreamReader streamReader = null;
    MemoryStream memoryStream = null;
    try {
      memoryStream = new MemoryStream();
      XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
      xmlWriterSettings.Encoding = encoding;
      xmlWriterSettings.Indent = false;
      XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
      XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
      xmlSerializerNamespaces.Add("tns", "http://www.apis-it.hr/fin/2012/types/f73");
      xmlSerializerNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
      Serializer.Serialize(xmlWriter, this, xmlSerializerNamespaces);
      memoryStream.Seek(0L, SeekOrigin.Begin);
      streamReader = new StreamReader(memoryStream, encoding);
      return streamReader.ReadToEnd();
    } finally {
      streamReader?.Dispose();
      memoryStream?.Dispose();
    }
  }

  public virtual string Serialize () {
    return Serialize(Encoding.UTF8);
  }

  public static bool Deserialize (string input, out T obj, out Exception exception) {
    exception = null;
    obj = default(T);
    try {
      obj = Deserialize(input);
      return true;
    } catch (Exception ex) {
      Exception ex2 = exception = ex;
      return false;
    }
  }

  public static bool Deserialize (string input, out T obj) {
    Exception exception = null;
    return Deserialize(input, out obj, out exception);
  }

  public static T Deserialize (string input) {
    StringReader stringReader = null;
    try {
      stringReader = new StringReader(input);
      return (T) Serializer.Deserialize(XmlReader.Create(stringReader));
    } finally {
      stringReader?.Dispose();
    }
  }

  public static T Deserialize (Stream s) {
    return (T) Serializer.Deserialize(s);
  }

  public virtual bool SaveToFile (string fileName, Encoding encoding, out Exception exception) {
    exception = null;
    try {
      SaveToFile(fileName, encoding);
      return true;
    } catch (Exception ex) {
      Exception ex2 = exception = ex;
      return false;
    }
  }

  public virtual bool SaveToFile (string fileName, out Exception exception) {
    return SaveToFile(fileName, Encoding.UTF8, out exception);
  }

  public virtual void SaveToFile (string fileName) {
    SaveToFile(fileName, Encoding.UTF8);
  }

  public virtual void SaveToFile (string fileName, Encoding encoding) {
    StreamWriter streamWriter = null;
    try {
      string value = Serialize(encoding);
      streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
      streamWriter.WriteLine(value);
      streamWriter.Close();
    } finally {
      streamWriter?.Dispose();
    }
  }

  public static bool LoadFromFile (string fileName, Encoding encoding, out T obj, out Exception exception) {
    exception = null;
    obj = default(T);
    try {
      obj = LoadFromFile(fileName, encoding);
      return true;
    } catch (Exception ex) {
      Exception ex2 = exception = ex;
      return false;
    }
  }

  public static bool LoadFromFile (string fileName, out T obj, out Exception exception) {
    return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
  }

  public static bool LoadFromFile (string fileName, out T obj) {
    Exception exception = null;
    return LoadFromFile(fileName, out obj, out exception);
  }

  public static T LoadFromFile (string fileName) {
    return LoadFromFile(fileName, Encoding.UTF8);
  }

  public static T LoadFromFile (string fileName, Encoding encoding) {
    FileStream fileStream = null;
    StreamReader streamReader = null;
    try {
      fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
      streamReader = new StreamReader(fileStream, encoding);
      string input = streamReader.ReadToEnd();
      streamReader.Close();
      fileStream.Close();
      return Deserialize(input);
    } finally {
      fileStream?.Dispose();
      streamReader?.Dispose();
    }
  }
}
