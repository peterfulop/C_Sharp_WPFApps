using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace _211116__doboverseny.methods
{
    class DbMethods
    {

        public static void getAndFillGrid(DataGrid grid)
        {
            using (var db = new DoboszamokEntities())
            {
                grid.ItemsSource = db.versenyzok.Include("versenyszamok").ToList();
                grid.Items.Refresh();
            }
        }

        public static void searchAndFillGrid(DataGrid grid, bool rajtszam, string keres)
        {
            using (var db = new DoboszamokEntities())
            {

                if (rajtszam && int.TryParse(keres, out int szam))
                {
                    var filtered = db.versenyzok.Include("versenyszamok").Where(x => x.rajtszam == szam).ToList();
                    grid.ItemsSource = filtered;
                    grid.Items.Refresh();
                }
                else if(!rajtszam && !int.TryParse(keres, out int b))
                {
                     var filtered = db.versenyzok.Include("versenyszamok").Where(x => x.nev.Contains(keres)).ToList();
                     grid.ItemsSource = filtered;
                     grid.Items.Refresh();
                }
                else
                {
                    MessageBox.Show("Hibás bementi paraméterek!");
                }
            }
        }

        public static void addNewPerson(string nev, DateTime szuletes, string nem, int versenyszamId, string korosztaly)
        {
            using (var db = new DoboszamokEntities())
            {
                var uj = new versenyzok
                {
                    nev = nev,
                    szuletes = szuletes,
                    fk_versenyszam_id = versenyszamId,
                    korosztaly = korosztaly,
                    nem = nem
                };
                db.versenyzok.Add(uj);
                db.SaveChanges();
            }
        }

        public static void editPerson(int rajtszam, string nev, DateTime szuletes, string nem, int versenyszamId, string korosztaly)
        {
            using (var db = new DoboszamokEntities())
            {
                var user = db.versenyzok.FirstOrDefault(x => x.rajtszam == rajtszam);
                user.nev = nev;
                user.szuletes = szuletes;
                user.nem = nem;
                user.fk_versenyszam_id = versenyszamId;
                user.korosztaly = korosztaly;
                db.SaveChanges();
            }
        }

        public static void deletePerson(int rajtszam)
        {
            using (var db = new DoboszamokEntities())
            {
                var delete = db.versenyzok.Where(x => x.rajtszam == rajtszam);
                db.versenyzok.RemoveRange(delete);
                db.SaveChanges();
            }
        }

        public static void exportData(bool versenyzok, string versenyszam)
        {
            using (var db = new DoboszamokEntities())
            {


                if (versenyzok)
                {
                    Console.WriteLine("Versenyzok export");

                    var export = db.versenyzok.Include("versenyszamok").ToList();

                    
                    using (var fs = new FileStream($"1_export.csv", FileMode.CreateNew))
                    {
                        using (var sw = new StreamWriter(fs,Encoding.UTF8))
                        {
                            foreach (var item in export)
                            {
                                sw.WriteLine($"{item.rajtszam};{item.nev};{item.szuletes};{item.nem};{item.korosztaly}");
                            }
                        }
                    }

                }
                else
                {
                    Console.WriteLine("Versenyszamok export");

                    var export = db.versenyzok.Include("versenyszamok").Where(x=>x.versenyszamok.nev == versenyszam).ToList();

                    using (var fs = new FileStream($"2_export.csv", FileMode.CreateNew))
                    {
                        using (var sw = new StreamWriter(fs, Encoding.UTF8))
                        {
                            foreach (var item in export)
                            {
                                sw.WriteLine($"{item.versenyszamok.nev};{item.nev}");
                            }
                        }
                    }
                }

                MessageBox.Show("Exportálás sikeres!");

            }
        }



        public static List<versenyszamok> getVersenyszamok()
        {
            using (var db = new DoboszamokEntities())
            {
                return db.versenyszamok.ToList();
            }
        }

        public static int getVersenyszamId(string versenyszam)
        {
            using (var db = new DoboszamokEntities())
            {
                return Convert.ToInt32(db.versenyszamok.FirstOrDefault(x=>x.nev == versenyszam).Id.ToString());
            }
        }
    }
}
