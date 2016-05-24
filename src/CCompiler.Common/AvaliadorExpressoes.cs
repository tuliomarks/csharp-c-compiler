using System.Collections.Generic;

namespace CCompiler.Common
{
    public static class AvaliadorExpressoes
    {

        public static int IdTokenAtual { get; set; }
        public static string TokenAtual { get; set; }

        public static void Inicializa(string exp)
        {
            TempCount = 0;
            LabelCount = 0;
            Pos = 0;
            Exp = exp.ToCharArray();
            LeToken();
        }

        public static int MemoriaBase { get; set; }

        public static List<Variavel> TabelaVariaveis { get; set; }

        #region Lexico

        private static readonly List<string> Reservadas = new List<string>() { "int", "float", "char", "struct", "while", "break", "continue", "short", "double", "long" };
        private const int TkInt = 0;
        private const int TkFloat = 1;
        private const int TkChar = 2;
        private const int TkStruct = 3;
        private const int TkWhile = 4;
        private const int TkBreak = 5;
        private const int TkContinue = 6;
        private const int TkShort = 7;
        private const int TkDouble = 8;
        private const int TkLong = 9;

        private const int TkConst = 100;
        private const int TkVirgula = 101;
        private const int TkPontoVirgula = 102;
        private const int TkId = 103;
        private const int TkAbreColch = 104;
        private const int TkFechaColch = 105;
        private const int TkFimArquivo = 106;
        private const int TkAbreParentese = 107;
        private const int TkFechaParentese = 108;
        private const int TkMais = 109;
        private const int TkMenos = 110;
        private const int TkAsterisco = 111;
        private const int TkBarraDivisao = 112;
        private const int TkDoisPontos = 113;
        private const int TkIgual = 114;
        private const int TkAbreChaves = 115;
        private const int TkFechaChaves = 116;
        private const int TkAtribuicao = 117;
        private const int TkMaior = 118;
        private const int TkMaiorIgual = 119;
        private const int TkMenor = 120;
        private const int TkMenorIgual = 121;
        private const int TkDiferente = 122;
        private const int TkNegacao = 123;

        private static char[] Exp { get; set; }
        private static int Pos { get; set; }

