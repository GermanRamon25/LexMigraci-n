using LexMigración.Models;
using LexMigración.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

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

        // --- *** LÓGICA DE MIGRACIÓN 1 ACTUALIZADA *** ---
        private void BtnMigrarAnexoProtocolo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LstLogMigraciones.Items.Clear();
                Log("Iniciando migración de Anexo a Protocolo...");

                var anexos = _dbService.ObtenerAnexos();
                var protocolosExistentes = _dbService.ObtenerProtocolos();
                int migracionesExitosas = 0;
                int migracionesOmitidas = 0;

                foreach (var anexo in anexos)
                {
                    // Creamos un identificador único para el protocolo basado en el ID del anexo.
                    string numeroEscrituraUnico = $"TEMP-ANEXO-{anexo.Id}";

                    // CAMBIO CLAVE: Ahora buscamos si ya existe un protocolo con este ID único.
                    if (protocolosExistentes.Any(p => p.NumeroEscritura == numeroEscrituraUnico))
                    {
                        Log($"⚠️  Se omitió Anexo ID:{anexo.Id} para '{anexo.ExpedienteId}' (ya fue migrado).");
                        migracionesOmitidas++;
                        continue;
                    }

                    var nuevoProtocolo = new ProtocoloModel
                    {
                        ExpedienteId = anexo.ExpedienteId,
                        Fecha = anexo.CreatedAt,
                        // Usamos el identificador único que creamos.
                        NumeroEscritura = numeroEscrituraUnico,
                        Extracto = !string.IsNullOrEmpty(anexo.ContenidoArchivo) ? new string(anexo.ContenidoArchivo.Take(150).ToArray()) + "..." : "Sin contenido.",
                        TextoCompleto = anexo.ContenidoArchivo,
                        Firmado = false,
                        Volumen = anexo.Volumen,
                        Libro = anexo.Libro,
                        Folio = anexo.NumeroEscritura
                    };
                    _dbService.GuardarProtocolo(nuevoProtocolo);
                    Log($"✔️  Anexo ID:{anexo.Id} ('{anexo.NombreArchivo}') migrado exitosamente.");
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

        // (El resto del archivo no necesita cambios)
        private void BtnMigrarProtocoloIndice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LstLogMigraciones.Items.Clear();
                Log("Iniciando migración de Protocolo a Índice...");

                var protocolos = _dbService.ObtenerProtocolos();
                var indicesExistentes = _dbService.ObtenerRegistrosIndice();
                var expedientes = _dbService.ObtenerExpedientes();
                int migracionesExitosas = 0;
                int migracionesOmitidas = 0;

                foreach (var protocolo in protocolos)
                {
                    if (indicesExistentes.Any(i => i.NumeroEscritura == protocolo.NumeroEscritura))
                    {
                        Log($"⚠️  Se omitió Protocolo '{protocolo.NumeroEscritura}' (ya existe en el Índice).");
                        migracionesOmitidas++;
                        continue;
                    }

                    var expedienteAsociado = expedientes.FirstOrDefault(exp => exp.Identificador == protocolo.ExpedienteId);
                    string otorgante = expedienteAsociado?.NombreCliente ?? "DESCONOCIDO";
                    string operacion = expedienteAsociado?.TipoCaso ?? "NO ESPECIFICADA";

                    var nuevoRegistro = new RegistroIndice
                    {
                        NumeroEscritura = protocolo.NumeroEscritura,
                        Fecha = protocolo.Fecha,
                        Otorgante = otorgante,
                        Operacion = operacion,
                        Volumen = protocolo.Volumen,
                        Libro = protocolo.Libro,
                        Folio = protocolo.Folio
                    };

                    _dbService.GuardarRegistroIndice(nuevoRegistro);
                    Log($"✔️  Protocolo '{protocolo.NumeroEscritura}' migrado al Índice exitosamente.");
                    migracionesExitosas++;
                }

                Log($"--- Migración finalizada. {migracionesExitosas} registros creados en el índice, {migracionesOmitidas} omitidos. ---");
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