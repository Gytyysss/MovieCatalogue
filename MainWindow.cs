using System;
using System.Windows.Forms;
using System.Drawing;

namespace MovieCatalogGUI;

public partial class MainWindow : Form
{
    public Catalog Catalog;
    private ListBox movieListBox;
    private Button addMovieButton;
    private Button editButton;
    private Button deleteButton;
    private Button importButton;
    private TextBox searchBox;
    private bool isDarkTheme = true; // Add this field to your class

    public MainWindow()
    {
        InitializeComponent();
        this.BackColor = Color.FromArgb(30, 30, 30); // Shadowy black background
        Catalog = new Catalog();

        // Create TableLayoutPanel
        var table = new TableLayoutPanel();
        table.Dock = DockStyle.Fill;
        table.ColumnCount = 4;
        table.RowCount = 3;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Top row
        table.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Middle row (list)
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Bottom row (buttons)
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        this.Controls.Add(table);

        // Search box (spans all columns)
        searchBox = new TextBox();
        searchBox.Dock = DockStyle.Fill;
        searchBox.PlaceholderText = "Search movies...";
        searchBox.TextChanged += (s, e) => FilterMovieList(searchBox.Text);
        table.Controls.Add(searchBox, 0, 0);
        table.SetColumnSpan(searchBox, 4);

        // Movie list
        movieListBox = new ListBox();
        movieListBox.Dock = DockStyle.Fill;
        movieListBox.HorizontalScrollbar = true;
        table.Controls.Add(movieListBox, 0, 1);
        table.SetColumnSpan(movieListBox, 4);
        
        // Add Movie button
        addMovieButton = new Button();
        addMovieButton.Text = "Add Movie";
        addMovieButton.Dock = DockStyle.Fill;
        addMovieButton.Font = new Font("Courier New", 12, FontStyle.Bold);
        addMovieButton.Text = addMovieButton.Text.ToUpper();
        addMovieButton.Click += (s, e) =>
        {
            using (var form = new AddMovieForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var movie = new Movie
                    {
                        Title = form.Title,
                        Director = form.Director,
                        Genre = form.Genre,
                        Year = int.TryParse(form.Year, out int y) ? y : 0,
                        Rating = float.TryParse(form.Rating, out float r) ? r : 0
                    };
                    Catalog.AddMovie(movie);
                    Catalog.ExportToExcel(); // This updates the Excel file
                    FilterMovieList(searchBox.Text);
                }
            }
        };
        table.Controls.Add(addMovieButton, 0, 2);
        table.Controls.Add(addMovieButton, 0, 2);
        // Import button
        importButton = new Button();
        importButton.Text = "Import";
        importButton.Dock = DockStyle.Fill;
        importButton.Font = new Font("Courier New", 12, FontStyle.Bold);
        importButton.Text = importButton.Text.ToUpper();
        importButton.Click += (s, e) =>
        {
            Catalog.ImportFromExcelAndSync();
            FilterMovieList(searchBox.Text);
            MessageBox.Show("Movies imported and synchronized with Excel and JSON.");
        };
        table.Controls.Add(importButton, 1, 2);
        table.Controls.Add(importButton, 1, 2);

        // Edit button
        editButton = new Button();
        editButton.Text = "Edit";
        editButton.Dock = DockStyle.Fill;
        editButton.Font = new Font("Courier New", 12, FontStyle.Bold);
        editButton.Text = editButton.Text.ToUpper();
        editButton.Visible = false;
        editButton.Click += EditButton_Click;
        table.Controls.Add(editButton, 2, 2);

        // Delete button
        deleteButton = new Button();
        deleteButton.Text = "Delete";
        deleteButton.Dock = DockStyle.Fill;
        deleteButton.Font = new Font("Courier New", 12, FontStyle.Bold);
        deleteButton.Text = deleteButton.Text.ToUpper();
        deleteButton.Visible = false;
        deleteButton.Click += DeleteButton_Click;
        table.Controls.Add(deleteButton, 3, 2);

        movieListBox.SelectedIndexChanged += (s, e) =>
        {
            var valid = movieListBox.SelectedIndex >= 0 && !(movieListBox.SelectedItem?.ToString()?.Contains("No movies found.") ?? true);
            editButton.Visible = valid;
            deleteButton.Visible = valid;
        };

        // Toggle Theme button
        var themeToggleButton = new Button();
        themeToggleButton.Text = "Toggle Theme";
        themeToggleButton.Dock = DockStyle.Fill;
        themeToggleButton.Font = new Font("Courier New", 12, FontStyle.Bold);
        themeToggleButton.Click += (s, e) =>
        {
            isDarkTheme = !isDarkTheme;
            ApplyTheme();
        };
        table.Controls.Add(themeToggleButton, 0, 3);
        table.SetColumnSpan(themeToggleButton, 4);

        ApplyTheme(); // Call ApplyTheme() at the end of your constructor
        FilterMovieList("");
        this.Resize += MainWindow_Resize;

        addMovieButton.Paint += ButtonWithContour_Paint;
        importButton.Paint += ButtonWithContour_Paint;
        editButton.Paint += ButtonWithContour_Paint;
        deleteButton.Paint += ButtonWithContour_Paint;

