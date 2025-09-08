// Importación de librerías necesarias para trabajar con ventanas WPF, controles, diálogos de archivos, etc.
using Microsoft.Win32;
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

namespace LexMigración
{
    // Definición de la ventana Anexo_A, que hereda de Window (ventana estándar WPF)
    public partial class Anexo_A : Window
    {
        // Constructor del formulario - inicializa los componentes visuales definidos en XAML
        public Anexo_A()
        {
            InitializeComponent();
        }
        
        // Evento disparado al hacer clic en el botón Agregar Documento
        private void BtnAgregarDocumento_Click(object sender, RoutedEventArgs e)
        {
            // Crear diálogo para seleccionar archivos
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Selecciona documentos"; // Título del diálogo
            dlg.Filter = "Archivos PDF|*.pdf|Todos los archivos|*.*"; // Filtrado de archivos (PDF o todos)
            dlg.Multiselect = true; // Permitir seleccionar múltiples archivos
            
            // Mostrar el diálogo y verificar si el usuario seleccionó archivos
            if (dlg.ShowDialog() == true)
            {
                // Mostrar mensaje con la cantidad de archivos seleccionados
                MessageBox.Show($"Seleccionaste {dlg.FileNames.Length} archivos.");
                
                // Aquí se podría agregar la lógica para manejar o almacenar los archivos seleccionados
            }
        }
        
        // Evento que se ejecuta cuando cambia la selección en el DataGrid DgAnexos
        private void DgAnexos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Si no hay ningún ítem seleccionado, salir del método
            if (DgAnexos.SelectedItem == null)
                return;
            
            // Obtener el ítem seleccionado de forma dinámica (sin tipo específico)
            dynamic anexo = DgAnexos.SelectedItem;
            
            // Actualizar controles de la interfaz con los datos del anexo seleccionado
            CbExpediente.SelectedValue = anexo.ExpedienteId;
            TxtFoliadoInicio.Text = anexo.FoliadoInicio.ToString();
            TxtFoliadoFin.Text = anexo.FoliadoFin.ToString();
            TxtTotalHojas.Text = anexo.TotalHojas.ToString();
            CbEstado.Text = anexo.Estado;
            DpFechaCreacion.SelectedDate = anexo.CreatedAt;
        }
        
        // Evento disparado al hacer clic en el botón Guardar
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Mensaje temporal indicando que aquí se debe implementar la lógica para guardar un anexo
            MessageBox.Show("Guardar anexo - Implementar lógica aquí");
            
            // Aquí se debe implementar la funcionalidad para guardar los datos capturados en la base de datos
        }
        
        // Evento disparado al hacer clic en el botón Actualizar
        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            // Mensaje temporal indicando que aquí se debe implementar la lógica para actualizar un anexo existente
            MessageBox.Show("Actualizar anexo - Implementar lógica aquí");
            
            // Aquí se debe implementar la funcionalidad para actualizar los datos del anexo en la base de datos
        }
    }
}
