using System.Globalization;
using System.Text.RegularExpressions;

namespace BaobabRidgeWildlifeProject
{
    public partial class Form1 : Form
    {
        public readonly string filepath = Path.Combine(AppContext.BaseDirectory, "animals.txt");
        public Form1()
        {
            InitializeComponent();
            SetUpDataGridBView();
            EnsureStaterFileExists();
            RefreshGrid();
        }

        private void SetUpDataGridBView()
        {
            dvgAnimals.AutoGenerateColumns = false;
            dvgAnimals.Columns.Clear();

            dvgAnimals.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Id",
                HeaderText = "Animal ID"
            });

            dvgAnimals.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Name",
                HeaderText = "Name"
            });

            dvgAnimals.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Species",
                HeaderText = "Species"
            });

            dvgAnimals.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Age",
                HeaderText = "Age"
            });

            dvgAnimals.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "RecoveryScore",
                HeaderText = "Recovery Score"
            });

            dvgAnimals.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Status",
                HeaderText = "Status"
            });

            dvgAnimals.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "HousingUnit",
                HeaderText = "Housing Unit"
            });

        }

        public void EnsureStaterFileExists()
        {
            try
            {
                if (!File.Exists(filepath))
                {
                    List<string> staterLines = new List<string>//allows us to set some defualt data
                    {
                       "WR-0001|Thabo|African Penguin|4|85|Release-Ready|Pre-Release Camp",
                       "WR-0002|Naledi|Serval|3|62|Recovering|Outdoor Enclosure"
                    };
                    File.WriteAllLines(filepath, staterLines);
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show($"File erro during intitialization: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //this will relaod animals fromfile and rebind dvgAnimals
        public void RefreshGrid()
        {
            List<Animal> animalslist = new List<Animal>();
            int skippedLines = 0;

            if (File.Exists(filepath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(filepath);
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        try
                        {
                            Animal animal = Animal.Parse(line);
                            animalslist.Add(animal);
                        }
                        catch
                        {
                            skippedLines++;
                        }
                    }
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    MessageBox.Show($"Error reading file: {ex.Message}", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            dvgAnimals.DataSource = null;
            dvgAnimals.DataSource = animalslist;
            if (skippedLines > 0)
            {
                MessageBox.Show($"Loaded records successfull. Skipped {skippedLines} invalid lines", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ---------- Validation ----------
        private bool ValidateAnimalInputs(bool checkUniqueId, out string id, out string name, out string species, out int age, out int score)
        {
            id = txtAnimalid.Text.Trim().ToUpperInvariant();
            name = txtNames.Text.Trim();
            species = txtSpecies.Text.Trim();

            age = 0;
            score = 0;

            // Animal ID validation
            if (!Regex.IsMatch(id, @"^WR-\d{4}$"))
            {
                MessageBox.Show("Animal ID must follow the following format: WR- follwed by 4 digits. ", "Invalid Animal ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                txtAnimalid.Focus();
                txtAnimalid.SelectAll();

                return false;
            }

            // Check if animal ID already exists
            if (checkUniqueId)
            {
                try
                {
                    if (File.Exists(filepath))
                    {
                        foreach (string line in File.ReadAllLines(filepath))
                        {
                            string[] parts = line.Split('|');

                            if (parts.Length == 7 && parts[0].Trim().Equals(id, StringComparison.OrdinalIgnoreCase))
                            {
                                MessageBox.Show("This Animal ID already exists.", "Duplicate Animal ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                txtAnimalid.Focus();
                                txtAnimalid.SelectAll();

                                return false;
                            }
                        }
                    }
                }
                catch (IOException) // Handles file reading errors
                {
                    MessageBox.Show("The animal file could not be read... Please try again.", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return false;
                }
                catch (UnauthorizedAccessException) // Handles file application permissions
                {
                    MessageBox.Show("You do not have permission to read the animal file.", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return false;
                }
            }

            // Names Validation
            if (string.IsNullOrWhiteSpace(name) || name.Length > 30 || name.Contains('|'))
            {
                MessageBox.Show("Name is required, must be at most 30 characters, and cannot contain '|'", "Invalid Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                txtNames.Focus();
                txtNames.SelectAll();

                return false;
            }

            // Species Validation
            if (string.IsNullOrWhiteSpace(species) || species.Length > 30 || species.Contains('|'))
            {
                MessageBox.Show("Species is required, must be at most 30 characters, and cannot contain '|'.", "Invalid Species", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                txtSpecies.Focus();
                txtSpecies.SelectAll();

                return false;
            }

            // Age Validation
            if (!int.TryParse(txtAge.Text.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out age) || age < 0 || age > 100)
            {
                MessageBox.Show("Age must be a whole number between 0 and 100.", "Invalid Age", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                txtAge.Focus();
                txtAge.SelectAll();

                return false;
            }

            // Recovery Score Validation
            if (!int.TryParse(txtRecoveryScore.Text.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out score) || score < 0 || score > 100)
            {
                MessageBox.Show("Recovery Score must be a whole number between 0 and 100.", "Invalid Recovery Score", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                txtRecoveryScore.Focus();
                txtRecoveryScore.SelectAll();

                return false;
            }

            return true;

        }

        // ---------- Add button ----------
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateAnimalInputs(true, out string id, out string name, out string species, out int age, out int score))
            {
                return;
            }

            AddAnimal animal = new AddAnimal(filepath);

            bool success = animal.AddNewAnimal(id, name, species, age, score, out string message);

            MessageBox.Show(message);

            if (success)
            {
                ClearInputBoxes();
                RefreshGrid();
            }
        }
        //edit mode
        private void SetEditMode(bool edit)
        {
            txtAnimalid.ReadOnly = edit;// not endabled
            btnAdd.Enabled = !edit; //disabled 
            btnSaveChanges.Enabled = edit; //enabled

        }
        // ---------- Clear fields ----------
        private void ClearInputBoxes()
        {
            txtAnimalid.Clear();
            txtNames.Clear();
            txtSpecies.Clear();
            txtAge.Clear();
            txtRecoveryScore.Clear();
            SetEditMode(false);
        }
        private List<Animal> GetAnimalsFromFile()
        {
            List<Animal> animals = new List<Animal>();
            if (File.Exists(filepath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(filepath);
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        try
                        {
                            Animal animal = Animal.Parse(line);
                            animals.Add(animal);
                        }
                        catch
                        {
                            MessageBox.Show("Invalid data format in file.", "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    MessageBox.Show($"Error reading file: {ex.Message}", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return animals;
        }
        //fINDS ANIMALS BY id , NOT CASE SENSITIVE , returns null if ni animal
        private Animal? FindAnimal(string id)
        {
            List<Animal> animals = GetAnimalsFromFile();
            return animals.Find(a => a.Id.Equals(id.Trim(), StringComparison.OrdinalIgnoreCase));
        }
        // lOADING INTO TEXT BOXES 
        private void LoadAnimals(Animal animal)
        {
            txtAnimalid.Text = animal.Id;
            txtNames.Text = animal.Name;
            txtSpecies.Text = animal.Species;
            txtAge.Text = animal.Age.ToString();
            txtRecoveryScore.Text = animal.RecoveryScore.ToString();

            // loadedAnimalId= animal.Id;
            SetEditMode(true);       // EDIT MODE ON 
        }

        private void dvgAnimals_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            Animal? animal = dvgAnimals.Rows[e.RowIndex].DataBoundItem as Animal;
            if (animal != null)
            {
                LoadAnimals(animal);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchId = txtSearchID.Text.Trim();
            if (string.IsNullOrEmpty(searchId))
            {
                MessageBox.Show("Please enter an Animal ID to search.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Animal? foundAnimal = FindAnimal(searchId);
            if (foundAnimal != null)
            {
                LoadAnimals(foundAnimal);
            }
            else
            {
                MessageBox.Show($"No animal found with ID: {searchId}", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSearchID.Clear();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputBoxes();
            
        }
    }
}