        addMovieButton.UseCompatibleTextRendering = true;
        importButton.UseCompatibleTextRendering = true;
        editButton.UseCompatibleTextRendering = true;
        deleteButton.UseCompatibleTextRendering = true;
    }

    private void FilterMovieList(string searchText)
    {
        movieListBox.Items.Clear();
        var movies = Catalog.GetAllMovies();
        foreach (var movie in movies)
        {
            string display = $"{movie.Title}, {movie.Year}, {movie.Genre}, {movie.Director}, Rating: {movie.Rating}";
            if (display.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                movieListBox.Items.Add(display);
            }
        }
        if (movieListBox.Items.Count == 0)
        {
            movieListBox.Items.Add("No movies found.");
        }
    }

    private void EditButton_Click(object sender, EventArgs e)
    {
        var index = movieListBox.SelectedIndex;
        if (index < 0) return;

        var movies = Catalog.GetAllMovies();
        if (index >= movies.Count) return;

        var movie = movies[index];
        using (var form = new AddMovieForm())
        {
            form.SetMovie(movie);
            if (form.ShowDialog() == DialogResult.OK)
            {
                movie.Title = form.Title;
                movie.Director = form.Director;
                movie.Genre = form.Genre;
                movie.Year = int.TryParse(form.Year, out int y) ? y : 0;
                movie.Rating = float.TryParse(form.Rating, out float r) ? r : 0;
                Catalog.UpdateMovie(index, movie);
                Catalog.ExportToExcel(); // Automatically export
                FilterMovieList(searchBox.Text);
            }
        }
    }

    private void DeleteButton_Click(object sender, EventArgs e)
    {
        int index = movieListBox.SelectedIndex;
        if (index < 0) return;

        var movies = Catalog.GetAllMovies();
        if (index >= movies.Count) return;

        var result = MessageBox.Show("Are you sure you want to delete this movie?", "Confirm Delete", MessageBoxButtons.YesNo);
        if (result == DialogResult.Yes)
        {
            Catalog.DeleteMovieAt(index);
            Catalog.ExportToExcel(); // Automatically export
            FilterMovieList(searchBox.Text);
        }
    }

    private void MainWindow_Resize(object sender, EventArgs e)
    {
        // Example: scale font size between 10 and 20 based on window width
        int minFont = 10;
        int maxFont = 20;
        int minWidth = 600;
        int maxWidth = 1200;
        int width = this.ClientSize.Width;

        float fontSize = minFont + (maxFont - minFont) * (width - minWidth) / (float)(maxWidth - minWidth);
        fontSize = Math.Max(minFont, Math.Min(maxFont, fontSize));

        var font = new Font("Courier New", fontSize, FontStyle.Bold);

        addMovieButton.Font = font;
        importButton.Font = font;
        editButton.Font = font;
        deleteButton.Font = font;
    }

    private void ApplyTheme()
    {
        if (isDarkTheme)
        {
            this.BackColor = Color.FromArgb(30, 30, 30);
            searchBox.BackColor = Color.FromArgb(45, 45, 45);
            searchBox.ForeColor = Color.White;
            movieListBox.BackColor = Color.FromArgb(45, 45, 45);
            movieListBox.ForeColor = Color.White;
            addMovieButton.BackColor = Color.FromArgb(50, 50, 50);
            addMovieButton.ForeColor = Color.Green;
            importButton.BackColor = Color.FromArgb(50, 50, 50);
            importButton.ForeColor = Color.Orange;
            editButton.BackColor = Color.FromArgb(50, 50, 50);
            editButton.ForeColor = Color.Yellow;
            deleteButton.BackColor = Color.FromArgb(50, 50, 50);
            deleteButton.ForeColor = Color.Red;
        }
        else
        {
            this.BackColor = Color.White;
            searchBox.BackColor = Color.White;
            searchBox.ForeColor = Color.Black;
            movieListBox.BackColor = Color.White;
            movieListBox.ForeColor = Color.Black;
            addMovieButton.BackColor = Color.White;
            addMovieButton.ForeColor = Color.Green;
            importButton.BackColor = Color.White;
            importButton.ForeColor = Color.Orange;
            editButton.BackColor = Color.White;
            editButton.ForeColor = Color.Yellow;
            deleteButton.BackColor = Color.White;
            deleteButton.ForeColor = Color.Red;
        }
    }

    private void ButtonWithContour_Paint(object sender, PaintEventArgs e)
    {
        var btn = sender as Button;
        if (btn == null) return;

        string text = btn.Text;
        var g = e.Graphics;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        // Center text
        SizeF textSize = g.MeasureString(text, btn.Font);
        float x = (btn.Width - textSize.Width) / 2;
        float y = (btn.Height - textSize.Height) / 2;

        // Draw contour by offsetting in 8 directions
        using (var outlineBrush = new SolidBrush(Color.Black))
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    g.DrawString(text, btn.Font, outlineBrush, x + dx, y + dy);
                }
            }
        }

        // Draw main text
        using (var textBrush = new SolidBrush(btn.ForeColor))
        {
            g.DrawString(text, btn.Font, textBrush, x, y);
        }
    }
}
