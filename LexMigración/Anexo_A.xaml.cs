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

namespace LexMigración
{
    public partial class Anexo_A : Window
    {
        // ... (propiedades y constructor no cambian) ...
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

        // ... (Todos los métodos existentes como Guardar, Eliminar, Cargar, etc., se mantienen igual) ...

        // --- *** NUEVO MÉTODO PARA LA MIGRACIÓN *** ---
        private void BtnMigrar_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validar que se haya seleccionado un testimonio
            if (DgAnexos.SelectedItem is Anexo testimonioSeleccionado)
            {
                // 2. Mostrar mensaje de confirmación
                MessageBoxResult confirmacion = MessageBox.Show(
                    $"¿Estás seguro de que deseas migrar el testimonio para el expediente '{testimonioSeleccionado.ExpedienteId}' a Protocolo e Índice?\n\nEsta acción no se puede deshacer.",
                    "Confirmar Migración",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirmacion == MessageBoxResult.Yes)
                {
                    try
                    {
                        // 3. Lógica de Migración
                        // Se crea un identificador único para evitar duplicados
                        string numeroEscrituraUnico = $"TEMP-ANEXO-{testimonioSeleccionado.Id}";

                        // Validar si ya fue migrado a Protocolo
                        if (_dbService.ObtenerProtocolos().Any(p => p.NumeroEscritura == numeroEscrituraUnico))
                        {
                            MessageBox.Show("Este testimonio ya fue migrado anteriormente a Protocolo.", "Migración Omitida", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }

                        // --- Migración a Protocolo ---
                        var nuevoProtocolo = new ProtocoloModel
                        {
                            ExpedienteId = testimonioSeleccionado.ExpedienteId,
                            Fecha = testimonioSeleccionado.CreatedAt,
                            NumeroEscritura = numeroEscrituraUnico,
                            Extracto = !string.IsNullOrEmpty(testimonioSeleccionado.ContenidoArchivo) ? new string(testimonioSeleccionado.ContenidoArchivo.Take(150).ToArray()) + "..." : "Sin contenido.",
                            TextoCompleto = testimonioSeleccionado.ContenidoArchivo,
                            Firmado = false,
                            Volumen = testimonioSeleccionado.Volumen,
                            Libro = testimonioSeleccionado.Libro,
                            Folio = testimonioSeleccionado.NumeroEscritura
                        };
                        _dbService.GuardarProtocolo(nuevoProtocolo);

                        // --- Migración a Índice ---
                        var expedienteAsociado = _dbService.ObtenerExpedientes().FirstOrDefault(exp => exp.Identificador == nuevoProtocolo.ExpedienteId);
                        string otorgante = expedienteAsociado?.NombreCliente ?? "DESCONOCIDO";
                        string operacion = expedienteAsociado?.TipoCaso ?? "NO ESPECIFICADA";

                        var nuevoRegistroIndice = new RegistroIndice
                        {
                            NumeroEscritura = nuevoProtocolo.NumeroEscritura,
                            Fecha = nuevoProtocolo.Fecha,
                            Otorgante = otorgante,
                            Operacion = operacion,
                            Volumen = nuevoProtocolo.Volumen,
                            Libro = nuevoProtocolo.Libro,
                            Folio = nuevoProtocolo.Folio
                        };
                        _dbService.GuardarRegistroIndice(nuevoRegistroIndice);

                        MessageBox.Show("¡Migración completada exitosamente!\nSe ha creado una entrada en Protocolo y en Índice.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ocurrió un error durante la migración: " + ex.Message, "Error Fatal", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                // Si no hay nada seleccionado
                MessageBox.Show("Por favor, selecciona un testimonio de la lista para poder migrarlo.", "Ningún Testimonio Seleccionado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Verifica si el carácter es un número (0-9)
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true; // Ignora el carácter si no es un dígito
            }
        }

        private void AlphanumericValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Verifica si el carácter es una letra o un dígito
            if (!char.IsLetterOrDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true; // Ignora el carácter si no es alfanumérico
            }
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
    }
}