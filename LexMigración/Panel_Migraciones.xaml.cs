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

        // --- LÓGICA DE MIGRACIÓN 1 CORREGIDA: USA EL NÚMERO DE ESCRITURA REAL (Y NO EL TEMPORAL) ---
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
                    // 🚨 CORRECCIÓN CLAVE: Usamos el número REAL (anexo.NumeroEscritura). 
                    // Si el usuario lo dejó vacío, usamos un fallback, NO el TEMP-ANEXO-X.
                    string numeroEscrituraFinal = string.IsNullOrEmpty(anexo.NumeroEscritura) ?
                        $"FALLBACK-{anexo.Id}" : anexo.NumeroEscritura;

                    if (protocolosExistentes.Any(p => p.NumeroEscritura == numeroEscrituraFinal))
                    {
                        Log($"⚠️  Se omitió Anexo ID:{anexo.Id} ('{anexo.NombreArchivo}') (ya existe como escritura: {numeroEscrituraFinal}).");
                        migracionesOmitidas++;
                        continue;
                    }

                    var nuevoProtocolo = new ProtocoloModel
                    {
                        ExpedienteId = anexo.ExpedienteId,
                        Fecha = anexo.CreatedAt,
                        // Usamos el número de escritura real.
                        NumeroEscritura = numeroEscrituraFinal,
                        Extracto = !string.IsNullOrEmpty(anexo.ContenidoArchivo) ? new string(anexo.ContenidoArchivo.Take(150).ToArray()) + "..." : "Sin contenido.",
                        TextoCompleto = anexo.ContenidoArchivo,
                        Firmado = false,
                        Volumen = anexo.Volumen,
                        Libro = anexo.Libro,
                        // Folio ya fue eliminado del modelo ProtocoloModel
                    };
                    _dbService.GuardarProtocolo(nuevoProtocolo);
                    Log($"✔️  Anexo ID:{anexo.Id} ('{anexo.NombreArchivo}') migrado exitosamente con No. {numeroEscrituraFinal}.");
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

        // --- LÓGICA DE MIGRACIÓN 2: PURGA Y RECONSTRUCCIÓN DEL ÍNDICE ---
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

                // 1. PURGA TOTAL DE DATOS TEMPORALES EN ORIGEN Y DESTINO

                // Eliminamos registros Protocolo TEMPORALES de la fuente (ya no deberían crearse, pero limpiamos los antiguos).
                Log("1. Limpiando protocolos temporales de la fuente...");
                foreach (var protocolo in protocolos.Where(p => p.NumeroEscritura != null && p.NumeroEscritura.StartsWith("TEMP-ANEXO-")).ToList())
                {
                    _dbService.EliminarProtocolo(protocolo);
                    protocolosEliminados++;
                }
                Log($"🗑️ {protocolosEliminados} protocolos temporales eliminados del origen.");

                // Eliminamos TODOS los registros del Índice para reconstruir desde cero.
                Log("2. Eliminando TODOS los registros de Índice para reconstrucción...");
                foreach (var indice in indicesExistentes)
                {
                    _dbService.EliminarRegistroIndice(indice);
                    indicesEliminados++;
                }
                Log($"🗑️ {indicesEliminados} registros de Índice eliminados.");

                // Volvemos a obtener Protocolos, ahora limpios.
                indicesExistentes = _dbService.ObtenerRegistrosIndice();
                protocolos = _dbService.ObtenerProtocolos().ToList();

                // 3. MIGRACIÓN FINAL (Protocolo → Índice)
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