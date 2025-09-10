using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using LexMigración.Models;
using LexMigración.Services;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;//

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
            CargarAnexos();
        }

        private void CargarAnexos()
        {
            try
            {
                DgAnexos.ItemsSource = _dbService.ObtenerAnexos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los anexos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void BtnAgregarDocumento_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Selecciona un documento";
            dlg.Filter = "Archivos de Texto (*.txt)|*.txt|Documento Word (*.docx)|*.docx|Todos los archivos (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    _nombreArchivoSeleccionado = Path.GetFileName(dlg.FileName);
                    string extension = Path.GetExtension(dlg.FileName).ToLower();

                    if (extension == ".txt")
                    {
                        _contenidoArchivoSeleccionado = File.ReadAllText(dlg.FileName);
                    }
                    else if (extension == ".docx")
                    {
                        _contenidoArchivoSeleccionado = ExtraerTextoDocx(dlg.FileName);
                    }
                    else
                    {
                        MessageBox.Show("Solo se permiten archivos de texto (.txt) o Word (.docx).", "Tipo de Archivo No Soportado",
                                        MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    TxtNombreArchivo.Text = _nombreArchivoSeleccionado;
                    TxtEditorContenido.Text = _contenidoArchivoSeleccionado;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo leer el archivo: " + ex.Message, "Error al cargar archivo",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string ExtraerTextoDocx(string filePath)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
            {
                Body body = wordDoc.MainDocumentPart.Document.Body;
                return body?.InnerText ?? "";
            }
        }

        private void DgAnexos_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DgAnexos.SelectedItem is Anexo anexo)
            {
                CbExpediente.Text = anexo.ExpedienteId;
                CbEstado.Text = anexo.Estado;
                TxtNombreArchivo.Text = anexo.NombreArchivo ?? "Ningún archivo asociado.";
                TxtEditorContenido.Text = anexo.ContenidoArchivo;
            }
            else
            {
                TxtEditorContenido.Clear();
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var nuevoAnexo = new Anexo
                {
                    ExpedienteId = CbExpediente.Text ?? "EXP-NUEVO",
                    Estado = (CbEstado.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString() ?? "Listo",
                    NombreArchivo = _nombreArchivoSeleccionado,
                    ContenidoArchivo = _contenidoArchivoSeleccionado,
                    CreatedAt = DateTime.Today
                };
                _dbService.GuardarAnexo(nuevoAnexo);
                MessageBox.Show("Anexo guardado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                CargarAnexos();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el anexo: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("Contenido actualizado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    CargarAnexos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar el contenido: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un anexo de la tabla para actualizar su contenido.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
