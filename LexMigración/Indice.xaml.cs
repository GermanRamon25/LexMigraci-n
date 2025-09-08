using LexMigración.Models; // Necesario para RegistroIndice
using LexMigración.Services; // Necesario para DatabaseService
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LexMigración
{
    public partial class Indice : Window
    {
        private DatabaseService _dbService;
        private ObservableCollection<RegistroIndice> _registrosEnMemoria;

        public Indice()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            InicializarDatos();
        }

        private void InicializarDatos()
        {
            // Cargar los datos desde la base de datos
            var registrosDb = _dbService.ObtenerRegistrosIndice();
            _registrosEnMemoria = new ObservableCollection<RegistroIndice>(registrosDb);

            // Asignar al DataGrid
            DgIndice.ItemsSource = _registrosEnMemoria;
        }

        private void Filtros_Changed(object sender, RoutedEventArgs e)
        {
            // (La lógica de filtrado sigue siendo útil y no necesita cambios)
            var filtroOtorgante = TxtFiltroOtorgante.Text?.ToLower() ?? "";
            var filtroOperacion = TxtFiltroOperacion.Text?.ToLower() ?? "";
            var filtroNumero = TxtFiltroNumero.Text?.ToLower() ?? "";
            var filtroFecha = DpFiltroFecha.SelectedDate;

            var registrosFiltrados = _registrosEnMemoria.Where(r =>
                (string.IsNullOrEmpty(filtroOtorgante) || r.Otorgante.ToLower().Contains(filtroOtorgante)) &&
                (string.IsNullOrEmpty(filtroOperacion) || r.Operacion.ToLower().Contains(filtroOperacion)) &&
                (string.IsNullOrEmpty(filtroNumero) || r.NumeroEscritura.ToLower().Contains(filtroNumero)) &&
                (!filtroFecha.HasValue || r.Fecha.Date == filtroFecha.Value.Date)
            ).ToList();

            // Actualizar la vista del DataGrid con los resultados filtrados
            DgIndice.ItemsSource = new ObservableCollection<RegistroIndice>(registrosFiltrados);
        }

        private void DgIndice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgIndice.SelectedItem is RegistroIndice registro)
            {
                TxtMensaje.Text = $"Seleccionado: Escritura {registro.NumeroEscritura}";
            }
        }

        private void BtnMigrarProtocolo_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para migrar protocolo
            TxtMensaje.Text = "Iniciando migración de protocolo...";
        }

        private void BtnGuardarCambios_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ahora esto guardará los cambios en la base de datos
                foreach (var registro in _registrosEnMemoria)
                {
                    // (Aquí se necesitaría una lógica más avanzada para detectar
                    // si el registro es nuevo o modificado, pero para empezar
                    // guardaremos todo)
                    _dbService.GuardarRegistroIndice(registro);
                }

                TxtMensaje.Text = "Cambios guardados exitosamente";
                TxtMensaje.Foreground = Brushes.Green;
            }
            catch (Exception ex)
            {
                TxtMensaje.Text = $"Error al guardar: {ex.Message}";
                TxtMensaje.Foreground = Brushes.Red;
            }
        }

        private void BtnAgregarFila_Click(object sender, RoutedEventArgs e)
        {
            var nuevoRegistro = new RegistroIndice
            {
                Otorgante = "Nuevo Otorgante",
                Operacion = "",
                NumeroEscritura = "",
                Volumen = "",
                Libro = "",
                Fecha = DateTime.Today,
                Folio = ""
            };

            _registrosEnMemoria.Add(nuevoRegistro);
            DgIndice.ItemsSource = _registrosEnMemoria; // Refresca la vista

            DgIndice.SelectedItem = nuevoRegistro;
            DgIndice.ScrollIntoView(nuevoRegistro);
        }
    }

    // Clase modelo para los registros del índice
    // RECOMENDACIÓN: Mover esta clase a su propio archivo en la carpeta /Models
    public class RegistroIndice : INotifyPropertyChanged
    {
        // -----  CORRECCIÓN: Se añade la clave principal (Primary Key)  -----
        public int Id { get; set; }
        // --------------------------------------------------------------------

        private string _otorgante;
        private string _operacion;
        private string _numeroEscritura;
        private string _volumen;
        private string _libro;
        private DateTime _fecha;
        private string _folio;

        public string Otorgante
        {
            get => _otorgante;
            set { _otorgante = value; OnPropertyChanged(); }
        }

        public string Operacion
        {
            get => _operacion;
            set { _operacion = value; OnPropertyChanged(); }
        }

        public string NumeroEscritura
        {
            get => _numeroEscritura;
            set { _numeroEscritura = value; OnPropertyChanged(); }
        }

        public string Volumen
        {
            get => _volumen;
            set { _volumen = value; OnPropertyChanged(); }
        }

        public string Libro
        {
            get => _libro;
            set { _libro = value; OnPropertyChanged(); }
        }

        public DateTime Fecha
        {
            get => _fecha;
            set { _fecha = value; OnPropertyChanged(); }
        }

        public string Folio
        {
            get => _folio;
            set { _folio = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}