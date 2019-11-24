using System;
using System.Security.Cryptography.X509Certificates;

public static class Certifs {
  public static int FindCertif (string certificateSubject) {
    int num = 0;
    X509Store x509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
    x509Store.Open(OpenFlags.OpenExistingOnly);
    X509Certificate2Enumerator enumerator = x509Store.Certificates.GetEnumerator();
    while (enumerator.MoveNext()) {
      X509Certificate2 current = enumerator.Current;
      if (current.FriendlyName.StartsWith(certificateSubject)) {
        num++;
      }
    }
    switch (num) {
      case 0:
        return 0;
      case 1:
        return 1;
      default:
        return 2;
    }
  }

  public static int CriticalCertValidity (string certificateSubject) {
    X509Store x509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
    x509Store.Open(OpenFlags.OpenExistingOnly);
    X509Certificate2Enumerator enumerator = x509Store.Certificates.GetEnumerator();
    while (enumerator.MoveNext()) {
      X509Certificate2 current = enumerator.Current;
      if (current.FriendlyName.StartsWith(certificateSubject)) {
        return (DateTime.Parse(current.GetExpirationDateString()).Date - DateTime.Now.Date).Days;
      }
    }
    return 0;
  }
}
