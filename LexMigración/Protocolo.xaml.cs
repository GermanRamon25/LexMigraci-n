using LexMigración.Models;
using LexMigración.Services;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Media;

namespace LexMigración
{
    public partial class Protocolo : Window
    {
        // ... (código existente sin cambios)
        private readonly DatabaseService _dbService;

        public Protocolo()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            CargarProtocolos();
        }

        private void CargarProtocolos()
        {
            try
            {
                var protocolos = _dbService.ObtenerProtocolos();
                DgProtocolos.ItemsSource = protocolos;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error al cargar los protocolos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Archivo: LexMigración/Protocolo.xaml.cs

        private void DgProtocolos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgProtocolos.SelectedItem is ProtocoloModel protocolo)
            {
                // 🚨 Se elimina TxtExtracto.Text = protocolo.Extracto;
                TxtTextoCompleto.Text = protocolo.TextoCompleto;
                ChkFirmado.IsChecked = protocolo.Firmado;

                // Si tienes campos Volumen/Libro/Folio en el XAML, aquí se cargarían.
            }
            else
            {
                // 🚨 Se elimina TxtExtracto.Clear();
                TxtTextoCompleto.Clear();
                ChkFirmado.IsChecked = false;

                // Si tienes campos Volumen/Libro/Folio en el XAML, aquí se limpiarían.
            }
        }

        private void BtnGuardarProtocolo_Click(object sender, RoutedEventArgs e)
        {
            if (DgProtocolos.SelectedItem is ProtocoloModel protocoloSeleccionado)
            {
                // 🚨 Se elimina la asignación de Extracto al modelo
                // protocoloSeleccionado.Extracto = TxtExtracto.Text; 

                protocoloSeleccionado.TextoCompleto = TxtTextoCompleto.Text;
                protocoloSeleccionado.Firmado = ChkFirmado.IsChecked ?? false;

                // Si tienes código para guardar Volumen/Libro/Folio aquí, asegúrate de que se mantenga.

                try
                {
                    _dbService.ActualizarProtocolo(protocoloSeleccionado);
                    MessageBox.Show("Protocolo actualizado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    CargarProtocolos();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error al guardar los cambios: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un protocolo de la lista para guardar.", "Ningún Protocolo Seleccionado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnEliminarProtocolo_Click(object sender, RoutedEventArgs e)
        {
            if (DgProtocolos.SelectedItem is ProtocoloModel protocoloSeleccionado)
            {
                MessageBoxResult resultado = MessageBox.Show(
                    $"¿Estás seguro de que deseas eliminar el protocolo con la escritura '{protocoloSeleccionado.NumeroEscritura}'?",
                    "Confirmar Eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.Yes)
                {
                    try
                    {
                        _dbService.EliminarProtocolo(protocoloSeleccionado);
                        MessageBox.Show("El protocolo ha sido eliminado exitosamente.", "Eliminado", MessageBoxButton.OK, MessageBoxImage.Information);
                        CargarProtocolos();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Ocurrió un error al eliminar el protocolo: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un protocolo de la tabla para eliminar.", "Ningún Protocolo Seleccionado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}