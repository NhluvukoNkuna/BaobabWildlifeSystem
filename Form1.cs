using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace BaobabWildlifeSystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Helper method to check if ID already exists in animals.txt
        private bool IsIdUnique(string animalId)
        {
            string filePath = "animals.txt";
            if (!File.Exists(filePath)) return true;

            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split('|');
                if (parts.Length > 0 && parts[0].Trim().Equals(animalId, StringComparison.OrdinalIgnoreCase))
                {
                    return false; // ID already exists
                }
            }
            return true;
        }

        // Validation logic for Add feature
        private bool ValidateInput(out string animalId, out string name, out string species, out int age, out int score)
        {
            animalId = string.Empty;
            name = string.Empty;
            species = string.Empty;
            age = 0;
            score = 0;

            // 1. Animal ID Validation (WR- followed by exactly 4 digits)
            animalId = txtAnimalID.Text.Trim().ToUpper();
            if (!System.Text.RegularExpressions.Regex.IsMatch(animalId, @"^WR-\d{4}$"))
            {
                MessageBox.Show("Animal ID must be in the format WR- followed by exactly 4 digits (e.g., WR-0007).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAnimalID.Focus();
                return false;
            }

            // Check unique ID
            if (!IsIdUnique(animalId))
            {
                MessageBox.Show("Animal ID must be unique. This ID already exists.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAnimalID.Focus();
                return false;
            }

            // 2. Name Validation (1-30 characters, no pipe character)
            name = txtName.Text.Trim();
            if (string.IsNullOrEmpty(name) || name.Length > 30 || name.Contains('|'))
            {
                MessageBox.Show("Name is required, must be between 1 and 30 characters, and cannot contain the pipe character ('|').", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            // 3. Species Validation (1-30 characters, no pipe character)
            species = txtSpecies.Text.Trim();
            if (string.IsNullOrEmpty(species) || species.Length > 30 || species.Contains('|'))
            {
                MessageBox.Show("Species is required, must be between 1 and 30 characters, and cannot contain the pipe character ('|').", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSpecies.Focus();
                return false;
            }

            // 4. Age Validation (Whole number from 0 to 100)
            if (!int.TryParse(txtAge.Text.Trim(), out age) || age < 0 || age > 100)
            {
                MessageBox.Show("Age must be a whole number between 0 and 100.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAge.Focus();
                return false;
            }

            // 5. Recovery Score Validation (Whole number from 0 to 100)
            if (!int.TryParse(txtRecoveryScore.Text.Trim(), out score) || score < 0 || score > 100)
            {
                MessageBox.Show("Recovery Score must be a whole number between 0 and 100.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRecoveryScore.Focus();
                return false;
            }

            return true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Execute validation
                if (!ValidateInput(out string id, out string name, out string species, out int age, out int score))
                {
                    return;
                }

                // Create animal object (automatically calculates status and housing unit)
                Animal newAnimal = new Animal(id, name, species, age, score);

                // Append record to animals.txt
                string filePath = "animals.txt";
                File.AppendAllText(filePath, newAnimal.ToFileString() + Environment.NewLine);

                // Confirmation and feedback
                MessageBox.Show($"Animal added successfully!\nStatus: {newAnimal.Status}\nHousing Unit: {newAnimal.HousingUnit}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear input boxes (Ready for next entry)
                txtAnimalID.Clear();
                txtName.Clear();
                txtSpecies.Clear();
                txtAge.Clear();
                txtRecoveryScore.Clear();
                txtAnimalID.Focus();

                // Note: Call your grid refresh method here once Member 2 builds the view component
                RefreshGrid();
            }
            catch (Exception ex)
            {
                // Graceful error handling per requirement
                MessageBox.Show($"An error occurred while saving the animal: {ex.Message}", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGenerateSummary_Click(object sender, EventArgs e)
        {
           List<Animal> animals;
           int skippedCount;

          //Load animals from file(ANIMAL.TXT)
          try
          {
            animals = AnimalStorage.LoadAnimals(out skippedCount);

          }
          catch (UnauthorizedAccessException)
          {
             MessageBox.Show("You do not have permission to read this file.");
             return;
          }
          catch(IOException )
          {
            MessageBox.Show("You cannot read the animal file. Please check the file and try again.");
            return;
          }

          // Calculating the total number of animals
            int totalAnimals = animals.Count;

         // Calculating total average age and recovery score. We show not N/A when there are no animals
            string averageAge = "N/A";
            string averageRecoveryScore = "N/A";

         if (totalAnimals > 0) 
         { 
             double ageTotal = 0;
             double totalRecoveryScore = 0;

            foreach (Animal animal in animals)
            {
              ageTotal += animal.Age;
              totalRecoveryScore += animal.RecoveryScore;
            }

             averageAge = (ageTotal / totalAnimals).ToString("F2");
             averageRecoveryScore = (totalRecoveryScore / totalAnimals).ToString("F2");

          }

         // Counting animals in each status
         int criticalCount = 0;
         int seriousCount = 0;
         int stableCount = 0;
         int recoveringCount = 0;
         int releaseReadyCount = 0;

        foreach (Animal animal in animals)
        {
           switch (animal.Status)
           {
             case "Critical":
                criticalCount++;
                break;
             case "Serious":
                seriousCount++;
                break;
             case "Stable":
                stableCount++;
                break;
             case "Recovering":
                recoveringCount++;
                break;
             case "Release-Ready":
                releaseReadyCount++;
                break;
            }
          }

          // Displaying the summary in a message box
          lblTotalAnimals.Text = totalAnimals.ToString();
          lblAverageAge.Text = averageAge;
          lblAverageRecoveryScore.Text = averageRecoveryScore;
          lblCriticalCount.Text = criticalCount.ToString();
          lblSeriousCount.Text = seriousCount.ToString();
          lblStableCount.Text = stableCount.ToString();
          lblRecoveringCount.Text = recoveringCount.ToString();
          lblReleaseReadyCount.Text = releaseReadyCount.ToString();

          //Creating Summary report 
          string summaryReport =
          "BOABAB WILDLIFE REHABILITATION CENTRE\n" +
          "ANIMAL SUMMARY REPORT\n" +
          "Generated on: " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "\n" +
          "-----------------------------------------\n" +
          "Total Animals: " + totalAnimals + "\n" +
          "Average Age: " + averageAge + "\n" +
          "------------------------------------------\n" +
          "Average Recovery Score: " + averageRecoveryScore + "\n" +
          "Critical: " + criticalCount + "\n" +
          "Serious: " + seriousCount + "\n" +
          "Stable: " + stableCount + "\n" +
          "Recovering: " + recoveringCount + "\n" +
          "Release-Ready: " + releaseReadyCount + "\n";

          string summaryFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "summary.txt");

          try
          {
             System.IO.File.WriteAllText(summaryFilePath, summaryReport, System.Text.Encoding.UTF8);

             MessageBox.Show("Summary report generated successfully at: " + summaryFilePath);

          }
          catch (FileNotFoundException)
          {
             MessageBox.Show("The animal file could not be found. Please check if animals.txt exists in the bin/");

          }
          catch (UnauthorizedAccessException)
          {
             MessageBox.Show("The summary could not be saved because you do not have permission.");
          }
          catch (IOException )
          {
            MessageBox.Show("The summary file could not be saved due to an I/O error.");
          }
    
       }

    }
}

        
    

