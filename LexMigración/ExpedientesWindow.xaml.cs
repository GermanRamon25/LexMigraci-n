using LexMigración.Models;
using LexMigración.Services;
using System;
using System.Linq; // <-- Asegúrate de que este 'using' esté presente
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace LexMigración
{
    public partial class ExpedientesWindow : Window
    {
        private readonly DatabaseService _dbService;

        public ExpedientesWindow()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            CargarExpedientes();
        }

        // --- (El resto de tus métodos como CargarExpedientes, Guardar, Eliminar, etc., no cambian) ---

        private void CargarExpedientes()
        {
            try { DgExpedientes.ItemsSource = _dbService.ObtenerExpedientes(); }
            catch (Exception ex) { MessageBox.Show("Error al cargar expedientes: " + ex.Message); }
        }

        private void LimpiarFormulario()
        {
            DgExpedientes.SelectedItem = null;
            TxtIdentificador.Clear();
            TxtNombreCliente.Clear();
            TxtTipoCaso.Clear();
            TxtIdentificador.IsEnabled = true;
        }

        private void DgExpedientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgExpedientes.SelectedItem is Expediente expediente)
            {
                TxtIdentificador.Text = expediente.Identificador;
                TxtNombreCliente.Text = expediente.NombreCliente;
                TxtTipoCaso.Text = expediente.TipoCaso;
                TxtIdentificador.IsEnabled = false;
            }
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtIdentificador.Text) || string.IsNullOrWhiteSpace(TxtNombreCliente.Text))
            {
                MessageBox.Show("El Identificador y el Nombre del Cliente son obligatorios.", "Datos Faltantes");
                return;
            }
            try
            {
                if (DgExpedientes.SelectedItem is Expediente expedienteSeleccionado)
                {
                    expedienteSeleccionado.NombreCliente = TxtNombreCliente.Text;
                    expedienteSeleccionado.TipoCaso = TxtTipoCaso.Text;
                    _dbService.ActualizarExpediente(expedienteSeleccionado);
                    MessageBox.Show("Expediente actualizado.");
                }
                else
                {
                    var nuevoExpediente = new Expediente
                    {
                        Identificador = TxtIdentificador.Text.Trim(),
                        NombreCliente = TxtNombreCliente.Text.Trim(),
                        TipoCaso = TxtTipoCaso.Text.Trim(),
                        FechaCreacion = DateTime.Now
                    };
                    _dbService.GuardarExpediente(nuevoExpediente);
                    MessageBox.Show("Expediente creado.");
                }
                CargarExpedientes();
                LimpiarFormulario();
            }
            catch (Exception ex) { MessageBox.Show("Error al guardar: " + ex.Message); }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (DgExpedientes.SelectedItem is Expediente exp)
            {
                if (MessageBox.Show($"¿Seguro que deseas eliminar '{exp.Identificador}'?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _dbService.EliminarExpediente(exp);
                        MessageBox.Show("Expediente eliminado.");
                        CargarExpedientes();
                        LimpiarFormulario();
                    }
                    catch (Exception ex) { MessageBox.Show("Error al eliminar: " + ex.Message); }
                }
            }
        }

        // --- MÉTODO DE IMPRESIÓN ACTUALIZADO ---
        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            if (DgExpedientes.SelectedItem is Expediente expedienteSeleccionado)
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    // Ahora creamos el reporte completo
                    FlowDocument doc = CrearReporteCompleto(expedienteSeleccionado);

                    printDialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, $"Reporte del Expediente - {expedienteSeleccionado.Identificador}");
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un expediente de la lista para imprimir.", "Ningún Expediente Seleccionado");
            }
        }

        // --- *** LÓGICA MEJORADA PARA CREAR EL REPORTE COMPLETO *** ---
        private FlowDocument CrearReporteCompleto(Expediente expediente)
        {
            FlowDocument doc = new FlowDocument();
            doc.PagePadding = new Thickness(50);
            doc.ColumnWidth = 800;
            doc.FontFamily = new FontFamily("Arial");

            // --- 1. TÍTULO Y DATOS DEL EXPEDIENTE ---
            doc.Blocks.Add(new Paragraph(new Run("Reporte Completo de Expediente")) { FontSize = 24, FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center });
            doc.Blocks.Add(new Paragraph(new Run($"Identificador: {expediente.Identificador}\nCliente: {expediente.NombreCliente}\nTipo de Caso: {expediente.TipoCaso}\nFecha de Creación: {expediente.FechaCreacion:dd/MM/yyyy}")) { FontSize = 12, Margin = new Thickness(0, 20, 0, 20) });

            // --- 2. SECCIÓN DE ANEXOS ---
            var anexos = _dbService.ObtenerAnexos().Where(a => a.ExpedienteId == expediente.Identificador).ToList();
            doc.Blocks.Add(new Paragraph(new Run("Anexos Asociados")) { FontSize = 18, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 15, 0, 5) });
            if (anexos.Any())
            {
                foreach (var anexo in anexos)
                {
                    doc.Blocks.Add(new Paragraph(new Run($" •  Archivo: {anexo.NombreArchivo ?? "N/A"}, Estado: {anexo.Estado}, Volumen: {anexo.Volumen}, Libro: {anexo.Libro}, Folio: {anexo.NumeroEscritura}")) { Margin = new Thickness(20, 0, 0, 5) });
                }
            }
            else
            {
                doc.Blocks.Add(new Paragraph(new Run("  No se encontraron anexos para este expediente.")));
            }

            // --- 3. SECCIÓN DE PROTOCOLOS ---
            var protocolos = _dbService.ObtenerProtocolos().Where(p => p.ExpedienteId == expediente.Identificador).ToList();
            doc.Blocks.Add(new Paragraph(new Run("Protocolos Generados")) { FontSize = 18, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 15, 0, 5) });
            if (protocolos.Any())
            {
                foreach (var proto in protocolos)
                {
                    doc.Blocks.Add(new Paragraph(new Run($" •  No. Escritura: {proto.NumeroEscritura}, Fecha: {proto.Fecha:dd/MM/yyyy}, Firmado: {(proto.Firmado ? "Sí" : "No")}")) { Margin = new Thickness(20, 0, 0, 5) });
                }
            }
            else
            {
                doc.Blocks.Add(new Paragraph(new Run("  No se encontraron protocolos para este expediente.")));
            }

            // --- 4. SECCIÓN DE ÍNDICE ---
            var indices = _dbService.ObtenerRegistrosIndice().Where(i => protocolos.Any(p => p.NumeroEscritura == i.NumeroEscritura)).ToList();
            doc.Blocks.Add(new Paragraph(new Run("Registros en Índice")) { FontSize = 18, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 15, 0, 5) });
            if (indices.Any())
            {
                foreach (var indice in indices)
                {
                    doc.Blocks.Add(new Paragraph(new Run($" •  No. Escritura: {indice.NumeroEscritura}, Operación: {indice.Operacion}, Otorgante: {indice.Otorgante}")) { Margin = new Thickness(20, 0, 0, 5) });
                }
            }
            else
            {
                doc.Blocks.Add(new Paragraph(new Run("  No se encontraron registros en el índice para este expediente.")));
            }

            return doc;
        }
    }
}