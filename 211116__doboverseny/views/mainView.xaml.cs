using System;
using System.Collections.Generic;
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
using _211116__doboverseny.methods;

namespace _211116__doboverseny.views
{
    /// <summary>
    /// Interaction logic for mainView.xaml
    /// </summary>
    public partial class mainView : Page
    {

        private int activeUserId { get; set; }

        public mainView()
        {
            InitializeComponent();
            DbMethods.getAndFillGrid(dg_versenyzok);
            var versenyszamok = DbMethods.getVersenyszamok();
            setInputs();

        }


        private void setInputs()
        {
            inp_nem.ItemsSource = new List<string>() { "ferfi", "no" };

            DbMethods.getVersenyszamok().ForEach(x =>
            {
                inp_verseny.Items.Add(x.nev);
                export_options.Items.Add(x.nev);
            });
        }

        private void resetInputs()
        {
            update_btn.Visibility = Visibility.Hidden;
            delete_btn.Visibility = Visibility.Hidden;
            back_btn.Visibility = Visibility.Hidden;
            add_btn.Visibility = Visibility.Visible;

            inp_name.Clear();
            inp_kor.Clear();
            inp_verseny.SelectedIndex = 0;
            inp_nem.SelectedIndex = 0;
        }



        private void search_btn_Click(object sender, RoutedEventArgs e)
        {
            var rajtszam = radio_rajtszam.IsChecked.Value;
            var kereses = inp_search.Text;

            if (!string.IsNullOrWhiteSpace(kereses))
            {
                DbMethods.searchAndFillGrid(dg_versenyzok, rajtszam, kereses);
            }
            else
            {
                MessageBox.Show("Nem adtál meg adatot!");
                DbMethods.getAndFillGrid(dg_versenyzok);
            }

        }

        private void export_btn_Click(object sender, RoutedEventArgs e)
        {
            DbMethods.exportData(radio_versenyzo.IsChecked.Value, export_options.SelectedItem.ToString());

        }

        private void refresh_btn_Click(object sender, RoutedEventArgs e)
        {
            DbMethods.getAndFillGrid(dg_versenyzok);
        }

        private void dg_versenyzok_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = dg_versenyzok.SelectedItem;

            update_btn.Visibility = Visibility.Visible;
            delete_btn.Visibility = Visibility.Visible;
            back_btn.Visibility = Visibility.Visible;
            add_btn.Visibility = Visibility.Hidden;

            if (!dg_versenyzok.SelectedIndex.Equals(-1))
            {
                using (var db = new DoboszamokEntities())
                {
                    versenyzok selection = (versenyzok)row;

                    inp_name.Text = selection.nev;
                    inp_date.SelectedDate = selection.szuletes;
                    inp_verseny.SelectedItem = selection.versenyszamok.nev;
                    inp_nem.SelectedItem = selection.nem;
                    inp_kor.Text = selection.korosztaly;
                    activeUserId = selection.rajtszam;
                }
            }


        }

        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            var nev = inp_name.Text;
            var szuletes = inp_date.SelectedDate;
            var nem = inp_nem.SelectedItem.ToString();
            var versenyszamId = DbMethods.getVersenyszamId(inp_verseny.SelectedItem.ToString());
            var korosztaly = inp_kor.Text;

            if (string.IsNullOrWhiteSpace(nev) || string.IsNullOrWhiteSpace(nem) || string.IsNullOrWhiteSpace(korosztaly) || versenyszamId == 0 || inp_date.SelectedDate is null)
            {
                MessageBox.Show("Minden adat kitöltése kötelező!");
                return;
            }

            DbMethods.addNewPerson(nev, szuletes.Value, nem, versenyszamId, korosztaly);
            MessageBox.Show("A bevitel sikeres volt!");
            DbMethods.getAndFillGrid(dg_versenyzok);
            resetInputs();

        }

        private void update_btn_Click(object sender, RoutedEventArgs e)
        {
            var nev = inp_name.Text;
            var szuletes = inp_date.SelectedDate;
            var nem = inp_nem.SelectedItem.ToString();
            var versenyszamId = DbMethods.getVersenyszamId(inp_verseny.SelectedItem.ToString());
            var korosztaly = inp_kor.Text;

            if (string.IsNullOrWhiteSpace(nev) || string.IsNullOrWhiteSpace(nem) || string.IsNullOrWhiteSpace(korosztaly) || versenyszamId == 0 || inp_date.SelectedDate is null)
            {
                MessageBox.Show("Minden adat kitöltése kötelező!");
                return;
            }

            DbMethods.editPerson(activeUserId, nev, szuletes.Value, nem, versenyszamId, korosztaly);
            MessageBox.Show("A módosítás sikeres volt!");
            DbMethods.getAndFillGrid(dg_versenyzok);
            resetInputs();

        }

        private void delete_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!dg_versenyzok.SelectedIndex.Equals(-1))
            {
                DbMethods.deletePerson(activeUserId);
                MessageBox.Show("A törlés sikeres volt!");
                DbMethods.getAndFillGrid(dg_versenyzok);
                resetInputs();

            }

        }

        private void back_btn_Click(object sender, RoutedEventArgs e)
        {
            resetInputs();
        }
    }
}
