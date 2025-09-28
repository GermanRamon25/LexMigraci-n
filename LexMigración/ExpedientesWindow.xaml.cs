using LexMigración.Models;
using LexMigración.Services;
using System;
using System.Windows;
using System.Windows.Controls;

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

        private void CargarExpedientes()
        {
            try
            {
                DgExpedientes.ItemsSource = _dbService.ObtenerExpedientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los expedientes: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LimpiarFormulario()
        {
            DgExpedientes.SelectedItem = null;
            TxtIdentificador.Clear();
            TxtNombreCliente.Clear();
            TxtTipoCaso.Clear();
            TxtIdentificador.IsEnabled = true; // Habilitar para nuevos registros
        }

        private void DgExpedientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgExpedientes.SelectedItem is Expediente expediente)
            {
                TxtIdentificador.Text = expediente.Identificador;
                TxtNombreCliente.Text = expediente.NombreCliente;
                TxtTipoCaso.Text = expediente.TipoCaso;
                TxtIdentificador.IsEnabled = false; // El identificador no se puede cambiar al editar
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
                MessageBox.Show("El Identificador y el Nombre del Cliente son obligatorios.", "Datos Faltantes", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Si hay un item seleccionado, estamos actualizando. Si no, estamos creando uno nuevo.
                if (DgExpedientes.SelectedItem is Expediente expedienteSeleccionado)
                {
                    // Actualizar
                    expedienteSeleccionado.NombreCliente = TxtNombreCliente.Text;
                    expedienteSeleccionado.TipoCaso = TxtTipoCaso.Text;
                    _dbService.ActualizarExpediente(expedienteSeleccionado);
                    MessageBox.Show("Expediente actualizado exitosamente.", "Éxito");
                }
                else
                {
                    // Crear nuevo
                    var nuevoExpediente = new Expediente
                    {
                        Identificador = TxtIdentificador.Text.Trim(),
                        NombreCliente = TxtNombreCliente.Text.Trim(),
                        TipoCaso = TxtTipoCaso.Text.Trim(),
                        FechaCreacion = DateTime.Now
                    };
                    _dbService.GuardarExpediente(nuevoExpediente);
                    MessageBox.Show("Expediente creado exitosamente.", "Éxito");
                }
                CargarExpedientes();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el expediente: " + ex.Message, "Error");
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (DgExpedientes.SelectedItem is Expediente expedienteSeleccionado)
            {
                if (MessageBox.Show($"¿Seguro que deseas eliminar el expediente '{expedienteSeleccionado.Identificador}'? Esto podría afectar a los anexos asociados.", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _dbService.EliminarExpediente(expedienteSeleccionado);
                        MessageBox.Show("Expediente eliminado.", "Éxito");
                        CargarExpedientes();
                        LimpiarFormulario();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al eliminar: " + ex.Message, "Error");
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un expediente de la lista para eliminar.", "Ningún Expediente Seleccionado");
            }
        }
    }
}