        private static void LeToken()
        {
            int estado = 0;
            char? c = Exp[Pos];

            while (true)
            {
                if (estado == 0)
                {
                    if (c == '{')
                    {
                        LeChar();
                        IdTokenAtual = TkAbreChaves;
                        break;
                    }
                    if (c == '}')
                    {
                        LeChar();
                        IdTokenAtual = TkFechaChaves;
                        break;
                    }
                    if (c == ',')
                    {
                        LeChar();
                        IdTokenAtual = TkVirgula;
                        break;
                    }
                    if (c == ':')
                    {
                        LeChar();
                        IdTokenAtual = TkDoisPontos;
                        break;
                    }
                    if (c == '!')
                    {
                        c = LeChar();
                        estado = 5;
                        continue;
                    }
                    if (c == '=')
                    {
                        c = LeChar();
                        estado = 2;
                        continue;
                    }
                    if (c == '>')
                    {
                        c = LeChar();
                        estado = 3;
                        continue;
                    }
                    if (c == '<')
                    {
                        c = LeChar();
                        estado = 4;
                        continue;
                    }
                    if (c == ';')
                    {
                        LeChar();
                        IdTokenAtual = TkPontoVirgula;
                        break;
                    }
                    if (c == '(')
                    {
                        LeChar();
                        IdTokenAtual = TkAbreParentese;
                        break;
                    }
                    if (c == ')')
                    {
                        LeChar();
                        IdTokenAtual = TkFechaParentese;
                        break;
                    }
                    if (c == '[')
                    {
                        LeChar();
                        IdTokenAtual = TkAbreColch;
                        break;
                    }
                    if (c == ']')
                    {
                        LeChar();
                        IdTokenAtual = TkFechaColch;
                        break;
                    }
                    if (c == '*')
                    {
                        LeChar();
                        IdTokenAtual = TkAsterisco;
                        break;
                    }
                    if (c == '/')
                    {
                        LeChar();
                        IdTokenAtual = TkBarraDivisao;
                        break;
                    }
                    if (c == '+')
                    {
                        LeChar();
                        IdTokenAtual = TkMais;
                        break;
                    }
                    if (c == '-')
                    {
                        LeChar();
                        IdTokenAtual = TkMenos;
                        break;
                    }
                    if (c == '\0')
                    {
                        IdTokenAtual = TkFimArquivo;
                        break;
                    }

                    if (c >= '0' && c <= '9')
                    {
                        TokenAtual = c.ToString();
                        c = LeChar();
                        estado = 1;
                    }
                    else if (c >= 'a' && c <= 'z' || c == '_')
                    {
                        TokenAtual = c.ToString();
                        c = LeChar();
                        estado = 1;
                    }
                    else if (c == null || c == '\n' || c == '\r' || c == '\t' || c == ' ')
                    {
                        c = LeChar();
                    }
                }
                else if (estado == 1)
                {
                    if (c >= 'a' && c <= 'z' || c == '_' || c >= '0' && c <= '9')
                    {
                        TokenAtual += c;
                        c = LeChar();
                    }
                    else
                    {
                        IdTokenAtual = EhPalavraReservada(TokenAtual);
                        break;
                    }
                }
                else if (estado == 2)
                {
                    if (c == '=')
                    {
                        LeChar();
                        IdTokenAtual = TkIgual;
                        break;
                    }
                    else
                    {
                        // nesse caso é uma atribuicao
                        IdTokenAtual = TkAtribuicao;
                        break;
                    }
                }
                else if (estado == 3)
                {
                    if (c == '=')
                    {
                        LeChar();
                        IdTokenAtual = TkMaiorIgual;
                        break;
                    }
                    else
                    {
                        // nesse caso é uma atribuicao
                        IdTokenAtual = TkMaior;
                        break;
                    }
                }
                else if (estado == 4)
                {
                    if (c == '=')
                    {
                        LeChar();
                        IdTokenAtual = TkMenorIgual;
                        break;
                    }
                    else
                    {
                        // nesse caso é uma atribuicao
                        IdTokenAtual = TkMenor;
                        break;
                    }
                }
                else if (estado == 5)
                {
                    if (c == '=')
                    {
                        LeChar();
                        IdTokenAtual = TkDiferente;
                        break;
                    }
                    else
                    {
                        // nesse caso é uma atribuicao
                        IdTokenAtual = TkNegacao;
                        break;
                    }
                }


            }
        }

        private static int EhPalavraReservada(string token)
        {
            if (TokenAtual[0] >= '0' && TokenAtual[0] <= '9')
            {
                return TkConst;
            }

            for (int i = 0; i < Reservadas.Count; i++)
            {
                if (token == Reservadas[i]) return i;
            }
            return TkId;
        }

        private static char LeChar()
        {

            if (Pos >= Exp.Length) return '\0'; // fim da leitura geral
            Pos++;
            var c = Exp[Pos];

            return c;
        }
        #endregion

        #region Formata Texto

        public static string EscreveRotulo(string rotulo)
        {
            return string.Format("{0}: \n", rotulo);
        }

        public static string EscreveCodigo(string format, params object[] args)
        {
            if (!string.IsNullOrEmpty(format) && args.Length == 0)
                return string.Format("{0}", format);

            if (args.Length == 0) return string.Empty;

            // corrige o comando
            if (!format.EndsWith("\n")) format += "\n";

            return string.Format(format, args);
        }
        public static string EscreveCodigo(string comando, string atribuicao, string op1, string op2)
        {
            return string.Format("{0} := {1}{2}{3}\n", atribuicao, op1, comando, op2);
        }

        public static string EscreveCodigoIf(string expressao, string gotoLabel)
        {
            return string.Format("if {0} goto {1}\n", expressao, gotoLabel);
        }

        #endregion

        private static int TempCount { get; set; }

