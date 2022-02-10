using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using osuTools.Beatmaps;

namespace InfoReader.Tools
{
    public static class PathTools
    {
        
        static readonly List<char> InvalidFileNameCharacters = new List<char>();
        static readonly List<char> InvalidPathCharacters = new List<char>();
        static PathTools()
        {
            InvalidFileNameCharacters.AddRange(Path.GetInvalidFileNameChars());
            InvalidPathCharacters.AddRange(Path.GetInvalidPathChars());
        }

        public static string GetBeatmapFileName(string path, Beatmap beatmap, string extend)
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
            switch (level)
            {
                case 0: return beatmap.ToString();
                case 1: return $"{beatmap.Artist} - {beatmap.Title}";
                case 2: return beatmap.Title;
                case 3: return beatmap.BeatmapSetId.ToString();
                default: throw new ArgumentException("Level: \n0: FullName\n1: Artist - Title\n2: Title\n3: BeatmapSetId (Maybe invalid)");
            }
        }

        public static bool IsPathTooLong(string path, string fileName, string extend) =>
            path.Length + fileName.Length + extend.Length > 266;
    }
}
