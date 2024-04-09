using CommunityToolkit.Mvvm.Input;
using ProyectoUnidad2.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProyectoUnidad2.ViewModel
{
    public enum Ventanas { Home, ListaTemporadas, AgregarTemporada, EditarTemporada, EliminarTemporada, ListaCapitulos, AgregarCapitulo, EditarCapitulo, EliminarCapitulo }
    public class TBBTViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Temporadas> Temporadas { get; set; } = new();
        public IEnumerable<Temporadas> ordenTemporadas => Temporadas.OrderBy(x => x.NumeroTemporada).Select(x => x);
        public IEnumerable<Capitulos> CapitulosOrdenados => Temporada.Capitulos.OrderBy(x => x.NumeroCapitulo).Select(x => x);
        public ICommand AgregarCapituloCommand { get; set; }
        public ICommand EliminarCapituloCommand { get; set; }
        public ICommand EditarCapituloCommand { get; set; }
        public ICommand AgregarTemporadaCommand { get; set; }
        public ICommand EliminarTemporadaCommand { get; set; }
        public ICommand EditarTemporadaCommand { get; set; }
        public ICommand CambiarVistaCommand { get; set; }
        public Temporadas? Temporada { get; set; }
        public Capitulos? Capitulo { get; set; }
        public string Error { get; set; } = "";
        public Ventanas Ventana { get; set; } = Ventanas.Home;
        int posAnterior;

        public TBBTViewModel()
        {
            EliminarCapituloCommand = new RelayCommand(EliminarCapitulo);
            AgregarCapituloCommand = new RelayCommand(AgregarCapitulo);
            EditarCapituloCommand = new RelayCommand(EditarCapitulo);
            AgregarTemporadaCommand = new RelayCommand(AgregarTemporada);
            EditarTemporadaCommand = new RelayCommand(EditarTemporada);
            EliminarTemporadaCommand = new RelayCommand(EliminarTemporada);
            CambiarVistaCommand = new RelayCommand<Ventanas>(CambiarVista);
            Deserializar();
        }

        void CambiarVista(Ventanas vistas)
        {

            if (vistas == Ventanas.AgregarTemporada)
            {
                Temporada = new();
            }
            if (vistas == Ventanas.AgregarCapitulo && Temporada != null)
            {
                Capitulo = new();
            }

            if (vistas == Ventanas.EditarTemporada && Temporada != null)
            {
                var clon = new Temporadas
                {
                    Sinopsis = Temporada.Sinopsis,
                    NumeroTemporada = Temporada.NumeroTemporada,
                    Titulo = Temporada.Titulo,
                    URLimagen = Temporada.URLimagen
                };

                posAnterior = Temporadas.IndexOf(Temporada);

                Temporada = clon;
            }
            if (vistas == Ventanas.EditarCapitulo && Capitulo != null && Temporada != null)
            {
                var clon = new Capitulos
                {
                    Sinopsis = Capitulo.Sinopsis,
                    NumeroCapitulo = Capitulo.NumeroCapitulo,
                    Titulo = Capitulo.Titulo,
                    URLimagen = Capitulo.URLimagen
                };

                posAnterior = Temporada.Capitulos.IndexOf(Capitulo);

                Capitulo = clon;
            }

            Error = "";
            Actualizar(nameof(Error));

            Ventana = vistas;
            Actualizar(nameof(Ventana));

            Actualizar(nameof(Temporada));
            Actualizar(nameof(Capitulo));
            Actualizar(nameof(ordenTemporadas));

        }

        void AgregarCapitulo()
        {
            if (Capitulo != null && Temporada != null)
            {
                Error = "";
                if (Temporada.Capitulos.Any(x => x.NumeroCapitulo == Capitulo.NumeroCapitulo) || Capitulo.NumeroCapitulo <= 0)
                {
                    Error = "Escriba un numero de capitulo valido.";
                }
                if (string.IsNullOrWhiteSpace(Capitulo.Titulo))
                {
                    Error = "Escriba el titulo del capitulo.";
                }
                if (string.IsNullOrWhiteSpace(Capitulo.Sinopsis))
                {
                    Error = "Escriba la sinopsis del capitulo";
                }
                if (string.IsNullOrWhiteSpace(Capitulo.URLimagen))
                {
                    Capitulo.URLimagen = Temporada.URLimagen;
                }
                else if (!Capitulo.URLimagen.StartsWith("http") || !Capitulo.URLimagen.EndsWith(".jpg"))
                {
                    Error = "Escriba la dirección de una imagen en JPG.";
                }

                if (!string.IsNullOrWhiteSpace(Error))
                {
                    Actualizar(nameof(Error));
                    return;
                }
                Temporada.Capitulos.Add(Capitulo);
                Serializar();
                Ventana = Ventanas.ListaCapitulos;
                Actualizar(nameof(Ventana));
                Actualizar(nameof(CapitulosOrdenados));
            }
        }
        void AgregarTemporada()
        {
            if (Temporada != null)
            {
                Error = "";
                if (Temporadas.Any(x => x.NumeroTemporada == Temporada.NumeroTemporada) || Temporada.NumeroTemporada <= 0)
                {
                    Error = "Escriba un numero de temporada valido.";
                }
                if (string.IsNullOrWhiteSpace(Temporada.Titulo))
                {
                    Error = "Escriba el titulo de la temporada.";
                }
                if (string.IsNullOrWhiteSpace(Temporada.Sinopsis))
                {
                    Error = "Escriba la sinopsis de la temporada";
                }
                if (string.IsNullOrWhiteSpace(Temporada.URLimagen) || !Temporada.URLimagen.StartsWith("http") || !Temporada.URLimagen.EndsWith(".jpg"))
                {
                    Error = "Escriba la dirección de una imagen en JPG.";
                }

                if (!string.IsNullOrWhiteSpace(Error))
                {
                    Actualizar(nameof(Error));
                    return;
                }
                Temporadas.Add(Temporada);
                Serializar();
                Ventana = Ventanas.ListaTemporadas;
                Actualizar(nameof(Ventana));
                Actualizar(nameof(ordenTemporadas));

            }
        }
        void EliminarCapitulo()
        {
            if (Capitulo != null && Temporada != null)
            {
                Temporada.Capitulos.Remove(Capitulo);
                Serializar();
                Ventana = Ventanas.ListaCapitulos;
                Actualizar(nameof(Ventana));
                Actualizar(nameof(CapitulosOrdenados));

            }
        }
        void EliminarTemporada()
        {
            if (Temporada != null)
            {
                Temporadas.Remove(Temporada);
                Serializar();
                Ventana = Ventanas.ListaTemporadas;
                Actualizar(nameof(Ventana));
                Actualizar(nameof(ordenTemporadas));
            }

        }
        void EditarCapitulo()
        {
            if (Capitulo != null && Temporada != null)
            {
                Error = "";

                if (string.IsNullOrWhiteSpace(Capitulo.Titulo))
                {
                    Error = "Escriba el titulo del capitulo.";
                }
                if (string.IsNullOrWhiteSpace(Capitulo.Sinopsis))
                {
                    Error = "Escriba la sinopsis del capitulo";
                }
                if (string.IsNullOrWhiteSpace(Capitulo.URLimagen))
                {
                    Capitulo.URLimagen = Temporada.URLimagen;
                }
                else if(!Capitulo.URLimagen.StartsWith("http") || !Capitulo.URLimagen.EndsWith(".jpg"))
                {
                    Error = "Escriba la dirección de una imagen en JPG.";
                }

                Actualizar(nameof(Error));


                if (Error == "")
                {
                    Temporada.Capitulos[posAnterior] = Capitulo;
                    Serializar();

                    Ventana = Ventanas.ListaCapitulos;
                    Actualizar(nameof(Ventana));
                    Actualizar(nameof(CapitulosOrdenados));
                }

            }
        }
        void EditarTemporada()
        {
            if (Temporada != null)
            {
                Error = "";

                if (Temporadas.Any(x => x.NumeroTemporada == Temporada.NumeroTemporada) && Temporada.NumeroTemporada <= 0)
                {
                    Error = "Escriba un numero de temporada valido.";
                }
                if (string.IsNullOrWhiteSpace(Temporada.Titulo))
                {
                    Error = "Escriba el titulo de la temporada.";
                }
                if (string.IsNullOrWhiteSpace(Temporada.Sinopsis))
                {
                    Error = "Escriba la sinopsis de la temporada";
                }
                if (string.IsNullOrWhiteSpace(Temporada.URLimagen) || !Temporada.URLimagen.StartsWith("http") || !Temporada.URLimagen.EndsWith(".jpg"))
                {
                    Error = "Escriba la dirección de una imagen en JPG.";
                }

                Actualizar(nameof(Error));


                if (Error == "")
                {
                    var capitulosGuardados = Temporadas[posAnterior].Capitulos;
                    Temporadas[posAnterior] = Temporada;
                    Temporada.Capitulos = capitulosGuardados;
                    Serializar();

                    Ventana = Ventanas.ListaTemporadas;
                    Actualizar(nameof(Ventana));
                    Actualizar(nameof(ordenTemporadas));
                }

            }
        }

        void Serializar()
        {
            File.WriteAllText("Series.json", JsonSerializer.Serialize(Temporadas));
        }
        void Deserializar()
        {
            if(File.Exists("Series.json"))
            {
                var datos = JsonSerializer.Deserialize<ObservableCollection<Temporadas>>(File.ReadAllText("Series.json"));
                if (datos != null)
                {
                    foreach (var Item in datos)
                    {
                        Temporadas.Add(Item);
                    }
                }
            }
        }

        void Actualizar(string? name)
        {
            PropertyChanged?.Invoke(this, new(name));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
