using System;
using System.Collections.Generic;
using System.IO; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaobabRidgeWildlifeProject
{
    internal class AddAnimal
    {
        private readonly string filepath;

        // File path for the animals
        public AddAnimal(string filepath)
        {
            this.filepath = filepath;
        }

        // Create new animal record to save 
        public bool AddNewAnimal(string id, string name, string species, int age, int score, out string message)
        {
            // Animal status and housing unit
            string status;
            string housingUnit;

            if (score <= 19)
            {
                status = "Critical";
                housingUnit = "Intensive Care Unit";
            }
            else if (score <= 39)
            {
                status = "Serious";
                housingUnit = "High-Dependency Ward";
            }
            else if (score <= 59)
            {
                status = "Stable";
                housingUnit = "Recovery Ward";
            }
            else if (score <= 79)
            {
                status = "Recovering";
                housingUnit = "Outdoor Enclosure";
            }
            else
            {
                status = "Release-Ready";
                housingUnit = "Pre-Release Camp";
            }

            // Animal object
            Animal newAnimal = new Animal(id, name, species, age, score, status, housingUnit);

            string animalLine =
                $"{newAnimal.Id}|{newAnimal.Name}|{newAnimal.Species}|" +
                $"{newAnimal.Age}|{newAnimal.RecoveryScore}|" +
                $"{newAnimal.Status}|{newAnimal.HousingUnit}";

            try
            {
                // Adds the new record without replacing another
                if (File.Exists(filepath) && new FileInfo(filepath).Length > 0)
                {
                    string existingText = File.ReadAllText(filepath);
                    if (!existingText.EndsWith("\n"))
                    {
                        File.AppendAllText(filepath, Environment.NewLine);
                    }
                }
                File.AppendAllText(filepath, animalLine + Environment.NewLine);

                message = "Animal Added Successfully!";
                return true;
            }
            catch (IOException)
            {
                // Errors for when writing to the file 
                message = "An error occurred while saving the animal.";

                return false;
            }
            catch (UnauthorizedAccessException)
            {
                // Error for file permissions
                message = "You do not have permission to save the animal file.";

                return false;
            }
        }
    }
}
