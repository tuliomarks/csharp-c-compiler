using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CCompiler.Common;

namespace AvalExpressoes
{
    class Program
    {
        static void Main(string[] args)
        {
            string comando = string.Empty;

            while(comando != "E")
            {
                Console.Clear();
                Console.WriteLine("Digite a expressao ou nome do arquivo:");
                var exp = Console.ReadLine();
                if (!string.IsNullOrEmpty(exp))
                {
                    Campo e = new Campo();

                    if (File.Exists(exp))
                    {
                        exp = File.ReadAllText(exp);
                        Console.WriteLine(exp);
                    }

                    Console.WriteLine("");
                    AvaliadorExpressoes.Inicializa(string.Format("{0}\0",exp));
                    if (AvaliadorExpressoes.ListaCmd(e))
                    {
                        Console.WriteLine("========================= Resultado Comandos: \n{0}", e.Cod);
                    }
                    else
                    {
                        Console.WriteLine("SINTAXE ERRADA!");
                    }
                    
                }
                Console.WriteLine("ENTER para continuar...");
                comando = Console.ReadLine();
            }                             
        }
    }
}
