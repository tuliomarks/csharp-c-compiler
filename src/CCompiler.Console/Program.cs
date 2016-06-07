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
                Console.WriteLine("Digite o nome do arquivo de entrada:");
                var exp = Console.ReadLine();
                var conteudo = string.Empty;
                if (!string.IsNullOrEmpty(exp))
                {
                    Campo e = new Campo();

                    if (File.Exists(exp))
                    {
                        conteudo = File.ReadAllText(exp);
                        Console.WriteLine(conteudo);
                    }

                    Console.WriteLine("");
                    AvaliadorExpressoes.Inicializar(string.Format("{0}\0", conteudo));
                    try
                    {
                        if (AvaliadorExpressoes.ExternalDeclaration(e))
                        {
                            Console.WriteLine("==== Resultado Comandos C3E: \n{0}", e.Cod);
                            File.WriteAllText("saida_" + exp, e.Cod);
                        }

                        else
                        {
                            Console.WriteLine("==== Sintaxe incorreta em ({0}, {1}) {2}", AvaliadorExpressoes.LinhaTokenAtual, AvaliadorExpressoes.ColunaTokenAtual, AvaliadorExpressoes.TokenAtual);
                            foreach (var exception in AvaliadorExpressoes.Exceptions)
                            {
                                Console.WriteLine(exception.Message);
                            }
                        }
                            
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
