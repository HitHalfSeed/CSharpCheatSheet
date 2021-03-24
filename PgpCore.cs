// PgpCore
using System;
using System.IO;
using PgpCore; // Install-Package PgpCore

namespace PGP_Console_App
{
    class Program
    {
        private static readonly string ROOT = @"C:\Dev\Temp";
        private static readonly string UNENCRYPTED_FILE = @"Encrypt Me.txt";
        private static readonly string SIGNED_FILE = UNENCRYPTED_FILE + ".sig";
        private static readonly string ENCRYPTED_FILE = SIGNED_FILE + ".gpg";
        private static readonly string DECRYPTED_FILE = @"Decrypted file.txt.sig";

        private static readonly string ROOT_KEYS = @"C:\Dev\Temp\PGP Keys";
        private static readonly string PUBLIC_KEY = @"public-key.asc";
        private static readonly string PRIVATE_KEY = @"private-key.ppk";
        private static readonly string RANDOM_PUBLIC_KEY = @"random-public-key.asc";

        static void Main(string[] args)
        {
            var publicKey = new EncryptionKeys(File.ReadAllText(Path.Combine(ROOT_KEYS, PUBLIC_KEY)));
            var privateKey = new EncryptionKeys(File.ReadAllText(Path.Combine(ROOT_KEYS, PRIVATE_KEY)), "Password1!");

            using (var publicPgp = new PGP(publicKey))
            using (var privatePgp = new PGP(privateKey))
            {
                privatePgp.ClearSignFile(
                    Path.Combine(ROOT, UNENCRYPTED_FILE),
                    Path.Combine(ROOT, SIGNED_FILE));

                publicPgp.EncryptFile(
                    Path.Combine(ROOT, SIGNED_FILE),
                    Path.Combine(ROOT, ENCRYPTED_FILE));
                
                // Likely would want to use EncryptFileAndSign instead of two passes
                
                privatePgp.DecryptFile(
                    Path.Combine(ROOT, ENCRYPTED_FILE),
                    Path.Combine(ROOT, DECRYPTED_FILE));

                Console.WriteLine("It was me who signed the file: {0}", publicPgp.VerifyClearFile(Path.Combine(ROOT, DECRYPTED_FILE)));
            }

            using (var pgp = new PGP(new EncryptionKeys(File.ReadAllText(Path.Combine(ROOT_KEYS, RANDOM_PUBLIC_KEY)))))
            {
                Console.WriteLine("It was me who signed the file: {0}", pgp.VerifyClearFile(Path.Combine(ROOT, DECRYPTED_FILE)));
            }
        }
    }
}
