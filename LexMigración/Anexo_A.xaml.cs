using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using LexMigración.Models;
using LexMigración.Services;
// --- LIBRERÍAS NUEVAS PARA LEER DOCUMENTOS DE WORD ---
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

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

        // --- MÉTODO NUEVO PARA EXTRAER TEXTO DE WORD (.docx) ---
        private string ExtractTextFromWord(string filePath)
        {
            try
            {
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
                {
                    var body = wordDoc.MainDocumentPart.Document.Body;
                    return body.InnerText;
                }
            }
            catch (Exception ex)
            {
                return $"Error al leer el documento de Word: {ex.Message}";
            }
        }

        // --- LÓGICA ACTUALIZADA PARA MANEJAR .txt y .docx ---
        private void BtnAgregarDocumento_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Selecciona un documento para adjuntar";
            dlg.Filter = "Documentos (*.docx;*.txt)|*.docx;*.txt|Todos los archivos (*.*)|*.*";

            if (dlg.ShowDialog() == true)
            {
                _nombreArchivoSeleccionado = Path.GetFileName(dlg.FileName);
                TxtNombreArchivo.Text = _nombreArchivoSeleccionado;
                string extension = Path.GetExtension(dlg.FileName).ToLower();

                // Decidir cómo leer el archivo basado en su extensión
                if (extension == ".txt")
                {
                    _contenidoArchivoSeleccionado = File.ReadAllText(dlg.FileName);
                }
                else if (extension == ".docx")
                {
                    _contenidoArchivoSeleccionado = ExtractTextFromWord(dlg.FileName);
                }
                else
                {
                    _contenidoArchivoSeleccionado = null;
                    TxtEditorContenido.Text = $"Vista previa no disponible para archivos '{extension}'.";
                    return;
                }

                TxtEditorContenido.Text = _contenidoArchivoSeleccionado;
            }
        }

        // --- El resto del código no necesita cambios drásticos ---

        private void CargarExpedientes()
        {
            try { CbExpediente.ItemsSource = _dbService.ObtenerExpedientes(); }
            catch (Exception ex) { MessageBox.Show("Error al cargar expedientes: " + ex.Message); }
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
        }

        private void DgAnexos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgAnexos.SelectedItem is Anexo anexo)
            {
                CbExpediente.SelectedValue = anexo.ExpedienteId;
                CbEstado.Text = anexo.Estado;
                TxtNombreArchivo.Text = anexo.NombreArchivo ?? "Ningún archivo asociado.";
                if (!string.IsNullOrEmpty(anexo.ContenidoArchivo))
                {
                    TxtEditorContenido.Text = anexo.ContenidoArchivo;
                }
                else if (!string.IsNullOrEmpty(anexo.NombreArchivo))
                {
                    TxtEditorContenido.Text = $"Vista previa no disponible para el archivo '{anexo.NombreArchivo}'.";
                }
                else
                {
                    TxtEditorContenido.Clear();
                }
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
                    CreatedAt = DateTime.Today
                };
                _dbService.GuardarAnexo(nuevoAnexo);
                MessageBox.Show("Anexo guardado exitosamente.", "Éxito");
                CargarAnexos();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el anexo: " + ex.Message, "Error");
            }
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
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar el contenido: " + ex.Message, "Error");
                }
            }
            else
            {
                MessageBox.Show("Selecciona un anexo de la tabla para actualizar.", "Aviso");
            }
        }
    }
}