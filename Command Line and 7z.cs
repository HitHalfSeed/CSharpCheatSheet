using System;
using System.IO;
using System.Diagnostics;

namespace Random
{
    class Program
    {
        private static string rootDirectory = @"C:\temp\CSharp";
        private static string archiveDirectory = Path.Combine(rootDirectory, @"7zCommandLineDemo");
        private static string archiveName = "archive.7z";
        private static string password = "password";

        static void Main(string[] args)
        {
            CreateDirectory(archiveDirectory);
            File.WriteAllText(Path.Join(archiveDirectory, "foo.txt"), "Content goes here");
            File.WriteAllLines(Path.Join(archiveDirectory, "bar.csv"), new string[] { "id,name", "1,foo", "2,bar" });

            // https://sevenzip.osdn.jp/chm/cmdline/index.htm for full options
            ExecuteCommand($"cd {archiveDirectory} & 7z a {archiveName} -p{password} -sdel -mhe"); // -sdel (delete files after zip). -mhe (encrypt filenames)
            ExecuteCommand($"7z x {Path.Combine(archiveDirectory, archiveName)} -o{archiveDirectory} -p{password}");

            DeleteDirectory(rootDirectory, archiveDirectory);
        }

        public static void ExecuteCommand(string command)
        {
            var processStartInfo = new ProcessStartInfo("cmd.exe") // using System.Diagnostics
            {
                Arguments = "/C " + command, // Carries out the command specified by string and then terminates
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            var process = Process.Start(processStartInfo);
            process.WaitForExit();

            switch (process.ExitCode)
            {
                case 0:
                    return;
                default:
                    throw new Exception($"7z returned exit code: {process.ExitCode}");
            }
        }
        public static void CreateDirectory(string fullPath)
        {
            var currentPath = string.Empty;
            foreach (var path in fullPath.Split(Path.DirectorySeparatorChar))
            {
                currentPath += path + Path.DirectorySeparatorChar;
                if (Directory.Exists(currentPath))
                    continue;

                Directory.CreateDirectory(currentPath);
            }
        }

        public static void DeleteDirectory(string deleteUpToPath, string pathToDelete)
        {
            deleteUpToPath = CleanDirectoryPath(deleteUpToPath);
            pathToDelete = CleanDirectoryPath(pathToDelete);

            if (!pathToDelete.StartsWith(deleteUpToPath))
                throw new Exception($"{nameof(pathToDelete)} doesn't being with {nameof(deleteUpToPath)}");

            var directory = new DirectoryInfo(pathToDelete);
            while (directory.FullName != deleteUpToPath)
            {
                DeleteDirectoryAndContents(directory.FullName);
                directory = directory.Parent;
            }
        }

        public static void DeleteDirectoryAndContents(string path)
        {
            if (!Directory.Exists(path))
                return;

            foreach (var file in Directory.GetFiles(path)) 
            {
                var fileInfo = new FileInfo(file);
                File.Delete(file);
            }
            foreach (var directory in Directory.GetDirectories(path))
            {
                DeleteDirectoryAndContents(directory);
            }

            Directory.Delete(path);
        }

        public static string CleanDirectoryPath(string path)
        {
            return path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}
