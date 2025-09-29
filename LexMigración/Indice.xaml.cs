using LexMigración.Models;
using LexMigración.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
// --- *** AÑADIR ESTOS 'USING' PARA LA IMPRESIÓN *** ---
using System.Windows.Documents;

namespace LexMigración
{
    public partial class Indice : Window
    {
        private DatabaseService _dbService;
        private ObservableCollection<RegistroIndice> _registrosEnMemoria;

        public Indice()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            InicializarDatos();
        }

        private void InicializarDatos()
        {
            var registrosDb = _dbService.ObtenerRegistrosIndice();
            _registrosEnMemoria = new ObservableCollection<RegistroIndice>(registrosDb);
            DgIndice.ItemsSource = _registrosEnMemoria;
        }

        private void Filtros_Changed(object sender, RoutedEventArgs e)
        {
            var filtroOtorgante = TxtFiltroOtorgante.Text?.ToLower() ?? "";
            var filtroOperacion = TxtFiltroOperacion.Text?.ToLower() ?? "";
            var filtroNumero = TxtFiltroNumero.Text?.ToLower() ?? "";
            var filtroFecha = DpFiltroFecha.SelectedDate;

            var registrosFiltrados = _registrosEnMemoria.Where(r =>
                (string.IsNullOrEmpty(filtroOtorgante) || r.Otorgante.ToLower().Contains(filtroOtorgante)) &&
                (string.IsNullOrEmpty(filtroOperacion) || r.Operacion.ToLower().Contains(filtroOperacion)) &&
                (string.IsNullOrEmpty(filtroNumero) || r.NumeroEscritura.ToLower().Contains(filtroNumero)) &&
                (!filtroFecha.HasValue || r.Fecha.Date == filtroFecha.Value.Date)
            ).ToList();

            DgIndice.ItemsSource = new ObservableCollection<RegistroIndice>(registrosFiltrados);
        }

