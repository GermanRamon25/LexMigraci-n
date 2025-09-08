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

namespace LexMigración
{
    public partial class Protocolo : Window
    {
        public Protocolo()
        {
            InitializeComponent();
        }

        private void DgProtocolos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Aquí puedes manejar la selección, por ejemplo cargar detalles en el formulario a la derecha
            if (DgProtocolos.SelectedItem == null)
                return;

            dynamic protocolo = DgProtocolos.SelectedItem;
            TxtExtracto.Text = protocolo.Extracto;
            TxtTextoCompleto.Text = protocolo.TextoCompleto;
            ChkFirmado.IsChecked = protocolo.Firmado;
        }

        private void BtnMigrarAnexo_Click(object sender, RoutedEventArgs e)
        {
            // Aquí colocarás la lógica para migrar datos de Anexo a Protocolo
            MessageBox.Show("Migración Anexo → Protocolo ejecutada.");
        }
    }
}