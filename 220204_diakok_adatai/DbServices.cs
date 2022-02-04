using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace _220204_diakok_adatai
{
    class DbServices
    {
        public static List<string> GetKepzesList()
        {
            var db = new DiakadatDBEntities();
            return db.Kepzes.Select(x=>x.Nev).ToList();
        }

        public static List<string> GetOsztalyList()
        {
            var db = new DiakadatDBEntities();
            return db.Tanulo.Select(x => x.Osztaly).Distinct().ToList();
        }

        public static int GetKepzesId(string kepzes)
        {
            var db = new DiakadatDBEntities();
            return db.Kepzes.FirstOrDefault(x => x.Nev == kepzes).Id;
        }

        public static void FillDataGrid(DataGrid dg)
        {
            var db = new DiakadatDBEntities();
            dg.ItemsSource =  db.Tanulo.Include("Kepzes").ToList();
        }

        public static void SearchByStundetId(DataGrid dg, string tanuloId)
        {
            var db = new DiakadatDBEntities();

            var res = db.Tanulo.Include("Kepzes")
                .Where(x=>x.TanuloId.Contains(tanuloId))
                .ToList();
            if(res.Count == 0)
            {
                MessageBox.Show($"Nincs találat a(z) {tanuloId} azonosítóra!","Nincs találat",MessageBoxButton.OK,MessageBoxImage.Exclamation);
                return;
            }
            dg.ItemsSource = res;
        }

        public static void SearchByKepzes(DataGrid dg, string kepzes)
        {
            var db = new DiakadatDBEntities();
            var res = db.Tanulo.Include("Kepzes")
                .Where(x => x.Kepzes.Nev.ToLower().Contains(kepzes.ToLower()))
                .ToList();
            if (res.Count == 0)
            {
                MessageBox.Show($"Nincs találat a(z) {kepzes} képzésre!", "Nincs találat", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            dg.ItemsSource = res;
        }

        public static void CreateTanulo(string tanuloId, string nev, DateTime szuletes, string osztaly, int kepzesId,float atlag)
        {
            var db = new DiakadatDBEntities();

            var ujTanulo = new Tanulo()
            {
                TanuloId = tanuloId,
                Nev = nev,
                Szuletes = szuletes,
                Osztaly = osztaly,
                Fk_Kepzes_Id = kepzesId,
                Atlag = atlag
            };

            db.Tanulo.Add(ujTanulo);
            db.SaveChanges();
            MessageBox.Show("A tanuló hozzáadása sikeres volt!", "Sikeres adatfelvitel", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void UpdateTanulo(int selectedId, string tanuloId, string nev, DateTime szuletes, string osztaly, int kepzesId, float atlag)
        {
            var db = new DiakadatDBEntities();

            var edit = db.Tanulo.FirstOrDefault(x => x.Id == selectedId);

            edit.TanuloId = tanuloId;
            edit.Nev = nev;
            edit.Szuletes = szuletes;
            edit.Osztaly = osztaly;
            edit.Fk_Kepzes_Id= kepzesId;
            edit.Atlag = atlag;

            db.SaveChanges();
            MessageBox.Show("A tanuló módosítása sikeres volt!", "Sikeres adatmódosítás", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void DeleteTanulo(int selectedId)
        {
            var db = new DiakadatDBEntities();
            var selected = db.Tanulo.FirstOrDefault(x => x.Id == selectedId);
            db.Tanulo.Remove(selected);
            db.SaveChanges();
            MessageBox.Show("A tanuló törlése sikeres volt!", "Sikeres törlés", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ExportAllDiak()
        {
            var db = new DiakadatDBEntities();
            var export = db.Tanulo.Include("Kepzes").ToList();

            if(export.Count == 0)
            {
                MessageBox.Show("Az adatbázis nem tartalmaz adatokat!", "Hiba! Sikertelen exportálás", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dg = new SaveFileDialog();
            dg.Filter = "CSV fileok (.csv) | .csv";
            dg.ShowDialog();

            if (dg.FileName == null) return;

            using (var fs= new FileStream(dg.FileName,FileMode.Create))
            {
                using (var sw = new StreamWriter(fs,Encoding.UTF8))
                {
                    foreach (var e in export)
                    {
                        sw.WriteLine($"{e.Nev};{e.Kepzes.Nev}");
                    }
                }
            }
           MessageBox.Show("A tanulók exportálása sikeres volt!", "Sikeres exportálás", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ExportAsOsztaly(string osztaly)
        {
            var db = new DiakadatDBEntities();
            var export = db.Tanulo.Include("Kepzes")
                .Where(x=>x.Osztaly == osztaly)
                .ToList();

            if (export.Count == 0)
            {
                MessageBox.Show($"Az adatbázis nem tartalmaz '{osztaly}' adatokat!", "Hiba! Sikertelen exportálás", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dg = new SaveFileDialog();
            dg.Filter = "CSV fileok (.csv) | .csv";
            dg.ShowDialog();

            if (dg.FileName == null) return;

            using (var fs = new FileStream(dg.FileName, FileMode.Create))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    foreach (var e in export)
                    {
                        sw.WriteLine($"{e.Osztaly};{e.Nev};{e.Kepzes.Nev}");
                    }
                }
            }
            MessageBox.Show("A tanulók exportálása sikeres volt!", "Sikeres exportálás", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ExportAllKepzes()
        {
            var db = new DiakadatDBEntities();
            var export = db.Kepzes.ToList();

            if (export.Count == 0)
            {
                MessageBox.Show($"Az adatbázis nem tartalmaz adatokat!", "Hiba! Sikertelen exportálás", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dg = new SaveFileDialog();
            dg.Filter = "CSV fileok (.csv) | .csv";
            dg.ShowDialog();

            if (dg.FileName == null) return;

            using (var fs = new FileStream(dg.FileName, FileMode.Create))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    foreach (var e in export)
                    {
                        sw.WriteLine($"{e.Nev}");
                    }
                }
            }
            MessageBox.Show("A képzések exportálása sikeres volt!", "Sikeres exportálás", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
