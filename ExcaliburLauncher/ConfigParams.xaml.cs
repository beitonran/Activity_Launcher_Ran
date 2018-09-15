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

namespace ExcaliburLauncher
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            lstCards.Items.Add("1394PCI Series");
            lstCards.Items.Add("1533PCI/MCH");
            lstCards.Items.Add("1553PCMCIA");
            lstCards.Items.Add("1553PCMCIA/EP");
            lstCards.Items.Add("1553PCMCIA/PX");
            lstCards.Items.Add("1553PCMCIA/Px or /EP");
            lstCards.Items.Add("3910PCI");
            lstCards.Items.Add("4000PCI Series");
            lstCards.Items.Add("429PCMCIA");
            lstCards.Items.Add("664PCI(e)");
            lstCards.Items.Add("DAS-429UNET/RTx");
            lstCards.Items.Add("ES-1553UNET/Px");
            lstCards.Items.Add("EthernetPCIe");
            lstCards.Items.Add("EXC-1553UNET/Px");
            lstCards.Items.Add("Express Card");


        }
    }
}
