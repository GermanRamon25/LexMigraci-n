using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using LexMigración.Models;
using LexMigración.Services;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Media;

namespace LexMigración
{
    public partial class Anexo_A : Window
    {
        private readonly DatabaseService _dbService;
        private string _nombreArchivoSeleccionado;
        private string _contenidoArchivoSeleccionado;

        public Anexo_A()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            CargarExpedientes();
            CargarAnexos();
        }

        // ... (Todos los métodos como CargarExpedientes, Guardar, Eliminar, etc. no cambian)
        private void CargarExpedientes()
        {
            try { CbExpediente.ItemsSource = _dbService.ObtenerExpedientes(); }
            catch (Exception ex) { MessageBox.Show("Error al cargar expedientes: " + ex.Message); }
        }

        private string ExtractTextFromWord(string filePath)
        {
            try
            {
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
                { return wordDoc.MainDocumentPart.Document.Body.InnerText; }
            }
            catch (Exception ex)
            { return $"Error al leer el documento: {ex.Message}"; }
        }

        private void BtnAgregarDocumento_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog { Filter = "Documentos (*.docx;*.txt)|*.docx;*.txt|Todos (*.*)|*.*" };
            if (dlg.ShowDialog() == true)
            {
                _nombreArchivoSeleccionado = Path.GetFileName(dlg.FileName);
                TxtNombreArchivo.Text = _nombreArchivoSeleccionado;
                string extension = Path.GetExtension(dlg.FileName).ToLower();
                if (extension == ".txt") _contenidoArchivoSeleccionado = File.ReadAllText(dlg.FileName);
                else if (extension == ".docx") _contenidoArchivoSeleccionado = ExtractTextFromWord(dlg.FileName);
                else _contenidoArchivoSeleccionado = null;
                TxtEditorContenido.Text = _contenidoArchivoSeleccionado ?? $"Vista previa no disponible para '{extension}'.";
            }
        }

        private void CargarAnexos()
        {
            try { DgAnexos.ItemsSource = _dbService.ObtenerAnexos(); }
            catch (Exception ex) { MessageBox.Show("Error al cargar anexos: " + ex.Message); }
        }

        private void LimpiarFormulario()
        {
            CbExpediente.SelectedIndex = -1;
            CbEstado.SelectedIndex = -1;
            TxtNombreArchivo.Text = "Ningún archivo seleccionado.";
            TxtEditorContenido.Clear();
            _nombreArchivoSeleccionado = null;
            _contenidoArchivoSeleccionado = null;
            DgAnexos.SelectedItem = null;
            TxtVolumen.Clear();
            TxtLibro.Clear();
            TxtNumeroEscritura.Clear();
        }

        private void DgAnexos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgAnexos.SelectedItem is Anexo anexo)
            {
                CbExpediente.SelectedValue = anexo.ExpedienteId;
                CbEstado.Text = anexo.Estado;
                TxtNombreArchivo.Text = anexo.NombreArchivo ?? "N/A";
                TxtEditorContenido.Text = anexo.ContenidoArchivo ?? string.Empty;
                TxtVolumen.Text = anexo.Volumen;
                TxtLibro.Text = anexo.Libro;
                TxtNumeroEscritura.Text = anexo.NumeroEscritura;
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (CbExpediente.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecciona un expediente asociado.", "Dato Faltante", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                var nuevoAnexo = new Anexo
                {
                    ExpedienteId = CbExpediente.SelectedValue.ToString(),
                    Estado = (CbEstado.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Listo",
                    NombreArchivo = _nombreArchivoSeleccionado,
                    ContenidoArchivo = _contenidoArchivoSeleccionado,
                    CreatedAt = DateTime.Today,
                    Volumen = TxtVolumen.Text,
                    Libro = TxtLibro.Text,
                    // 🚨 CORRECCIÓN FINAL: Usamos la nueva propiedad NumeroEscritura
                    NumeroEscritura = TxtNumeroEscritura.Text

                    // *** SE ELIMINAN TODAS LAS LÍNEAS DE FOLIO/FOLIADO INICIO/FIN/TOTALHOJAS ***
                    // Ya no son necesarias ni en el modelo ni en la base de datos.
                };
                _dbService.GuardarAnexo(nuevoAnexo);
                MessageBox.Show("Anexo guardado exitosamente.", "Éxito");
                CargarAnexos();
                LimpiarFormulario();
            }
            catch (Exception ex) { MessageBox.Show("Error al guardar el anexo: " + ex.Message, "Error"); }
        }
        private void BtnActualizarContenido_Click(object sender, RoutedEventArgs e)
        {
            if (DgAnexos.SelectedItem is Anexo anexoSeleccionado)
            {
                try
                {
                    anexoSeleccionado.ContenidoArchivo = TxtEditorContenido.Text;
                    _dbService.ActualizarAnexo(anexoSeleccionado);
                    MessageBox.Show("Contenido actualizado exitosamente.", "Éxito");
                    CargarAnexos();
                }
                catch (Exception ex) { MessageBox.Show("Error al actualizar: " + ex.Message, "Error"); }
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (DgAnexos.SelectedItem is Anexo anexoSeleccionado)
            {
                if (MessageBox.Show($"¿Seguro que deseas eliminar el anexo '{anexoSeleccionado.NombreArchivo}'?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _dbService.EliminarAnexo(anexoSeleccionado);
                        MessageBox.Show("Anexo eliminado.", "Éxito");
                        CargarAnexos();
                        LimpiarFormulario();
                    }
                    catch (Exception ex) { MessageBox.Show("Error al eliminar: " + ex.Message, "Error"); }
                }
            }
        }

        // --- *** CÓDIGO DE IMPRESIÓN REVERTIDO A LA VERSIÓN ORIGINAL *** ---
        private void BtnImprimirLista_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = CrearDocumentoAnexos();
                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Listado de Anexos");
            }
        }

        private FlowDocument CrearDocumentoAnexos()
        {
            FlowDocument doc = new FlowDocument();
            doc.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run("Listado General de Anexos")) { FontSize = 20, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            System.Windows.Documents.Table tabla = new System.Windows.Documents.Table() { CellSpacing = 0, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(1.5, GridUnitType.Star) });
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(2, GridUnitType.Star) });
            tabla.RowGroups.Add(new TableRowGroup());

            System.Windows.Documents.TableRow header = new System.Windows.Documents.TableRow() { Background = Brushes.LightGray };
            header.Cells.Add(new System.Windows.Documents.TableCell(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run("Expediente")) { FontWeight = FontWeights.Bold, Padding = new Thickness(5) }));
            header.Cells.Add(new System.Windows.Documents.TableCell(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run("Estado")) { FontWeight = FontWeights.Bold, Padding = new Thickness(5) }));
            header.Cells.Add(new System.Windows.Documents.TableCell(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run("Archivo Adjunto")) { FontWeight = FontWeights.Bold, Padding = new Thickness(5) }));
            tabla.RowGroups[0].Rows.Add(header);

            foreach (Anexo item in DgAnexos.ItemsSource)
            {
                System.Windows.Documents.TableRow fila = new System.Windows.Documents.TableRow();
                fila.Cells.Add(new System.Windows.Documents.TableCell(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(item.ExpedienteId)) { Padding = new Thickness(5) }));
                fila.Cells.Add(new System.Windows.Documents.TableCell(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(item.Estado)) { Padding = new Thickness(5) }));
                fila.Cells.Add(new System.Windows.Documents.TableCell(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(item.NombreArchivo ?? "N/A")) { Padding = new Thickness(5) }));
                tabla.RowGroups[0].Rows.Add(fila);
            }
            doc.Blocks.Add(tabla);
            return doc;
        }
    }
}