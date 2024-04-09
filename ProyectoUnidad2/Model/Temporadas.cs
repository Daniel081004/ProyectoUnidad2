using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoUnidad2.Model
{
    public class Temporadas
    {
        public string Titulo { get; set; } = "";
        public string URLimagen { get; set; } = "";
        public string Sinopsis { get; set; } = "";
        public byte NumeroTemporada { get; set; }
        public ObservableCollection<Capitulos> Capitulos { get; set; } = new();
    }
}