        private static string GeraRegTemp()
        {
            TempCount++;
            return "T" + TempCount;
        }

        private static int LabelCount { get; set; }

        private static string GeraLabel()
        {
            LabelCount++;
            return "L" + LabelCount;
        }

        private static int GerarEndereco(int tamanho)
        {
            var ret = MemoriaBase;
            MemoriaBase += tamanho;
            return ret;

        }

        private static void AdicionarVariavelTabela(string id, string tipo, int endereco)
        {
            if (TabelaVariaveis == null) TabelaVariaveis = new List<Variavel>();
            TabelaVariaveis.Add(new Variavel() { Id = id, Endereco = endereco, Tipo = tipo });
        }

        // ----------------------------------------------------
        /****
         implementar a declaração de variaveis
        ****/
        // Dec -> Tipo {DecLista.TipoVar = Tipo.TipoVar; DecLista.TamanhoVar = Tipo.TamanhoVar} DecLista;
        // Tipo -> short int {Tipo.TipoVar = short int; Tipo.TamanhoVar = 2}
        // Tipo -> int {Tipo.TipoVar = int ; Tipo.TamanhoVar = 4}
        // Tipo -> long int {Tipo.TipoVar = long int; Tipo.TamanhoVar = 4}
        // Tipo -> long long int {Tipo.TipoVar = long long int; Tipo.TamanhoVar = 8}
        // Tipo -> double {Tipo.TipoVar = double ; Tipo.TamanhoVar = 8}
        // Tipo -> float {Tipo.TipoVar = float ; Tipo.TamanhoVar = 4}
        // DecLista -> id {PoeVariavel(id, ListaDim.ListaVar, endereco); endereco += ListaDim.TamanhoVar;}  {RestoDecLista.TipoVar = DecLista.TipoVar; RestoDecLista.TamanhoVar = DecLista.TamanhoVar;}  RestoDecLista
        // RestoDecLista -> , {DecLista.TipoVar =  RestoDecLista.TipoVar, DecLista.TamanhoVar =  RestoDecLista.TamanhoVar} DecLista
        // RestoDecLista -> <vazio>   

