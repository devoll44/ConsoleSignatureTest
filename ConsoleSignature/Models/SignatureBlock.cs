using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSignature.Models
{
    public class SignatureBlock
    {
        public byte[] BlockBytes { get; set; }
        public uint BlockNumber { get; set; }

        public string GetBlockSHA256Str()
        {
            using (SHA256 SHA256 = SHA256.Create())
            {
                var hashBytes = SHA256.ComputeHash(BlockBytes);
                return BlockNumber + ". " + Convert.ToBase64String(hashBytes);
            }
        }
    }
}
