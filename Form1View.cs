using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace BaobabWildlifeSystem
{
    // This partial class handles the DataGridView configuration and data loading for Form1.
    public partial class Form1
    {
        // Runs when the form is loaded: sets up the grid and populates it with data from animals.txt.
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Does not run the grid configuration and data loading if the form is open in Visual Studio Designer.
            if (DesignMode)
            {
                return;
            }

            ConfigureGrid();
            RefreshGrid(true);
        }

        // Sets up the grid: read-only, full-row selection, adn the necessary columns.
        private void ConfigureGrid()
        {
            // Columns are manually defined so that headers and their order match the spec.
            dgvAnimals.AutoGenerateColumns = false;
            dgvAnimals.ReadOnly = true;
            dgvAnimals.AllowUserToAddRows = false;
            dgvAnimals.AllowUserToDeleteRows = false;
            dgvAnimals.AllowUserToResizeRows = false;
            dgvAnimals.MultiSelect = false;
            dgvAnimals.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAnimals.RowHeadersVisible = false;
            dgvAnimals.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvAnimals.Columns.Clear();
            AddGridColumn("Animal ID","AnimalID");
            AddGridColumn("Name", "Name");
            AddGridColumn("Species", "Species");
            AddGridColumn("Age", "Age");
            AddGridColumn("Recovery Score", "RecoveryScore");
            AddGridColumn("Status", "Status");
            AddGridColumn("Housing Unit", "HousingUnit");
        }

        // Adds a column to the DataGridView linked to a specific property of the Animal class.
        private void AddGridColumn(string header, string propertyName)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.HeaderText = header;
            column.DataPropertyName = propertyName;
            column.Name = "col" + propertyName;
            // Stop sorting so that the order of columns remains consistent.
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvAnimals.Columns.Add(column);
        }

        // Reloads animals.txt and refreshes the grid. Will be called after every add, update, and delete.
        // Pass true only on startup, so the skipped lines message box is not shown every time the grid is refreshed.
        private void RefreshGrid(bool reportskippedlines = false)
        {
            // Start with an empty list and a skipped count of 0. If the file is read successfully, these will be updated.
            List<Animal> animals = new List<Animal>();
            int skippedCount = 0;
            try
            {
                animals = AnimalStorage.LoadAnimals(out skippedCount);
            }
            // File exists but program cannot open it.
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("The Program does not have permission to read animals.txt.","File Access Error", MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            // File is open in another program.
            catch (IOException)
            {
                MessageBox.Show("animals.txt could not be read. Please check if the file is open in another program.", "File Access Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Binding a new list replaces old rows, so there are no duplicates.
            dgvAnimals.DataSource = new BindingList<Animal>(animals);
            // Binding automatically selects the first row, which is not desired. Clear the selection so that no rows are selected.
            dgvAnimals.ClearSelection();

            if (reportskippedlines && skippedCount > 0)
            {
                MessageBox.Show($"{skippedCount} invalid lines were skipped.", "Skipped Records", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
