using System.IO;

namespace InfoReader.Tools
{
    public static class FileTools
    {
        public static void RecurseDelete(string dir)
        {
            if (!Directory.Exists(dir))
            {
                return;
            }
            string[] dirs = Directory.GetDirectories(dir);
            if (dir.Length > 0)
            {
                foreach (var directory in dirs)
                {
                    RecurseDelete(directory);
                }
            }
            string[] files = Directory.GetFiles(dir);
            foreach (var file in files)
            {
                File.Delete(file);
            }

            foreach (var directory in dirs)
            {
                Directory.Delete(directory);
            }
        }

        public static void ConfirmDirectory(string path)
        {
            string? dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                if (string.IsNullOrEmpty(dir))
                {
                    dir = path;
                }
                Directory.CreateDirectory(dir);
            }
        }
    }
}
