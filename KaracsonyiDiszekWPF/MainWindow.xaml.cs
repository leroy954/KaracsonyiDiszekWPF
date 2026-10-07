// MainWindow.xaml.cs

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace karacsonyGUI
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<NapiAngyalkaAdat> napiAdatok =
            new ObservableCollection<NapiAngyalkaAdat>();

        private int aktualisKeszlet = 0;
        private int osszesKeszitett = 0;
        private int osszesEladott = 0;

        private bool sotetTema = false;

        public MainWindow()
        {
            InitializeComponent();

            lstEredmenyek.ItemsSource = napiAdatok;

            NapokFeltoltese();
            StatisztikaFrissitese();
            UresListaFrissitese();
        }

        private void NapokFeltoltese()
        {
            cmbNap.Items.Clear();

            for (int i = 1; i <= 40; i++)
            {
                cmbNap.Items.Add(i);
            }

            if (cmbNap.Items.Count > 0)
            {
                cmbNap.SelectedIndex = 0;
            }
        }

        private void BtnHozzaad_Click(object sender, RoutedEventArgs e)
        {
            UzenetElrejtese();

            bool keszitettSikeres =
                int.TryParse(txtKeszitett.Text, out int keszitett);

            bool eladottSikeres =
                int.TryParse(txtEladott.Text, out int eladott);

            if (!keszitettSikeres || !eladottSikeres)
            {
                UzenetMegjelenitese("Csak egész számot adhat meg!", false);
                return;
            }

            if (keszitett < 0 || eladott < 0)
            {
                UzenetMegjelenitese("Negatív számot nem adhat meg!", false);
                return;
            }

            if (cmbNap.SelectedItem == null)
            {
                UzenetMegjelenitese("Nincs több választható nap!", false);
                return;
            }

            int nap = (int)cmbNap.SelectedItem;

            if (eladott > aktualisKeszlet + keszitett)
            {
                UzenetMegjelenitese("Túl sok az eladott angyalka!", false);
                return;
            }

            aktualisKeszlet += keszitett - eladott;
            osszesKeszitett += keszitett;
            osszesEladott += eladott;

            NapiAngyalkaAdat ujAdat = new NapiAngyalkaAdat(
                nap,
                keszitett,
                eladott,
                aktualisKeszlet
            );

            napiAdatok.Add(ujAdat);

            txtKeszitett.Text = "0";
            txtEladott.Text = "0";

            while (cmbNap.Items.Count > 0 &&
                   (int)cmbNap.Items[0] <= nap)
            {
                cmbNap.Items.RemoveAt(0);
            }

            if (cmbNap.Items.Count > 0)
            {
                cmbNap.SelectedIndex = 0;
            }
            else
            {
                cmbNap.IsEnabled = false;
                btnHozzaad.IsEnabled = false;

                UzenetMegjelenitese(
                    "A 40. nap is rögzítve lett. Nincs több választható nap.",
                    true
                );
            }

            StatisztikaFrissitese();
            UresListaFrissitese();

            lstEredmenyek.ScrollIntoView(ujAdat);
        }

        private void BtnMaiNap_Click(object sender, RoutedEventArgs e)
        {
            if (cmbNap.Items.Count == 0)
            {
                UzenetMegjelenitese(
                    "Már nincs több rögzíthető nap.",
                    false
                );
                return;
            }

            cmbNap.SelectedIndex = 0;

            UzenetMegjelenitese(
                "A következő rögzíthető nap kiválasztva: " +
                cmbNap.SelectedItem + ". nap.",
                true
            );
        }

        private void BtnMintaAdat_Click(object sender, RoutedEventArgs e)
        {
            if (cmbNap.Items.Count == 0)
            {
                UzenetMegjelenitese(
                    "Már nincs több rögzíthető nap.",
                    false
                );
                return;
            }

            int mintaKeszitett = 3;
            int maxEladhato = aktualisKeszlet + mintaKeszitett;
            int mintaEladott = maxEladhato >= 1 ? 1 : 0;

            txtKeszitett.Text = mintaKeszitett.ToString();
            txtEladott.Text = mintaEladott.ToString();

            UzenetMegjelenitese(
                "Mintaértékek kitöltve. A Hozzáadás gombbal rögzítheti őket.",
                true
            );
        }

        private void BtnOsszegzes_Click(object sender, RoutedEventArgs e)
        {
            int napokSzama = napiAdatok.Count;

            string szoveg =
                "Rögzített napok: " + napokSzama + "\n" +
                "Összes elkészített angyalka: " + osszesKeszitett + " db\n" +
                "Összes eladott angyalka: " + osszesEladott + " db\n" +
                "Jelenlegi készlet: " + aktualisKeszlet + " db";

            if (napokSzama > 0)
            {
                double atlag = (double)osszesKeszitett / napokSzama;

                szoveg += "\nÁtlagosan elkészített angyalka naponta: " +
                           Math.Round(atlag, 2) + " db";
            }

            MessageBox.Show(
                szoveg,
                "Angyalka összegzés",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void BtnTema_Click(object sender, RoutedEventArgs e)
        {
            sotetTema = !sotetTema;

            if (sotetTema)
            {
                Background = new SolidColorBrush(
                    Color.FromRgb(30, 43, 53)
                );

                UzenetMegjelenitese(
                    "Sötét téma bekapcsolva.",
                    true
                );
            }
            else
            {
                Background = new SolidColorBrush(
                    Color.FromRgb(241, 246, 250)
                );

                UzenetMegjelenitese(
                    "Világos téma bekapcsolva.",
                    true
                );
            }
        }

        private void BtnTorles_Click(object sender, RoutedEventArgs e)
        {
            if (napiAdatok.Count == 0)
            {
                UzenetMegjelenitese(
                    "Nincs törölhető adat a listában.",
                    false
                );
                return;
            }

            MessageBoxResult valasz = MessageBox.Show(
                "Biztosan törölni szeretné az összes rögzített napi adatot?",
                "Lista törlése",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (valasz == MessageBoxResult.No)
            {
                return;
            }

            napiAdatok.Clear();

            aktualisKeszlet = 0;
            osszesKeszitett = 0;
            osszesEladott = 0;

            txtKeszitett.Text = "0";
            txtEladott.Text = "0";

            cmbNap.IsEnabled = true;
            btnHozzaad.IsEnabled = true;

            NapokFeltoltese();
            StatisztikaFrissitese();
            UresListaFrissitese();

            UzenetMegjelenitese(
                "Az összes napi adat sikeresen törölve lett.",
                true
            );
        }

        private void FoScrollViewer_ScrollChanged(
            object sender,
            ScrollChangedEventArgs e)
        {
            if (foScrollViewer.ScrollableHeight <= 0)
            {
                return;
            }

            double arany =
                foScrollViewer.VerticalOffset /
                foScrollViewer.ScrollableHeight;

            if (arany < 0)
            {
                arany = 0;
            }

            if (arany > 1)
            {
                arany = 1;
            }

            // A lista tetején világoskék: RGB(143, 180, 199)
            // A lista alján sötétkék:   RGB(25, 67, 91)
            byte piros = (byte)(143 - (118 * arany));
            byte zold = (byte)(180 - (113 * arany));
            byte kek = (byte)(199 - (108 * arany));

            Color ujSzin = Color.FromRgb(
                piros,
                zold,
                kek
            );

            ScrollBar scrollbar =
                VisualisElemKeresese<ScrollBar>(foScrollViewer);

            if (scrollbar == null)
            {
                return;
            }

            Thumb thumb =
                VisualisElemKeresese<Thumb>(scrollbar);

            if (thumb == null)
            {
                return;
            }

            thumb.Background =
                new SolidColorBrush(ujSzin);
        }

        private T VisualisElemKeresese<T>(
            DependencyObject szulo
        ) where T : DependencyObject
        {
            if (szulo == null)
            {
                return null;
            }

            for (
                int i = 0;
                i < VisualTreeHelper.GetChildrenCount(szulo);
                i++
            )
            {
                DependencyObject gyerek =
                    VisualTreeHelper.GetChild(szulo, i);

                T megtalaltElem = gyerek as T;

                if (megtalaltElem != null)
                {
                    return megtalaltElem;
                }

                T eredmeny =
                    VisualisElemKeresese<T>(gyerek);

                if (eredmeny != null)
                {
                    return eredmeny;
                }
            }

            return null;
        }

        private void StatisztikaFrissitese()
        {
            txtAktualisKeszlet.Text = aktualisKeszlet + " db";
            txtOsszesKeszitett.Text = osszesKeszitett + " db";
            txtOsszesEladott.Text = osszesEladott + " db";
            txtNapokSzama.Text = "Rögzített napok: " + napiAdatok.Count;
        }

        private void UresListaFrissitese()
        {
            if (napiAdatok.Count == 0)
            {
                txtUresLista.Visibility = Visibility.Visible;
            }
            else
            {
                txtUresLista.Visibility = Visibility.Collapsed;
            }
        }

        private void UzenetMegjelenitese(
            string uzenet,
            bool sikeres
        )
        {
            txtUzenet.Text = uzenet;

            if (sikeres)
            {
                borderUzenet.Background =
                    new SolidColorBrush(
                        Color.FromRgb(231, 247, 237)
                    );

                borderUzenet.BorderBrush =
                    new SolidColorBrush(
                        Color.FromRgb(121, 185, 147)
                    );

                txtUzenet.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(22, 129, 84)
                    );
            }
            else
            {
                borderUzenet.Background =
                    new SolidColorBrush(
                        Color.FromRgb(253, 235, 235)
                    );

                borderUzenet.BorderBrush =
                    new SolidColorBrush(
                        Color.FromRgb(231, 155, 151)
                    );

                txtUzenet.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(197, 55, 49)
                    );
            }

            borderUzenet.Visibility = Visibility.Visible;
        }

        private void UzenetElrejtese()
        {
            borderUzenet.Visibility = Visibility.Collapsed;
            txtUzenet.Text = "";
        }
    }

    public class NapiAngyalkaAdat
    {
        public int Nap { get; set; }
        public int Keszitett { get; set; }
        public int Eladott { get; set; }
        public int Keszlet { get; set; }

        public string NapSzoveg
        {
            get
            {
                return Nap + ". nap";
            }
        }

        public string KeszitettSzoveg
        {
            get
            {
                return "Elkészített: +" + Keszitett;
            }
        }

        public string EladottSzoveg
        {
            get
            {
                return "Eladott: -" + Eladott;
            }
        }

        public string KeszletSzoveg
        {
            get
            {
                return "Készlet: " + Keszlet + " db";
            }
        }

        public NapiAngyalkaAdat(
            int nap,
            int keszitett,
            int eladott,
            int keszlet
        )
        {
            Nap = nap;
            Keszitett = keszitett;
            Eladott = eladott;
            Keszlet = keszlet;
        }
    }
}