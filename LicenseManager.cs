using System;
using System.IO;
using System.Management;  // Pour obtenir numéro de série machine

public static class LicenseManager
{
    private static string licenseFile = "license.dat";

    public static string GetMachineSerial()
    {
        var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS");
        foreach (ManagementObject obj in searcher.Get())
        {
            return obj["SerialNumber"].ToString();
        }
        return "UNKNOWN";
    }

    public static bool IsActivated()
    {
        if (!File.Exists(licenseFile)) return false;
        string savedSerial = File.ReadAllText(licenseFile).Trim();
        return savedSerial == GetMachineSerial();
    }

    public static bool Activate(string activationCode)
    {
        // Exemple simple : on considère que le code d’activation est le hash du numéro de série
        string serial = GetMachineSerial() ;
        string expectedCode = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(serial));
        //Console.WriteLine("Activation code : " + expectedCode);

        if (activationCode == expectedCode)
        {
            File.WriteAllText(licenseFile, expectedCode);
            return true;
        }
        return false;
    }
}
