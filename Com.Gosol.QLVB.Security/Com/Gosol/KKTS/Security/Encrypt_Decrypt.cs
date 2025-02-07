using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Com.Gosol.QLVB.Security.Com.Gosol.QLVB.Security
{
    public static class Encrypt_Decrypt
    {
        public static string KeySecret_AES = "jh1f23bnhr3dt6h#1e%13463d&&@G%^&";//độ dài Key phải là 32 bit
        public static string vector_IV_AES = ""; // độ dài vector IV_AES phải là 16 bit

        /// <summary>
        /// Hàm mã hóa dữ liệu sử dụng thuật toán AES
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string EncryptStrings_Aes(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }
            byte[] encrypted;

            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(KeySecret_AES);
                aesAlg.IV = Encoding.UTF8.GetBytes(vector_IV_AES);

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            string dataEncrypt = Convert.ToBase64String(encrypted);
            // Return the encrypted bytes from the memory stream.
            return dataEncrypt;
        }

        /// <summary>
        /// hàm giải mã dữ liệu sừ dụng thuật toán AES
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns> 
        public static string DecryptString_Aes(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return cipherText;
            }
            byte[] dataByteArr = Convert.FromBase64String(cipherText);
            // Declare the string used to hold
            // the decrypted text.
            string DataPlaintext = null;

            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(KeySecret_AES); ;
                aesAlg.IV = Encoding.UTF8.GetBytes(vector_IV_AES);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(dataByteArr))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            DataPlaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return DataPlaintext;
        }

        /// <summary>
        /// hàm mã hóa file sử dụng thuật toán DES
        /// </summary>
        /// <param name="fileSource"></param>
        /// <returns></returns>
        public static byte[] EncryptFile_AES(byte[] fileSource)
        {
            try
            {
                // Create a MemoryStream.
                MemoryStream mStream = new MemoryStream();

                // Create a new DES object.
                //Aes DESalg = Aes.Create();
                AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider();
                aesAlg.Key = Encoding.UTF8.GetBytes(KeySecret_AES);
                aesAlg.IV = Encoding.UTF8.GetBytes(vector_IV_AES);

                // Create a CryptoStream using the MemoryStream
                // and the passed key and initialization vector (IV).
                CryptoStream cStream = new CryptoStream(mStream,
                    aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV),
                    CryptoStreamMode.Write);

                // Write the byte array to the crypto stream and flush it.
                cStream.Write(fileSource, 0, fileSource.Length);
                cStream.FlushFinalBlock();

                // Get an array of bytes from the
                // MemoryStream that holds the
                // encrypted data.
                byte[] fileEncrypted = mStream.ToArray();
                // Close the streams.

                cStream.Close();
                mStream.Close();

                // Return the encrypted buffer.
                return fileEncrypted;
            }
            //catch (CryptographicException e)
            //{
            //    return null;
            //}
            catch (Exception eX)
            {
                throw eX;
                return null;
            }
        }



        /// <summary>
        /// hàm giải mã file mã hóa bởi thuật toán DES
        /// </summary>
        /// <param name="fileEncrypted"></param>
        /// <returns></returns>
        public static string DecryptFile_AES(byte[] fileEncrypted)
        {
            try
            {
                // Create a new MemoryStream using the passed
                // array of encrypted data.
                MemoryStream msDecrypt = new MemoryStream(fileEncrypted);

                // Create a new DES object.
                //AES DESalg = DES.Create();
                //DESalg.Key = Encoding.UTF8.GetBytes(KeySecret_DES);
                //DESalg.IV = Encoding.UTF8.GetBytes(vector_IV_DES);
                AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider();
                aesAlg.Key = Encoding.UTF8.GetBytes(KeySecret_AES);
                aesAlg.IV = Encoding.UTF8.GetBytes(vector_IV_AES);

                // Create a CryptoStream using the MemoryStream
                // and the passed key and initialization vector (IV).
                CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                    aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV),
                    CryptoStreamMode.Read);

                //// Create buffer to hold the decrypted data.
                //byte[] fromEncrypt = new byte[fileEncrypted.Length];

                // Read the decrypted data out of the crypto stream
                // and place it into the temporary buffer.
                csDecrypt.Read(fileEncrypted, 0, fileEncrypted.Length);

                byte[] fileDecrypted = msDecrypt.ToArray();
                string FileBase64 = Convert.ToBase64String(fileDecrypted);
                return FileBase64;
            }
            catch (Exception ex)
            {
                throw ex;
                return null;
            }
        }
    }
}

