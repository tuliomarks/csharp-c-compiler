using System.Collections.Generic;

namespace CCompiler.Common
{
    public class Variavel
    {
        public string Id { get; set; }
        public string Tipo { get; set; }
        public Escopo Ref { get; set; }

        public Variavel Return { get; set; }
        public List<Variavel> Params { get; set; }

        public Variavel()
        {
            Params = new List<Variavel>();
        }

    }
}
