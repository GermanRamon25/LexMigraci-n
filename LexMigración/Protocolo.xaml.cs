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

        private void DgProtocolos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgProtocolos.SelectedItem is ProtocoloModel protocolo)
            {
                TxtExtracto.Text = protocolo.Extracto;
                TxtTextoCompleto.Text = protocolo.TextoCompleto;
                ChkFirmado.IsChecked = protocolo.Firmado;
            }
            else
            {
                TxtExtracto.Clear();
                TxtTextoCompleto.Clear();
                ChkFirmado.IsChecked = false;
            }
        }

        private void BtnGuardarProtocolo_Click(object sender, RoutedEventArgs e)
        {
            if (DgProtocolos.SelectedItem is ProtocoloModel protocoloSeleccionado)
            {
                protocoloSeleccionado.Extracto = TxtExtracto.Text;
                protocoloSeleccionado.TextoCompleto = TxtTextoCompleto.Text;
                protocoloSeleccionado.Firmado = ChkFirmado.IsChecked ?? false;

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

        // --- *** CÓDIGO DE IMPRESIÓN REVERTIDO A LA VERSIÓN ORIGINAL *** ---
        private void BtnImprimirLista_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = CrearDocumentoProtocolos();
                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Listado de Protocolos");
            }
        }

        private FlowDocument CrearDocumentoProtocolos()
        {
            FlowDocument doc = new FlowDocument();
            doc.PagePadding = new Thickness(50);
            doc.Blocks.Add(new Paragraph(new Run("Listado General de Protocolos")) { FontSize = 20, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            Table tabla = new Table() { CellSpacing = 0, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(2, GridUnitType.Star) });
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });
            tabla.RowGroups.Add(new TableRowGroup());

            TableRow header = new TableRow() { Background = Brushes.LightGray };
            header.Cells.Add(new TableCell(new Paragraph(new Run("No. Escritura")) { FontWeight = FontWeights.Bold, Padding = new Thickness(5) }));
            header.Cells.Add(new TableCell(new Paragraph(new Run("Fecha")) { FontWeight = FontWeights.Bold, Padding = new Thickness(5) }));
            header.Cells.Add(new TableCell(new Paragraph(new Run("Firmado")) { FontWeight = FontWeights.Bold, Padding = new Thickness(5) }));
            tabla.RowGroups[0].Rows.Add(header);

            foreach (ProtocoloModel item in DgProtocolos.ItemsSource)
            {
                TableRow fila = new TableRow();
                fila.Cells.Add(new TableCell(new Paragraph(new Run(item.NumeroEscritura)) { Padding = new Thickness(5) }));
                fila.Cells.Add(new TableCell(new Paragraph(new Run(item.Fecha.ToString("dd/MM/yyyy"))) { Padding = new Thickness(5) }));
                fila.Cells.Add(new TableCell(new Paragraph(new Run(item.Firmado ? "Sí" : "No")) { Padding = new Thickness(5) }));
                tabla.RowGroups[0].Rows.Add(fila);
            }
            doc.Blocks.Add(tabla);
            return doc;
        }
    }
}