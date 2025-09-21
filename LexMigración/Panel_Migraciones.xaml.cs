using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LexMigración.Services; // <-- Asegúrate de que esta línea esté
using LexMigración.Models;
using System.Windows;

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

        // --- *** NUEVO MÉTODO PARA AÑADIR MENSAJES AL LOG *** ---
        private void Log(string message, bool isError = false)
        {
            // Creamos un nuevo item para la lista del log
            var item = new System.Windows.Controls.ListBoxItem
            {
                Content = $"[{DateTime.Now:HH:mm:ss}] {message}",
                Foreground = isError ? Brushes.Red : Brushes.Black
            };
            // Lo añadimos a la lista visible en la pantalla
            LstLogMigraciones.Items.Insert(0, item); // Insertar al principio para ver lo más nuevo primero
        }

        private void BtnMigrarAnexoProtocolo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LstLogMigraciones.Items.Clear(); // Limpiamos el log anterior
                Log("Iniciando migración de Anexo a Protocolo...");

                var anexos = _dbService.ObtenerAnexos();
                var protocolosExistentes = _dbService.ObtenerProtocolos();
                int migracionesExitosas = 0;
                int migracionesOmitidas = 0;

                foreach (var anexo in anexos)
                {
                    if (protocolosExistentes.Any(p => p.ExpedienteId == anexo.ExpedienteId))
                    {
                        Log($"⚠️  Se omitió el Anexo para el expediente '{anexo.ExpedienteId}' (ya existe un Protocolo).");
                        migracionesOmitidas++;
                        continue;
                    }

                    var nuevoProtocolo = new ProtocoloModel
                    {
                        ExpedienteId = anexo.ExpedienteId,
                        Fecha = anexo.CreatedAt,
                        NumeroEscritura = $"TEMP-{anexo.ExpedienteId}",
                        Extracto = !string.IsNullOrEmpty(anexo.ContenidoArchivo) ? new string(anexo.ContenidoArchivo.Take(150).ToArray()) + "..." : "Sin contenido.",
                        TextoCompleto = anexo.ContenidoArchivo,
                        Firmado = false
                    };
                    _dbService.GuardarProtocolo(nuevoProtocolo);
                    Log($"✔️  Anexo '{anexo.ExpedienteId}' migrado exitosamente.");
                    migracionesExitosas++;
                }

                Log($"--- Migración finalizada. {migracionesExitosas} nuevos protocolos creados, {migracionesOmitidas} omitidos. ---");
                MessageBox.Show("Proceso de migración finalizado. Revisa el log para ver los detalles.", "Proceso Terminado");
            }
            catch (Exception ex)
            {
                Log($"❌ ERROR FATAL: {ex.Message}", true);
                MessageBox.Show("Ocurrió un error grave durante la migración. Revisa el log.", "Error de Migración");
            }
        }

        private void BtnMigrarProtocoloIndice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LstLogMigraciones.Items.Clear(); // Limpiamos el log anterior
                Log("Iniciando migración de Protocolo a Índice...");

                var protocolos = _dbService.ObtenerProtocolos();
                var indicesExistentes = _dbService.ObtenerRegistrosIndice();
                int migracionesExitosas = 0;
                int migracionesOmitidas = 0;

                foreach (var protocolo in protocolos)
                {
                    if (indicesExistentes.Any(i => i.NumeroEscritura == protocolo.NumeroEscritura))
                    {
                        Log($"⚠️  Se omitió el Protocolo '{protocolo.NumeroEscritura}' (ya existe en el Índice).");
                        migracionesOmitidas++;
                        continue;
                    }

                    var nuevoRegistro = new RegistroIndice
                    {
                        NumeroEscritura = protocolo.NumeroEscritura,
                        Fecha = protocolo.Fecha,
                        Otorgante = "PENDIENTE",
                        Operacion = "PENDIENTE",
                        Volumen = "S/V",
                        Libro = "S/L",
                        Folio = "S/F"
                    };
                    _dbService.GuardarRegistroIndice(nuevoRegistro);
                    Log($"✔️  Protocolo '{protocolo.NumeroEscritura}' migrado al Índice exitosamente.");
                    migracionesExitosas++;
                }

                Log($"--- Migración finalizada. {migracionesExitosas} nuevos registros creados en el índice, {migracionesOmitidas} omitidos. ---");
                MessageBox.Show("Proceso de migración finalizado. Revisa el log para ver los detalles.", "Proceso Terminado");
            }
            catch (Exception ex)
            {
                Log($"❌ ERROR FATAL: {ex.Message}", true);
                MessageBox.Show("Ocurrió un error grave durante la migración. Revisa el log.", "Error de Migración");
            }
        }
    }
}