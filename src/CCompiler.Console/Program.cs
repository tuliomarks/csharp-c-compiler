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

            while (comando != "E")
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
                    AvaliadorExpressoes.Inicializar(string.Format("{0}\0", exp));
                    try
                    {
                        if (AvaliadorExpressoes.Statement(e))
                            Console.WriteLine("==== Resultado Comandos C3E: \n{0}", e.Cod);
                        else
                            Console.WriteLine("==== Sintaxe incorreta em ({0}, {1}) {2}", AvaliadorExpressoes.LinhaTokenAtual, AvaliadorExpressoes.ColunaTokenAtual, AvaliadorExpressoes.TokenAtual);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                Console.WriteLine("ENTER para continuar...");
                comando = Console.ReadLine();
            }
        }
    }
}
