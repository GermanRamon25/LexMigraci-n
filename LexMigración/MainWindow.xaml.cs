using LexMigración.Services;
using System.Windows;
using System.Windows.Input;

namespace LexMigración
{
    public partial class MainWindow : Window
    {
        private DatabaseService _dbService;
        private Testimonio testimonioWindow;
        private Protocolo protocoloWindow;
        private Indice indiceWindow;
        private ExpedientesWindow expedientesWindow;

        public MainWindow(string nombreUsuario)
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            InitializeButtons();

           
            TxtUsuarioLogeado.Text = nombreUsuario;
        }

        private void InitializeButtons()
        {
            BtnTestimonios.Click += BtnTestimonios_Click;
            BtnExpedientes.Click += BtnExpedientes_Click; 
            BtnProtocolos.Click += BtnProtocolos_Click;
            BtnIndice.Click += BtnIndice_Click;
            
        }

        // --- MÉTODOS COMPLETOS PARA LA NAVEGACIÓN ---
        private void BtnTestimonios_Click(object sender, RoutedEventArgs e)
        {
            CloseOtherWindows("Testimonio");
            if (testimonioWindow == null || !testimonioWindow.IsLoaded)
            {
                testimonioWindow = new Testimonio();
            }
            testimonioWindow.Show();
            testimonioWindow.Activate();
        }

        // --- NUEVO MÉTODO PARA CERRAR SESIÓN Y VOLVER AL LOGIN ---
        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
           
            CloseOtherWindows("None");

           
            Login loginWindow = new Login();
            loginWindow.Show();

            
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

        

        private void CloseOtherWindows(string except)
        {
            if (except != "Testimonio" && testimonioWindow != null && testimonioWindow.IsLoaded) testimonioWindow.Close();
            if (except != "Expedientes" && expedientesWindow != null && expedientesWindow.IsLoaded) expedientesWindow.Close();
            if (except != "Protocolo" && protocoloWindow != null && protocoloWindow.IsLoaded) protocoloWindow.Close();
            if (except != "Indice" && indiceWindow != null && indiceWindow.IsLoaded) indiceWindow.Close();
            
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