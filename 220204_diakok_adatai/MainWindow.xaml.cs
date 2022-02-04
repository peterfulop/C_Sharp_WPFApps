using System;
using System.Collections.Generic;
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

namespace _220204_diakok_adatai
{
    public partial class MainWindow : Window
    {
        public int SelectedId { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FillComboBox(ComboBox cb, List<string> l)
        {
            cb.Items.Clear();
            foreach (var k in l)
            {
                cb.Items.Add(k.ToString());
            }
            cb.SelectedIndex = 0;
        }

        private void SetEditOrAdd(bool add=true)
        {
            if (add)
            {
                add_or_update_label.Content = "Adatok felvitele";
                form_save_btn.Content = "Hozzáadás";
                form_delete_btn.Visibility = Visibility.Hidden;
                form_back_btn.Visibility=Visibility.Hidden;
            }
            else
            {
                add_or_update_label.Content = "Adatok szerkesztése";
                form_save_btn.Content = "Módosítás";
                form_delete_btn.Visibility = Visibility.Visible;
                form_back_btn.Visibility=Visibility.Visible;
            }
        }
        private void ClearForm()
        {
            form_input_id.Clear();
            form_input_nev.Clear();
            form_input_szuletes.SelectedDate = DateTime.Now;
            form_input_osztaly.Clear();
            form_input_atlag.Clear();
        }

        private void UpdateScreen()
        {
            DbServices.FillDataGrid(dg_diakok);
            ClearForm();
            SetEditOrAdd();
            FillComboBox(export_combo_osztaly, DbServices.GetOsztalyList());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillComboBox(export_combo_osztaly,DbServices.GetOsztalyList());
            FillComboBox(form_input_kepzes,DbServices.GetKepzesList());
            SetEditOrAdd();
            DbServices.FillDataGrid(dg_diakok);
        }

        private void search_btn_submit_Click(object sender, RoutedEventArgs e)
        {
            if(search_radio_kepzes.IsChecked == true)
            {
                DbServices.SearchByKepzes(dg_diakok, search_input.Text);
            }
            else
            {
                DbServices.SearchByStundetId(dg_diakok, search_input.Text);
            }

        }

        private void search_btn_reset_Click(object sender, RoutedEventArgs e)
        {
            DbServices.FillDataGrid(dg_diakok);
            search_input.Clear();
            search_radio_kepzes.IsChecked = true;
        }

        private void dg_diakok_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            var s = (Tanulo)dg_diakok.SelectedItem;

            if(s != null)
            {
                SetEditOrAdd(false);

                SelectedId = s.Id;
                form_input_id.Text = s.TanuloId;
                form_input_nev.Text = s.Nev;
                form_input_szuletes.SelectedDate = s.Szuletes;
                form_input_osztaly.Text = s.Osztaly;
                form_input_kepzes.Text = s.Kepzes.Nev;
                form_input_atlag.Text = s.Atlag.ToString("0.0");
            }
        }

        private void form_save_btn_Click(object sender, RoutedEventArgs e)
        {
            var tanuloId = form_input_id.Text.Trim();
            var nev = form_input_nev.Text.Trim();
            var szuletes = form_input_szuletes;
            var osztaly = form_input_osztaly.Text.Trim();
            var kepzes = form_input_kepzes.Text.Trim();
            var atlag = form_input_atlag.Text.Trim().Replace('.', ',');


            if (string.IsNullOrWhiteSpace(tanuloId) ||
                string.IsNullOrWhiteSpace(nev) ||
                string.IsNullOrWhiteSpace(osztaly) ||
                string.IsNullOrWhiteSpace(kepzes) ||
                string.IsNullOrWhiteSpace(atlag) ||
                szuletes.SelectedDate is null)
            {
                MessageBox.Show("Minden mező kitöltése kötelező!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!float.TryParse(atlag, out float x) || float.Parse(atlag) < 1 || float.Parse(atlag) > 5)
            {
                MessageBox.Show("Az átlag mező értéke csak 1 és 5 közé eső szám lehet!", "Hiba a formátumban!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if(tanuloId.Length <10 || tanuloId.Length > 10)
            {
                MessageBox.Show("A tanuló azonosítója egy 10 karaterből álló számsor!", "Hiba a formátumban!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedId > 0)
            {
                var res = MessageBox.Show("Biztosan módosítani szeretnéd az adatokat?", "Megerősítési kérelem!", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(res == MessageBoxResult.No)
                {
                    return;
                }

                DbServices.UpdateTanulo(SelectedId,
                    tanuloId,
                    nev,
                    szuletes.SelectedDate.Value,
                    osztaly,
                    DbServices.GetKepzesId(kepzes),
                    float.Parse(atlag));
            }
            else
            {
                DbServices.CreateTanulo(tanuloId,
                    nev,
                    szuletes.SelectedDate.Value,
                    osztaly,
                    DbServices.GetKepzesId(kepzes),
                    float.Parse(atlag));
            }

            UpdateScreen();
        }

        private void form_delete_btn_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Biztosan törölni szeretnéd a tételt?", "Megerősítési kérelem!", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.No)
            {
                return;
            }
            DbServices.DeleteTanulo(SelectedId);
            UpdateScreen();
        }

        private void form_back_btn_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            SetEditOrAdd();
            SelectedId = 0;
        }

        private void export_radio_osztaly_Click(object sender, RoutedEventArgs e)
        {
            export_combo_osztaly.IsEnabled = true;
        }

        private void export_radio_diak_Click(object sender, RoutedEventArgs e)
        {
            export_combo_osztaly.IsEnabled = false;
        }

        private void export_radio_kepzes_Click(object sender, RoutedEventArgs e)
        {
            export_combo_osztaly.IsEnabled = false;
        }

        private void export_btn_submit_Click(object sender, RoutedEventArgs e)
        {
            if(export_radio_diak.IsChecked == true)
            {
                DbServices.ExportAllDiak();
            }else if(export_radio_kepzes.IsChecked == true)
            {
                DbServices.ExportAllKepzes();
            }
            else if(export_radio_osztaly.IsChecked == true && export_combo_osztaly.Text.Length > 0)
            {
                DbServices.ExportAsOsztaly(export_combo_osztaly.Text.ToString());
            }
        }


    }
}
