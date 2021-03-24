// File Handling
using System;
using System.IO;

namespace PlayingAround
{
    class Program
    {
        private static readonly string _root = @"C:\temp\C# Training\Files";

        static void Main(string[] args)
        {
            var program = new Program();
            program.Run();
            program.FileInfoDemo();
        }

        public void Run()
        {
            if (Directory.Exists(_root))
                DeleteFolder(_root);

            Directory.CreateDirectory(_root);

            var path = Path.Join(_root, DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt"); // Path.Join so you don't need to worry about "" + Path.DirectorySeparatorChar + "";

            if (!File.Exists(path))
            {
                var fileStream = File.Create(path); // Returns a FileStream so you have to close it!
                fileStream.Close();
                //File.Create(path).Close(); // Could be shortened to
            }

            var fileContent = new string[] { "Firstname,Surname", "Joe,Bloggs", "Foo,Bar" };

            File.WriteAllText(path, "This will overwrite current content");
            File.WriteAllLines(path, fileContent); // Will overwrite

            using (StreamWriter file = new StreamWriter(path)) // Will overwrite
            {
                foreach (var line in fileContent)
                    if (!string.IsNullOrWhiteSpace(line))
                        file.WriteLine(line);
            }

            var lineNumber = -1;
            using (StreamWriter file = new StreamWriter(path, true)) // Will append
            {
                foreach (var line in fileContent)
                {
                    if (++lineNumber == 0) // Skip headers
                        continue;

                    if (!string.IsNullOrWhiteSpace(line))
                        file.WriteLine(line);
                }
            }

            var filesToDeletePath = Path.Join(new string[] { _root, "Delete" });
            Directory.CreateDirectory(filesToDeletePath);

            foreach (var txtFile in Directory.GetFiles(_root, "*.txt"))
            {
                var csvFile = Path.ChangeExtension(txtFile, ".csv");
                File.Copy(txtFile, csvFile);
                File.Move(txtFile, Path.Join(filesToDeletePath, Path.GetFileName(txtFile))); // Cuts the file
            }

            DeleteFolder(filesToDeletePath);

            DeleteFolder(_root); // Clean up
        }

        public void FileInfoDemo()
        {
            long total = 0;
            long current = 0;
            
            foreach (var file in Directory.GetFiles(_root))
                total += new FileInfo(file).Length; // Gets file size in bytes

            foreach (var file in Directory.GetFiles(_root))
            {
                System.Threading.Thread.Sleep(200); // For readability use: new TimeSpan(0, 0, 1) if you can
                current += new FileInfo(file).Length;
                Console.WriteLine("{0:0.##}% complete", (current / (double)total) * 100);
            }
        }

        public void CreateFolder(string path)
        {
            var currentPath = string.Empty;
            foreach (var a in Path.GetDirectoryName(path).Split(Path.DirectorySeparatorChar))
            {
                currentPath = Path.Join(currentPath, a);
                if (!Directory.Exists(currentPath))
                    Directory.CreateDirectory(currentPath);
            }
        }

        public void DeleteFolder(string path)
        {
            foreach (var p in Directory.GetDirectories(path))
            {
                DeleteFolder(p); // Navigates to lowest directory
            }
            foreach (var filePath in Directory.GetFiles(path))
            {
                File.Delete(filePath);
            }
            Directory.Delete(path);
        }
    }
}
