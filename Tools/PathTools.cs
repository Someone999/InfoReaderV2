using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using osuTools.Beatmaps;

namespace InfoReader.Tools
{
    public static class PathTools
    {
        private static readonly List<char> InvalidFileNameCharacters = new List<char>();
        private static readonly List<char> InvalidPathCharacters = new List<char>();
        static PathTools()
        {
            InvalidFileNameCharacters.AddRange(Path.GetInvalidFileNameChars());
            InvalidPathCharacters.AddRange(Path.GetInvalidPathChars());
        }

        public static string GetFileName(string path, Beatmap beatmap, string extend)
        {
            int lvl = 0;
            string fileName = ShortFileName(path, beatmap, lvl);
            
            while (IsPathTooLong(path, fileName, extend))
            {
                lvl++;
                if (lvl > 3)
                {
                    throw new ArgumentException("FileName is too long.");
                }
                fileName = ShortFileName(path, beatmap, lvl);
            }
            var invalidPathChrIdx = GetInvalidPathCharsIndexes(path);
            var invalidFileNameChrIdx = GetInvalidFileNameCharsIndexes(fileName);
            var nPath = ClearInvalidCharacters(path, invalidPathChrIdx);
            var nFileName = ClearInvalidCharacters(fileName, invalidFileNameChrIdx) + $"{extend}";
            return Path.Combine(nPath, nFileName);
        }

        internal static readonly  Regex FileNumberMatcher = new Regex(@".*?\-(\d+)\.[\w]{1,4}");

        public static int GetFileNumber(string fileName, bool fileNameEndsWithDashNumber)
        {
            if (!fileNameEndsWithDashNumber)
            {
                Match match = FileNumberMatcher.Match(fileName);
                if (!match.Success || !int.TryParse(match.Groups[1].Value, out var id1))
                {
                    throw new ArgumentException("This file name does not contain a file number.");
                }

                return id1;
            }

            MatchCollection matches = FileNumberMatcher.Matches(fileName);
            int last = matches.Count - 1;
            if (matches.Count < 2 || !int.TryParse(matches[last].Groups[1].Value,out var id2))
            {
                throw new ArgumentException("This file name does not contain a file number.");
            }

            return id2;
        }
        public static string AddNumber(string oriFileName, int id)
        {
            string fileName = Path.GetFileNameWithoutExtension(oriFileName);
            string ext = Path.GetExtension(oriFileName) ?? string.Empty;
            string dir = Path.GetDirectoryName(oriFileName) ?? string.Empty;
            string newName = fileName + "-" + id;
            if (!string.IsNullOrEmpty(ext))
            {
                newName += ext;
            }

            string path = newName;
            if (!string.IsNullOrEmpty(dir))
            {
                path = Path.Combine(dir, newName);
            }

            return path;
        }

        public static string ClearInvalidCharacters(string path, int[] idx)
        {
            StringBuilder nDir = new StringBuilder(path);
            foreach (var i in idx)
            {
                nDir[i] = ' ';
            }

            return nDir.ToString();
        }

        public static int[] GetInvalidPathCharsIndexes(string path)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < path.Length; i++)
            {
                if (InvalidPathCharacters.Contains(path[i]))
                {
                    indexes.Add(i);
                }
            }
            return indexes.ToArray();
        }

        public static int[] GetInvalidFileNameCharsIndexes(string fileName)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < fileName.Length; i++)
            {
                if (InvalidFileNameCharacters.Contains(fileName[i]))
                {
                    indexes.Add(i);
                }
            }
            return indexes.ToArray();
        }

        /// <summary>
        /// Short the file name.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="beatmap"></param>
        /// <param name="level">0: FullName,1: Artist - Title,2: Title,3: BeatmapSetId (Maybe invalid) </param>
        /// <returns></returns>
        public static string ShortFileName(string path, Beatmap beatmap, int level)
        {
            return level switch
            {
                0 => Path.Combine(path, beatmap.ToString()),
                1 => Path.Combine(path, $"{beatmap.Artist} - {beatmap.Title}"),
                2 => Path.Combine(path, beatmap.Title),
                3 => Path.Combine(path, beatmap.BeatmapSetId.ToString()),
                _ => throw new ArgumentException(
                    "Level: \n0: FullName\n1: Artist - Title\n2: Title\n3: BeatmapSetId (Maybe invalid)")
            };
        }

        public static bool IsPathTooLong(string path, string fileName, string extend) =>
            path.Length + fileName.Length + extend.Length > 266;
    }
}
