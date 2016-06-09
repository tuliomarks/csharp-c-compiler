using System.Collections.Generic;

namespace CCompiler.Common
{
    public class Campo
    {
        public string Place { get; set; }
        public string Cod { get; set; }
       
        public string Rotulo1 { get; set; }
        public string Rotulo2 { get; set; }

        public string Tipo { get; set; }
        public int TamanhoVar { get; set; }
        public string DimensaoVetor { get; set; }

        public string RotuloDefault { get; set; }
        public string CodTestesSwitch { get; set; }

        public string Proc { get; set; }
        public Variavel Retorno { get; set; }
        public List<Variavel> Argumentos { get; set; }
    }
}
