using System.Windows.Forms;


namespace MovieCatalogGUI;

public class MainController
{
    public Catalog Catalog;

    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainWindow());
    }
}