using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _220117_szakszerviz
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    class DbSzerviz
    {


        public static List<string> GetSzolgaltatasList()
        {
            var db = new SzervizDBEntities();
            return db.Szolgaltatas.Select(x => x.Nev).ToList();
        }

        public static int GetSzolgaltatasId(string szolgaltatas)
        {
            var db = new SzervizDBEntities();
            return db.Szolgaltatas.FirstOrDefault(x => x.Nev == szolgaltatas).Id;
        }

        public static void GetAllDataToGrid(DataGrid dg)
        {
            var db = new SzervizDBEntities();
            dg.ItemsSource = db.Szerviz.Include("Szolgaltatas").ToList();
        }

        public static void SearchService(DataGrid dg, string rendszam)
        {
            var db = new SzervizDBEntities();
            var result = db.Szerviz.Include("Szolgaltatas").Where(x => x.Rendszam.Contains(rendszam)).ToList();
            dg.ItemsSource = result.ToList();
        }

        public static void CreatService(string rendszam, string marka, string tipus, DateTime forgalomban, int szolgaltatasId)
        {
            var db = new SzervizDBEntities();

            var ns = new Szerviz(){
                Rendszam = rendszam,
                Marka = marka,
                Tipus = tipus,
                Forgalomban = forgalomban,
                FK_Szolgaltatas_Id = szolgaltatasId,
                FelvetelDatuma = DateTime.Now,
            };

            db.Szerviz.Add(ns);
            db.SaveChanges();
            MessageBox.Show("A tétel hozzáadása sikeres volt!", "Sikeres művelet", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        public static void UpdateService(int ServiceId, string rendszam, string marka, string tipus, DateTime forgalomban, int szolgaltatasId)
        {
            var db = new SzervizDBEntities();
            var update = db.Szerviz.FirstOrDefault(x => x.Id == ServiceId);

            update.Rendszam = rendszam;
            update.Marka = marka; 
            update.Tipus = tipus;
            update.Forgalomban = forgalomban;
            update.FK_Szolgaltatas_Id = szolgaltatasId;

            db.SaveChanges();
            MessageBox.Show("A tétel frissítése sikeres volt!", "Sikeres művelet", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void DeleteService(int ServiceId)
        {
            var db = new SzervizDBEntities();
            var delete = db.Szerviz.FirstOrDefault(x => x.Id == ServiceId);
            db.Szerviz.Remove(delete);
            db.SaveChanges();
            MessageBox.Show("A tétel törlése sikeres volt!","Sikeres művelet",MessageBoxButton.OK,MessageBoxImage.Information);
        }


        public static void ExportAllData()
        {

            var db = new SzervizDBEntities();
            var export = db.Szerviz.Include("Szolgaltatas").ToList();

            if (export.Count == 0) {
                MessageBox.Show($"A lista nem tartalmaz adatokat!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            var dg = new SaveFileDialog
            {
                Filter = "CSV fileok (.csv) | .csv"
            };
            dg.ShowDialog();

            using (var fs = new FileStream(dg.FileName,FileMode.Create))
            {
                using (var sw = new StreamWriter(fs,Encoding.UTF8))
                {
                    foreach (var s in export)
                    {
                        sw.WriteLine($"{s.Id};{s.Rendszam};{s.Marka};{s.Tipus};{s.Forgalomban};{s.Szolgaltatas.Nev};{s.FelvetelDatuma}");
                    }
                }
            }

            MessageBox.Show("A lista exportálása sikeres volt!", "Sikeres művelet", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        public static void ExportSelectedData(string szolgaltatas)
        {
            var db = new SzervizDBEntities();
            var export = db.Szerviz.Include("Szolgaltatas").Where(x=>x.Szolgaltatas.Nev == szolgaltatas).ToList();

            if (export.Count == 0)
            {
                MessageBox.Show($"A kiválasztott {szolgaltatas} szolgáltatásra nincs találat!","Hiba",MessageBoxButton.OK,MessageBoxImage.Warning);
                return;
            }

            var dg = new SaveFileDialog
            {
                Filter = "CSV fileok (.csv) | .csv"
            };
            dg.ShowDialog();

            using (var fs = new FileStream(dg.FileName, FileMode.Create))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    foreach (var s in export)
                    {
                        sw.WriteLine($"{s.Id};{s.Rendszam};{s.Marka};{s.Tipus};{s.Forgalomban};{s.Szolgaltatas.Nev};{s.FelvetelDatuma}");
                    }
                }
            }

            MessageBox.Show("A lista exportálása sikeres volt!", "Sikeres művelet", MessageBoxButton.OK, MessageBoxImage.Information);

        }


    }

    public partial class MainWindow : Window
    {

        public int SelectedRow { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            SelectedRow = 0;
        }


        private void FillComboBoxes(ComboBox cb)
        {
            foreach (var item in DbSzerviz.GetSzolgaltatasList())
            {
                cb.Items.Add(item);
            }
        }

        private void SetEditOrAdd(bool add=true)
        {
            if (add)
            {
                edit_label.Content = "Adatok felvitele";
                submit_data_btn.Content = "Hozzáadás";
                delete_data_btn.Visibility = Visibility.Hidden;
                back_data_btn.Visibility = Visibility.Hidden;
            }
            else
            {
                edit_label.Content = "Adatok módosítása";
                submit_data_btn.Content = "Módosítás";
                delete_data_btn.Visibility = Visibility.Visible;
                back_data_btn.Visibility = Visibility.Visible;
            }
        }

        private void ClearForm()
        {
            rendszam_input.Clear();
            marka_input.Clear();
            tipus_input.Clear();
            szolg_input.SelectedIndex = 0;
            forgalomban_input.SelectedDate = DateTime.Now;
        }


        #region EVENT HANDLERS

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DbSzerviz.GetAllDataToGrid(dg_szerviz);
            FillComboBoxes(szolg_input);
            FillComboBoxes(export_szolg_input);
            SetEditOrAdd(true);
        }


        #region SearchMethods
        private void Search_submit_btn_Click(object sender, RoutedEventArgs e)
        {
            DbSzerviz.SearchService(dg_szerviz, search_input.Text);
        }

        private void Search_reset_btn_Click(object sender, RoutedEventArgs e)
        {
            DbSzerviz.GetAllDataToGrid(dg_szerviz);
            search_input.Clear();
        }


        #endregion


        #region DbMethods

        private void dg_szerviz_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SetEditOrAdd(false);

            var s = (Szerviz)dg_szerviz.SelectedItem;

            if (s != null)
            {
                SelectedRow = s.Id;

                rendszam_input.Text = s.Rendszam;
                marka_input.Text = s.Marka;
                tipus_input.Text = s.Tipus;
                szolg_input.Text = s.Szolgaltatas.Nev;
                forgalomban_input.SelectedDate = s.Forgalomban;
            }

        }

        private void Submit_data_btn_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(rendszam_input.Text) ||
                string.IsNullOrWhiteSpace(marka_input.Text) ||
                string.IsNullOrWhiteSpace(tipus_input.Text) ||
                forgalomban_input.SelectedDate is null ||
                szolg_input.SelectedItem is null)
            {
                MessageBox.Show("Minden adat megadása kötelező!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                if (SelectedRow > 0)
                {
                    // Update Service
                    var res = MessageBox.Show("Biztosan módosítani szeretnéd a kijelölt tételt?", "Módosítás", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                    {
                        DbSzerviz.UpdateService(SelectedRow,
                       rendszam_input.Text,
                       marka_input.Text,
                       tipus_input.Text,
                       forgalomban_input.SelectedDate.Value,
                       DbSzerviz.GetSzolgaltatasId(szolg_input.Text));
                    }
                    else
                    {
                        return;
                    }
                   
                }
                else
                {
                    // Add New Service
                    DbSzerviz.CreatService(
                        rendszam_input.Text,
                        marka_input.Text,
                        tipus_input.Text,
                        forgalomban_input.SelectedDate.Value,
                        DbSzerviz.GetSzolgaltatasId(szolg_input.Text));
                }

                DbSzerviz.GetAllDataToGrid(dg_szerviz);
                ClearForm();
                SetEditOrAdd();
            }

        }

        private void Delete_data_btn_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Biztosan törölni szeretnéd a kijelölt tételt?","Törlés",MessageBoxButton.YesNo,MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                DbSzerviz.DeleteService(SelectedRow);
            }
            else
            {
                return;
            }

            DbSzerviz.GetAllDataToGrid(dg_szerviz);
            ClearForm();
            SetEditOrAdd();
        }

        private void Back_data_btn_Click(object sender, RoutedEventArgs e)
        {
            SetEditOrAdd();
            SelectedRow = 0;
            ClearForm();
        }


        #endregion


        #region ExportMethods

        private void Export_radio_all_Click(object sender, RoutedEventArgs e)
        {
            export_szolg_input.IsEnabled = false;
        }

        private void Export_radio_szolg_Click(object sender, RoutedEventArgs e)
        {
            export_szolg_input.IsEnabled = true;
        }

        private void export_btn_Click(object sender, RoutedEventArgs e)
        {

                if (export_radio_all.IsChecked == true)
                {
                    DbSzerviz.ExportAllData();
                }
                else
                {
                    DbSzerviz.ExportSelectedData(export_szolg_input.Text);
                }

               
          
        }

        #endregion

        #endregion

    }
}
