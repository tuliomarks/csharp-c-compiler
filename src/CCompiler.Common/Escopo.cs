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

        public Escopo()
        {
            Variaveis = new List<Variavel>();
        }
    }
}
