using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCompiler.Common
{
    public class Escopo
    {
        public List<Variavel> Variaveis { get; set; }
        public Escopo Ref { get; set; }

        public Escopo(Escopo referencia)
        {
            Ref = referencia;
            Variaveis = new List<Variavel>();
        }
    }
}
