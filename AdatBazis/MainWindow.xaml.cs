using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdatBazis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string beAllomanyUt, kiAllomanyUt;
        private SqliteConnection connection;
        private List<Jelentkezes> adatok;
       
        public MainWindow()
        {
            InitializeComponent();

            
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Sqlite database (*.sqlite3)|*.sqlite3| Sqlite database (*.db)|*.db";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == true)
            {
                connection = new($"Filename={ofd.FileName}");
                connection.Open();
                adatok = new();
                string queryText = "SELECT * FROM jelentkezes";
                SqliteCommand command = new(queryText, connection);
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    adatok.Add(new(
                        reader.GetString(0),
                        reader.GetChar(1),
                        reader.GetInt32(2),
                        reader.GetString(3)));
                }
                reader.Close();
                dgOnly.ItemsSource = adatok;
            }
        }

        private void dgKeres_on_change(object sender, TextChangedEventArgs e)
        {
            if (adatok.Count > 0)
            {
                dgOnly.ItemsSource = adatok.Where(x => x.SzakNeveTartalmazza(txtKeres.Text)).ToList();

            }
        }

        private void dgLegnagyobbatKeres_on_change(object sender, TextChangedEventArgs e)
        {
            if (adatok.Where(x => x.Szak == txtLegmagasabbPontok.Text).Count() > 0)
            {
                if (adatok.Count > 3)
                {

                    lbMaxok.ItemsSource = adatok.Where(x => x.Szak == txtLegmagasabbPontok.Text).OrderBy(x => x.Pontszam).Take(3).Select(x => x.Nev).ToList();

                }
            }
            else
            {
                lbMaxok.ItemsSource = new string[] { "Nincsen ilyen szak" };
            }
            
        }

        private void btnTorol_Click(object sender, RoutedEventArgs e)
        {
            adatok.Remove(dgOnly.SelectedItem as Jelentkezes);
            dgOnly.Items.Refresh();
        }

        private void fiukeslanyok_Click(object sender, RoutedEventArgs e)
        {
            lbFiuk.Content = $"Infós fiúk száma: {adatok.Count(x=>x.Nem=='f')}";
            lbLanyok.Content = $"Infós lányok száma: {adatok.Count(x => x.Nem == 'l')}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                beAllomanyUt = openFileDialog.FileName;
            }

            //kiallgomb, kimenet kivalasztasa

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "sqlite3 files (*.sqlite3)|*.sqlite3";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == true)
            {
                kiAllomanyUt = saveFileDialog.FileName;
            }

            //Átalakítás

            try
            {
                connection = new($"Filename={kiAllomanyUt}");
                connection.Open();
                var kiolvasottAdat = File.ReadAllLines(beAllomanyUt, Encoding.UTF8);
                string[] mezoNevek = kiolvasottAdat[0].Split(';');
                string commandText = $"CREATE TABLE IF NOT EXISTS jelentkezes({mezoNevek[0]} VARCHAR(100) PRIMARY KEY, {mezoNevek[1]} VARCHAR(1),  {mezoNevek[2]} INTEGER, {mezoNevek[3]} VARCHAR(100));";
                SqliteCommand command = new SqliteCommand(commandText, connection);
                command.ExecuteNonQuery();
                foreach (var item in kiolvasottAdat.Skip(1))
                {
                    string[] resz = item.Split(';');
                    commandText = $"INSERT INTO jelentkezes({kiolvasottAdat[0].Replace(';', ',')}) VALUES ('{resz[0]}','{resz[1]}','{resz[2]}','{resz[3]}')";
                    command = new(commandText, connection);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ";");
            }


        }
    }
}