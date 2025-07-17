using System;
using System.Windows.Forms;
using System.Drawing;

namespace MovieCatalogGUI;

public partial class MainWindow : Form
{
    public Catalog Catalog;
    private ListBox movieListBox;
    private Button editButton;
    private Button deleteButton;
    private TextBox searchBox;

    public MainWindow()
    {
        InitializeComponent();
        Catalog = new Catalog();

        AddAddMovieButton();
        AddImportFromExcelButton();

        movieListBox = new ListBox();
        movieListBox.Location = new Point(20, 60);
        movieListBox.Size = new Size(600, 200);
        movieListBox.HorizontalScrollbar = true;
        this.Controls.Add(movieListBox);

        searchBox = new TextBox();
        searchBox.Name = "searchBox";
        searchBox.PlaceholderText = "Search movies...";
        searchBox.Location = new Point(20, 20);
        searchBox.Size = new Size(200, 25);
        searchBox.TextChanged += (s, e) => FilterMovieList(searchBox.Text);
        this.Controls.Add(searchBox);

        FilterMovieList("");

        editButton = new Button();
        editButton.Text = "Edit";
        editButton.Location = new Point(450, 270);
        editButton.Visible = false;
        editButton.Click += EditButton_Click;
        this.Controls.Add(editButton);

        deleteButton = new Button();
        deleteButton.Text = "Delete";
        deleteButton.Location = new Point(545, 270);
        deleteButton.Visible = false;
        deleteButton.Click += DeleteButton_Click;
        this.Controls.Add(deleteButton);

        movieListBox.SelectedIndexChanged += (s, e) =>
        {
            var valid = movieListBox.SelectedIndex >= 0 && !(movieListBox.SelectedItem?.ToString()?.Contains("No movies found.") ?? true);
            editButton.Visible = valid;
            deleteButton.Visible = valid;
        };
    }

    private void AddAddMovieButton()
    {
        Button addMovieButton = new Button();
        addMovieButton.Text = "Add Movie";
        addMovieButton.Location = new Point(260, 20);
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
        this.Controls.Add(addMovieButton);
    }

    private void AddImportFromExcelButton()
    {
        Button importButton = new Button();
        importButton.Text = "Import";
        importButton.Location = new Point(460, 20);
        importButton.Click += (s, e) =>
        {
            Catalog.ImportFromExcelAndSync();
            FilterMovieList(searchBox.Text);
            MessageBox.Show("Movies imported and synchronized with Excel and JSON.");
        };
        this.Controls.Add(importButton);
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
}
