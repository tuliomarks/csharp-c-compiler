using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq.Expressions;

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

        private static readonly List<string> Reservadas = new List<string>() { "int", "float", "char", "struct", "while", "break", "continue", "short", "double", "long", "do", "if", "else" };
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
        private const int TkDo = 10;
        private const int TkIf = 11;
        private const int TkElse = 12;

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
        private const int TkAnd = 124;  //&
        private const int TkLogicalAnd = 125; // &&
        private const int TkOr = 126; // |
        private const int TkLogicalOr = 127; // ||
        private const int TkMaisMais = 128; // ++
        private const int TkMenosMenos = 129; // --
        private const int TkPercentual = 130; // --
        private const int TkExclusiveOr = 131; // --


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
                    if (c == '&')
                    {
                        c = LeChar();
                        estado = 6;
                        continue;
                    }
                    if (c == '|')
                    {
                        c = LeChar();
                        estado = 7;
                        continue;
                    }
                    if (c == '+')
                    {
                        c = LeChar();
                        estado = 8;
                        continue;
                    }
                    if (c == '-')
                    {
                        c = LeChar();
                        estado = 9;
                        continue;
                    }
                    if (c == '/')
                    {
                        c = LeChar();
                        estado = 10;
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
                    if (c == '%')
                    {
                        LeChar();
                        IdTokenAtual = TkPercentual;
                        break;
                    }
                    if (c == '^')
                    {
                        LeChar();
                        IdTokenAtual = TkExclusiveOr;
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
                        IdTokenAtual = TkNegacao;
                        break;
                    }
                }
                else if (estado == 6)
                {
                    if (c == '&')
                    {
                        LeChar();
                        IdTokenAtual = TkLogicalAnd;
                        break;
                    }
                    else
                    {
                        IdTokenAtual = TkAnd;
                        break;
                    }
                }
                else if (estado == 7)
                {
                    if (c == '|')
                    {
                        LeChar();
                        IdTokenAtual = TkLogicalOr;
                        break;
                    }
                    else
                    {
                        IdTokenAtual = TkOr;
                        break;
                    }
                }
                else if (estado == 8)
                {
                    if (c == '+')
                    {
                        LeChar();
                        IdTokenAtual = TkMaisMais;
                        break;
                    }
                    else
                    {
                        IdTokenAtual = TkMais;
                        break;
                    }
                }
                else if (estado == 9)
                {
                    if (c == '-')
                    {
                        LeChar();
                        IdTokenAtual = TkMenosMenos;
                        break;
                    }
                    else
                    {
                        // nesse caso é uma atribuicao
                        IdTokenAtual = TkMenos;
                        break;
                    }
                }
                else if (estado == 10)
                {
                    if (c == '/')
                    {
                       c = LeChar(); // commentario
                        estado = 11;
                        continue;
                    }
                    else if (c == '*')
                    {
                        c = LeChar(); // commentario multilinhas
                        estado = 12;
                        continue;
                    }
                    else
                    {
                        // nesse caso é uma atribuicao
                        IdTokenAtual = TkBarraDivisao;
                        break;
                    }
                }
                else if (estado == 11)
                {
                    if (c == '\n')
                        estado = 0;
                    c = LeChar();
                }
                else if (estado == 12)
                {
                    if (c == '*')
                    {
                        c = LeChar();
                        if (c == '/')
                        {
                            estado = 0;
                        }
                    }
                    c = LeChar();
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
        public static string EscreveCodigo(string comando, string atribuido, string op1, string op2)
        {
            return string.Format("{0} := {1}{2}{3}\n", atribuido, op1, comando, op2);
        }

        public static string EscreveCodigoIf(string expressao, string gotoLabel)
        {
            return string.Format("if {0} goto {1}\n", expressao, gotoLabel);
        }

        public static string EscreveCodigoIfZ(string expressao, string gotoLabel)
        {
            return string.Format("ifZ {0} goto {1}\n", expressao, gotoLabel);
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
        // Statement -> LabeledStatement | CompoundStatement | ExpressionStatement | SelectionStatement | IterationStatement | JumpStatement

        // CompoundStatement -> { BlockItemList } | <vazio>
        // BlockItemList -> BlockItem | BlockItemList BlockItem
        // BlockItem -> Declaration Statement

        // ExpressionStatement -> Expression; | ;

        // SelectionStatement -> if (E) CMD  | if (E) Statement  else Statement

        // CMD -> if (E) CMD 
        // CMD -> if (E) CMD else CMD
        // CMD -> do CMD while (C)
        // CMD -> while (C) CMD
        // CMD -> break
        // CMD -> continue
        // CMD -> id = E       


        // listaCmd -> CMD; listaCont
        // listaCont -> CMD; listaCont
        // listaCont -> <vazio>;                                 

        // Expression -> AssigmentExpression ExpressionRec
        // ExpressionRec -> , AssigmentExpression ExpressionRec | <vazio>

        // AssigmentExpression -> LogicalOrExpression AssigmentExpression

        // AssigmentExpression -> = AssigmentExpression 
        // AssigmentExpression -> *= AssigmentExpression 
        // AssigmentExpression -> /= AssigmentExpression 
        // AssigmentExpression -> %= AssigmentExpression 
        // AssigmentExpression -> += AssigmentExpression 
        // AssigmentExpression -> -= AssigmentExpression        

        // LogicalOrExpression -> LogicalAndExpression LogicalOrExpressionRec
        // LogicalOrExpressionRec -> || LogicalAndExpression LogicalOrExpressionRec
        // LogicalOrExpressionRec -> <vazio>

        // LogicalAndExpression -> InclusiveOrExpression LogicalAndExpressionRec
        // LogicalAndExpressionRec -> && InclusiveOrExpression LogicalAndExpressionRec
        // LogicalAndExpressionRec -> <vazio>

        // InclusiveOrExpression -> ExclusiveOrExpression InclusiveOrExpressionRec
        // InclusiveOrExpressionRec -> | ExclusiveOrExpression InclusiveOrExpressionRec
        // InclusiveOrExpressionRec -> <vazio>

        // ExclusiveOrExpression -> AndExpression ExclusiveOrExpressionRec
        // ExclusiveOrExpressionRec -> ^ AndExpression ExclusiveOrExpressionRec
        // ExclusiveOrExpressionRec -> <vazio>

        // AndExpression -> EqualityExpression AndExpressionRec
        // AndExpressionRec -> & EqualityExpression AndExpressionRec
        // AndExpressionRec -> <vazio>

        // EqualityExpression -> RelationalExpression EqualityExpressionRec
        // EqualityExpressionRec -> == RelationalExpression EqualityExpressionRec
        // EqualityExpressionRec -> != RelationalExpression EqualityExpressionRec
        // EqualityExpressionRec -> <vazio>

        // RelationalExpression -> AdditiveExpression RelationalExpressionRec
        // RelationalExpressionRec -> < AdditiveExpression RelationalExpressionRec
        // RelationalExpressionRec -> > AdditiveExpression RelationalExpressionRec
        // RelationalExpressionRec -> <= AdditiveExpression RelationalExpressionRec
        // RelationalExpressionRec -> >= AdditiveExpression RelationalExpressionRec
        // RelationalExpressionRec -> <vazio>

        // AdditiveExpression -> MultiplicativeExpression AdditiveExpressionRec
        // AdditiveExpressionRec -> + MultiplicativeExpression AdditiveExpressionRec
        // AdditiveExpressionRec -> - MultiplicativeExpression AdditiveExpressionRec
        // AdditiveExpressionRec -> <vazio>

        // MultiplicativeExpression -> PrimaryExpression MultiplicativeExpressionRec
        // MultiplicativeExpressionRec -> * PrimaryExpression MultiplicativeExpressionRec
        // MultiplicativeExpressionRec -> / PrimaryExpression MultiplicativeExpressionRec
        // MultiplicativeExpressionRec -> % PrimaryExpression MultiplicativeExpressionRec
        // MultiplicativeExpressionRec -> <vazio>

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

        /*public static bool Statement(Campo statement)
        {
            var c = new Campo();
            var e = new Campo();
            var listacmd = new Campo();
            var cmd1 = new Campo();
            var cmd2 = new Campo();

            #region if
            if (IdTokenAtual == TkIf)
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

                            cmd1.Rotulo1 = GeraLabel();
                            cmd1.Rotulo2 = GeraLabel();
                            if (Statement(cmd1))
                            {
                                if (IdTokenAtual == TkElse)
                                {
                                    LeToken();

                                    if (Statement(cmd2))
                                    {
                                        statement.Cod += EscreveCodigo(c.Cod);
                                        statement.Cod += EscreveCodigoIfZ(c.Place, cmd1.Rotulo1); // goto else                                    
                                        statement.Cod += EscreveCodigo(cmd1.Cod);
                                        statement.Cod += EscreveCodigo("goto {0}", cmd1.Rotulo2);
                                        statement.Cod += EscreveRotulo(cmd1.Rotulo1);
                                        statement.Cod += EscreveCodigo(cmd2.Cod);
                                        statement.Cod += EscreveRotulo(cmd1.Rotulo2);
                                        return true;
                                    }
                                }
                                else
                                {
                                    statement.Cod += EscreveCodigo(c.Cod);
                                    statement.Cod += EscreveCodigoIfZ(c.Place, cmd1.Rotulo1); // goto else                                    
                                    statement.Cod += EscreveCodigo(cmd1.Cod);
                                    statement.Cod += EscreveRotulo(cmd1.Rotulo1);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region do while
            else if (IdTokenAtual == TkDo)
            {
                LeToken();
                cmd1.Rotulo1 = GeraLabel();
                cmd1.Rotulo2 = GeraLabel();
                if (Statement(cmd1))
                {
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
                                    statement.Cod += EscreveRotulo(cmd1.Rotulo1);
                                    statement.Cod += EscreveCodigo(cmd1.Cod);
                                    statement.Cod += EscreveCodigo(c.Cod);
                                    statement.Cod += EscreveCodigoIf(c.Place, cmd1.Rotulo1);
                                    statement.Cod += EscreveCodigo("goto {0}", cmd1.Rotulo2);
                                    statement.Cod += EscreveRotulo(cmd1.Rotulo2);
                                    return true;
                                }
                            }
                        }
                    }

                }
            }
            #endregion
            #region while
            else if (IdTokenAtual == TkWhile)
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

                            cmd1.Rotulo1 = GeraLabel();
                            cmd1.Rotulo2 = GeraLabel();
                            if (Statement(cmd1))
                            {
                                statement.Cod += EscreveRotulo(cmd1.Rotulo1);
                                statement.Cod += EscreveCodigo(c.Cod);
                                statement.Cod += EscreveCodigoIfZ(c.Place, cmd1.Rotulo2);
                                statement.Cod += EscreveCodigo(cmd1.Cod);
                                statement.Cod += EscreveCodigo("goto {0}", cmd1.Rotulo1);
                                statement.Cod += EscreveRotulo(cmd1.Rotulo2);
                                return true;
                            }
                        }

                    }

                }
            }
            #endregion
            #region break
            else if (IdTokenAtual == TkBreak)
            {
                if (string.IsNullOrEmpty(statement.Rotulo2))
                    return false;

                statement.Cod += EscreveCodigo("goto {0}", statement.Rotulo2);
                LeToken();
                return true;

            }
            #endregion
            #region continue
            else if (IdTokenAtual == TkContinue)
            {
                if (string.IsNullOrEmpty(statement.Rotulo1))
                    return false;

                statement.Cod += EscreveCodigo("goto {0}", statement.Rotulo1);
                LeToken();
                return true;

            }
            #endregion
            else if (IdTokenAtual == TkId)
            {
                statement.Place = TokenAtual;
                LeToken();
                if (IdTokenAtual == TkAtribuicao)
                {
                    LeToken();
                    if (E(e))
                    {
                        statement.Cod += EscreveCodigo(e.Cod);
                        statement.Cod += EscreveCodigo(string.Empty, statement.Place, e.Place, string.Empty);
                        return true;
                    }
                }
            }
            else if (IdTokenAtual == TkAbreChaves)
            {
                LeToken();

                // propaga os labels 
                listacmd.Rotulo2 = statement.Rotulo2;
                listacmd.Rotulo1 = statement.Rotulo1;
                if (ListaCmd(listacmd))
                {
                    if (IdTokenAtual == TkFechaChaves)
                    {
                        LeToken();
                        statement.Cod = listacmd.Cod;
                        return true;
                    }
                }
            }
            return false;
        }
        */
        public static bool Statement(Campo statement)
        {
            var compoundStatement = new Campo();
            var expressionStatement = new Campo();
            var selectionStatement = new Campo();

            if (CompoundStatement(compoundStatement))
            {
                statement.Cod = compoundStatement.Cod;
                return true;
            }
            else if (ExpressionStatement(expressionStatement))
            {
                statement.Cod = expressionStatement.Cod;
                return true;
            }
            else if (SelectionStatement(selectionStatement))
            {
                statement.Cod = selectionStatement.Cod;
                return true;
            }
            return false;
        }

        public static bool CompoundStatement(Campo compoundStatement)
        {
            var statement = new Campo();

            if (IdTokenAtual == TkAbreChaves)
            {
                LeToken();

                // propaga os labels 
                statement.Rotulo2 = compoundStatement.Rotulo2;
                statement.Rotulo1 = compoundStatement.Rotulo1;
                if (Statement(statement))
                {
                    if (IdTokenAtual == TkFechaChaves)
                    {
                        LeToken();
                        compoundStatement.Cod = statement.Cod;
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool BlockItemList(Campo blockItemList)
        {
            var blockItem = new Campo();
            var blockItemList1 = new Campo();
            if (BlockItem(blockItem))
            {
                blockItemList.Cod = blockItem.Cod;
                if (BlockItemList(blockItemList1))
                {
                    blockItemList.Cod += blockItemList1.Cod;
                }
                return true;
            }
            return false;

        }

        public static bool BlockItem(Campo blockItem)
        {
            //var declaration = new Campo();
            var statement = new Campo();
            //if (Declaration(blockItem))
            //{
            if (Statement(statement))
            {
                blockItem.Cod = statement.Cod;
                return true;
            }
            //}
            return false;

        }

        public static bool SelectionStatement(Campo selectionStatement)
        {
            var c = new Campo();
            var statement1 = new Campo();
            var statement2 = new Campo();

            if (IdTokenAtual == TkIf)
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

                            statement1.Rotulo1 = GeraLabel();
                            statement1.Rotulo2 = GeraLabel();
                            if (Statement(statement1))
                            {
                                if (IdTokenAtual == TkElse)
                                {
                                    LeToken();
                                    if (Statement(statement2))
                                    {
                                        selectionStatement.Cod += EscreveCodigo(c.Cod);
                                        selectionStatement.Cod += EscreveCodigoIfZ(c.Place, statement1.Rotulo1); // goto else                                    
                                        selectionStatement.Cod += EscreveCodigo(statement1.Cod);
                                        selectionStatement.Cod += EscreveCodigo("goto {0}", statement1.Rotulo2);
                                        selectionStatement.Cod += EscreveRotulo(statement1.Rotulo1);
                                        selectionStatement.Cod += EscreveCodigo(statement2.Cod);
                                        selectionStatement.Cod += EscreveRotulo(statement1.Rotulo2);
                                        return true;
                                    }
                                }
                                else
                                {
                                    selectionStatement.Cod += EscreveCodigo(c.Cod);
                                    selectionStatement.Cod += EscreveCodigoIfZ(c.Place, statement1.Rotulo1); // goto else                                    
                                    selectionStatement.Cod += EscreveCodigo(statement1.Cod);
                                    selectionStatement.Cod += EscreveRotulo(statement1.Rotulo1);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static bool ExpressionStatement(Campo expressionStatement)
        {
            var expression = new Campo();
            if (Expression(expression))
            {
                if (IdTokenAtual == TkPontoVirgula)
                {
                    LeToken();
                    expressionStatement.Cod = expression.Cod;
                    return true;
                }
            }
            else if (IdTokenAtual == TkPontoVirgula)
            {
                LeToken();
                return true;
            }
            return false;
        }

        public static bool Expression(Campo expression)
        {
            var assignmentExpression = new Campo();
            var expressionRecS = new Campo();
            var expressionRecH = new Campo();
            if (AssignmentExpression(assignmentExpression))
            {
                expressionRecH.Cod = assignmentExpression.Cod;
                expressionRecH.Place = assignmentExpression.Place;
                if (ExpressionRec(expressionRecH, expressionRecS))
                {
                    expression.Cod = expressionRecS.Cod;
                    expression.Place = expressionRecS.Place;
                    return true;
                }
            }
            return false;
        }

        public static bool ExpressionRec(Campo expressionRecH, Campo expressionRecS)
        {
            var assignmentExpression = new Campo();
            var expressionRec1H = new Campo();
            var expressionRec1S = new Campo();
            if (IdTokenAtual == TkVirgula)
            {
                LeToken();
                if (AssignmentExpression(assignmentExpression))
                {
                    expressionRec1H.Cod += expressionRecH.Cod;
                    expressionRec1H.Cod += assignmentExpression.Cod;
                    if (ExpressionRec(expressionRec1H, expressionRec1S))
                    {
                        expressionRecS.Cod = expressionRec1S.Cod;
                        expressionRecS.Place = expressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else
            {
                expressionRecS.Cod = expressionRecH.Cod;
                expressionRecS.Place = expressionRecH.Place;
                return true;
            }
        }

        /*
        public static bool AssignmentExpression(Campo assignmentExpression)
        {
            var c = new Campo();
            if (IdTokenAtual == TkId)
            {
                assignmentExpression.Place = TokenAtual;
                LeToken();
                if (IdTokenAtual == TkAtribuicao)
                {
                    LeToken();
                    if (C(c))
                    {
                        assignmentExpression.Cod += EscreveCodigo(c.Cod);
                        assignmentExpression.Cod += EscreveCodigo(string.Empty, assignmentExpression.Place, c.Place, string.Empty);
                        return true;
                    }
                }
            }
            else if (LogicalOrExpression(logicalOrExpression))
            {
                
            }
            return false;
        }

         */

        public static bool AssignmentExpression(Campo assignmentExpression)
        {
            var logicalOrExpression = new Campo();
            var assignmentOperator = new Campo();
            var assignmentExpression1 = new Campo();

            if (LogicalOrExpression(logicalOrExpression))
            {
                assignmentExpression.Cod = logicalOrExpression.Cod;
                assignmentExpression.Place = logicalOrExpression.Place;

                if (AssignmentOperator(assignmentOperator)) // identifica o tipo de operador
                {
                    if (AssignmentExpression(assignmentExpression1))
                    {
                        assignmentExpression.Cod += EscreveCodigo(assignmentExpression1.Cod);
                        assignmentExpression.Cod += EscreveCodigo(string.Empty, logicalOrExpression.Place, assignmentExpression1.Place, string.Empty);
                        return true;
                    }
                }
                return true;
            }            
            return false;
        }

        public static bool AssignmentOperator(Campo assignmentOperator)
        {
            if (IdTokenAtual == TkAtribuicao)
            {
                assignmentOperator.Cod = TokenAtual;
                LeToken();
                return true;
            }
            return false;
        }

        public static bool LogicalOrExpression(Campo logicalOrExpression)
        {
            var logicalAndExpression = new Campo();
            var logicalOrExpressionRecH = new Campo();
            var logicalOrExpressionRecS = new Campo();
            if (LogicalAndExpression(logicalAndExpression))
            {
                logicalOrExpressionRecH.Cod = logicalAndExpression.Cod;
                logicalOrExpressionRecH.Place = logicalAndExpression.Place;
                if (LogicalOrExpressionRec(logicalOrExpressionRecH, logicalOrExpressionRecS))
                {
                    logicalOrExpression.Cod = logicalOrExpressionRecS.Cod;
                    logicalOrExpression.Place = logicalOrExpressionRecS.Place;
                    return true;
                }
            }
            return false;
        }

        public static bool LogicalOrExpressionRec(Campo logicalOrExpressionRecH, Campo logicalOrExpressionRecS)
        {
            var logicalAndExpression = new Campo();
            var logicalOrExpressionRec1H = new Campo();
            var logicalOrExpressionRec1S = new Campo();
            if (IdTokenAtual == TkLogicalOr)
            {
                LeToken();
                if (LogicalAndExpression(logicalAndExpression))
                {
                    logicalOrExpressionRec1H.Place = GeraRegTemp();
                    logicalOrExpressionRec1H.Cod += EscreveCodigo(logicalOrExpressionRecH.Cod);
                    logicalOrExpressionRec1H.Cod += EscreveCodigo(logicalAndExpression.Cod);
                    logicalOrExpressionRec1H.Cod += EscreveCodigo("||", logicalOrExpressionRec1H.Place, logicalOrExpressionRecH.Place, logicalAndExpression.Place);
                    if (LogicalOrExpressionRec(logicalOrExpressionRec1H, logicalOrExpressionRec1S))
                    {
                        logicalOrExpressionRecS.Cod = logicalOrExpressionRec1S.Cod;
                        logicalOrExpressionRecS.Place = logicalOrExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }            
            else
            {
                logicalOrExpressionRecS.Cod = logicalOrExpressionRecH.Cod;
                logicalOrExpressionRecS.Place = logicalOrExpressionRecH.Place;
                return true;
            }
        }

        public static bool LogicalAndExpression(Campo logicalAndExpression)
        {
            var inclusiveOrExpression = new Campo();
            var logicalAndExpressionRecH = new Campo();
            var logicalAndExpressionRecS = new Campo();
            if (InclusiveOrExpression(inclusiveOrExpression))
            {
                logicalAndExpressionRecH.Cod = inclusiveOrExpression.Cod;
                logicalAndExpressionRecH.Place = inclusiveOrExpression.Place;
                if (LogicalAndExpressionRec(logicalAndExpressionRecH, logicalAndExpressionRecS))
                {
                    logicalAndExpression.Cod = logicalAndExpressionRecS.Cod;
                    logicalAndExpression.Place = logicalAndExpressionRecS.Place;
                    return true;
                }
            }
            return false;
        }

        public static bool LogicalAndExpressionRec(Campo logicalAndExpressionRecH, Campo logicalAndExpressionRecS)
        {
            var inclusiveOrExpression = new Campo();
            var logicalAndExpressionRec1H = new Campo();
            var logicalAndExpressionRec1S = new Campo();
            if (IdTokenAtual == TkLogicalAnd)
            {
                LeToken();
                if (InclusiveOrExpression(inclusiveOrExpression))
                {
                    logicalAndExpressionRec1H.Place = GeraRegTemp();
                    logicalAndExpressionRec1H.Cod += EscreveCodigo(logicalAndExpressionRecH.Cod);
                    logicalAndExpressionRec1H.Cod += EscreveCodigo(inclusiveOrExpression.Cod);
                    logicalAndExpressionRec1H.Cod += EscreveCodigo("&&", logicalAndExpressionRec1H.Place, logicalAndExpressionRecH.Place, inclusiveOrExpression.Place);
                    if (LogicalAndExpressionRec(logicalAndExpressionRec1H, logicalAndExpressionRec1S))
                    {
                        logicalAndExpressionRecS.Cod = logicalAndExpressionRec1S.Cod;
                        logicalAndExpressionRecS.Place = logicalAndExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else
            {
                logicalAndExpressionRecS.Cod = logicalAndExpressionRecH.Cod;
                logicalAndExpressionRecS.Place = logicalAndExpressionRecH.Place;
                return true;
            }
        }

        public static bool InclusiveOrExpression(Campo inclusiveOrExpression)
        {
            var exclusiveOrExpression = new Campo();
            var inclusiveOrExpressionRecH = new Campo();
            var inclusiveOrExpressionRecS = new Campo();
            if (ExclusiveOrExpression(exclusiveOrExpression))
            {
                inclusiveOrExpressionRecH.Cod = exclusiveOrExpression.Cod;
                inclusiveOrExpressionRecH.Place = exclusiveOrExpression.Place;
                if (InclusiveOrExpressionRec(inclusiveOrExpressionRecH, inclusiveOrExpressionRecS))
                {
                    inclusiveOrExpression.Cod = inclusiveOrExpressionRecS.Cod;
                    inclusiveOrExpression.Place = inclusiveOrExpressionRecS.Place;
                    return true;
                }
            }
            return false;
        }

        public static bool InclusiveOrExpressionRec(Campo inclusiveOrExpressionRecH, Campo inclusiveOrExpressionRecS)
        {
            var exclusiveOrExpression = new Campo();
            var inclusiveOrExpressionRec1H = new Campo();
            var inclusiveOrExpressionRec1S = new Campo();
            if (IdTokenAtual == TkOr)
            {
                LeToken();
                if (ExclusiveOrExpression(exclusiveOrExpression))
                {
                    inclusiveOrExpressionRec1H.Place = GeraRegTemp();
                    inclusiveOrExpressionRec1H.Cod += EscreveCodigo(inclusiveOrExpressionRecH.Cod);
                    inclusiveOrExpressionRec1H.Cod += EscreveCodigo(exclusiveOrExpression.Cod);
                    inclusiveOrExpressionRec1H.Cod += EscreveCodigo("|", inclusiveOrExpressionRec1H.Place, inclusiveOrExpressionRecH.Place, exclusiveOrExpression.Place);
                    if (InclusiveOrExpressionRec(inclusiveOrExpressionRec1H, inclusiveOrExpressionRec1S))
                    {
                        inclusiveOrExpressionRecS.Cod = inclusiveOrExpressionRec1S.Cod;
                        inclusiveOrExpressionRecS.Place = inclusiveOrExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else
            {
                inclusiveOrExpressionRecS.Cod = inclusiveOrExpressionRecH.Cod;
                inclusiveOrExpressionRecS.Place = inclusiveOrExpressionRecH.Place;
                return true;
            }
        }

        public static bool ExclusiveOrExpression(Campo exclusiveOrExpression)
        {
            var andExpression = new Campo();
            var exclusiveOrExpressionRecH = new Campo();
            var exclusiveOrExpressionRecS = new Campo();
            if (AndExpression(andExpression))
            {
                exclusiveOrExpressionRecH.Cod = andExpression.Cod;
                exclusiveOrExpressionRecH.Place = andExpression.Place;
                if (ExclusiveOrExpressionRec(exclusiveOrExpressionRecH, exclusiveOrExpressionRecS))
                {
                    exclusiveOrExpression.Cod = exclusiveOrExpressionRecS.Cod;
                    exclusiveOrExpression.Place = exclusiveOrExpressionRecS.Place;
                    return true;
                }
            }
            return false;
        }

        public static bool ExclusiveOrExpressionRec(Campo exclusiveOrExpressionRecH, Campo exclusiveOrExpressionRecS)
        {
            var andExpression = new Campo();
            var exclusiveOrExpressionRec1H = new Campo();
            var exclusiveOrExpressionRec1S = new Campo();
            if (IdTokenAtual == TkExclusiveOr)
            {
                LeToken();
                if (AndExpression(andExpression))
                {
                    exclusiveOrExpressionRec1H.Place = GeraRegTemp();
                    exclusiveOrExpressionRec1H.Cod += EscreveCodigo(exclusiveOrExpressionRecH.Cod);
                    exclusiveOrExpressionRec1H.Cod += EscreveCodigo(andExpression.Cod);
                    exclusiveOrExpressionRec1H.Cod += EscreveCodigo("^", exclusiveOrExpressionRec1H.Place, exclusiveOrExpressionRecH.Place, andExpression.Place);
                    if (ExclusiveOrExpressionRec(exclusiveOrExpressionRec1H, exclusiveOrExpressionRec1S))
                    {
                        exclusiveOrExpressionRecS.Cod = exclusiveOrExpressionRec1S.Cod;
                        exclusiveOrExpressionRecS.Place = exclusiveOrExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else
            {
                exclusiveOrExpressionRecS.Cod = exclusiveOrExpressionRecH.Cod;
                exclusiveOrExpressionRecS.Place = exclusiveOrExpressionRecH.Place;
                return true;
            }
        }

        public static bool AndExpression(Campo andExpression)
        {
            var equalityExpression = new Campo();
            var andExpressionRecH = new Campo();
            var andExpressionRecS = new Campo();
            if (EqualityExpression(equalityExpression))
            {
                andExpressionRecH.Cod = equalityExpression.Cod;
                andExpressionRecH.Place = equalityExpression.Place;
                if (AndExpressionRec(andExpressionRecH, andExpressionRecS))
                {
                    andExpression.Cod = andExpressionRecS.Cod;
                    andExpression.Place = andExpressionRecS.Place;
                    return true;
                }
            }
            return false;
        }

        public static bool AndExpressionRec(Campo andExpressionRecH, Campo andExpressionRecS)
        {
            var equalityExpression = new Campo();
            var andExpressionRec1H = new Campo();
            var andExpressionRec1S = new Campo();
            if (IdTokenAtual == TkAnd)
            {
                LeToken();
                if (EqualityExpression(equalityExpression))
                {
                    andExpressionRec1H.Place = GeraRegTemp();
                    andExpressionRec1H.Cod += EscreveCodigo(andExpressionRecH.Cod);
                    andExpressionRec1H.Cod += EscreveCodigo(equalityExpression.Cod);
                    andExpressionRec1H.Cod += EscreveCodigo("&", andExpressionRec1H.Place, andExpressionRecH.Place, equalityExpression.Place);
                    if (AndExpressionRec(andExpressionRec1H, andExpressionRec1S))
                    {
                        andExpressionRecS.Cod = andExpressionRec1S.Cod;
                        andExpressionRecS.Place = andExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else
            {
                andExpressionRecS.Cod = andExpressionRecH.Cod;
                andExpressionRecS.Place = andExpressionRecH.Place;
                return true;
            }
        }

        public static bool EqualityExpression(Campo equalityExpression)
        {
            var relationalExpression = new Campo();
            var equalityExpressionRecH = new Campo();
            var equalityExpressionRecS = new Campo();
            if (RelationalExpression(relationalExpression))
            {
                equalityExpressionRecH.Cod = relationalExpression.Cod;
                equalityExpressionRecH.Place = relationalExpression.Place;
                if (EqualityExpressionRec(equalityExpressionRecH, equalityExpressionRecS))
                {
                    equalityExpression.Cod = equalityExpressionRecS.Cod;
                    equalityExpression.Place = equalityExpressionRecS.Place;
                    return true;
                }
            }
            return false;
        }

        public static bool EqualityExpressionRec(Campo equalityExpressionRecH, Campo equalityExpressionRecS)
        {
            var relationalExpression = new Campo();
            var equalityExpressionRec1H = new Campo();
            var equalityExpressionRec1S = new Campo();
            if (IdTokenAtual == TkIgual)
            {
                LeToken();
                if (RelationalExpression(relationalExpression))
                {
                    equalityExpressionRec1H.Place = GeraRegTemp();
                    equalityExpressionRec1H.Cod += EscreveCodigo(equalityExpressionRecH.Cod);
                    equalityExpressionRec1H.Cod += EscreveCodigo(relationalExpression.Cod);
                    equalityExpressionRec1H.Cod += EscreveCodigo("==", equalityExpressionRec1H.Place, equalityExpressionRecH.Place, relationalExpression.Place);
                    if (EqualityExpressionRec(equalityExpressionRec1H, equalityExpressionRec1S))
                    {
                        equalityExpressionRecS.Cod = equalityExpressionRec1S.Cod;
                        equalityExpressionRecS.Place = equalityExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else if (IdTokenAtual == TkDiferente)
            {
                LeToken();
                if (RelationalExpression(relationalExpression))
                {
                    equalityExpressionRec1H.Place = GeraRegTemp();
                    equalityExpressionRec1H.Cod += EscreveCodigo(equalityExpressionRecH.Cod);
                    equalityExpressionRec1H.Cod += EscreveCodigo(relationalExpression.Cod);
                    equalityExpressionRec1H.Cod += EscreveCodigo("!=", equalityExpressionRec1H.Place, equalityExpressionRecH.Place, relationalExpression.Place);
                    if (EqualityExpressionRec(equalityExpressionRec1H, equalityExpressionRec1S))
                    {
                        equalityExpressionRecS.Cod = equalityExpressionRec1S.Cod;
                        equalityExpressionRecS.Place = equalityExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else
            {
                equalityExpressionRecS.Cod = equalityExpressionRecH.Cod;
                equalityExpressionRecS.Place = equalityExpressionRecH.Place;
                return true;
            }
        }

        public static bool RelationalExpression(Campo relationalExpression)
        {
            var addictiveExpression = new Campo();
            var relationalExpressionRecH = new Campo();
            var relationalExpressionRecS = new Campo();
            if (AddictiveExpression(addictiveExpression))
            {
                relationalExpressionRecH.Cod = addictiveExpression.Cod;
                relationalExpressionRecH.Place = addictiveExpression.Place;
                if (RelationalExpressionRec(relationalExpressionRecH, relationalExpressionRecS))
                {
                    relationalExpression.Cod = relationalExpressionRecS.Cod;
                    relationalExpression.Place = relationalExpressionRecS.Place;
                    return true;
                }
            }
            return false;
        }

        public static bool RelationalExpressionRec(Campo relationalExpressionRecH, Campo relationalExpressionRecS)
        {
            var addictiveExpression = new Campo();
            var relationalExpressionRec1H = new Campo();
            var relationalExpressionRec1S = new Campo();
            if (IdTokenAtual == TkMenor)
            {
                LeToken();
                if (AddictiveExpression(addictiveExpression))
                {
                    relationalExpressionRec1H.Place = GeraRegTemp();
                    relationalExpressionRec1H.Cod += EscreveCodigo(relationalExpressionRecH.Cod);
                    relationalExpressionRec1H.Cod += EscreveCodigo(addictiveExpression.Cod);
                    relationalExpressionRec1H.Cod += EscreveCodigo("<", relationalExpressionRec1H.Place, relationalExpressionRecH.Place, addictiveExpression.Place);
                    if (RelationalExpressionRec(relationalExpressionRec1H, relationalExpressionRec1S))
                    {
                        relationalExpressionRecS.Cod = relationalExpressionRec1S.Cod;
                        relationalExpressionRecS.Place = relationalExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else if (IdTokenAtual == TkMaior)
            {
                LeToken();
                if (AddictiveExpression(addictiveExpression))
                {
                    relationalExpressionRec1H.Place = GeraRegTemp();
                    relationalExpressionRec1H.Cod += EscreveCodigo(relationalExpressionRecH.Cod);
                    relationalExpressionRec1H.Cod += EscreveCodigo(addictiveExpression.Cod);
                    relationalExpressionRec1H.Cod += EscreveCodigo(">", relationalExpressionRec1H.Place, relationalExpressionRecH.Place, addictiveExpression.Place);
                    if (RelationalExpressionRec(relationalExpressionRec1H, relationalExpressionRec1S))
                    {
                        relationalExpressionRecS.Cod = relationalExpressionRec1S.Cod;
                        relationalExpressionRecS.Place = relationalExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else if (IdTokenAtual == TkMenorIgual)
            {
                LeToken();
                if (AddictiveExpression(addictiveExpression))
                {
                    relationalExpressionRec1H.Place = GeraRegTemp();
                    relationalExpressionRec1H.Cod += EscreveCodigo(relationalExpressionRecH.Cod);
                    relationalExpressionRec1H.Cod += EscreveCodigo(addictiveExpression.Cod);
                    relationalExpressionRec1H.Cod += EscreveCodigo("<=", relationalExpressionRec1H.Place, relationalExpressionRecH.Place, addictiveExpression.Place);
                    if (RelationalExpressionRec(relationalExpressionRec1H, relationalExpressionRec1S))
                    {
                        relationalExpressionRecS.Cod = relationalExpressionRec1S.Cod;
                        relationalExpressionRecS.Place = relationalExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else if (IdTokenAtual == TkMaiorIgual)
            {
                LeToken();
                if (AddictiveExpression(addictiveExpression))
                {
                    relationalExpressionRec1H.Place = GeraRegTemp();
                    relationalExpressionRec1H.Cod += EscreveCodigo(relationalExpressionRecH.Cod);
                    relationalExpressionRec1H.Cod += EscreveCodigo(addictiveExpression.Cod);
                    relationalExpressionRec1H.Cod += EscreveCodigo(">=", relationalExpressionRec1H.Place, relationalExpressionRecH.Place, addictiveExpression.Place);
                    if (RelationalExpressionRec(relationalExpressionRec1H, relationalExpressionRec1S))
                    {
                        relationalExpressionRecS.Cod = relationalExpressionRec1S.Cod;
                        relationalExpressionRecS.Place = relationalExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else 
            {
                relationalExpressionRecS.Cod = relationalExpressionRecH.Cod;
                relationalExpressionRecS.Place = relationalExpressionRecH.Place;
                return true;
            }
        }

        public static bool AddictiveExpression(Campo addictiveExpression)
        {
            var multiplicativeExpression = new Campo();
            var addictiveExpressionRecH = new Campo();
            var addictiveExpressionRecS = new Campo();
            if (MultiplicativeExpression(multiplicativeExpression))
            {
                addictiveExpressionRecH.Cod = multiplicativeExpression.Cod;
                addictiveExpressionRecH.Place = multiplicativeExpression.Place;
                if (AddictiveExpressionRec(addictiveExpressionRecH, addictiveExpressionRecS))
                {
                    addictiveExpression.Cod = addictiveExpressionRecS.Cod;
                    addictiveExpression.Place = addictiveExpressionRecS.Place;
                    return true;
                }
            }
            return false;
        }

        public static bool AddictiveExpressionRec(Campo addictiveExpressionRecH, Campo addictiveExpressionRecS)
        {
            var multiplicativeExpression = new Campo();
            var addictiveExpressionRec1H = new Campo();
            var addictiveExpressionRec1S = new Campo();
            if (IdTokenAtual == TkMais)
            {
                LeToken();
                if (MultiplicativeExpression(multiplicativeExpression))
                {
                    addictiveExpressionRec1H.Place = GeraRegTemp();
                    addictiveExpressionRec1H.Cod += EscreveCodigo(addictiveExpressionRecH.Cod);
                    addictiveExpressionRec1H.Cod += EscreveCodigo(multiplicativeExpression.Cod);
                    addictiveExpressionRec1H.Cod += EscreveCodigo("+", addictiveExpressionRec1H.Place, addictiveExpressionRecH.Place, multiplicativeExpression.Place);
                    if (AddictiveExpressionRec(addictiveExpressionRec1H, addictiveExpressionRec1S))
                    {
                        addictiveExpressionRecS.Cod = addictiveExpressionRec1S.Cod;
                        addictiveExpressionRecS.Place = addictiveExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else if (IdTokenAtual == TkMenos)
            {
                LeToken();
                if (MultiplicativeExpression(multiplicativeExpression))
                {
                    addictiveExpressionRec1H.Place = GeraRegTemp();
                    addictiveExpressionRec1H.Cod += EscreveCodigo(addictiveExpressionRecH.Cod);
                    addictiveExpressionRec1H.Cod += EscreveCodigo(multiplicativeExpression.Cod);
                    addictiveExpressionRec1H.Cod += EscreveCodigo("-", addictiveExpressionRec1H.Place, addictiveExpressionRecH.Place, multiplicativeExpression.Place);
                    if (AddictiveExpressionRec(addictiveExpressionRec1H, addictiveExpressionRec1S))
                    {
                        addictiveExpressionRecS.Cod = addictiveExpressionRec1S.Cod;
                        addictiveExpressionRecS.Place = addictiveExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else
            {
                addictiveExpressionRecS.Cod = addictiveExpressionRecH.Cod;
                addictiveExpressionRecS.Place = addictiveExpressionRecH.Place;
                return true;
            }
        }

        public static bool MultiplicativeExpression(Campo multiplicativeExpression)
        {
            var primaryExpression = new Campo();
            var multiplicativeExpressionRecH = new Campo();
            var multiplicativeExpressionRecS = new Campo();
            if (PrimaryExpression(primaryExpression))
            {
                multiplicativeExpressionRecH.Cod = primaryExpression.Cod;
                multiplicativeExpressionRecH.Place = primaryExpression.Place;
                if (MultiplicativeExpressionRec(multiplicativeExpressionRecH, multiplicativeExpressionRecS))
                {
                    multiplicativeExpression.Cod = multiplicativeExpressionRecS.Cod;
                    multiplicativeExpression.Place = multiplicativeExpressionRecS.Place;
                    return true;
                }
            }
            return false;
        }

        public static bool MultiplicativeExpressionRec(Campo multiplicativeExpressionRecH, Campo multiplicativeExpressionRecS)
        {
            var primaryExpression = new Campo();
            var multiplicativeExpressionRec1H = new Campo();
            var multiplicativeExpressionRec1S = new Campo();
            if (IdTokenAtual == TkAsterisco)
            {
                LeToken();
                if (PrimaryExpression(primaryExpression))
                {
                    multiplicativeExpressionRec1H.Place = GeraRegTemp();
                    multiplicativeExpressionRec1H.Cod += EscreveCodigo(multiplicativeExpressionRecH.Cod);
                    multiplicativeExpressionRec1H.Cod += EscreveCodigo(primaryExpression.Cod);
                    multiplicativeExpressionRec1H.Cod += EscreveCodigo("*", multiplicativeExpressionRec1H.Place, multiplicativeExpressionRecH.Place, primaryExpression.Place);
                    if (MultiplicativeExpressionRec(multiplicativeExpressionRec1H, multiplicativeExpressionRec1S))
                    {
                        multiplicativeExpressionRecS.Cod = multiplicativeExpressionRec1S.Cod;
                        multiplicativeExpressionRecS.Place = multiplicativeExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else if (IdTokenAtual == TkBarraDivisao)
            {
                LeToken();
                if (PrimaryExpression(primaryExpression))
                {
                    multiplicativeExpressionRec1H.Place = GeraRegTemp();
                    multiplicativeExpressionRec1H.Cod += EscreveCodigo(multiplicativeExpressionRecH.Cod);
                    multiplicativeExpressionRec1H.Cod += EscreveCodigo(primaryExpression.Cod);
                    multiplicativeExpressionRec1H.Cod += EscreveCodigo("/", multiplicativeExpressionRec1H.Place, multiplicativeExpressionRecH.Place, primaryExpression.Place);
                    if (MultiplicativeExpressionRec(multiplicativeExpressionRec1H, multiplicativeExpressionRec1S))
                    {
                        multiplicativeExpressionRecS.Cod = multiplicativeExpressionRec1S.Cod;
                        multiplicativeExpressionRecS.Place = multiplicativeExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else if (IdTokenAtual == TkPercentual)
            {
                LeToken();
                if (PrimaryExpression(primaryExpression))
                {
                    multiplicativeExpressionRec1H.Place = GeraRegTemp();
                    multiplicativeExpressionRec1H.Cod += EscreveCodigo(multiplicativeExpressionRecH.Cod);
                    multiplicativeExpressionRec1H.Cod += EscreveCodigo(primaryExpression.Cod);
                    multiplicativeExpressionRec1H.Cod += EscreveCodigo("%", multiplicativeExpressionRec1H.Place, multiplicativeExpressionRecH.Place, primaryExpression.Place);
                    if (MultiplicativeExpressionRec(multiplicativeExpressionRec1H, multiplicativeExpressionRec1S))
                    {
                        multiplicativeExpressionRecS.Cod = multiplicativeExpressionRec1S.Cod;
                        multiplicativeExpressionRecS.Place = multiplicativeExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else
            {
                multiplicativeExpressionRecS.Cod = multiplicativeExpressionRecH.Cod;
                multiplicativeExpressionRecS.Place = multiplicativeExpressionRecH.Place;
                return true;
            }
        }

        /*
        public static bool UnaryExpression(Campo unaryExpression)
        {
            var postFixExpression = new Campo();
            var unaryExpression1 = new Campo();
            if (PostFixExpression(postFixExpression))
            {
                unaryExpression.Cod = postFixExpression.Cod;
                return true;
            }
            else if (IdTokenAtual == TkMaisMais)
            {
                LeToken();
                if (UnaryExpression(unaryExpression1))
                {
                    unaryExpression.Cod = unaryExpression1.Cod;
                    return true;
                }
            }
            else if (IdTokenAtual == TkMenosMenos)
            {
                LeToken();
                if (UnaryExpression(unaryExpression1))
                {
                    unaryExpression.Cod = unaryExpression1.Cod;
                    return true;
                }
            }
            return false;
        }
         */
        /*         
        public static bool PostFixExpression(Campo postFixExpression)
        {
            var primaryExpression = new Campo();
            var unaryExpression1 = new Campo();
            if (PrimaryExpression(primaryExpression))
            {
                postFixExpression.Place = primaryExpression.Place;
                if (IdTokenAtual == TkMaisMais)
                {
                    LeToken();
                    if (UnaryExpression(unaryExpression1))
                    {
                        postFixExpression.Cod += unaryExpression1.Cod;
                        return true;
                    }
                }
                else if (IdTokenAtual == TkMenosMenos)
                {
                    LeToken();
                    if (UnaryExpression(unaryExpression1))
                    {
                        postFixExpression.Cod += unaryExpression1.Cod;
                        return true;
                    }
                }
                return true;
            }
            return false;
        }
        */

        public static bool PrimaryExpression(Campo primaryExpression)
        {
            var expression = new Campo();
            if (IdTokenAtual == TkConst)
            {
                primaryExpression.Place = TokenAtual;
                LeToken();
                return true;
            }
            else if (IdTokenAtual == TkId)
            {
                primaryExpression.Place = TokenAtual;
                LeToken();
                return true;
            }
            else if (IdTokenAtual == TkAbreParentese)
            {
                LeToken();
                if (Expression(expression))
                {
                    if (IdTokenAtual == TkFechaParentese)
                    {
                        LeToken();
                        primaryExpression.Cod = expression.Cod;
                        primaryExpression.Place = expression.Place;
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
            cmd.Rotulo2 = listacmd.Rotulo2;
            cmd.Rotulo1 = listacmd.Rotulo1;
            if (Statement(cmd))
            {
                if (IdTokenAtual == TkPontoVirgula)
                {
                    LeToken();

                    // propaga os labels 
                    listacont.Rotulo2 = cmd.Rotulo2;
                    listacont.Rotulo1 = cmd.Rotulo1;
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
            cmd.Rotulo2 = listacont.Rotulo2;
            cmd.Rotulo1 = listacont.Rotulo1;
            if (Statement(cmd))
            {
                if (IdTokenAtual == TkPontoVirgula)
                {
                    LeToken();

                    // propaga os labels 
                    listacont1.Rotulo2 = cmd.Rotulo2;
                    listacont1.Rotulo1 = cmd.Rotulo1;
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

        public static bool E(Campo expression)
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
                    expression.Cod = rs.Cod;
                    expression.Place = rs.Place;
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
                f.Place = TokenAtual;
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

