using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class Indice : Window
    {
        private ObservableCollection<RegistroIndice> _registrosOriginales;
        private ObservableCollection<RegistroIndice> _registrosFiltrados;

        public Indice()
        {
            InitializeComponent();
            InicializarDatos();
        }

        private void InicializarDatos()
        {
            // Inicializar colección de datos
            _registrosOriginales = new ObservableCollection<RegistroIndice>();
            _registrosFiltrados = new ObservableCollection<RegistroIndice>();

            // Cargar datos de ejemplo
            CargarDatosEjemplo();

            // Asignar al DataGrid
            DgIndice.ItemsSource = _registrosFiltrados;
        }

        private void CargarDatosEjemplo()
        {
            // Datos de ejemplo basados en tu imagen
            _registrosOriginales.Add(new RegistroIndice
            {
                Otorgante = "ABACAPITAL SOCIEDAD ANÓNIMA DE CAPITAL VARIABLE, SOCIEDAD FINANCIERA DE OBJETO MÚLTIPLE, ENTIDAD NO REGULADA",
                Operacion = "COMPRAVENTA",
                NumeroEscritura = "2279",
                Volumen = "IV",
                Libro = "2",
                Fecha = new DateTime(2023, 6, 22),
                Folio = "204"
            });

            _registrosOriginales.Add(new RegistroIndice
            {
                Otorgante = "ABACAPITAL SOCIEDAD ANÓNIMA DE CAPITAL VARIABLE, SOCIEDAD FINANCIERA DE OBJETO MÚLTIPLE, ENTIDAD NO REGULADA",
                Operacion = "COMPRAVENTA",
                NumeroEscritura = "2281",
                Volumen = "IV",
                Libro = "2",
                Fecha = new DateTime(2023, 6, 28),
                Folio = "207"
            });

            _registrosOriginales.Add(new RegistroIndice
            {
                Otorgante = "ABACAPITAL SOCIEDAD ANÓNIMA DE CAPITAL VARIABLE, SOCIEDAD FINANCIERA DE OBJETO MÚLTIPLE, ENTIDAD NO REGULADA",
                Operacion = "COMPRAVENTA",
                NumeroEscritura = "2327",
                Volumen = "IV",
                Libro = "2",
                Fecha = new DateTime(2023, 9, 7),
                Folio = "257"
            });

            // Copiar a la colección filtrada
            foreach (var registro in _registrosOriginales)
            {
                _registrosFiltrados.Add(registro);
            }
        }

        private void Filtros_Changed(object sender, RoutedEventArgs e)
        {
            FiltrarRegistros();
        }

        private void FiltrarRegistros()
        {
            var filtroOtorgante = TxtFiltroOtorgante.Text?.ToLower() ?? "";
            var filtroOperacion = TxtFiltroOperacion.Text?.ToLower() ?? "";
            var filtroNumero = TxtFiltroNumero.Text?.ToLower() ?? "";
            var filtroFecha = DpFiltroFecha.SelectedDate;

            var registrosFiltrados = _registrosOriginales.Where(r =>
                (string.IsNullOrEmpty(filtroOtorgante) || r.Otorgante.ToLower().Contains(filtroOtorgante)) &&
                (string.IsNullOrEmpty(filtroOperacion) || r.Operacion.ToLower().Contains(filtroOperacion)) &&
                (string.IsNullOrEmpty(filtroNumero) || r.NumeroEscritura.ToLower().Contains(filtroNumero)) &&
                (!filtroFecha.HasValue || r.Fecha.Date == filtroFecha.Value.Date)
            ).ToList();

            _registrosFiltrados.Clear();
            foreach (var registro in registrosFiltrados)
            {
                _registrosFiltrados.Add(registro);
            }
        }

        private void DgIndice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgIndice.SelectedItem is RegistroIndice registro)
            {
                // Aquí puedes manejar la selección de un registro
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
                // Aquí implementarías la lógica para guardar los cambios en la base de datos
                // Por ejemplo, recorrer _registrosFiltrados y actualizar en BD

                TxtMensaje.Text = "Cambios guardados exitosamente";
                TxtMensaje.Foreground = System.Windows.Media.Brushes.Green;
            }
            catch (Exception ex)
            {
                TxtMensaje.Text = $"Error al guardar: {ex.Message}";
                TxtMensaje.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void BtnAgregarFila_Click(object sender, RoutedEventArgs e)
        {
            var nuevoRegistro = new RegistroIndice
            {
                Otorgante = "",
                Operacion = "",
                NumeroEscritura = "",
                Volumen = "",
                Libro = "",
                Fecha = DateTime.Today,
                Folio = ""
            };

            _registrosOriginales.Add(nuevoRegistro);
            _registrosFiltrados.Add(nuevoRegistro);

            // Seleccionar la nueva fila para edición
            DgIndice.SelectedItem = nuevoRegistro;
            DgIndice.ScrollIntoView(nuevoRegistro);
        }
    }

    // Clase modelo para los registros del índice
    public class RegistroIndice : INotifyPropertyChanged
    {
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
            set
            {
                _otorgante = value;
                OnPropertyChanged();
            }
        }

        public string Operacion
        {
            get => _operacion;
            set
            {
                _operacion = value;
                OnPropertyChanged();
            }
        }

        public string NumeroEscritura
        {
            get => _numeroEscritura;
            set
            {
                _numeroEscritura = value;
                OnPropertyChanged();
            }
        }

        public string Volumen
        {
            get => _volumen;
            set
            {
                _volumen = value;
                OnPropertyChanged();
            }
        }

        public string Libro
        {
            get => _libro;
            set
            {
                _libro = value;
                OnPropertyChanged();
            }
        }

        public DateTime Fecha
        {
            get => _fecha;
            set
            {
                _fecha = value;
                OnPropertyChanged();
            }
        }

        public string Folio
        {
            get => _folio;
            set
            {
                _folio = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}