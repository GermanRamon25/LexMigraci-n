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

namespace LexMigración
{
    public partial class Panel_Migraciones : Window
    {
        private DatabaseService _dbService;

        public Panel_Migraciones()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
        }

        private void BtnMigrarAnexoProtocolo_Click(object sender, RoutedEventArgs e)
        {
            var anexos = _dbService.ObtenerAnexos();

            foreach (var anexo in anexos)
            {
                var nuevoProtocolo = new ProtocoloModel // <-- CAMBIO AQUÍ
                {
                    ExpedienteId = anexo.ExpedienteId,
                    Fecha = anexo.CreatedAt,
                    NumeroEscritura = "", // Asigna un valor por defecto o déjalo vacío
                    Extracto = "",
                    TextoCompleto = "",
                    Firmado = false
                };
                _dbService.GuardarProtocolo(nuevoProtocolo);
            }

            MessageBox.Show("Migración de Anexo a Protocolo completada.");
        }

        private void BtnMigrarProtocoloIndice_Click(object sender, RoutedEventArgs e)
        {
            var protocolos = _dbService.ObtenerProtocolos();
            foreach (var protocolo in protocolos)
            {
                var nuevoRegistro = new RegistroIndice
                {
                    NumeroEscritura = protocolo.NumeroEscritura,
                    Fecha = protocolo.Fecha,
                    // ... mapear otros campos
                };
                _dbService.GuardarRegistroIndice(nuevoRegistro);
            }
            MessageBox.Show("Migración de Protocolo a Índice completada.");
        }
    }
}