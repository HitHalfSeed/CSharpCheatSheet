// Compression
using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip; // PM> install-package SharpZipLib

namespace PlayingAround
{
    class Program
    {
        private static readonly string _root = @"C:\temp\C# Training\Compression";
        private static readonly string _folderToZip = Path.Join(_root, "FolderToZip");
        private static readonly string _fileToZip = Path.Join(_folderToZip, "FileToZip.txt");
        private static readonly string _zipFile = Path.Join(_root, "Zipped.zip");

        static void Main(string[] args)
        {
            new Program().Run();
        }

        public void Run()
        {
            CleanUp();

            Directory.CreateDirectory(_root);
            Directory.CreateDirectory(_folderToZip);
            File.WriteAllText(_fileToZip, "Content");

            // Can only be used to zip files without a password
            System.IO.Compression.ZipFile.CreateFromDirectory(_folderToZip, _zipFile); // using System.IO.Compression;
            Delete(_zipFile);

            // Otherwise SharpZipLib can be used
            ZipUsingSharpZipLib(_folderToZip, _zipFile);

            CleanUp();
        }

        public static void ZipUsingSharpZipLib(string directoryToZip, string zipDestination)
        {
            var buffer = new byte[4096];
            using var output = new ZipOutputStream(File.Create(zipDestination));

            output.SetLevel(9);
            output.Password = "password";

            foreach (var file in Directory.GetFiles(directoryToZip))
            {
                var entry = new ZipEntry(Path.GetFileName(file));
                entry.DateTime = DateTime.Now;
                output.PutNextEntry(entry);

                using var fileStream = File.OpenRead(file);
                int sourceBytes;
                do
                {
                    // FileStream.Read() populates buffer with bytes of length specified and returns the number of bytes read
                    sourceBytes = fileStream.Read(buffer, 0, buffer.Length); 
                    output.Write(buffer, 0, sourceBytes);
                } while (sourceBytes > 0);
            }
        }

        public static void CleanUp()
        {
            Delete(_zipFile);
            Delete(_fileToZip);
            Delete(_folderToZip);
            Delete(_root);
        }

        public static void Delete(string path)
        {
            if (Path.HasExtension(path))
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            else
            {
                if (Directory.Exists(path) && Directory.GetFiles(path).Length == 0)
                    Directory.Delete(path);
            }
        }
    }
}
