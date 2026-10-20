using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BaobabWildlifeSystem
{
    public partial class Form1
    {
        // a friendly message for a file error
        private void ShowUpdateDeleteError(Exception ex)
        {
            if (ex is UnauthorizedAccessException)
            {
                MessageBox.Show("The program does not have permission to change animals.txt. " +
                    "Check that the file is not read-only.", "Permission Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (ex is IOException)
            {
                MessageBox.Show("animals.txt could not be accessed. It may be open in another program. " +
                    "Close it and try again.", "File Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Finds an animal by ID. Returns null if not found.
        private Animal FindAnimal(string animalId)
        {
            List<Animal> animals = AnimalStorage.LoadAnimals(out int skippedCount);
            return animals.FirstOrDefault(a => a.AnimalId.Equals(animalId, StringComparison.OrdinalIgnoreCase));
        }

        // Switches the form between Add mode (false) and Edit mode (true).
        private void SetEditMode(bool editing)
        {
            txtAnimalID.ReadOnly = editing;   // the ID cannot change after saving
            btnAdd.Enabled = !editing;
            btnSave.Enabled = editing;
        }

        // Copies an animal into the input boxes and enters edit mode.
        private void LoadAnimalIntoForm(Animal animal)
        {
            txtAnimalID.Text = animal.AnimalId;
            txtName.Text = animal.Name;
            txtSpecies.Text = animal.Species;
            txtAge.Text = animal.Age.ToString();
            txtRecoveryScore.Text = animal.RecoveryScore.ToString();
            SetEditMode(true);
        }

        // Empties all input boxes and returns the form to Add mode.
        private void ClearEditForm()
        {
            txtAnimalID.Clear();
            txtName.Clear();
            txtSpecies.Clear();
            txtAge.Clear();
            txtRecoveryScore.Clear();
            txtSearch.Clear();
            SetEditMode(false);
            txtAnimalID.Focus();
        }

        private string FormatRecord(Animal animal)
        {
            return $"{animal.AnimalId}|{animal.Name}|{animal.Species}|{animal.Age}|{animal.RecoveryScore}|{animal.Status}|{animal.HousingUnit}";
        }

        // Validates the input boxes.
        private bool ValidateEditInput(out string id, out string name, out string species, out int age, out int score)
        {
            age = 0;
            score = 0;
            id = txtAnimalID.Text.Trim().ToUpper();
            name = txtName.Text.Trim();
            species = txtSpecies.Text.Trim();

            if (!Regex.IsMatch(id, @"^WR-\d{4}$"))
            {
                ShowEditValidationError("Animal ID must be WR- followed by exactly 4 digits (e.g., WR-0007).", txtAnimalID);
                return false;
            }
            if (name.Length < 1 || name.Length > 30 || name.Contains('|'))
            {
                ShowEditValidationError("Name is required, must be 1 to 30 characters and cannot contain the pipe character (|).", txtName);
                return false;
            }
            if (species.Length < 1 || species.Length > 30 || species.Contains('|'))
            {
                ShowEditValidationError("Species is required, must be 1 to 30 characters and cannot contain the pipe character (|).", txtSpecies);
                return false;
            }
            if (!int.TryParse(txtAge.Text.Trim(), out age) || age < 0 || age > 100)
            {
                ShowEditValidationError("Age must be a whole number between 0 and 100.", txtAge);
                return false;
            }
            if (!int.TryParse(txtRecoveryScore.Text.Trim(), out score) || score < 0 || score > 100)
            {
                ShowEditValidationError("Recovery Score must be a whole number between 0 and 100.", txtRecoveryScore);
                return false;
            }
            return true;
        }

        // Shows a validation message and returns the cursor to the offending field.
        private void ShowEditValidationError(string message, TextBox fieldToFocus)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            fieldToFocus.Focus();
        }


        // Rewrites animals.txt replacing ONLY the line whose ID matches. Returns false if not found.
        private bool UpdateRecordInFile(Animal updatedAnimal)
        {
            string filePath = AnimalStorage.FilePath;
            if (!File.Exists(filePath)) return false;

            List<string> lines = File.ReadAllLines(filePath, Encoding.UTF8)
                                     .Where(line => !string.IsNullOrWhiteSpace(line))
                                     .ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                Animal existing = AnimalStorage.ParseLine(lines[i]); // null for malformed lines
                if (existing != null &&
                    existing.AnimalId.Equals(updatedAnimal.AnimalId, StringComparison.OrdinalIgnoreCase))
                {
                    lines[i] = FormatRecord(updatedAnimal); // only this line changes
                    File.WriteAllLines(filePath, lines, new UTF8Encoding(false));
                    return true;
                }
            }
            return false;
        }


        // Clicking a grid row loads that animal into the input boxes.
        private void dgvAnimals_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; 

            try
            {
                string animalId = dgvAnimals.Rows[e.RowIndex].Cells[0].Value?.ToString() ?? "";
                Animal animal = FindAnimal(animalId);
                if (animal != null)
                {
                    LoadAnimalIntoForm(animal);
                }
            }
            catch (Exception ex)
            {
                ShowUpdateDeleteError(ex);
            }
        }

        // Searches by the typed ID and loads the animal, or says it was not found.
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchId = txtSearch.Text.Trim();
            if (searchId.Length == 0)
            {
                MessageBox.Show("Please type an Animal ID to search for.", "Search",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSearch.Focus();
                return;
            }

            try
            {
                Animal animal = FindAnimal(searchId);
                if (animal == null)
                {
                    MessageBox.Show($"No animal with ID '{searchId.ToUpper()}' was found.", "Not Found",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                LoadAnimalIntoForm(animal);
            }
            catch (Exception ex)
            {
                ShowUpdateDeleteError(ex);
            }
        }

        // Validates, recalculates status/housing and saves only the edited record.
        private void btnSave_Click(object sender, EventArgs e)
        { 
            try
            {
                if (!ValidateEditInput(out string id, out string name, out string species, out int age, out int score))
                {
                    return; 
                }

                Animal updatedAnimal = new Animal(id, name, species, age, score);

                if (!UpdateRecordInFile(updatedAnimal))
                {
                    MessageBox.Show("This animal could not be found in animals.txt, so nothing was saved.",
                        "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show($"Animal updated successfully!\nStatus: {updatedAnimal.Status}\n" +
                                $"Housing Unit: {updatedAnimal.HousingUnit}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearEditForm();
                RefreshGrid();
            }
            catch (Exception ex)
            {
                ShowUpdateDeleteError(ex);
            }
        }

        // Leaves edit mode and empties the boxes.
        private void btnClear_Click(object sender, EventArgs e)
        {
        ClearEditForm();
        }
    }
}
