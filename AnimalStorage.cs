using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BaobabWildlifeSystem
{
    //This class handles reading animals.txt from disk.
    public static class AnimalStorage
    {
        // Returns the full path of animals.txt, always in the same directory as the executable.
        public static string FilePath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "animals.txt"); }
        }

        // Checks if animals.txt exists, and creates it if it doesn't.
        public static void FileExistCheck()
        {
            if (!File.Exists(FilePath))
            {
                // Disposes the file immediately so nothing else is blocked from accessing it.
                File.Create(FilePath).Dispose();
            }
        }
        // Loads all animals from the file, returning a list of Animal objects and the number of skipped lines.
        public static List<Animal> LoadAnimals(out int skippedCount)
        {
            skippedCount = 0;
            List<Animal> animals = new List<Animal>();

            FileExistCheck();

            foreach (string line in File.ReadAllLines(FilePath, Encoding.UTF8))
            {
                // Blank lines are skipped without incrementing the skippedCount, as they are not considered invalid data.
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                Animal animal = ParseLine(line);
                // If the line is invalid, increment skippedCount and continue to the next line.
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
        // Converts a file line into an Animal object, returning null if the line is invalid.
        public static Animal ParseLine(string line)
        {
            string[] parts = line.Split('|');
            // A valid record has exactly 7 parts seperated by pipes.
            if (parts.Length != 7)
            {
                return null;
            }
            // Trim whitespace from each part to ensure accurate validation.
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Trim();
            }
            string id = parts[0].ToUpper();
            // The ID must match the pattern "WR-XXXX" where X is a digit.
            if (!Regex.IsMatch(id, @"^WR-\d{4}$"))
            {
                return null;
            }
            // Name and species must be non-empty and no longer than 30 characters.
            if (parts[1].Length == 0 || parts[1].Length > 30 || parts[2].Length == 0 || parts[2].Length > 30)
            {
                return null;
            }

            int age;
            int score;
            // Age and recovery score must be whole numbers.
            if (!int.TryParse(parts[3], out age) || !int.TryParse(parts[4], out score))
            {
                return null;
            }
            // Values must be between 0-100, any values outside this range are considered invalid.
            if (age < 0 || age > 100 || score < 0 || score > 100)
            {
                return null;
            }
            // The Animal constructor will calculate the status and housing unit based on the age and recovery score.
            // The stored values in the file are not needed here.
            return new Animal(id, parts[1], parts[2], age, score);
        }
    }
}
