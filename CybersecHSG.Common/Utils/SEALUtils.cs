using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Research.SEAL;

namespace CybersecHSG.Common.Utils
{
    public class SEALUtils
    {
        public static string CiphertextToBase64String(Ciphertext ciphertext)
        {
            using (var ms = new MemoryStream())
            {
                ciphertext.Save(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
        }
        public static Ciphertext BuildCiphertextFromBase64String(string base64, SEALContext context)
        {
            var payload = Convert.FromBase64String(base64);

            using (var ms = new MemoryStream(payload))
            {
                var ciphertext = new Ciphertext();
                ciphertext.Load(context, ms);

                return ciphertext;
            }
        }
        public static SEALContext GetContext()
        {
            var encryptionParameters = new EncryptionParameters(SchemeType.BFV);
            encryptionParameters.PolyModulusDegree = 4096;
            encryptionParameters.CoeffModulus = CoeffModulus.BFVDefault(4096);
            encryptionParameters.PlainModulus = new Modulus(1024);
            
            return new SEALContext(encryptionParameters);
        }
    }
}
