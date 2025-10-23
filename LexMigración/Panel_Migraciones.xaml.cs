using LexMigración.Models;
using LexMigración.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;


namespace LexMigración
{
    public partial class Panel_Migraciones : Window
    {
        private readonly DatabaseService _dbService;

        public Panel_Migraciones()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
        }

        private void Log(string message, bool isError = false)
        {
            var item = new System.Windows.Controls.ListBoxItem
            {
                Content = $"[{DateTime.Now:HH:mm:ss}] {message}",
                Foreground = isError ? Brushes.Red : Brushes.Black
            };
            LstLogMigraciones.Items.Insert(0, item);
        }

       
        private void BtnMigrarTestimonioProtocolo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LstLogMigraciones.Items.Clear();
                Log("Iniciando migración de Testimonio a Protocolo...");

                var testimonios = _dbService.ObtenerTestimonios();
                var protocolosExistentes = _dbService.ObtenerProtocolos();
                int migracionesExitosas = 0;
                int migracionesOmitidas = 0;

                foreach (TestimonioModel testimonio in testimonios)
                {
                    
                    string numeroEscrituraFinal = string.IsNullOrEmpty(testimonio.NumeroEscritura) ?
                        $"FALLBACK-{testimonio.Id}" : testimonio.NumeroEscritura;

                    if (protocolosExistentes.Any(p => p.NumeroEscritura == numeroEscrituraFinal))
                    {
                        Log($"⚠️  Se omitió Testimonio ID:{testimonio.Id} ('{testimonio.NombreArchivo}') (ya existe como escritura: {numeroEscrituraFinal}).");
                        migracionesOmitidas++;
                        continue;
                    }

                    var nuevoProtocolo = new ProtocoloModel
                    {
                        ExpedienteId = testimonio.ExpedienteId,
                        Fecha = testimonio.CreatedAt,
                        
                        NumeroEscritura = numeroEscrituraFinal,
                        Extracto = !string.IsNullOrEmpty(testimonio.ContenidoArchivo) ? new string(testimonio.ContenidoArchivo.Take(150).ToArray()) + "..." : "Sin contenido.",
                        TextoCompleto = testimonio.ContenidoArchivo,
                        Firmado = false,
                        Volumen = testimonio.Volumen,
                        Libro = testimonio.Libro,
                        
                    };
                    _dbService.GuardarProtocolo(nuevoProtocolo);
                    Log($"✔️  Testimonio ID:{testimonio.Id} ('{testimonio.NombreArchivo}') migrado exitosamente con No. {numeroEscrituraFinal}.");
                    migracionesExitosas++;
                }

                Log($"--- Migración finalizada. {migracionesExitosas} protocolos creados, {migracionesOmitidas} omitidos. ---");
                MessageBox.Show("Proceso finalizado. Revisa el log para detalles.", "Proceso Terminado");
            }
            catch (Exception ex)
            {
                Log($"❌ ERROR FATAL: {ex.Message}", true);
                MessageBox.Show("Ocurrió un error. Revisa el log.", "Error de Migración");
            }
        }

        
        private void BtnMigrarProtocoloIndice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LstLogMigraciones.Items.Clear();
                Log("Iniciando purga de datos del Índice y reconstrucción...");

                var protocolos = _dbService.ObtenerProtocolos().ToList();
                var indicesExistentes = _dbService.ObtenerRegistrosIndice().ToList();
                var expedientes = _dbService.ObtenerExpedientes();

                int protocolosEliminados = 0;
                int indicesEliminados = 0;
                int migracionesExitosas = 0;

                
                Log("1. Limpiando protocolos temporales de la fuente...");
                foreach (var protocolo in protocolos.Where(p => p.NumeroEscritura != null && p.NumeroEscritura.StartsWith("TEMP-ANEXO-")).ToList())
                {
                    _dbService.EliminarProtocolo(protocolo);
                    protocolosEliminados++;
                }
                Log($"🗑️ {protocolosEliminados} protocolos temporales eliminados del origen.");

                
                Log("2. Eliminando TODOS los registros de Índice para reconstrucción...");
                foreach (var indice in indicesExistentes)
                {
                    _dbService.EliminarRegistroIndice(indice);
                    indicesEliminados++;
                }
                Log($"🗑️ {indicesEliminados} registros de Índice eliminados.");

                
                indicesExistentes = _dbService.ObtenerRegistrosIndice();
                protocolos = _dbService.ObtenerProtocolos().ToList();

               
                Log("3. Migrando Protocolos limpios al Índice...");
                foreach (var protocolo in protocolos)
                {
                    string numeroEscrituraFinal = protocolo.NumeroEscritura;

                    if (indicesExistentes.Any(i => i.NumeroEscritura == numeroEscrituraFinal))
                    {
                        continue;
                    }

                    var expedienteAsociado = expedientes.FirstOrDefault(exp => exp.Identificador == protocolo.ExpedienteId);
                    string otorgante = expedienteAsociado?.NombreCliente ?? "DESCONOCIDO";
                    string operacion = expedienteAsociado?.TipoCaso ?? "NO ESPECIFICADA";

                    var nuevoRegistro = new RegistroIndice
                    {
                        NumeroEscritura = numeroEscrituraFinal,
                        Fecha = protocolo.Fecha,
                        Otorgante = otorgante,
                        Operacion = operacion,
                        Volumen = protocolo.Volumen,
                        Libro = protocolo.Libro
                    };

                    _dbService.GuardarRegistroIndice(nuevoRegistro);
                    Log($"✔️  Protocolo '{numeroEscrituraFinal}' migrado al Índice exitosamente.");
                    migracionesExitosas++;
                }

                Log($"--- Proceso finalizado. {migracionesExitosas} registros migrados/creados. ---");
                MessageBox.Show("Proceso finalizado. Revisa el log para detalles.", "Proceso Terminado");
            }
            catch (Exception ex)
            {
                Log($"❌ ERROR FATAL: {ex.Message}", true);
                MessageBox.Show("Ocurrió un error. Revisa el log.", "Error de Migración");
            }
        }
    }
}