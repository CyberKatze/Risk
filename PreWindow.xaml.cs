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
using System.Windows.Shapes;
using Risklib;

namespace Risk
{
    /// <summary>
    /// Interaction logic for PreWindow.xaml
    /// </summary>
    public partial class PreWindow : Window
    {
        Game preGame = new Game();
          PlayerUI pu = new PlayerUI();
        public PreWindow()
        {
            InitializeComponent();
            playername.Text = PlayerUI.defultname;
            imagelist.ItemsSource = pu.AvatarsImg;
            colorslist.ItemsSource = pu.Colors;
            
           
            

        }
       
        private void playername_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox).Text = "";
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            if(PlayerUI.row<7)
            { 
            Player p = new Player();
            p.Name = playername.Text;
            p.Color = colorslist.SelectedValue.ToString(); 
            p.Avatar = (imagelist.SelectedValue as BitmapImage);
            pu.Colors.Remove(p.Color);
            pu.AvatarsImg.Remove(p.Avatar);
            imagelist.ItemsSource = pu.AvatarsImg;
            colorslist.ItemsSource = pu.Colors;
                preGame.Players.Add(p);
            PlayerUI.row++;
            colorslist.SelectedItem = colorslist.Items[0];
            imagelist.SelectedItem = imagelist.Items[0];
            playername.Text = PlayerUI.defultname;
            it.Items.Add(p);
           }
        }

       

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow(preGame);
            mw.Show();
            this.Close();
        }
    }

    public class PlayerUI
        {

        public static int  row { get; set; } = 1;
        public static string defultname {
            get { return row + "بازیکن"; }
                
        } 
       
            private List<BitmapImage> avatarsimg = new List<BitmapImage>();
        private List<string> colors = new List<string>();
         public PlayerUI()
        {
            
           
            GetColors();

           for(int i=1; i<=7; i++)
            {
                avatarsimg.Add(new BitmapImage(new Uri(@"avatars/a"+i+".png", UriKind.Relative)));
            }
        }
          
        public List<BitmapImage> AvatarsImg
        {
            get
            {
                return avatarsimg;
            }
            set
            {
                avatarsimg = value;
            }
            
        }
        public List<string> Colors
        {


            get
            {

                return colors;
            }
            set
            {
                colors = value;
            }
        }

      
        private void GetColors()
        {

            colors.Add("Red");
            colors.Add("Blue");
            colors.Add("GreenYellow");
            colors.Add("Orange");
            colors.Add("BlueViolet");
            colors.Add("Aqua");
            colors.Add("Violet");
           
        }
    }

   

}
