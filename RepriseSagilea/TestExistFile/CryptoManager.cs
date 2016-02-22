using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace TestExistFile
{
    public class CryptoManager
    {
        public string SourcePath { get; set; }

        public string Vector { get; set; }

        public string Key { get; set; }

        public string DestinationPath { get; set; }

        public string NewFile { get; set; }

        public CryptoManager(string source, string key, string vector, string destination)
        {
            SourcePath = source;
            Key = key;
            Vector = vector;
            DestinationPath = destination;
            NewFile = string.Empty;
        }

        public bool EncryptFile(string sInputFilename, string sOutputFilename, byte[] key, byte[] iv)
        {
            var ret = true;
            var aes = new AesCryptoServiceProvider();
            try
            {
                aes.Key = key;
                aes.IV = iv;
                var fsInput = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read);
                var bytearrayinput = new byte[fsInput.Length];
                fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
                fsInput.Close();
                fsInput.Dispose();
                var fsEncrypted = new FileStream(sOutputFilename, FileMode.Create, FileAccess.Write);
                var encryptor = aes.CreateEncryptor();
                var cryptostream = new CryptoStream(fsEncrypted, encryptor, CryptoStreamMode.Write);
                cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
                cryptostream.Close();
                cryptostream.Dispose();
                fsEncrypted.Close();
                fsEncrypted.Dispose();
                encryptor.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("Le crypto service a planté (EncryptFile) : \r\n" + ex.Message);
                ret = false;
            }

            aes.Clear();
            aes.Dispose();

            return ret;
        }

        
        // waits until a file can be opened with write permission
        private int WaitReady(string fileName)
        {
            if (!File.Exists(fileName))
                return -1;

            while (true)
            {
                try
                {
                    using (Stream stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        if (stream != null)
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                }
                Thread.Sleep(500);
            }

            return 0;
        }

        

        public bool ProcessFileEncryption(string sourceFile)
        {
            try
            {
                var key = Key;
                var vector =Vector;

                // wait the file creation to be terminated
                if (WaitReady(sourceFile) < 0) return false;

                // JCH 201410117 - EVOLUTION : suppression de l'extension PDF lors du renomage du fichier crypté - string newFilePath = string.Format("{0}\\{1}{2}.crypt", rootpath, newName, extension);
                var newFilePath = string.Format("{0}\\{1}.crypt", DestinationPath, Path.GetFileNameWithoutExtension(sourceFile));

                // remove if already existing
                if (File.Exists(newFilePath)) File.Delete(newFilePath);

                if (EncryptFile(sourceFile, newFilePath, Convert.FromBase64String(key), Convert.FromBase64String(vector)))
                {
                    //File.Delete(sourceFile);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Le crypto service a planté (ProcessFileEncryption) : \r\n" + ex.Message);
            }
            return false;
        }

        

        public void OnEncryption(object sender, FileSystemEventArgs e)
        {
            ProcessFileEncryption(e.FullPath);
        }

        public void OnDecryption(object sender, FileSystemEventArgs e)
        {
            ProcessFileDecryption(e.FullPath);
        }

        public bool DecryptFile(string sInputFilename, string sOutputFilename, byte[] key, byte[] iv)
        {
            try
            {
                var aes = new AesCryptoServiceProvider {Key = key, IV = iv};
                var fsread = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read);
                var bytearrayinput = new byte[fsread.Length];
                var decryptor = aes.CreateDecryptor();

                var cryptostreamDecr = new CryptoStream(fsread, decryptor, CryptoStreamMode.Read);
                cryptostreamDecr.Read(bytearrayinput, 0, bytearrayinput.Length);
                var fsDecrypted = new FileStream(sOutputFilename, FileMode.Create, FileAccess.Write);
                fsDecrypted.Write(bytearrayinput, 0, bytearrayinput.Length);
                fsDecrypted.Close();
                cryptostreamDecr.Close();
                fsread.Close();
                aes.Clear();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool ProcessFileDecryption(string sourceFile)
        {
            try
            {
                var key = Key;
                var vector = Vector;
                // wait the file creation to be terminated
                if (WaitReady(sourceFile) < 0) return false;

                //suppression de l'extension CRYPT lors du renomage du fichier décrypté - string newFilePath = string.Format("{0}\\{1}{2}.pdf", rootpath, newName, extension);
                var newFilePath = string.Format("{0}\\{1}.pdf", DestinationPath, Path.GetFileNameWithoutExtension(sourceFile));

                // remove if already existing
                if (File.Exists(newFilePath)) File.Delete(newFilePath);

                if (DecryptFile(sourceFile, newFilePath, Convert.FromBase64String(key), Convert.FromBase64String(vector)))
                {
                    //File.Delete(sourceFile);
                    NewFile = newFilePath;
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Le crypto service a planté (ProcessFileDecryption) : \r\n" + ex.Message);
            }
            return false;
        }

        
    }
}
