using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaobabWildlifeSystem
{
    public class Animal
    {
        public string AnimalId { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public int Age { get; set; }
        public int RecoveryScore { get; set; }
        public string Status { get; private set; }
        public string HousingUnit { get; private set; }

        // Constructor
        public Animal(string animalId, string name, string species, int age, int recoveryScore)
        {
            AnimalId = animalId.Trim().ToUpper();
            Name = name.Trim();
            Species = species.Trim();
            Age = age;
            RecoveryScore = recoveryScore;
            CalculateStatusAndHousing();
        }

        
        // Single classification method determining status and housing unit 
        
        public void CalculateStatusAndHousing()
        {
            if (RecoveryScore >= 0 && RecoveryScore <= 19)
            {
                Status = "Critical";
                HousingUnit = "Intensive Care Unit";
            }
            else if (RecoveryScore >= 20 && RecoveryScore <= 39)
            {
                Status = "Serious";
                HousingUnit = "High-Dependency Ward";
            }
            else if (RecoveryScore >= 40 && RecoveryScore <= 59)
            {
                Status = "Stable";
                HousingUnit = "Recovery Ward";
            }
            else if (RecoveryScore >= 60 && RecoveryScore <= 79)
            {
                Status = "Recovering";
                HousingUnit = "Outdoor Enclosure";
            }
            else if (RecoveryScore >= 80 && RecoveryScore <= 100)
            {
                Status = "Release-Ready";
                HousingUnit = "Pre-Release Camp";
            }
        }

        
        // Formats the animal record into the pipe-delimited text structure.
        
        public string ToFileString()
        {
            return $"{AnimalId} | {Name} | {Species} | {Age} | {RecoveryScore} | {Status} | {HousingUnit}";
        }
    }
}
