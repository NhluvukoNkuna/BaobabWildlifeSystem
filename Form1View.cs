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
    public partial class Form1
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode)
            {
                return;
            }

            ConfigureGrid();
            RefreshGrid(true);
        }

        private void ConfigureGrid()
        {
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

        private void AddGridColumn(string header, string propertyName)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.HeaderText = header;
            column.DataPropertyName = propertyName;
            column.Name = "col" + propertyName;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvAnimals.Columns.Add(column);
        }

        private void RefreshGrid(bool reportskippedlines = false)
        {
            List<Animal> animals = new List<Animal>();
            int skippedCount = 0;
            try
            {
                animals = AnimalStorage.LoadAnimals(out skippedCount);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("The Program does not have permission to read animals.txt.","File Access Error", MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            catch (IOException)
            {
                MessageBox.Show("animals.txt could not be read. Please check if the file is open in another program.", "File Access Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            dgvAnimals.DataSource = new BindingList<Animal>(animals);
            dgvAnimals.ClearSelection();

            if (reportskippedlines && skippedCount > 0)
            {
                MessageBox.Show($"{skippedCount} invalid lines were skipped.", "Skipped Records", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
