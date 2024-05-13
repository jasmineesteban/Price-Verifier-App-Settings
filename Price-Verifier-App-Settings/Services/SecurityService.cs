using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Price_Verifier_App_Settings.Services
{
    internal class SecurityService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public SecurityService(string password, byte[] salt)
        {
            _key = DeriveKeyFromPassword(password, salt);
            _iv = new byte[16]; // AES uses a 128-bit (16-byte) IV
        }

        private byte[] DeriveKeyFromPassword(string password, byte[] salt)
        {
            var iterations = 1000;
            var desiredKeyLength = 32; // 256 bits
            var hashMethod = HashAlgorithmName.SHA256;

            using (var deriveBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), salt, iterations, hashMethod))
            {
                return deriveBytes.GetBytes(desiredKeyLength);
            }
        }


        public string Decrypt(string cipherText)
        {
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = _key;
                    aes.IV = _iv;

                    using (MemoryStream ms = new MemoryStream(cipherBytes))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader((Stream)cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (CryptographicException ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Decryption Error: {ex.Message}");

                // Show a more user-friendly message
                MessageBox.Show("An error occurred while decrypting the data. Please check the encryption key and the encrypted data.", "Decryption Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Exit the application
                Environment.Exit(1);
                return null;
            }
            catch (FormatException ex)
            {
                // Handle the FormatException as needed
                Console.WriteLine($"Format Error: {ex.Message}");

                // Show a more user-friendly message
                MessageBox.Show("The encrypted data is not in the correct format.", "Format Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Exit the application
                Environment.Exit(1);
                return null;
            }
        }


    }
}

