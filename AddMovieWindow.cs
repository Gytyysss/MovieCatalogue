using System.Windows.Forms;

namespace MovieCatalogGUI;

public class AddMovieForm : Form
{
    public Movie movie = new Movie();

    public string Title => txtTitle.Text;
    public string Director => txtDirector.Text;
    public string Genre => txtGenre.Text;
    public string Year => txtYear.Text;
    public string Rating => txtRating.Text;

    private TextBox txtTitle = new TextBox();
    private TextBox txtDirector = new TextBox();
    private TextBox txtGenre = new TextBox();
    private TextBox txtYear = new TextBox();
    private TextBox txtRating = new TextBox();

    public AddMovieForm()
    {
        this.Text = "Add Movie";
        this.Width = 300;
        this.Height = 300;

        var lblTitle = new Label() { Text = "Title:", Left = 10, Top = 20, Width = 80 };
        txtTitle.Left = 100; txtTitle.Top = 20; txtTitle.Width = 150;

        var lblDirector = new Label() { Text = "Director:", Left = 10, Top = 60, Width = 80 };
        txtDirector.Left = 100; txtDirector.Top = 60; txtDirector.Width = 150;

        var lblGenre = new Label() { Text = "Genre:", Left = 10, Top = 100, Width = 80 };
        txtGenre.Left = 100; txtGenre.Top = 100; txtGenre.Width = 150;

        var lblYear = new Label() { Text = "Year:", Left = 10, Top = 140, Width = 80 };
        txtYear.Left = 100; txtYear.Top = 140; txtYear.Width = 150;

        var lblRating = new Label() { Text = "Rating:", Left = 10, Top = 180, Width = 80 };
        txtRating.Left = 100; txtRating.Top = 180; txtRating.Width = 150;

        var btnOk = new Button() { Text = "OK", Left = 100, Top = 220, Width = 60, DialogResult = DialogResult.OK };
        var btnCancel = new Button() { Text = "Cancel", Left = 190, Top = 220, Width = 60, DialogResult = DialogResult.Cancel };

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

        this.AcceptButton = btnOk;
        this.CancelButton = btnCancel;
    }

    public void SetMovie(Movie movie)
    {
        txtTitle.Text = movie.Title;
        txtDirector.Text = movie.Director;
        txtGenre.Text = movie.Genre;
        txtYear.Text = movie.Year.ToString();
        txtRating.Text = movie.Rating.ToString();
    }
}