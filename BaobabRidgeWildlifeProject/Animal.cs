using System;
using System.Collections.Generic;
using System.Text;

namespace BaobabRidgeWildlifeProject
{
    internal class Animal
    {
        string id;
        string name;
        string species;
        int age;
        int recoveryScore;
        string status;
        string housingUnit;

        public Animal(string id, string name, string species, int age, int recoveryScore, string status, string housingUnit)
        {
            this.id = id;
            this.name = name;
            this.species = species;
            this.age = age;
            this.recoveryScore = recoveryScore;
            this.status = status;
            this.housingUnit = housingUnit;
        }

        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Species { get => species; set => species = value; }
        public int Age { get => age; set => age = value; }
        public int RecoveryScore { get => recoveryScore; set => recoveryScore = value; }
        public string Status { get => status; set => status = value; }
        public string HousingUnit { get => housingUnit; set => housingUnit = value; }

        public string TofileLine()
        {
            return $"{Id} | {Name} | {Species} | {Age} | {RecoveryScore} | {Status} | {HousingUnit}";
        }

        //this basically acts as tranlator as it acts as one raw text line from animal.txt and converts it into a structured Animal object
        //that the application can easily use, string the animal records as a single line
        public static Animal Parse(string line)
        {
            string[] parts = line.Split('|');
            if (parts.Length != 7)
            {
                throw new FormatException("Invalid field count");
            }
            return new Animal(
                parts[0].Trim(),
                parts[1].Trim(),
                parts[2].Trim(),
                int.Parse(parts[3].Trim()),
                int.Parse(parts[4].Trim()),
                parts[5].Trim(),
                parts[6].Trim());
        }

        public static (string Status, string HousingUnit) Classify(int score)
        {
            if (score >= 0 && score <= 19)
                return ("Critical", "Intensive Care Unit");
            if (score >= 20 && score <= 39)
                return ("Serious", "High-Dependency Ward");
            if (score >= 40 && score <= 59)
                return ("Stable", "Recovery Ward");
            if (score >= 60 && score <= 79)
                return ("Recovering", "Outdoor Enclosure");
            if (score >= 80 && score <= 100)
                return ("Release-Ready", "Pre-Release Camp");

            throw new ArgumentOutOfRangeException(nameof(score), "Score must be between 0 and 100.");
        }
    }
}
