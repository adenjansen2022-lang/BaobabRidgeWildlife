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


    }
}
