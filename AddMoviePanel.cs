using System;
using System.Windows.Forms;
using System.Drawing;

namespace MovieCatalogGUI;

public class AddMoviePanel : UserControl
{
    public event Action<Movie> MovieSaved;
    public event Action Cancelled;

    public bool IsEditMode { get; set; } // Add this property

    private TextBox txtTitle = new TextBox();
    private TextBox txtDirector = new TextBox();
    private TextBox txtGenre = new TextBox();
    private TextBox txtYear = new TextBox();
    private TextBox txtRating = new TextBox();

    public AddMoviePanel()
    {
        this.Dock = DockStyle.Fill;
        this.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);

        var lblTitle = new Label() { Text = "Title:", Left = 10, Top = 20, Width = 80, ForeColor = System.Drawing.Color.White };
        txtTitle.Left = 100; txtTitle.Top = 20; txtTitle.Width = 200;

        var lblDirector = new Label() { Text = "Director:", Left = 10, Top = 60, Width = 80, ForeColor = System.Drawing.Color.White };
        txtDirector.Left = 100; txtDirector.Top = 60; txtDirector.Width = 200;

        var lblGenre = new Label() { Text = "Genre:", Left = 10, Top = 100, Width = 80, ForeColor = System.Drawing.Color.White };
        txtGenre.Left = 100; txtGenre.Top = 100; txtGenre.Width = 200;

        var lblYear = new Label() { Text = "Year:", Left = 10, Top = 140, Width = 80, ForeColor = System.Drawing.Color.White };
        txtYear.Left = 100; txtYear.Top = 140; txtYear.Width = 200;

        var lblRating = new Label() { Text = "Rating:", Left = 10, Top = 180, Width = 80, ForeColor = System.Drawing.Color.White };
        txtRating.Left = 100; txtRating.Top = 180; txtRating.Width = 200;

        var btnOk = new Button() { Text = "SAVE", Left = 100, Top = 230, Width = 80, Font = new System.Drawing.Font("Courier New", 12, System.Drawing.FontStyle.Bold) };
        var btnCancel = new Button() { Text = "CANCEL", Left = 200, Top = 230, Width = 80, Font = new System.Drawing.Font("Courier New", 12, System.Drawing.FontStyle.Bold) };

        btnOk.Click += (s, e) =>
        {
            var movie = new Movie
            {
                Title = txtTitle.Text,
                Director = txtDirector.Text,
                Genre = txtGenre.Text,
                Year = int.TryParse(txtYear.Text, out int y) ? y : 0,
                Rating = float.TryParse(txtRating.Text, out float r) ? r : 0
            };
            MovieSaved?.Invoke(movie);
        };

        btnCancel.Click += (s, e) => Cancelled?.Invoke();

        this.Controls.Add(lblTitle);
        this.Controls.Add(txtTitle);
        this.Controls.Add(lblDirector);
        this.Controls.Add(txtDirector);
        this.Controls.Add(lblGenre);
        this.Controls.Add(txtGenre);
        this.Controls.Add(lblYear);
        this.Controls.Add(txtYear);
        this.Controls.Add(lblRating);
        this.Controls.Add(txtRating);
        this.Controls.Add(btnOk);
        this.Controls.Add(btnCancel);
    }

    public void SetMovie(Movie movie)
    {
        txtTitle.Text = movie.Title;
        txtDirector.Text = movie.Director;
        txtGenre.Text = movie.Genre;
        txtYear.Text = movie.Year.ToString();
        txtRating.Text = movie.Rating.ToString();
    }

    public void ApplyTheme(bool isDarkTheme)
    {
        if (isDarkTheme)
        {
            this.BackColor = Color.FromArgb(40, 40, 40);
            foreach (Control c in this.Controls)
            {
                if (c is TextBox)
                {
                    c.BackColor = Color.FromArgb(45, 45, 45);
                    c.ForeColor = Color.White;
                }
                else if (c is Label)
                {
                    c.ForeColor = Color.White;
                }
                else if (c is Button)
                {
                    c.BackColor = Color.FromArgb(50, 50, 50);
                    c.ForeColor = Color.Black;
                    c.Font = new Font("Courier New", 12, FontStyle.Bold);
                }
            }
        }
        else
        {
            this.BackColor = Color.White;
            foreach (Control c in this.Controls)
            {
                if (c is TextBox)
                {
                    c.BackColor = Color.White;
                    c.ForeColor = Color.Black;
                }
                else if (c is Label)
                {
                    c.ForeColor = Color.Black;
                }
                else if (c is Button)
                {
                    c.BackColor = Color.White;
                    c.ForeColor = Color.Black;
                    c.Font = new Font("Courier New", 12, FontStyle.Bold);
                }
            }
        }
    }
}