        private void DgIndice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgIndice.SelectedItem is RegistroIndice registro)
            {
                TxtMensaje.Text = $"Seleccionado: Escritura {registro.NumeroEscritura}";
            }
        }

        private void BtnGuardarCambios_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var registro in _registrosEnMemoria)
                {
                    _dbService.GuardarRegistroIndice(registro);
                }
                TxtMensaje.Text = "Cambios guardados exitosamente";
                TxtMensaje.Foreground = Brushes.Green;
            }
            catch (Exception ex)
            {
                TxtMensaje.Text = $"Error al guardar: {ex.Message}";
                TxtMensaje.Foreground = Brushes.Red;
            }
        }

        private void BtnAgregarFila_Click(object sender, RoutedEventArgs e)
        {
            var nuevoRegistro = new RegistroIndice
            {
                Otorgante = "Nuevo Otorgante",
                Operacion = "",
                NumeroEscritura = "",
                Volumen = "",
                Libro = "",
                Fecha = DateTime.Today,
                Folio = ""
            };
            _registrosEnMemoria.Add(nuevoRegistro);
            DgIndice.SelectedItem = nuevoRegistro;
            DgIndice.ScrollIntoView(nuevoRegistro);
        }

        private void BtnEliminarFila_Click(object sender, RoutedEventArgs e)
        {
            if (DgIndice.SelectedItem is RegistroIndice registroSeleccionado)
            {
                MessageBoxResult resultado = MessageBox.Show(
                    $"¿Estás seguro de que deseas eliminar el registro del índice para la escritura '{registroSeleccionado.NumeroEscritura}'?",
                    "Confirmar Eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.Yes)
                {
                    try
                    {
                        _dbService.EliminarRegistroIndice(registroSeleccionado);
                        _registrosEnMemoria.Remove(registroSeleccionado);

                        TxtMensaje.Text = "Registro eliminado exitosamente.";
                        TxtMensaje.Foreground = Brushes.Green;
                    }
                    catch (Exception ex)
                    {
                        TxtMensaje.Text = $"Error al eliminar: {ex.Message}";
                        TxtMensaje.Foreground = Brushes.Red;
                    }
                }
            }
            else
            {
                TxtMensaje.Text = "Selecciona una fila para eliminar.";
                TxtMensaje.Foreground = Brushes.Red;
            }
        }

        // --- *** MÉTODOS NUEVOS PARA IMPRIMIR LA LISTA *** ---
        private void BtnImprimirLista_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                // Importante: El documento se creará a partir de la vista actual de la tabla,
                // respetando los filtros que hayas aplicado.
                FlowDocument doc = CrearDocumentoIndice();
                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Listado del Índice");
            }
        }

        private FlowDocument CrearDocumentoIndice()
        {
            FlowDocument doc = new FlowDocument();
            doc.PagePadding = new Thickness(50);
            doc.ColumnWidth = 800;
            doc.Blocks.Add(new Paragraph(new Run("Índice General de Escrituras")) { FontSize = 20, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            Table tabla = new Table() { CellSpacing = 0, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(2, GridUnitType.Star) }); // Otorgante
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(1.5, GridUnitType.Star) }); // Operación
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) }); // No. Escritura
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(100) }); // Fecha
            tabla.RowGroups.Add(new TableRowGroup());

            TableRow header = new TableRow() { Background = Brushes.LightGray };
            header.Cells.Add(new TableCell(new Paragraph(new Run("Otorgante")) { FontWeight = FontWeights.Bold, Padding = new Thickness(5) }));
            header.Cells.Add(new TableCell(new Paragraph(new Run("Operación")) { FontWeight = FontWeights.Bold, Padding = new Thickness(5) }));
            header.Cells.Add(new TableCell(new Paragraph(new Run("No. Escritura")) { FontWeight = FontWeights.Bold, Padding = new Thickness(5) }));
            header.Cells.Add(new TableCell(new Paragraph(new Run("Fecha")) { FontWeight = FontWeights.Bold, Padding = new Thickness(5) }));
            tabla.RowGroups[0].Rows.Add(header);

            foreach (RegistroIndice item in DgIndice.ItemsSource)
            {
                TableRow fila = new TableRow();
                fila.Cells.Add(new TableCell(new Paragraph(new Run(item.Otorgante)) { Padding = new Thickness(5) }));
                fila.Cells.Add(new TableCell(new Paragraph(new Run(item.Operacion)) { Padding = new Thickness(5) }));
                fila.Cells.Add(new TableCell(new Paragraph(new Run(item.NumeroEscritura)) { Padding = new Thickness(5) }));
                fila.Cells.Add(new TableCell(new Paragraph(new Run(item.Fecha.ToString("dd/MM/yyyy"))) { Padding = new Thickness(5) }));
                tabla.RowGroups[0].Rows.Add(fila);
            }
            doc.Blocks.Add(tabla);
            return doc;
        }
    }

    public class RegistroIndice : INotifyPropertyChanged
    {
        public int Id { get; set; }
        private string _otorgante;
        private string _operacion;
        private string _numeroEscritura;
        private string _volumen;
        private string _libro;
        private DateTime _fecha;
        private string _folio;

        public string Otorgante
        {
            get => _otorgante;
            set { _otorgante = value; OnPropertyChanged(); }
        }
        public string Operacion
        {
            get => _operacion;
            set { _operacion = value; OnPropertyChanged(); }
        }
        public string NumeroEscritura
        {
            get => _numeroEscritura;
            set { _numeroEscritura = value; OnPropertyChanged(); }
        }
        public string Volumen
        {
            get => _volumen;
            set { _volumen = value; OnPropertyChanged(); }
        }
        public string Libro
        {
            get => _libro;
            set { _libro = value; OnPropertyChanged(); }
        }
        public DateTime Fecha
        {
            get => _fecha;
            set { _fecha = value; OnPropertyChanged(); }
        }
        public string Folio
        {
            get => _folio;
            set { _folio = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}