using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WeatherFit.Models;
using WeatherFit.Services;

namespace WeatherFit
{
    public partial class Form1 : Form
    {
        private Panel searchPanel;
        private TableLayoutPanel tableLayout;
        private TextBox cityTextBox;
        private Button searchButton;
        private ListBox suggestionListBox;

        private TextBox closetNameTextBox;
        private Button createClosetButton;
        private ComboBox closetComboBox;
        private Button editClosetButton;
        private TextBox clothingItemTextBox;
        private Button addClothingButton;
        private FlowLayoutPanel clothingItemsPanel;
        private Button deleteClosetButton;
        private Button returnToMainButton;
        private Button sendPromptButton;
        private Label groqResponseLabel;
        private Label titleLabel;

        private WeatherService _weatherService;
        private LocationService _locationService;
        private ClosetDatabase _closetDatabase;
        private GroqService _groqService;

        private float aspectRatio;

        private string selectedLat;
        private string selectedLon;
        private string weatherOutput;

        private string darkblue = "#1d223c";
        private string lightblue = "#cde5f5";
        private string lightpink = "#e4e1fe";


        public Form1()
        {
            InitializeComponent();
            this.Width = 420;
            this.Height = 500;
            aspectRatio = (float)this.Width / this.Height;
            this.Text = "Weather Fit";
            this.BackColor = ColorTranslator.FromHtml(darkblue);

            _closetDatabase = new ClosetDatabase();
            _weatherService = new WeatherService();
            _locationService = new LocationService();
            _groqService = new GroqService();
            this.Resize += OnFormResize;

            InitializeUIComponents();
        }

        private void OnFormResize(object sender, EventArgs e)
        {
            int newHeight = (int)(this.Width / aspectRatio);

            this.Resize -= OnFormResize;

            this.Height = newHeight;

            this.Resize += OnFormResize;
        }


