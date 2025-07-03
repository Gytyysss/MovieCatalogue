using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace MovieCatalogGUI;

public class Catalog
{
    public readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "movies.json");

    public void AddMovie(Movie movie)
    {
        var movies = GetAllMovies();
        movies.Add(movie);
        File.WriteAllText(filePath, JsonConvert.SerializeObject(movies, Formatting.Indented));
        MessageBox.Show($"Movie '{movie.Title}' added successfully to {filePath}");
    }

    public void DeleteMovie()
    {
        if (!File.Exists(filePath))
        {
            MessageBox.Show("No movies file found.");
            return;
        }
        var jsonMovies = File.ReadAllText(filePath);
        var movies = JsonConvert.DeserializeObject<List<Movie>>(jsonMovies);

        var titleToDelete = Prompt.ShowDialog("Input movie title to delete:", "Delete Movie");
        if (titleToDelete != null && movies != null)
        {
            movies.RemoveAll(m => m.Title != null && m.Title.Equals(titleToDelete, StringComparison.OrdinalIgnoreCase));
            MessageBox.Show($"Movie '{titleToDelete}' deleted successfully.");
            File.WriteAllText(filePath, JsonConvert.SerializeObject(movies, Formatting.Indented));
        }
    }

    public void ExportToExcel(string excelPath = "movies.xlsx")
    {
        var movies = new List<Movie>();
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            var loaded = JsonConvert.DeserializeObject<List<Movie>>(json);
            if (loaded != null)
                movies = loaded;
        }
        else
        {
            MessageBox.Show("No movies found to export.");
            return;
        }

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Movies");
            worksheet.Cell(1, 1).Value = "Title";
            worksheet.Cell(1, 2).Value = "Director";
            worksheet.Cell(1, 3).Value = "Genre";
            worksheet.Cell(1, 4).Value = "Year";
            worksheet.Cell(1, 5).Value = "Rating";

            var row = 2;
            foreach (var movie in movies)
            {
                worksheet.Cell(row, 1).Value = movie.Title;
                worksheet.Cell(row, 2).Value = movie.Director;
                worksheet.Cell(row, 3).Value = movie.Genre;
                worksheet.Cell(row, 4).Value = movie.Year;
                worksheet.Cell(row, 5).Value = movie.Rating;
                row++;
            }

            workbook.SaveAs(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, excelPath));
        }
    }

    public List<Movie> GetAllMovies()
    {
        if (!File.Exists(filePath))
            return new List<Movie>();
        var json = File.ReadAllText(filePath);
        var movies = JsonConvert.DeserializeObject<List<Movie>>(json);
        return movies ?? new List<Movie>();
    }
    public void UpdateMovie(int index, Movie updatedMovie)
    {
        var movies = GetAllMovies();
        if (index >= 0 && index < movies.Count)
        {
            movies[index] = updatedMovie;
            File.WriteAllText(filePath, JsonConvert.SerializeObject(movies, Formatting.Indented));
            MessageBox.Show($"Movie at index {index} updated successfully.");
        }
        else
        {
            MessageBox.Show("Invalid movie index.");
        }
    }
    public void DeleteMovieAt(int index)
    {
        var movies = GetAllMovies();
        if (index >= 0 && index < movies.Count)
        {
            movies.RemoveAt(index);
            File.WriteAllText(filePath, JsonConvert.SerializeObject(movies, Formatting.Indented));
        }
        else
        {
            MessageBox.Show("Invalid movie index.");
        }
    }

    public List<Movie> ImportFromExcel(string excelPath = "movies.xlsx")
    {
        var movies = new List<Movie>();
        if (!File.Exists(excelPath))
        {
            MessageBox.Show("Excel file not found.");
            return movies;
        }

        using (var workbook = new XLWorkbook(excelPath))
        {
            var worksheet = workbook.Worksheet(1);
            var row = 2;
            while (!worksheet.Cell(row, 1).IsEmpty())
            {
                var movie = new Movie
                {
                    Title = worksheet.Cell(row, 1).GetString(),
                    Director = worksheet.Cell(row, 2).GetString(),
                    Genre = worksheet.Cell(row, 3).GetString(),
                    Year = int.TryParse(worksheet.Cell(row, 4).GetString(), out int y) ? y : 0,
                    Rating = float.TryParse(worksheet.Cell(row, 5).GetString(), out float r) ? r : 0
                };
                movies.Add(movie);
                row++;
            }
        }
        return movies;
    }
}
