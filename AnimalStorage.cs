using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BaobabWildlifeSystem
{
    public static class AnimalStorage
    {
        public static string FilePath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "animals.txt"); }
        }
        public static void FileExistCheck()
        {
            if (!File.Exists(FilePath))
            {
                File.Create(FilePath).Dispose();
            }
        }
        public static List<Animal> LoadAnimals(out int skippedCount)
        {
            skippedCount = 0;
            List<Animal> animals = new List<Animal>();

            FileExistCheck();

            foreach (string line in File.ReadAllLines(FilePath, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                Animal animal = ParseLine(line);
                if (animal == null)
                {
                    skippedCount++;
                }
                else
                {
                    animals.Add(animal);
                }
            }
            return animals;
        }
        public static Animal ParseLine(string line)
        {
            string[] parts = line.Split('|');
            if (parts.Length != 7)
            {
                return null;
            }
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Trim();
            }
            string id = parts[0].ToUpper();
            if (!Regex.IsMatch(id, @"^WR-\d{4}$"))
            {
                return null;
            }
            if (parts[1].Length == 0 || parts[1].Length > 30 || parts[2].Length == 0 || parts[2].Length > 30)
            {
                return null;
            }

            int age;
            int score;
            if (!int.TryParse(parts[3], out age) || !int.TryParse(parts[4], out score))
            {
                return null;
            }

            if (age < 0 || age > 100 || score < 0 || score > 100)
            {
                return null;
            }

            return new Animal(id, parts[1], parts[2], age, score);
        }
    }
}