        private void InitializeUIComponents()
        {
            tableLayout = new TableLayoutPanel
            {
                RowCount = 5,
                ColumnCount = 4,
                Dock = DockStyle.Fill
            };
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 36));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 16F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 16F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 16F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 16F));

            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));


            searchPanel = new Panel
            {
                Width = 220,
                Height = 200
            };



            cityTextBox = new TextBox { PlaceholderText = "Enter city...", Width = 200 };
            cityTextBox.TextChanged += OnCityTextChanged;
            cityTextBox.BackColor = ColorTranslator.FromHtml(lightpink);

            searchButton = new Button { Text = "Get Weather", Width = 100, Height = 30 };
            searchButton.Click += OnSearchCityClicked;
            searchButton.BackColor = ColorTranslator.FromHtml(lightblue);

            suggestionListBox = new ListBox { Height = 150, Width = 200, Visible = false };
            suggestionListBox.SelectedIndexChanged += OnSuggestionSelected;
            suggestionListBox.BackColor = ColorTranslator.FromHtml(lightpink);

            searchPanel.Controls.Add(cityTextBox);
            searchPanel.Controls.Add(suggestionListBox);
            suggestionListBox.Top = cityTextBox.Bottom + 5;



            closetNameTextBox = new TextBox { PlaceholderText = "New Closet", Width = 200 };
            closetNameTextBox.BackColor = ColorTranslator.FromHtml(lightpink);
            createClosetButton = new Button { Text = "Create Closet", Width = 100, Height = 30 };
            createClosetButton.Click += CreateClosetButton_Click;
            createClosetButton.BackColor = ColorTranslator.FromHtml(lightblue);

            closetComboBox = new ComboBox { Width = 200 };
            closetComboBox.SelectedIndexChanged += OnClosetSelected;
            closetComboBox.BackColor = ColorTranslator.FromHtml(lightpink);

            editClosetButton = new Button { Text = "Edit Closet", Width = 100, Height = 30, Visible = true };
            editClosetButton.Click += EditClosetButton_Click;
            editClosetButton.BackColor = ColorTranslator.FromHtml(lightblue);

            clothingItemTextBox = new TextBox { PlaceholderText = "Clothing Item", Width = 200, Visible = false };
            clothingItemTextBox.BackColor = ColorTranslator.FromHtml(lightpink);
            addClothingButton = new Button { Text = "Add Clothing", Width = 100, Height = 30, Visible = false };
            addClothingButton.Click += AddClothingButton_Click;
            addClothingButton.BackColor = ColorTranslator.FromHtml(lightblue);

            clothingItemsPanel = new FlowLayoutPanel { AutoScroll = true, Width = 350, Height = 200, Anchor = AnchorStyles.None };
            clothingItemsPanel.BackColor = ColorTranslator.FromHtml(lightpink);

            deleteClosetButton = new Button { Text = "Delete Closet", Width = 200, Height = 30, Visible = false };
            deleteClosetButton.Click += DeleteClosetButton_Click;
            deleteClosetButton.BackColor = ColorTranslator.FromHtml(lightblue);

            returnToMainButton = new Button { Text = "Done", Width = 200, Height = 30, Visible = true };
            returnToMainButton.Click += ReturnToMainButton_Click;
            returnToMainButton.BackColor = ColorTranslator.FromHtml(lightblue);

            sendPromptButton = new Button { Text = "Send Prompt", Width = 200, Height = 30 };
            sendPromptButton.Click += SendPromptButton_Click;
            sendPromptButton.BackColor = ColorTranslator.FromHtml(lightblue);

            groqResponseLabel = new Label { AutoSize = true, Width = 300, Height = 30, Anchor = AnchorStyles.None };
            groqResponseLabel.ForeColor = Color.White;

            titleLabel = new Label
            {
                Text = "WeatherFit",
                Width = 350,
                Height = 150,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Font = new Font("Arial", 16, FontStyle.Bold)
            };
            titleLabel.ForeColor = Color.White;


            tableLayout.Controls.Add(titleLabel, 1, 0);
            tableLayout.SetColumnSpan(titleLabel, 2);
            LoadMainLayout();


            this.Controls.Add(tableLayout);
            LoadClosetComboBox();
        }

        private async void OnCityTextChanged(object sender, EventArgs e)
        {
            string query = cityTextBox.Text;
            if (string.IsNullOrWhiteSpace(query))
            {
                suggestionListBox.Visible = false;
                return;
            }

            var suggestions = await _locationService.GetCitySuggestionsAsync(query);
            suggestionListBox.Items.Clear();
            foreach (var suggestion in suggestions)
            {
                suggestionListBox.Visible = true;
                suggestionListBox.Items.Add(suggestion);
            }
        }

        private void OnSuggestionSelected(object sender, EventArgs e)
        {
            if (suggestionListBox.SelectedItem is LocationData location)
            {
                cityTextBox.Text = location.DisplayName;

                selectedLat = location.Latitude;
                selectedLon = location.Longitude;
            }
        }

        private async void OnSearchCityClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedLat) && !string.IsNullOrEmpty(selectedLon))
            {
                suggestionListBox.Visible = false;
                weatherOutput = await _weatherService.GetWeatherDataAsync(selectedLat, selectedLon);
            }
        }

        private void CreateClosetButton_Click(object sender, EventArgs e)
        {
            var closetName = closetNameTextBox.Text;
            if (string.IsNullOrWhiteSpace(closetName))
            {
                MessageBox.Show("Closet name cannot be empty.");
                return;
            }

            var closet = new Closet { Name = closetName };
            _closetDatabase.AddCloset(closet);
            LoadClosetComboBox();
            closetNameTextBox.Clear();
        }

        private void LoadClosetComboBox()
        {
            clothingItemsPanel.Controls.Clear();
            closetComboBox.Items.Clear();
            clothingItemTextBox.Visible = false;
            addClothingButton.Visible = false;
            deleteClosetButton.Visible = false;
            var closets = _closetDatabase.GetClosets();
            closetComboBox.Items.Add("Select Closet");
            foreach (var closet in closets)
            {
                closetComboBox.Items.Add(closet);
            }
        }

        public void LoadClothingItemPanel(Closet selectedCloset)
        {
            clothingItemsPanel.Controls.Clear();
            clothingItemTextBox.Visible = true;
            addClothingButton.Visible = true;
            deleteClosetButton.Visible = true;
            foreach (var item in selectedCloset.ClothingItems)
            {
                var clothingItemTable = new TableLayoutPanel
                {
                    RowCount = 2,
                    ColumnCount = 2,
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink
                };
                clothingItemTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
                clothingItemTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

                clothingItemTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                clothingItemTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                var itemLabel = new Label
                {
                    Text = item.Description + (item.InLaundry ? " (In Laundry)" : ""),
                    AutoSize = true,
                    Width = clothingItemsPanel.Width - 20
                };
                var toggleButton = new Button
                {
                    Text = item.InLaundry ? "Remove from Laundry" : "Mark as Laundry",
                    AutoSize = true,
                    Width = 75
                };
                var deleteButton = new Button
                {
                    Text = "Delete",
                    AutoSize = true,
                    Width = 75
                };
                toggleButton.Click += (s, ev) => ToggleLaundryStatus(item, toggleButton);
                toggleButton.BackColor = ColorTranslator.FromHtml(lightblue);
                deleteButton.Click += (s, ev) => DeleteClothingButton_Click(item);
                deleteButton.BackColor = ColorTranslator.FromHtml(lightblue);
                clothingItemTable.Controls.Add(itemLabel, 0, 0);
                clothingItemTable.SetColumnSpan(itemLabel, 2);
                clothingItemTable.Controls.Add(toggleButton, 0, 1);
                clothingItemTable.Controls.Add(deleteButton, 1, 1);
                clothingItemsPanel.Controls.Add(clothingItemTable);
            }
        }

        private void LoadMainLayout()
        {
            tableLayout.Controls.Add(searchPanel, 1, 1);
            tableLayout.Controls.Add(searchButton, 2, 1);
            tableLayout.Controls.Add(closetNameTextBox, 1, 2);
            tableLayout.Controls.Add(createClosetButton, 2, 2);
            tableLayout.Controls.Add(closetComboBox, 1, 3);
            tableLayout.Controls.Add(editClosetButton, 2, 3);
            tableLayout.Controls.Add(sendPromptButton, 1, 4);
            tableLayout.SetColumnSpan(sendPromptButton, 2);
        }

        private void RemoveMainLayout()
        {
            tableLayout.Controls.Remove(searchPanel);
            tableLayout.Controls.Remove(searchButton);
            tableLayout.Controls.Remove(closetNameTextBox);
            tableLayout.Controls.Remove(createClosetButton);
            tableLayout.Controls.Remove(closetComboBox);
            tableLayout.Controls.Remove(editClosetButton);
            tableLayout.Controls.Remove(sendPromptButton);
        }

        private void OnClosetSelected(object sender, EventArgs e)
        {
            if (closetComboBox.SelectedItem is Closet selectedCloset)
            {
                LoadClothingItemPanel(selectedCloset);
            }
            else
            {
                clothingItemsPanel.Controls.Clear();
                clothingItemTextBox.Visible = false;
                addClothingButton.Visible = false;
                deleteClosetButton.Visible = false;
            }
        }

        private void EditClosetButton_Click(object sender, EventArgs e)
        {
            RemoveMainLayout();

            tableLayout.Controls.Add(clothingItemTextBox, 1, 1);
            tableLayout.Controls.Add(addClothingButton, 2, 1);
            tableLayout.Controls.Add(clothingItemsPanel, 0, 2);
            tableLayout.Controls.Add(deleteClosetButton, 1, 4);
            tableLayout.Controls.Add(returnToMainButton, 0, 1);
            tableLayout.SetColumnSpan(clothingItemsPanel, 4);
            tableLayout.SetColumnSpan(deleteClosetButton, 2);
            tableLayout.SetRowSpan(clothingItemsPanel, 2);
        }

        private void ReturnToMainButton_Click(object sender, EventArgs e)
        {
            if (tableLayout.Contains(deleteClosetButton))
            {
                tableLayout.Controls.Remove(clothingItemTextBox);
                tableLayout.Controls.Remove(addClothingButton);
                tableLayout.Controls.Remove(clothingItemsPanel);
                tableLayout.Controls.Remove(deleteClosetButton);
            }
            else
            {
                tableLayout.Controls.Remove(groqResponseLabel);
            }
            tableLayout.Controls.Remove(returnToMainButton);
            LoadMainLayout();
        }

        private void ToggleLaundryStatus(ClothingItem item, Button toggleButton)
        {
            item.InLaundry = !item.InLaundry;

            toggleButton.Text = item.InLaundry ? "Remove from Laundry" : "Mark as Laundry";

            var itemLabel = clothingItemsPanel.Controls.OfType<Label>().FirstOrDefault(lbl => lbl.Text.StartsWith(item.Description));
            if (itemLabel != null)
            {
                itemLabel.Text = item.Description + (item.InLaundry ? " (In Laundry)" : "");
            }

            var selectedCloset = closetComboBox.SelectedItem as Closet;
            _closetDatabase.UpdateCloset(selectedCloset);
        }


        private void AddClothingButton_Click(object sender, EventArgs e)
        {
            if (closetComboBox.SelectedItem is Closet selectedCloset)
            {
                var description = clothingItemTextBox.Text;
                if (string.IsNullOrWhiteSpace(description))
                {
                    MessageBox.Show("Clothing item description cannot be empty.");
                    return;
                }

                selectedCloset.ClothingItems.Add(new ClothingItem { Description = description.Trim().ToLower(), ClosetId = selectedCloset.Id.ToString() });
                _closetDatabase.UpdateCloset(selectedCloset);
                OnClosetSelected(null, null);
                clothingItemTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Please select a closet first.");
            }
        }
        private void DeleteClothingButton_Click(ClothingItem item)
        {
            if (closetComboBox.SelectedItem is Closet selectedCloset)
            {
                selectedCloset.ClothingItems.Remove(item);
                _closetDatabase.DeleteClothingItem(item);
                LoadClothingItemPanel(selectedCloset);
            }
            else
            {
                MessageBox.Show("Please select a closet to delete.");
            }
        }

        private void DeleteClosetButton_Click(object sender, EventArgs e)
        {
            if (closetComboBox.SelectedItem is Closet selectedCloset)
            {
                _closetDatabase.DeleteCloset(selectedCloset.Id);
                clothingItemsPanel.Controls.Clear();
            }
            else
            {
                MessageBox.Show("Please select a closet to delete.");
            }
        }

        private async void SendPromptButton_Click(object sender, EventArgs e)
        {
            RemoveMainLayout();
            tableLayout.Controls.Add(groqResponseLabel, 0, 2);
            tableLayout.Controls.Add(returnToMainButton, 1, 4);
            tableLayout.SetColumnSpan(groqResponseLabel, 4);
            tableLayout.SetRowSpan(groqResponseLabel, 2);
            tableLayout.SetColumnSpan(returnToMainButton, 2);
            groqResponseLabel.Text = "Loading...";
            var weather = weatherOutput;
            var closet = "";
            if (closetComboBox.SelectedItem is Closet selectedCloset)
            {
                foreach (var item in selectedCloset.ClothingItems)
                {
                    if (!item.InLaundry)
                    {
                        closet += item.Description + ",";
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(weather) || string.IsNullOrWhiteSpace(closet))
            {
                MessageBox.Show("Prompt cannot be empty.");
                return;
            }
            groqResponseLabel.Text = await _groqService.GetGroqAnalysis(weather, closet);
        }
    }
}
