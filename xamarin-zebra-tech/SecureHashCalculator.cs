using System;
using System.Security.Cryptography;
using System.Text;

namespace xamarin_zebra_tech
{
    public static class SecureHashCalculator
    {
        public static string CalculateSecureHash(string vendorID, string messageBody)
        {
            const string salt = "salt";
            string concatenatedData = salt + vendorID + messageBody;
            string sha256Hash = null;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(concatenatedData));
                StringBuilder hexString = new StringBuilder();

                foreach (byte hashByte in hashBytes)
                {
                    string hex = hashByte.ToString("x2");
                    hexString.Append(hex);
                }

                sha256Hash = hexString.ToString().ToUpper();
            }

            return sha256Hash;
        }

        public static string GetMessageBody(string vendorID, string locationID, string txnType,
                                        string invNo, string plateNo, string startTime,
                                        string endTime, string amount, string timeStamp)
        {
            string concatenatedData = vendorID + locationID + txnType + invNo +
                                      plateNo + startTime + endTime + amount + timeStamp;
            return concatenatedData;
        }

        private static readonly string SEPARATOR = ((char)30).ToString();
        public static string CalculateCRCChecksum(string secureHash, string vendorID, string messageBody)
        {
            string concatenatedData = secureHash + SEPARATOR + vendorID + SEPARATOR + messageBody;

            // Calculate CRC32 checksum
            uint crcValue;
            using (var crc32 = new Crc32())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(concatenatedData);
                crcValue = crc32.ComputeHash(bytes);
            }

            string crc32Checksum = crcValue.ToString("X").ToUpper();
            crc32Checksum = crc32Checksum.PadLeft(8, '0');

            return crc32Checksum;
        }
    }
}

public class Crc32 : HashAlgorithm
{
    private static readonly uint[] Table;
    private uint crcValue;

    static Crc32()
    {
        const uint polynomial = 0xEDB88320;
        Table = new uint[256];
        for (uint i = 0; i < 256; i++)
        {
            uint crc = i;
            for (int j = 8; j > 0; j--)
            {
                if ((crc & 1) == 1)
                    crc = (crc >> 1) ^ polynomial;
                else
                    crc >>= 1;
            }
            Table[i] = crc;
        }
    }

    public Crc32()
    {
        Initialize();
    }

    public override void Initialize()
    {
        crcValue = 0xFFFFFFFF;
    }

    protected override void HashCore(byte[] buffer, int start, int length)
    {
        for (int i = start; i < start + length; i++)
        {
            byte index = (byte)(((crcValue) & 0xff) ^ buffer[i]);
            crcValue = (crcValue >> 8) ^ Table[index];
        }
    }

    protected override byte[] HashFinal()
    {
        crcValue = ~crcValue;
        return BitConverter.GetBytes(crcValue);
    }

    public override int HashSize => 32;

    public uint ComputeHash(byte[] buffer)
    {
        HashCore(buffer, 0, buffer.Length);
        return crcValue;
    }
}