        public static bool Dec(Campo dec)
        {
            var tipo = new Campo();
            var decLista = new Campo();

            if (Tipo(tipo))
            {
                decLista.TamanhoVar = tipo.TamanhoVar;
                decLista.TipoVar = tipo.TipoVar;
                if (DecLista(decLista))
                {
                    if (IdTokenAtual == TkPontoVirgula)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return false;
        }

        public static bool DecLista(Campo decLista)
        {
            var restoDecLista = new Campo();
            if (IdTokenAtual == TkId)
            {
                LeToken();
                AdicionarVariavelTabela(TokenAtual, decLista.TipoVar, GerarEndereco(decLista.TamanhoVar));

                restoDecLista.TamanhoVar = decLista.TamanhoVar;
                restoDecLista.TipoVar = decLista.TipoVar;
                if (RestoDecLista(restoDecLista))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public static bool RestoDecLista(Campo restoDecLista)
        {
            var decLista = new Campo();
            if (IdTokenAtual == TkVirgula)
            {
                LeToken();
                decLista.TipoVar = restoDecLista.TipoVar;
                decLista.TamanhoVar = restoDecLista.TamanhoVar;

                if (DecLista(decLista))
                {
                    return true;
                }
                return false;
            }
            // vazio
            return true;
        }

        public static bool Tipo(Campo tipo)
        {
            if (IdTokenAtual == TkShort)
            {
                LeToken();
                tipo.TipoVar = "short";
                tipo.TamanhoVar = 2;
                return true;
            }
            else if (IdTokenAtual == TkInt)
            {
                LeToken();
                tipo.TipoVar = "int";
                tipo.TamanhoVar = 4;
                return true;
            }
            else if (IdTokenAtual == TkDouble)
            {
                LeToken();
                tipo.TipoVar = "double";
                tipo.TamanhoVar = 8;
                return true;
            }
            else if (IdTokenAtual == TkFloat)
            {
                LeToken();
                tipo.TipoVar = "float";
                tipo.TamanhoVar = 4;
                return true;
            }
            else if (IdTokenAtual == TkLong)
            {
                LeToken();
                if (IdTokenAtual == TkLong)
                {
                    LeToken();
                    if (IdTokenAtual == TkInt)
                    {
                        tipo.TipoVar = "long long int";
                        tipo.TamanhoVar = 8;
                        return true;
                    }
                }
                else
                {
                    tipo.TipoVar = "long int";
                    tipo.TamanhoVar = 4;
                    return true;
                }
            }
            return false;
        }

        // ----------------------------------------------------
        // CMD -> while C do CMD
        // CMD -> break
        // CMD -> continue
        // CMD -> id = E       
        // CMD -> { listaCmd }

        // listaCmd -> CMD; listaCont
        // listaCont -> CMD; listaCont
        // listaCont -> <vazio>;                          

        // C -> E { RB.h = E.val} RB { C.val = RB.s }
        // RB -> == E { RB1.s = RB.h + E.val} RB1 { RB.s = RB1.s } 
        // RB -> != E { RB1.s = RB.h + E.val} RB1 { RB.s = RB1.s } 
        // RB -> >= E { RB1.s = RB.h + E.val} RB1 { RB.s = RB1.s } 
        // RB -> > E { RB1.s = RB.h + E.val} RB1 { RB.s = RB1.s } 
        // RB -> <= E { RB1.s = RB.h + E.val} RB1 { RB.s = RB1.s } 
        // RB -> < E { RB1.s = RB.h + E.val} RB1 { RB.s = RB1.s } 
        // RB -> E { RB1.s = E.val} RB1 { RB.s = RB1.s }        
        // RB -> <vazio> { RB.s = RB.h }

        // E -> T { R.h = T.val} R { E.val = R.s }
        // R -> + T { R1.s = R.h + T.val} R1 { R.s = R1.s } 
        // R -> - T { R1.s = R.h + T.val} R1 { R.s = R1.s } 
        // R -> T { R1.s = T.val} R1 { R.s = R1.s }        
        // R -> <vazio> { R.s = R.h }

        // T -> F { RA.h = F.val} RA { T.val = RA.s }
        // RA -> * F { RA1.s = RA.h * F.val} RA1 { RA.s = RA1.s } 
        // RA -> / F { RA1.s = RA.h * F.val} RA1 { RA.s = RA1.s } 
        // RA -> F { RA1.s = F.val} RA1 { RA.s = RA1.s }        
        // RA -> <vazio> { R.s = R.h }

        // F -> ( E ) { RA.h = F.val} RA { T.val = RA.s }
        // F -> cte { F.val = cte.val }
        // F -> id { F.val = cte.val }

        // ----------------------------------------------------

        public static bool Cmd(Campo cmd)
        {
            var c = new Campo();
            var e = new Campo();
            var listacmd = new Campo();
            var cmd1 = new Campo();

            if (IdTokenAtual == TkWhile)
            {
                LeToken();
                if (IdTokenAtual == TkAbreParentese)
                {
                    LeToken();
                    if (C(c))
                    {
                        if (IdTokenAtual == TkFechaParentese)
                        {
                            LeToken();

                            cmd1.RotuloInicio = GeraLabel();
                            var rotuloTrue = GeraLabel();
                            cmd1.RotuloFim = GeraLabel();                          
                            if (Cmd(cmd1))
                            {
                                cmd.Cod += EscreveRotulo(cmd1.RotuloInicio);
                                cmd.Cod += EscreveCodigo(c.Cod);
                                cmd.Cod += EscreveCodigoIf(c.Place, rotuloTrue);
                                cmd.Cod += EscreveCodigo("goto {0}", cmd1.RotuloFim);
                                cmd.Cod += EscreveRotulo(rotuloTrue);
                                cmd.Cod += EscreveCodigo(cmd1.Cod);
                                cmd.Cod += EscreveCodigo("goto {0}", cmd1.RotuloInicio);
                                cmd.Cod += EscreveRotulo(cmd1.RotuloFim);
                                return true;
                            }
                        }

                    }

                }
            }
            else if (IdTokenAtual == TkBreak)
            {
                if (string.IsNullOrEmpty(cmd.RotuloFim))
                    return false;

                cmd.T += EscreveCodigo("goto", cmd.RotuloFim, "", "");
                LeToken();
                return true;

            }
            else if (IdTokenAtual == TkContinue)
            {
                if (string.IsNullOrEmpty(cmd.RotuloInicio))
                    return false;

                cmd.T += EscreveCodigo("goto", cmd.RotuloInicio, "", "");
                LeToken();
                return true;

            }
            else if (IdTokenAtual == TkId)
            {
                cmd.Place = TokenAtual;             
                LeToken();
                if (IdTokenAtual == TkAtribuicao)
                {
                    LeToken();
                    if (E(e))
                    {                        
                        cmd.Cod += EscreveCodigo(e.Cod);
                        cmd.Cod += EscreveCodigo(string.Empty, cmd.Place, e.Place, string.Empty);                      
                        return true;
                    }
                }
            }
            else if (IdTokenAtual == TkAbreChaves)
            {
                LeToken();

                // propaga os labels 
                listacmd.RotuloFim = cmd.RotuloFim;
                listacmd.RotuloInicio = cmd.RotuloInicio;
                if (ListaCmd(listacmd))
                {
                    if (IdTokenAtual == TkFechaChaves)
                    {
                        LeToken();
                        cmd.Cod = listacmd.Cod;
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool ListaCmd(Campo listacmd)
        {
            var cmd = new Campo();
            var listacont = new Campo();

            // propaga os labels 
            cmd.RotuloFim = listacmd.RotuloFim;
            cmd.RotuloInicio = listacmd.RotuloInicio;
            if (Cmd(cmd))
            {
                if (IdTokenAtual == TkPontoVirgula)
                {
                    LeToken();

                    // propaga os labels 
                    listacont.RotuloFim = cmd.RotuloFim;
                    listacont.RotuloInicio = cmd.RotuloInicio;
                    if (ListaCont(listacont))
                    {
                        listacmd.Cod += EscreveCodigo(cmd.Cod);
                        listacmd.Cod += EscreveCodigo(listacont.Cod);
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return false;
        }

        public static bool ListaCont(Campo listacont)
        {
            var cmd = new Campo();
            var listacont1 = new Campo();

            // propaga os labels 
            cmd.RotuloFim = listacont.RotuloFim;
            cmd.RotuloInicio = listacont.RotuloInicio;
            if (Cmd(cmd))
            {
                if (IdTokenAtual == TkPontoVirgula)
                {
                    LeToken();

                    // propaga os labels 
                    listacont1.RotuloFim = cmd.RotuloFim;
                    listacont1.RotuloInicio = cmd.RotuloInicio;
                    if (ListaCont(listacont1))
                    {
                        listacont.Cod += EscreveCodigo(cmd.Cod);
                        listacont.Cod += EscreveCodigo(listacont1.Cod);
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return true;
        }

        public static bool C(Campo c)
        {
            Campo e = new Campo();
            Campo rbh = new Campo();
            Campo rbs = new Campo();
            if (E(e))
            {
                rbh.Cod = e.Cod;
                rbh.Place = e.Place;
                if (RB(rbh, rbs))
                {
                    c.Cod = rbs.Cod;
                    c.Place = rbs.Place;
                    return true;
                }
                return false;
            }
            return false;
        }

        public static bool RB(Campo rbh, Campo rbs)
        {
            Campo rb1h = new Campo();
            Campo rb1s = new Campo();
            Campo e = new Campo();

            if (IdTokenAtual == TkIgual)
            {
                LeToken();
                if (E(e))
                {
                    rb1h.Place = GeraRegTemp();
                    rb1h.Cod += EscreveCodigo(rbh.Cod);
                    rb1h.Cod += EscreveCodigo(e.Cod);
                    rb1h.Cod += EscreveCodigo("==", rb1h.Place, rbh.Place, e.Place);
                    if (RB(rb1h, rb1s))
                    {
                        rbs.Cod = rb1s.Cod;
                        rbs.Place = rb1s.Place;
                        return true;
                    }
                }
            }
            else if (IdTokenAtual == TkDiferente)
            {
                LeToken();
                if (E(e))
                {
                    rb1h.Place = GeraRegTemp();
                    rb1h.Cod += EscreveCodigo(rbh.Cod);
                    rb1h.Cod += EscreveCodigo(e.Cod);
                    rb1h.Cod += EscreveCodigo("!=", rb1h.Place, rbh.Place, e.Place);
                    if (RB(rb1h, rb1s))
                    {
                        rbs.Cod = rb1s.Cod;
                        rbs.Place = rb1s.Place;
                        return true;
                    }
                }
            }
            else if (IdTokenAtual == TkMaiorIgual)
            {
                LeToken();
                if (E(e))
                {
                    rb1h.Place = GeraRegTemp();
                    rb1h.Cod += EscreveCodigo(rbh.Cod);
                    rb1h.Cod += EscreveCodigo(e.Cod);
                    rb1h.Cod += EscreveCodigo(">=", rb1h.Place, rbh.Place, e.Place);
                    if (RB(rb1h, rb1s))
                    {
                        rbs.Cod = rb1s.Cod;
                        rbs.Place = rb1s.Place;
                        return true;
                    }
                }
            }
            else if (IdTokenAtual == TkMaior)
            {
                LeToken();
                if (E(e))
                {
                    rb1h.Place = GeraRegTemp();
                    rb1h.Cod += EscreveCodigo(rbh.Cod);
                    rb1h.Cod += EscreveCodigo(e.Cod);
                    rb1h.Cod += EscreveCodigo(">", rb1h.Place, rbh.Place, e.Place);
                    if (RB(rb1h, rb1s))
                    {
                        rbs.Cod = rb1s.Cod;
                        rbs.Place = rb1s.Place;
                        return true;
                    }
                }
            }
            else if (IdTokenAtual == TkMenorIgual)
            {
                LeToken();
                if (E(e))
                {
                    rb1h.Place = GeraRegTemp();
                    rb1h.Cod += EscreveCodigo(rbh.Cod);
                    rb1h.Cod += EscreveCodigo(e.Cod);
                    rb1h.Cod += EscreveCodigo("<=", rb1h.Place, rbh.Place, e.Place);
                    if (RB(rb1h, rb1s))
                    {
                        rbs.Cod = rb1s.Cod;
                        rbs.Place = rb1s.Place;
                        return true;
                    }
                }
            }
            else if (IdTokenAtual == TkMenor)
            {
                LeToken();
                if (E(e))
                {

                    rb1h.Place = GeraRegTemp();
                    rb1h.Cod += EscreveCodigo(rbh.Cod);
                    rb1h.Cod += EscreveCodigo(e.Cod);
                    rb1h.Cod += EscreveCodigo("<", rb1h.Place, rbh.Place, e.Place);
                    if (RB(rb1h, rb1s))
                    {
                        rbs.Cod = rb1s.Cod;
                        rbs.Place = rb1s.Place;
                        return true;
                    }
                }
            }
            else
            {
                rbs.Cod = rbh.Cod;
                rbs.Place = rbh.Place;
                return true;
            }
            return false;
        }

        public static bool E(Campo e)
        {
            Campo t = new Campo();
            Campo rh = new Campo();
            Campo rs = new Campo();
            if (T(t))
            {
                rh.Cod = t.Cod;
                rh.Place = t.Place;
                if (R(rh, rs))
                {
                    e.Cod = rs.Cod;
                    e.Place = rs.Place;
                    return true;
                }
                return false;
            }
            return false;
        }

        public static bool R(Campo rh, Campo rs)
        {
            Campo r1h = new Campo();
            Campo r1s = new Campo();
            Campo t = new Campo();

            if (IdTokenAtual == TkMais)
            {
                LeToken();
                if (T(t))
                {
                    r1h.Place = GeraRegTemp();
                    r1h.Cod += EscreveCodigo(rh.Cod);
                    r1h.Cod += EscreveCodigo(t.Cod);
                    r1h.Cod += EscreveCodigo("+", r1h.Place, rh.Place, t.Place);
                    if (R(r1h, r1s))
                    {
                        rs.Cod = r1s.Cod;
                        rs.Place = r1s.Place;
                        return true;
                    }
                    return false;
                }
                return false;
            }
            else if (IdTokenAtual == TkMenos)
            {
                LeToken();
                if (T(t))
                {
                    r1h.Place = GeraRegTemp();
                    r1h.Cod += EscreveCodigo(rh.Cod);
                    r1h.Cod += EscreveCodigo(t.Cod);
                    r1h.Cod += EscreveCodigo("-", r1h.Place, rh.Place, t.Place);
                    if (R(r1h, r1s))
                    {
                        rs.Cod = r1s.Cod;
                        rs.Place = r1s.Place;
                        return true;
                    }
                    return false;
                }
                return false;
            }
            else
            {
                rs.Cod = rh.Cod;
                rs.Place = rh.Place;
                return true;
            }
        }

        public static bool T(Campo t)
        {
            Campo f = new Campo();
            Campo rah = new Campo();
            Campo ras = new Campo();
            if (F(f))
            {
                rah.Cod = f.Cod;
                rah.Place = f.Place;
                if (RA(rah, ras))
                {
                    t.Cod = ras.Cod;
                    t.Place = ras.Place;
                    return true;
                }
                return false;
            }
            return false;
        }

        public static bool RA(Campo rah, Campo ras)
        {
            Campo ra1h = new Campo();
            Campo ra1s = new Campo();
            Campo f = new Campo();

            if (IdTokenAtual == TkAsterisco)
            {
                LeToken();
                if (F(f))
                {
                    ra1h.Place = GeraRegTemp();
                    ra1h.Cod += EscreveCodigo(rah.Cod);
                    ra1h.Cod += EscreveCodigo(f.Cod);
                    ra1h.Cod += EscreveCodigo("*", ra1h.Place, rah.Place, f.Place);
                    if (RA(ra1h, ra1s))
                    {
                        ras.Cod = ra1s.Cod;
                        ras.Place = ra1s.Place;
                        return true;
                    }
                    return false;
                }
                return false;
            }
            else if (IdTokenAtual == TkBarraDivisao)
            {
                LeToken();
                if (F(f))
                {
                    ra1h.Place = GeraRegTemp();
                    ra1h.Cod += EscreveCodigo(rah.Cod);
                    ra1h.Cod += EscreveCodigo(f.Cod);
                    ra1h.Cod += EscreveCodigo("/", ra1h.Place, rah.Place, f.Place);
                    if (RA(ra1h, ra1s))
                    {
                        ras.Place = ra1s.Place;
                        ras.Cod = ra1s.Cod;
                        return true;
                    }
                    return false;
                }
                return false;
            }
            else // vazio
            {
                ras.Place = rah.Place;
                ras.Cod = rah.Cod;
                return true;
            }
        }

        public static bool F(Campo f)
        {
            Campo e = new Campo();

            if (IdTokenAtual == TkConst)
            {
                f.Place = GeraRegTemp();
                f.Cod = EscreveCodigo(string.Empty, f.Place, TokenAtual, string.Empty);
                LeToken();
                return true;
            }
            else if (IdTokenAtual == TkAbreParentese)
            {
                LeToken();
                if (E(e))
                {
                    if (IdTokenAtual == TkFechaParentese)
                    {
                        LeToken();
                        f.Place = e.Place;
                        f.Cod = e.Cod;
                        return true;
                    }
                    return false;
                }
                return false;
            }
            else if (IdTokenAtual == TkId)
            {
                f.Place = TokenAtual;
                f.Cod = "";
                LeToken();
                return true;
            }
            return false;
        }

    }
}

