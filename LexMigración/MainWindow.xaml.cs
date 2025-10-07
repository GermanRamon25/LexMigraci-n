using LexMigración.Services;
using System.Windows;
using System.Windows.Input;

namespace LexMigración
{
    public partial class MainWindow : Window
    {
        private DatabaseService _dbService;
        private Anexo_A anexoWindow;
        private Protocolo protocoloWindow;
        private Indice indiceWindow;
        private ExpedientesWindow expedientesWindow; // Variable para la nueva ventana

        public MainWindow(string nombreUsuario)
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            InitializeButtons();

            // Esto ahora funcionará porque el namespace es correcto
            TxtUsuarioLogeado.Text = nombreUsuario;
        }

        private void InitializeButtons()
        {
            BtnAnexos.Click += BtnAnexos_Click;
            BtnExpedientes.Click += BtnExpedientes_Click; // Conexión del nuevo botón
            BtnProtocolos.Click += BtnProtocolos_Click;
            BtnIndice.Click += BtnIndice_Click;
            // La línea de BtnMigraciones.Click ha sido eliminada.
        }

        // --- MÉTODOS COMPLETOS PARA LA NAVEGACIÓN ---
        private void BtnAnexos_Click(object sender, RoutedEventArgs e)
        {
            CloseOtherWindows("Anexo");
            if (anexoWindow == null || !anexoWindow.IsLoaded)
            {
                anexoWindow = new Anexo_A();
            }
            anexoWindow.Show();
            anexoWindow.Activate();
        }

        // --- NUEVO MÉTODO PARA CERRAR SESIÓN Y VOLVER AL LOGIN ---
        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            // Cierra todas las ventanas secundarias abiertas
            CloseOtherWindows("None");

            // Crea y muestra la ventana de Login
            Login loginWindow = new Login();
            loginWindow.Show();

            // Cierra la ventana principal (MainWindow)
            this.Close();
        }

        // --- MÉTODO NUEVO PARA EL BOTÓN DE EXPEDIENTES ---
        private void BtnExpedientes_Click(object sender, RoutedEventArgs e)
        {
            CloseOtherWindows("Expedientes");
            if (expedientesWindow == null || !expedientesWindow.IsLoaded)
            {
                expedientesWindow = new ExpedientesWindow();
            }
            expedientesWindow.Show();
            expedientesWindow.Activate();
        }

        private void BtnProtocolos_Click(object sender, RoutedEventArgs e)
        {
            CloseOtherWindows("Protocolo");
            if (protocoloWindow == null || !protocoloWindow.IsLoaded)
            {
                protocoloWindow = new Protocolo();
            }
            protocoloWindow.Show();
            protocoloWindow.Activate();
        }

        private void BtnIndice_Click(object sender, RoutedEventArgs e)
        {
            CloseOtherWindows("Indice");
            if (indiceWindow == null || !indiceWindow.IsLoaded)
            {
                indiceWindow = new Indice();
            }
            indiceWindow.Show();
            indiceWindow.Activate();
        }

        // El método BtnMigraciones_Click ha sido eliminado.

        private void CloseOtherWindows(string except)
        {
            if (except != "Anexo" && anexoWindow != null && anexoWindow.IsLoaded) anexoWindow.Close();
            if (except != "Expedientes" && expedientesWindow != null && expedientesWindow.IsLoaded) expedientesWindow.Close();
            if (except != "Protocolo" && protocoloWindow != null && protocoloWindow.IsLoaded) protocoloWindow.Close();
            if (except != "Indice" && indiceWindow != null && indiceWindow.IsLoaded) indiceWindow.Close();
            // La condición para cerrar la ventana de migraciones ha sido eliminada.
        }

        // --- Métodos para controlar la ventana ---
        private void MinimizeWindow_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void MaximizeRestoreWindow_Click(object sender, RoutedEventArgs e) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

        private void CloseWindow_Click(object sender, RoutedEventArgs e) => this.Close();

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}