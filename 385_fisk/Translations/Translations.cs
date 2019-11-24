using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

internal static class Translations {
  public static Dictionary<string, string> translations;

  static Translations () {
    translations = new Dictionary<string, string>();
    string str = "EN";
    if (!string.IsNullOrEmpty(AppLink.ActiveLanguage)) {
      str = AppLink.ActiveLanguage;
    }
    FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
    string directoryName = fileInfo.DirectoryName;
    string[] array = File.ReadAllLines(Path.Combine(directoryName, "Translations_" + str + ".txt"));
    string[] array2 = array;
    foreach (string text in array2) {
      string[] array3 = text.Split(new string[1]
      {
        "////"
      }, StringSplitOptions.None);
      if (!translations.ContainsKey(array3[0])) {
        translations.Add(array3[0], array3[1]);
      }
    }
  }

  public static string Translate (string text, Dictionary<string, string> placeholders = null) {
    if (translations.ContainsKey(text)) {
      if (placeholders != null) {
        string text2 = translations[text];
        foreach (KeyValuePair<string, string> placeholder in placeholders) {
          text2 = text2.Replace(placeholder.Key, placeholder.Value);
        }
        return text2;
      }
      return translations[text];
    }
    return text;
  }
}
