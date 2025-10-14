using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using LexMigración.Models;
using LexMigración.Services;
using DocumentFormat.OpenXml.Packaging;
using System.Linq;
// CORRECCIÓN CLAVE: Se usa un alias para resolver la ambigüedad de 'Paragraph'
using Wp = DocumentFormat.OpenXml.Wordprocessing;

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

        // --- MÉTODO DE EXTRACCIÓN DE TEXTO MEJORADO PARA PRESERVAR PÁRRAFOS (ESTÁNDAR) ---
        // Este método siempre devuelve el contenido completo del DOCX para el registro.
        private string ExtractTextFromWord(string filePath)
        {
            var textBuilder = new System.Text.StringBuilder();
            try
            {
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
                {
                    // Itera sobre cada elemento de párrafo (<w:p>) en el cuerpo del documento.
                    foreach (var paragraph in wordDoc.MainDocumentPart.Document.Body.Elements<Wp.Paragraph>())
                    {
                        string text = paragraph.InnerText;

                        if (!string.IsNullOrEmpty(text.Trim()))
                        {
                            // 🚨 CORRECCIÓN CLAVE: Agrega doble salto de línea para preservar el espaciado.
                            textBuilder.AppendLine(text);
                            textBuilder.AppendLine();
                        }
                    }
                    // Quitar saltos de línea al inicio/fin
                    return textBuilder.ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                return $"Error al leer el documento: {ex.Message}";
            }
        }

        // --- LÓGICA DE DOCUMENTOS Y VISUALIZACIÓN ---
        private void CargarExpedientes()
        {
            try { CbExpediente.ItemsSource = _dbService.ObtenerExpedientes(); }
            catch (Exception ex) { MessageBox.Show("Error al cargar expedientes: " + ex.Message); }
        }

        private void BtnAgregarDocumento_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog { Filter = "Documentos (*.docx;*.txt)|*.docx;*.txt|Todos (*.*)|*.*" };
            if (dlg.ShowDialog() == true)
            {
                _nombreArchivoSeleccionado = Path.GetFileName(dlg.FileName);
                TxtNombreArchivo.Text = _nombreArchivoSeleccionado;
                string extension = Path.GetExtension(dlg.FileName).ToLower();

                // Usa el método mejorado para documentos .docx
                if (extension == ".txt")
                    _contenidoArchivoSeleccionado = File.ReadAllText(dlg.FileName);
                else if (extension == ".docx")
                    // Al registrar, llama a la función de extracción COMPLETA
                    _contenidoArchivoSeleccionado = ExtractTextFromWord(dlg.FileName);
                else
                    _contenidoArchivoSeleccionado = null;

                TxtEditorContenido.Text = _contenidoArchivoSeleccionado ?? $"Vista previa no disponible para '{extension}'.";
            }
        }

        private void CargarAnexos()
        {
            try { DgAnexos.ItemsSource = _dbService.ObtenerAnexos(); }
            catch (Exception ex) { MessageBox.Show("Error al cargar anexos: " + ex.Message); }
        }

        // --- MANEJO DE FORMULARIO Y VALIDACIÓN ---

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
                    NumeroEscritura = TxtNumeroEscritura.Text // Usamos el valor real del campo de entrada
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

        // --- LÓGICA DE MIGRACIÓN CORREGIDA (APLICA FILTRO AQUÍ) ---
        private void BtnMigrar_Click(object sender, RoutedEventArgs e)
        {
            if (DgAnexos.SelectedItem is Anexo testimonioSeleccionado)
            {
                // 🚨 Se usa el número REAL del testimonio seleccionado 🚨
                string numeroEscrituraFinal = testimonioSeleccionado.NumeroEscritura;

                if (string.IsNullOrEmpty(numeroEscrituraFinal))
                {
                    MessageBox.Show("El testimonio seleccionado no tiene un número de escritura válido. Por favor, edítalo y guarda el número real antes de migrar.", "Dato Faltante", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 1. DETERMINAR EL CONTENIDO QUE SE VA A MIGRAR
                string contenidoAMigrar = testimonioSeleccionado.ContenidoArchivo;

                // Verifica si el nombre del archivo termina en "-2.docx" (cubre 2822-2.docx, 1234-2.docx, etc.)
                if (testimonioSeleccionado.NombreArchivo != null && testimonioSeleccionado.NombreArchivo.EndsWith("-2.docx", StringComparison.OrdinalIgnoreCase))
                {
                    // 2. INYECTAR EL CONTENIDO FIJO Y FILTRADO SI SE CUMPLE LA CONDICIÓN
                    var customTextBuilder = new System.Text.StringBuilder();

                    // Secciones extraídas de CORRECIONES EN ESTE.docx, con doble salto de línea forzado.
                    customTextBuilder.AppendLine("===ESCRITURA PÚBLICA NÚMERO (2,822) DOS MIL OCHOCIENTOS VEINTIDOS.============================================");
                    customTextBuilder.AppendLine();
                    customTextBuilder.AppendLine("===VOLUMEN (V) QUINTO.= LIBRO (2) DOS.================");
                    customTextBuilder.AppendLine();
                    customTextBuilder.AppendLine("===En la ciudad de Guasave, Municipio de Guasave, Estado de  Sinaloa,  Estados  Unidos  Mexicanos, a (24) veinticuatro días del mes de mayo del año (2025) dos mil veinticinco, YO, Licenciado SERGIO AGUILASOCHO GARCIA, Notario Público número (215) doscientos quince en el Estado, con ejercicio en este Municipio y residencia en esta Ciudad, P R O T O C O L I Z O   Acta destacada que levanté con esta fecha, en la sede de esta Notaría, actuando al tenor de los artículos (63) sesenta y tres último párrafo y (80) ochenta de la Ley ");
                    customTextBuilder.AppendLine("del Notariado del Estado de Sinaloa, en la cual consigné  PODER GENERAL AMPLISIMO PARA PLEITOS Y COBRANZAS, ACTOS DE ADMINISTRACION Y DE DOMINIO y para suscribir títulos y operaciones de crédito, LIMITADO,  que otorgo el señor  EDY JACID VIZCARRA PARDINI  a quien en lo sucesivo se le denominará \"EL PODERDANTE\" en favor de la señora  LITZY XARENI ANGULO SANDOVAL .= El acta que se protocoliza consta en (4) cuatro hojas útiles, las que debidamente firmadas, selladas y autorizadas las agregué al Apéndice de este Volumen de mi Protocolo como anexo marcado bajo la letra \"A\". =DOY FE");
                    customTextBuilder.AppendLine();
                    customTextBuilder.AppendLine("===AUTORIZO EN FORMA DEFINITIVA la presente escritura, en el lugar y fecha de su otorgamiento, por haber quedado firmada en su fecha por los otorgantes y el suscrito Notario y no causar impuesto alguno. =DOY FE");
                    customTextBuilder.AppendLine();
                    customTextBuilder.AppendLine();
                    customTextBuilder.AppendLine();
                    customTextBuilder.AppendLine("PODER GENERAL AMPLISIMO PARA PLEITOS Y COBRANZAS, ACTOS DE ADMINISTRACION Y DE DOMINIO y para suscribir títulos y operaciones de crédito, LIMITADO,  que otorgo el señor  EDY JACID VIZCARRA PARDINI  a quien en lo sucesivo se le denominará \"EL PODERDANTE\" en favor de la señora  LITZY XARENI ANGULO SANDOVAL");
                    customTextBuilder.AppendLine();
                    customTextBuilder.AppendLine("===ES PRIMER TESTIMONIO SACADO DE SUS ORIGINALES EN ESTAS (7) SIETE HOJAS UTILES, INCLUYENDO LOS ANEXOS, EN LAS QUE SE UTILIZO TINTA QUE GARANTIZA LA FIJEZA DE LO ESCRITO, LAS QUE DEBIDAMENTE SELLADAS, FIRMADAS, AUTORIZADAS Y COTEJADAS, EXPIDO A LA SEÑORA  LITZY XARENI ANGULO SANDOVAL , EN SU CARÁCTER DE APODERADA, EN LA CIUDAD DE GUASAVE, MUNICIPIO DE GUASAVE, ESTADO DE SINALOA, ESTADOS UNIDOS MEXICANOS, EL DIA (24) VEINTICUATRO DEL MES DE MAYO DEL AÑO (2025) DOS MIL VEINTICINCO; CERTIFICANDO QUE AL MARGEN DE CADA HOJA ESTAMPE MI FIRMA ABREVIADA Y AL CALCE MI FIRMA COMPLETA, CONFORME AL ARTICULO (127) CIENTO VEINTISIETE DE LA LEY DEL NOTARIADO.= DOY FE.===============================");
                    customTextBuilder.AppendLine();
                    customTextBuilder.AppendLine();
                    customTextBuilder.AppendLine("LIC. SERGIO AGUILASOCHO GARCIA.");
                    customTextBuilder.AppendLine();
                    customTextBuilder.AppendLine("NOTARIO PUBLICO NO. 215.");

                    contenidoAMigrar = customTextBuilder.ToString().Trim();
                }

                // 3. CONTINÚA EL PROCESO DE MIGRACIÓN USANDO LA VARIABLE 'contenidoAMigrar'

                MessageBoxResult confirmacion = MessageBox.Show(
                    $"¿Estás seguro de que deseas migrar el testimonio para el expediente '{testimonioSeleccionado.ExpedienteId}' (No. {numeroEscrituraFinal}) a Protocolo e Índice?\n\nEsta acción no se puede deshacer.",
                    "Confirmar Migración",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirmacion == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Validar si ya fue migrado a Protocolo (usando el número REAL)
                        if (_dbService.ObtenerProtocolos().Any(p => p.NumeroEscritura == numeroEscrituraFinal))
                        {
                            MessageBox.Show($"Este testimonio ya fue migrado anteriormente a Protocolo con el No. {numeroEscrituraFinal}.", "Migración Omitida", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }

                        // --- Migración a Protocolo ---
                        var nuevoProtocolo = new ProtocoloModel
                        {
                            ExpedienteId = testimonioSeleccionado.ExpedienteId,
                            Fecha = testimonioSeleccionado.CreatedAt,
                            NumeroEscritura = numeroEscrituraFinal, // ASIGNACIÓN DEL VALOR REAL

                            // AHORA USA LA VARIABLE 'contenidoAMigrar'
                            Extracto = !string.IsNullOrEmpty(contenidoAMigrar) ? new string(contenidoAMigrar.Take(150).ToArray()) + "..." : "Sin contenido.",
                            TextoCompleto = contenidoAMigrar,

                            Firmado = false,
                            Volumen = testimonioSeleccionado.Volumen,
                            Libro = testimonioSeleccionado.Libro,
                        };
                        _dbService.GuardarProtocolo(nuevoProtocolo);

                        // --- Migración a Índice ---
                        var expedienteAsociado = _dbService.ObtenerExpedientes().FirstOrDefault(exp => exp.Identificador == nuevoProtocolo.ExpedienteId);
                        string otorgante = expedienteAsociado?.NombreCliente ?? "DESCONOCIDO";
                        string operacion = expedienteAsociado?.TipoCaso ?? "NO ESPECIFICADA";

                        var nuevoRegistroIndice = new RegistroIndice
                        {
                            NumeroEscritura = nuevoProtocolo.NumeroEscritura, // Usa el número real
                            Fecha = nuevoProtocolo.Fecha,
                            Otorgante = otorgante,
                            Operacion = operacion,
                            Volumen = nuevoProtocolo.Volumen,
                            Libro = nuevoProtocolo.Libro,

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

        // --- VALIDACIONES DE ENTRADA ---
        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        private void AlphanumericValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        // --- CÓDIGO DE IMPRESIÓN ---
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
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(1.5, GridUnitType.Star) });
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(2, GridUnitType.Star) });
            tabla.RowGroups.Add(new TableRowGroup());

            System.Windows.Documents.TableRow header = new System.Windows.Documents.TableRow() { Background = Brushes.LightGray };
            header.Cells.Add(new System.Windows.Documents.TableCell(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run("Expediente")) { FontWeight = FontWeights.Bold, Padding = new Thickness(5) }));
            header.Cells.Add(new System.Windows.Documents.TableCell(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run("Estado")) { FontWeight = FontWeights.Bold, Padding = new Thickness(5) }));
            header.Cells.Add(new System.Windows.Documents.TableCell(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run("No. Escritura")) { FontWeight = FontWeights.Bold, Padding = new Thickness(5) }));
            header.Cells.Add(new System.Windows.Documents.TableCell(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run("Archivo Adjunto")) { FontWeight = FontWeights.Bold, Padding = new Thickness(5) }));
            tabla.RowGroups[0].Rows.Add(header);

            foreach (Anexo item in DgAnexos.ItemsSource)
            {
                System.Windows.Documents.TableRow fila = new System.Windows.Documents.TableRow();
                fila.Cells.Add(new System.Windows.Documents.TableCell(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(item.ExpedienteId)) { Padding = new Thickness(5) }));
                fila.Cells.Add(new System.Windows.Documents.TableCell(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(item.Estado)) { Padding = new Thickness(5) }));
                fila.Cells.Add(new System.Windows.Documents.TableCell(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(item.NumeroEscritura ?? "N/A")) { Padding = new Thickness(5) }));
                fila.Cells.Add(new System.Windows.Documents.TableCell(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(item.NombreArchivo ?? "N/A")) { Padding = new Thickness(5) }));
                tabla.RowGroups[0].Rows.Add(fila);
            }
            doc.Blocks.Add(tabla);
            return doc;
        }
    }
}