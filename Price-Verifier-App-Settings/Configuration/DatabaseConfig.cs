using Price_Verifier_App_Settings.Services;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml.Linq;
using System;

public class DatabaseConfig
{
    private readonly SecurityService _securityService;
    #region Additional
    private const string encryptionKey = "In the eye of the beholder doth lie beauty's true essence, for each gaze doth fashion its own fair visage";
    private readonly byte[] _salt = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };
    #endregion  

    public string Server { get; set; }
    public string Uid { get; set; }
    public string Port { get; set; }
    public string Pwd { get; set; }
    public string Database { get; set; }

    public DatabaseConfig()
    {
        // Initialize the SecurityService with the key and salt
        _securityService = new SecurityService(encryptionKey, _salt);

        var enviroment = System.Environment.CurrentDirectory;
        string projectDirectory = Directory.GetParent(enviroment).Parent.FullName;

        // Get the directory path of the currently executing assembly
        string appDirectory = projectDirectory; //Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        // Construct the config file path relative to the application directory
        string configFilePath = Path.Combine(appDirectory, "config.xml");

        try
        {
            if (File.Exists(configFilePath))
            {
                var doc = XDocument.Load(configFilePath);
                var databaseSettings = doc.Element("configuration").Element("databaseSettings");

                Server = databaseSettings.Element("add").Attribute("server").Value;
                Uid = _securityService.Decrypt(databaseSettings.Element("add").Attribute("uid").Value);
                Port = databaseSettings.Element("add").Attribute("port").Value;
                Pwd = _securityService.Decrypt(databaseSettings.Element("add").Attribute("pwd").Value);
                Database = databaseSettings.Element("add").Attribute("database").Value;
            }
            else
            {
                // Handle the case where the config file is not found
                throw new FileNotFoundException($"The configuration file 'config.xml' was not found in the directory '{appDirectory}'.");
            }
        }
        catch (CryptographicException ex)
        {
            // Log or handle the exception as needed
            Console.WriteLine($"Decryption Error: {ex.Message}");

            // Display the error message and exit the application
            MessageBox.Show("An error occurred while decrypting the data. Please check the encryption key and the encrypted data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }
        catch (FormatException ex)
        {
            // Handle the FormatException as needed
            Console.WriteLine($"Format Error: {ex.Message}");

            // Display the error message and exit the application
            MessageBox.Show("The encrypted data is not in the correct format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }
    }
}