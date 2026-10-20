using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BaobabWildlifeSystem
{

    public partial class Form1
    {

        // Rewrites animals.txt without the record that has the given ID. Returns false if not found.
        private bool DeleteRecordFromFile(string animalId)
        {
            string filePath = AnimalStorage.FilePath;
            if (!File.Exists(filePath)) return false;

            List<string> lines = File.ReadAllLines(filePath, Encoding.UTF8)
                                     .Where(line => !string.IsNullOrWhiteSpace(line))
                                     .ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                Animal existing = AnimalStorage.ParseLine(lines[i]);
                if (existing != null &&
                    existing.AnimalId.Equals(animalId, StringComparison.OrdinalIgnoreCase))
                {
                    lines.RemoveAt(i); // one record only
                    File.WriteAllLines(filePath, lines, new UTF8Encoding(false));
                    return true;
                }
            }
            return false;
        }


        // Deletes the selected grid row after a Yes/No confirmation. 
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAnimals.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an animal in the grid first.", "Delete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow selectedRow = dgvAnimals.SelectedRows[0];
            string animalId = selectedRow.Cells[0].Value?.ToString() ?? "";
            string animalName = selectedRow.Cells[1].Value?.ToString() ?? "";

            DialogResult answer = MessageBox.Show(
                $"Delete {animalId} ({animalName})? This cannot be undone.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (answer != DialogResult.Yes) return; // No: change nothing

            try
            {
                if (!DeleteRecordFromFile(animalId))
                {
                    MessageBox.Show("This animal could not be found in animals.txt.", "Not Found",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // If the deleted animal is loaded in the form, clear the form
                if (txtAnimalID.Text.Trim().Equals(animalId, StringComparison.OrdinalIgnoreCase))
                {
                    ClearEditForm();
                }
                RefreshGrid();
            }
            catch (Exception ex)
            {
                ShowUpdateDeleteError(ex);
            }
        }
    }
}
