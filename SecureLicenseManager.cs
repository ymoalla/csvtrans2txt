using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Management;  // Pour obtenir numéro de série machine

namespace wtrans2cnrps
{
    /// <summary>
    /// Gestionnaire de licences sécurisé - À distribuer avec l'application cliente
    /// </summary>
    public class SecureLicenseManager
    {
        private const string licenseFile = "license.lic";
        
        // Clé publique RSA (à intégrer dans l'application)
        // IMPORTANT: Remplacez ceci par votre CLÉ PUBLIQUE générée
        private const string PUBLIC_KEY = @"<RSAKeyValue><Modulus>wEEDGW471yLnwPLGDZAX+ZSZZXGOI9l4O3zJ9KRCJUUJ9D8l2f4SuEEKulvp/xNBgBauxRtovFN10GCGWbzV9o32aEO4+HpY9ZG1OU0iLMe45RJgi7LV9yvsHQUbgDLXaptHLKHnJjNJyh2yIbWZvAJR4jr/cyItP4i0pOsvstCbfhBukIpeSBdNOdMQg+OPwgSOq0+1iaG6q4rQSH13Q4yfiu14CZ1IGwcJZCPxRybErVYJhz5NXzx7EF3xsWxlP7OqcoVeS7cOzytZK02CU4XhDUZpyO7Jy6WtjOngg8ke/lEFZ+L1brWrq6Y/9UnGG1vjr+e6FwGr/5o+mGVaDQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        /// <summary>
        /// Vérifie et active la licence avec une signature cryptographique
        /// </summary>
        public static bool Activate(string activationCode)
        {
            try
            {
                if (PUBLIC_KEY.Contains("REMPLACER_PAR_VOTRE_CLE"))
                {
                    Console.WriteLine("ERREUR CONFIGURATION: Clé publique non configurée!");
                    return false;
                }

                // Le code d'activation contient : machine|date|email|signature
                string[] parts = activationCode.Split('|');
                if (parts.Length != 4)
                {
                    Console.WriteLine("Format de code d'activation invalide!");
                    return false;
                }

                string machineSerial = parts[0];
                string expirationDate = parts[1];
                string email = parts[2];
                string signature = parts[3];

                string licenseData = $"{machineSerial}|{expirationDate}|{email}";

                // Vérifie que les données incluent le numéro de série de cette machine
                string currentMachineSerial = GetMachineSerialNumber();
                if (machineSerial != currentMachineSerial)
                {
                    Console.WriteLine("❌ Cette licence n'est pas valide pour cette machine!");
                    Console.WriteLine($"   Attendu: {currentMachineSerial}");
                    Console.WriteLine($"   Reçu: {machineSerial}");
                    return false;
                }

                // Vérifie la signature cryptographique avec la clé publique
                if (VerifySignature(licenseData, signature))
                {
                    // Chiffre et sauvegarde la licence
                    SaveEncryptedLicense(activationCode);
                    Console.WriteLine("✅ Licence activée avec succès!");
                    Console.WriteLine($"   Email: {email}");
                    Console.WriteLine($"   Expire le: {expirationDate}");
                    return true;
                }

                Console.WriteLine("❌ Signature invalide - code d'activation incorrect!");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur d'activation: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Vérifie si une licence valide existe
        /// </summary>
        /// <param name="showMessages">Afficher les messages d'erreur dans la console</param>
        /// <returns>True si la licence est valide et active</returns>
        public static bool IsActivated(bool showMessages = false)
        {
            try
            {
                // Vérification 1: Le fichier de licence existe-t-il?
                if (!File.Exists(licenseFile))
                {
                    if (showMessages)
                        Console.WriteLine("❌ Aucun fichier de licence trouvé");
                    return false;
                }

                // Vérification 2: Déchiffrement de la licence
                string encryptedLicense = File.ReadAllText(licenseFile);
                string decryptedLicense;
                
                try
                {
                    decryptedLicense = DecryptLicense(encryptedLicense);
                }
                catch (Exception ex)
                {
                    if (showMessages)
                        Console.WriteLine($"❌ Impossible de déchiffrer la licence: {ex.Message}");
                    return false;
                }
                
                // Vérification 3: Format du code d'activation (doit avoir 4 parties)
                string[] parts = decryptedLicense.Split('|');
                if (parts.Length != 4)
                {
                    if (showMessages)
                        Console.WriteLine($"❌ Format de licence invalide (attendu 4 parties, trouvé {parts.Length})");
                    return false;
                }

                string machineSerial = parts[0];
                string expirationDateStr = parts[1];
                string email = parts[2];
                string signature = parts[3];

                string licenseData = $"{machineSerial}|{expirationDateStr}|{email}";

                // Vérification 4: La licence correspond-elle à cette machine?
                string currentMachineSerial = GetMachineSerialNumber();
                if (machineSerial != currentMachineSerial)
                {
                    if (showMessages)
                    {
                        Console.WriteLine("❌ Cette licence n'est pas valide pour cette machine");
                        Console.WriteLine($"   Machine actuelle: {currentMachineSerial}");
                        Console.WriteLine($"   Licence pour: {machineSerial}");
                    }
                    return false;
                }

                // Vérification 5: Date d'expiration
                if (DateTime.TryParse(expirationDateStr, out DateTime expirationDate))
                {
                    if (DateTime.Now > expirationDate)
                    {
                        if (showMessages)
                            Console.WriteLine($"❌ Licence expirée le: {expirationDate:dd/MM/yyyy}");
                        return false;
                    }
                }
                else
                {
                    if (showMessages)
                        Console.WriteLine($"❌ Date d'expiration invalide: {expirationDateStr}");
                    return false;
                }

                // Vérification 6: Signature cryptographique
                bool signatureValid = VerifySignature(licenseData, signature);
                
                if (!signatureValid && showMessages)
                    Console.WriteLine("❌ Signature cryptographique invalide - licence corrompue ou falsifiée");

                return signatureValid;
            }
            catch (CryptographicException ex)
            {
                if (showMessages)
                    Console.WriteLine($"❌ Erreur cryptographique: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                if (showMessages)
                    Console.WriteLine($"❌ Erreur lors de la vérification: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Vérifie la licence et affiche un message détaillé
        /// </summary>
        /// <returns>True si la licence est valide</returns>
        public static bool CheckLicenseWithDetails()
        {
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("🔍 Vérification de la licence");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine();

            bool isValid = IsActivated(showMessages: true);

            if (isValid)
            {
                LicenseInfo info = GetLicenseInfo();
                if (info != null)
                {
                    Console.WriteLine();
                    Console.WriteLine("✅ Licence valide et active");
                    Console.WriteLine($"   📧 Email: {info.Email}");
                    Console.WriteLine($"   📅 Expire le: {info.ExpirationDate:dd/MM/yyyy}");
                    
                    TimeSpan remaining = info.ExpirationDate - DateTime.Now;
                    if (remaining.TotalDays <= 30)
                    {
                        Console.WriteLine($"   ⚠️  Attention: Expire dans {(int)remaining.TotalDays} jours");
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════════════");
            
            return isValid;
        }

        /// <summary>
        /// Affiche le numéro de série de cette machine
        /// Le client doit vous envoyer ce numéro pour obtenir un code d'activation
        /// </summary>
        public static void ShowMachineSerial()
        {
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("🖥️  Numéro de série de cette machine");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine();
            
            string serial = GetMachineSerialNumber();
            
            Console.WriteLine(serial);
            Console.WriteLine();
            Console.WriteLine("📋 Instructions:");
            Console.WriteLine("   1. Copiez ce numéro de série");
            Console.WriteLine("   2. Envoyez-le à votre fournisseur");
            Console.WriteLine("   3. Vous recevrez un code d'activation");
            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════════════");
        }

        /// <summary>
        /// Obtient le numéro de série de la machine (version publique)
        /// </summary>
        public static string GetMachineSerialNumber()
        {
            // return GetMachineSerial();
            var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS");
            foreach (ManagementObject obj in searcher.Get())
            {
                return obj["SerialNumber"].ToString();
            }
            return "UNKNOWN";
            
        }

        /// <summary>
        /// Obtient les informations de la licence
        /// </summary>
        public static LicenseInfo GetLicenseInfo()
        {
            try
            {
                if (!File.Exists(licenseFile))
                    return null;

                string encryptedLicense = File.ReadAllText(licenseFile);
                string decryptedLicense = DecryptLicense(encryptedLicense);
                
                string[] parts = decryptedLicense.Split('|');
                if (parts.Length != 4)
                    return null;

                return new LicenseInfo
                {
                    MachineSerial = parts[0],
                    ExpirationDate = DateTime.Parse(parts[1]),
                    Email = parts[2],
                    IsValid = IsActivated()
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Vérifie une signature RSA
        /// </summary>
        private static bool VerifySignature(string data, string signatureBase64)
        {
            try
            {
                using (var rsa = new RSACryptoServiceProvider(2048))
                {
                    rsa.FromXmlString(PUBLIC_KEY);
                    
                    byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                    byte[] signatureBytes = Convert.FromBase64String(signatureBase64);

                    return rsa.VerifyData(dataBytes, SHA256.Create(), signatureBytes);
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sauvegarde la licence chiffrée
        /// </summary>
        private static void SaveEncryptedLicense(string license)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] key = DeriveKey();
                aes.Key = key;
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length);
                    
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(license);
                    }
                    
                    File.WriteAllText(licenseFile, Convert.ToBase64String(ms.ToArray()));
                }
            }
        }

        /// <summary>
        /// Déchiffre la licence sauvegardée
        /// </summary>
        private static string DecryptLicense(string encryptedLicense)
        {
            byte[] buffer = Convert.FromBase64String(encryptedLicense);
            
            using (Aes aes = Aes.Create())
            {
                byte[] key = DeriveKey();
                aes.Key = key;
                
                byte[] iv = new byte[aes.IV.Length];
                Array.Copy(buffer, 0, iv, 0, iv.Length);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream(buffer, iv.Length, buffer.Length - iv.Length))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Dérive une clé de 256 bits
        /// </summary>
        private static byte[] DeriveKey()
        {
            string machineSerial = GetMachineSerialNumber();
            byte[] entropy = GetEntropy();
            
            byte[] combined = Encoding.UTF8.GetBytes(machineSerial + Convert.ToBase64String(entropy));
            
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(combined);
            }
        }

        /// <summary>
        /// Obtient une entropie supplémentaire
        /// </summary>
        private static byte[] GetEntropy()
        {
            return Encoding.UTF8.GetBytes("wtrans2cnrps_v2_salt_2025");
        }

        /// <summary>
        /// Récupère le numéro de série de la machine (version simplifiée)
        /// </summary>
        private static string GetMachineSerial()
        {
            try
            {
                StringBuilder identifier = new StringBuilder();
                
                identifier.Append(Environment.MachineName);
                identifier.Append(Environment.UserName);
                identifier.Append(Environment.UserDomainName);
                identifier.Append(Environment.ProcessorCount);
                identifier.Append(Environment.Is64BitOperatingSystem);
                identifier.Append(Environment.OSVersion.Platform);
                identifier.Append(Environment.OSVersion.Version);
                
                try
                {
                    DriveInfo systemDrive = new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory));
                    identifier.Append(systemDrive.VolumeLabel);
                    identifier.Append(systemDrive.DriveFormat);
                    identifier.Append(GetVolumeSerial(systemDrive.Name));
                }
                catch { }

                using (var sha256 = SHA256.Create())
                {
                    byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(identifier.ToString()));
                    return Convert.ToBase64String(hashBytes);
                }
            }
            catch
            {
                return Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(Environment.MachineName + Environment.UserName)
                );
            }
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern bool GetVolumeInformation(
            string rootPathName,
            StringBuilder volumeNameBuffer,
            uint volumeNameSize,
            out uint volumeSerialNumber,
            out uint maximumComponentLength,
            out uint fileSystemFlags,
            StringBuilder fileSystemNameBuffer,
            uint fileSystemNameSize
        );

        private static string GetVolumeSerial(string driveLetter)
        {
            try
            {
                uint serialNumber = 0;
                uint maxComponentLength = 0;
                uint fileSystemFlags = 0;
                StringBuilder volumeName = new StringBuilder(256);
                StringBuilder fileSystemName = new StringBuilder(256);

                bool success = GetVolumeInformation(
                    driveLetter,
                    volumeName,
                    (uint)volumeName.Capacity,
                    out serialNumber,
                    out maxComponentLength,
                    out fileSystemFlags,
                    fileSystemName,
                    (uint)fileSystemName.Capacity
                );

                return success ? serialNumber.ToString("X8") : "";
            }
            catch
            {
                return "";
            }
        }
    }

    /// <summary>
    /// Informations sur la licence
    /// </summary>
    public class LicenseInfo
{
    public string MachineSerial { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string Email { get; set; }
    public bool IsValid { get; set; }
}
}
