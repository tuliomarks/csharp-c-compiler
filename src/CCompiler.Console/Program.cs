using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                    GeradorC3E.Inicializar(string.Format("{0}\0", conteudo));
                    try
                    {
                        if (GeradorC3E.ExternalDeclaration(e))
                        {
                            Console.WriteLine("==== Resultado Comandos C3E: \n{0}", e.Cod);
                            File.WriteAllText("saida_" + exp, e.Cod);
                        }

                        else
                        {
                            Console.WriteLine("==== Sintaxe incorreta em {2}", GeradorC3E.LinhaTokenAtual, GeradorC3E.ColunaTokenAtual, GeradorC3E.TokenAtual);
                            if (GeradorC3E.Exceptions.Any())
                                Console.WriteLine(GeradorC3E.Exceptions.First().Message);

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
