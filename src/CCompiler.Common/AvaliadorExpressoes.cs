using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;

namespace CCompiler.Common
{
    public static class AvaliadorExpressoes
    {

        public static int IdTokenAtual { get; set; }
        public static string TokenAtual { get; set; }
        public static int LinhaTokenAtual { get; set; }
        public static int ColunaTokenAtual { get; set; }
        public static List<string> TokensAguardados { get; set; }

        public static void Inicializar(string exp)
        {
            TempContador = 0;
            RotuloContador = 0;
            Pos = 0;
            Linha = 1;
            Coluna = 1;
            TokensAguardados = new List<string>();
            Exp = exp.ToCharArray();
            LerToken();
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
        private const int TkAddiction = 109;
        private const int TkSubtraction = 110;
        private const int TkMultiplication = 111;
        private const int TkDivision = 112;
        private const int TkDoisPontos = 113;
        private const int TkIgual = 114;
        private const int TkAbreChaves = 115;
        private const int TkFechaChaves = 116;
        private const int TkAssignment = 117;
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
        private const int TkDoublePlus = 128; // ++
        private const int TkDoubleMinus = 129; // --
        private const int TkRemainder = 130; // --
        private const int TkExclusiveOr = 131; // --
        private const int TkMultiplicationAssignment = 132; // *=
        private const int TkDivisionAssignment = 133; // /=
        private const int TkAdditionAssignment = 134; // +=
        private const int TkSubtractionAssignment = 135; // -=
        private const int TkRemainderAssignment = 136; // -=

        private static char[] Exp { get; set; }
        private static int Pos { get; set; }
        private static int Linha { get; set; }
        private static int Coluna { get; set; }

        private static void LerToken()
        {
            int estado = 0;
            char? c = Exp[Pos];

            while (true)
            {
                if (estado == 0)
                {
                    LinhaTokenAtual = Linha;
                    ColunaTokenAtual = Coluna;

                    TokenAtual = c.ToString();
                    if (c == '!')
                    {
                        c = LerChar();
                        estado = 5;
                        continue;
                    }
                    if (c == '=')
                    {
                        c = LerChar();
                        estado = 2;
                        continue;
                    }
                    if (c == '>')
                    {
                        c = LerChar();
                        estado = 3;
                        continue;
                    }
                    if (c == '<')
                    {
                        c = LerChar();
                        estado = 4;
                        continue;
                    }
                    if (c == '&')
                    {
                        c = LerChar();
                        estado = 6;
                        continue;
                    }
                    if (c == '|')
                    {
                        c = LerChar();
                        estado = 7;
                        continue;
                    }
                    if (c == '+')
                    {
                        c = LerChar();
                        estado = 8;
                        continue;
                    }
                    if (c == '-')
                    {
                        c = LerChar();
                        estado = 9;
                        continue;
                    }
                    if (c == '/')
                    {
                        c = LerChar();
                        estado = 10;
                        continue;
                    }
                    if (c == '*')
                    {
                        c = LerChar();
                        estado = 13;
                        continue;
                    }
                    if (c == '%')
                    {
                        c = LerChar();
                        estado = 14;
                        continue;
                    }

                    /* de 1 unico char */
                    if (c == '{')
                    {
                        LerChar();
                        IdTokenAtual = TkAbreChaves;
                        break;
                    }
                    if (c == '}')
                    {
                        LerChar();
                        IdTokenAtual = TkFechaChaves;
                        break;
                    }
                    if (c == ',')
                    {
                        LerChar();
                        IdTokenAtual = TkVirgula;
                        break;
                    }
                    if (c == ':')
                    {
                        LerChar();
                        IdTokenAtual = TkDoisPontos;
                        break;
                    }
                    if (c == ';')
                    {
                        LerChar();
                        IdTokenAtual = TkPontoVirgula;
                        break;
                    }
                    if (c == '(')
                    {
                        LerChar();
                        IdTokenAtual = TkAbreParentese;
                        break;
                    }
                    if (c == ')')
                    {
                        LerChar();
                        IdTokenAtual = TkFechaParentese;
                        break;
                    }
                    if (c == '[')
                    {
                        LerChar();
                        IdTokenAtual = TkAbreColch;
                        break;
                    }
                    if (c == ']')
                    {
                        LerChar();
                        IdTokenAtual = TkFechaColch;
                        break;
                    }
                    if (c == '^')
                    {
                        LerChar();
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
                        c = LerChar();
                        estado = 1;
                    }
                    else if (c >= 'a' && c <= 'z' || c == '_')
                    {
                        c = LerChar();
                        estado = 1;
                    }
                    else if (c == null || c == '\n' || c == '\r' || c == '\t' || c == ' ')
                    {
                        c = LerChar();
                    }
                }
                else if (estado == 1)
                {
                    if (c >= 'a' && c <= 'z' || c == '_' || c >= '0' && c <= '9')
                    {
                        TokenAtual += c;
                        c = LerChar();
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
                        LerChar();
                        IdTokenAtual = TkIgual;
                        break;
                    }
                    else
                    {
                        // nesse caso é uma atribuicao
                        IdTokenAtual = TkAssignment;
                        break;
                    }
                }
                else if (estado == 3)
                {
                    if (c == '=')
                    {
                        LerChar();
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
                        LerChar();
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
                        LerChar();
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
                        LerChar();
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
                        LerChar();
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
                        LerChar();
                        IdTokenAtual = TkDoublePlus;
                        break;
                    }
                    else if (c == '=')
                    {
                        LerChar();
                        IdTokenAtual = TkAdditionAssignment;
                        break;
                    }
                    else
                    {
                        IdTokenAtual = TkAddiction;
                        break;
                    }
                }
                else if (estado == 9)
                {
                    if (c == '-')
                    {
                        LerChar();
                        IdTokenAtual = TkDoubleMinus;
                        break;
                    }
                    else if (c == '=')
                    {
                        LerChar();
                        IdTokenAtual = TkSubtractionAssignment;
                        break;
                    }
                    else
                    {
                        // nesse caso é uma atribuicao
                        IdTokenAtual = TkSubtraction;
                        break;
                    }
                }
                else if (estado == 10)
                {
                    if (c == '/')
                    {
                        c = LerChar(); // commentario
                        estado = 11;
                        continue;
                    }
                    else if (c == '*')
                    {
                        c = LerChar(); // commentario multilinhas
                        estado = 12;
                        continue;
                    }
                    else if (c == '=')
                    {
                        LerChar();
                        IdTokenAtual = TkDivisionAssignment;
                        break;
                    }
                    else
                    {
                        // nesse caso é uma atribuicao
                        IdTokenAtual = TkDivision;
                        break;
                    }
                }
                else if (estado == 11)
                {
                    if (c == '\n')
                        estado = 0;
                    c = LerChar();
                    continue;
                }
                else if (estado == 12)
                {
                    if (c == '*')
                    {
                        c = LerChar();
                        if (c == '/')
                        {
                            estado = 0;
                        }
                    }
                    c = LerChar();
                }
                else if (estado == 13)
                {
                    if (c == '=')
                    {
                        LerChar();
                        IdTokenAtual = TkMultiplicationAssignment;
                        break;
                    }
                    else
                    {
                        // nesse caso é uma atribuicao
                        IdTokenAtual = TkMultiplication;
                        break;
                    }
                }
                else if (estado == 14)
                {
                    if (c == '=')
                    {
                        LerChar();
                        IdTokenAtual = TkRemainderAssignment;
                        break;
                    }
                    else
                    {
                        // nesse caso é uma atribuicao
                        IdTokenAtual = TkRemainder;
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

        private static char LerChar()
        {

            if (Pos >= Exp.Length) return '\0'; // fim da leitura geral
            Pos++;
            var c = Exp[Pos];

            Coluna++;
            if (c == '\n')
            {
                Linha++;
                Coluna = 1;
            }

            return c;
        }
        #endregion

        #region Formata Texto

        public static string EscreverRotulo(string rotulo)
        {
            return string.Format("{0}: \n", rotulo);
        }

        public static string EscreverCodigo(string format, params object[] args)
        {
            if (!string.IsNullOrEmpty(format) && args.Length == 0)
                return string.Format("{0}", format);

            if (args.Length == 0) return string.Empty;

            // corrige o comando
            if (!format.EndsWith("\n")) format += "\n";

            return string.Format(format, args);
        }
        public static string EscreverCodigo(string comando, string atribuido, string op1, string op2)
        {
            return string.Format("{0} := {1}{2}{3}\n", atribuido, op1, comando, op2);
        }

        public static string EscreverCodigoIf(string expressao, string gotoLabel)
        {
            return string.Format("if {0} goto {1}\n", expressao, gotoLabel);
        }

        public static string EscreverCodigoIfZ(string expressao, string gotoLabel)
        {
            return string.Format("ifZ {0} goto {1}\n", expressao, gotoLabel);
        }

        private static bool VerificarToken(int token)
        {
            return token == IdTokenAtual;
        }

        private static void GerarExcessao(string[] esperava)
        {
            throw new Exception(string.Format("({0},{1}) Esperava {2} e encontrou \"{3}\"", LinhaTokenAtual, ColunaTokenAtual, esperava.Aggregate((a, b) => a + ", " + b), TokenAtual));
        }

        #endregion

        private static int TempContador { get; set; }

        private static string GerarTemp()
        {
            TempContador++;
            return "T" + TempContador;
        }

        private static int RotuloContador { get; set; }

        private static string GerarRotulo()
        {
            RotuloContador++;
            return "L" + RotuloContador;
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
        // Declaration -> TypeSpecifier DeclaratorList;
        // DeclaratorList -> InitDeclarator DeclaratorListRec
        // DeclaratorListRec -> , InitDeclarator DeclaratorListRec
        // DeclaratorListRec -> <vazio>

        // InitDeclarator -> Declarator = AssignmentExpression
        // InitDeclarator -> Declarator

        // Declarator -> id 
        // Declarator -> ( id )

        // TypeSpecifier -> void
        // TypeSpecifier -> char
        // TypeSpecifier -> short
        // TypeSpecifier -> int
        // TypeSpecifier -> long
        // TypeSpecifier -> long int 
        // TypeSpecifier -> long long int
        // TypeSpecifier -> float
        // TypeSpecifier -> double

        public static bool Declaration(Campo declaration)
        {
            var typeSpecifier = new Campo();
            var declaratorList = new Campo();
            if (TypeSpecifier(typeSpecifier))
            {
                if (DeclaratorList(declaratorList))
                {
                    if (VerificarToken(TkPontoVirgula))
                    {
                        declaration.Cod = declaratorList.Cod;
                    }
                }
            }
            return false;
        }

        public static bool TypeSpecifier(Campo typeSpecifier)
        {
            if (VerificarToken(TkChar))
            {
                LerToken();
                typeSpecifier.Type = "char";
                return true;
            }
            if (VerificarToken(TkShort))
            {
                LerToken();
                typeSpecifier.Type = "short";
                return true;
            }
            if (VerificarToken(TkInt))
            {
                LerToken();
                typeSpecifier.Type = "int";
                return true;
            }
            if (VerificarToken(TkLong))
            {
                LerToken();
                typeSpecifier.Type = "long";
                if (VerificarToken(TkLong))
                {
                    LerToken();
                    typeSpecifier.Type += " long";
                    if (VerificarToken(TkInt))
                    {
                        LerToken();
                        typeSpecifier.Type += " int";
                    }
                }
                else if (VerificarToken(TkInt))
                {
                    LerToken();
                    typeSpecifier.Type += " int";
                }
                return true;
            }
            if (VerificarToken(TkFloat))
            {
                LerToken();
                typeSpecifier.Type = "float";
                return true;
            }
            if (VerificarToken(TkDouble))
            {
                LerToken();
                typeSpecifier.Type = "double";
                return true;
            }
            return false;
        }

        public static bool DeclaratorList(Campo declaratorList)
        {
            var initDeclarator = new Campo();
            var declaratorListRecH = new Campo();
            var declaratorListRecS = new Campo();
            if (InitDeclarator(initDeclarator))
            {
                declaratorListRecH.Cod = initDeclarator.Cod;
                if (DeclaratorListRec(declaratorListRecH, declaratorListRecS))
                {
                    declaratorList.Cod = declaratorListRecS.Cod;
                }
            }
            return false;
        }

        public static bool DeclaratorListRec(Campo declaratorListRecH, Campo declaratorListRecS)
        {
            var initDeclarator = new Campo();
            var declaratorListRec1H = new Campo();
            var declaratorListRec1S = new Campo();
            if (VerificarToken(TkVirgula))
            {
                LerToken();
                if (InitDeclarator(initDeclarator))
                {
                    declaratorListRec1H.Cod = declaratorListRecH.Cod;
                    declaratorListRec1H.Cod += EscreverCodigo(initDeclarator.Cod);
                    if (DeclaratorListRec(declaratorListRec1H, declaratorListRec1S))
                    {
                        declaratorListRecS.Cod = declaratorListRec1S.Cod;
                    }
                }
            }
            else
            {
                declaratorListRecS.Cod = declaratorListRecH.Cod;
                return true;
            }
            return false;
        }

        public static bool InitDeclarator(Campo initDeclarator)
        {
            var declarator = new Campo();
            var assignmentExpression = new Campo();
            if (Declarator(declarator))
            {
                if (VerificarToken(TkAssignment))
                {
                    LerToken();
                    if (AssignmentExpression(assignmentExpression))
                    {
                        initDeclarator.Cod += EscreverCodigo(declarator.Cod);
                        initDeclarator.Cod += EscreverCodigo(assignmentExpression.Cod);
                        initDeclarator.Cod += EscreverCodigo(string.Empty, declarator.Place, assignmentExpression.Place, string.Empty);
                        return true;
                    }
                }
                return true;
            }
            return false;
        }

        public static bool Declarator(Campo declarator)
        {
            var expression = new Campo();
            if (VerificarToken(TkId))
            {
                declarator.Place = TokenAtual;
                LerToken();
                return true;
            }
            else if (VerificarToken(TkAbreParentese))
            {
                LerToken();
                if (Expression(expression))
                {
                    if (IdTokenAtual == TkFechaParentese)
                    {
                        LerToken();
                        declarator.Cod = expression.Cod;
                        declarator.Place = expression.Place;
                        return true;
                    }
                    GerarExcessao(new[] { ")" });
                }
            }
            GerarExcessao(new[] { "identificador", "(" });
            return false;
        }

        // ----------------------------------------------------
        // Dec -> Tipo {DecLista.Type = Tipo.Type; DecLista.TamanhoVar = Tipo.TamanhoVar} DecLista;
        // Tipo -> short int {Tipo.Type = short int; Tipo.TamanhoVar = 2}
        // Tipo -> int {Tipo.Type = int ; Tipo.TamanhoVar = 4}
        // Tipo -> long int {Tipo.Type = long int; Tipo.TamanhoVar = 4}
        // Tipo -> long long int {Tipo.Type = long long int; Tipo.TamanhoVar = 8}
        // Tipo -> double {Tipo.Type = double ; Tipo.TamanhoVar = 8}
        // Tipo -> float {Tipo.Type = float ; Tipo.TamanhoVar = 4}
        // DecLista -> id {PoeVariavel(id, ListaDim.ListaVar, endereco); endereco += ListaDim.TamanhoVar;}  {RestoDecLista.Type = DecLista.Type; RestoDecLista.TamanhoVar = DecLista.TamanhoVar;}  RestoDecLista
        // RestoDecLista -> , {DecLista.Type =  RestoDecLista.Type, DecLista.TamanhoVar =  RestoDecLista.TamanhoVar} DecLista
        // RestoDecLista -> <vazio>   

        public static bool Dec(Campo dec)
        {
            var tipo = new Campo();
            var decLista = new Campo();

            if (Tipo(tipo))
            {
                decLista.TamanhoVar = tipo.TamanhoVar;
                decLista.Type = tipo.Type;
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
                LerToken();
                AdicionarVariavelTabela(TokenAtual, decLista.Type, GerarEndereco(decLista.TamanhoVar));

                restoDecLista.TamanhoVar = decLista.TamanhoVar;
                restoDecLista.Type = decLista.Type;
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
                LerToken();
                decLista.Type = restoDecLista.Type;
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
                LerToken();
                tipo.Type = "short";
                tipo.TamanhoVar = 2;
                return true;
            }
            else if (IdTokenAtual == TkInt)
            {
                LerToken();
                tipo.Type = "int";
                tipo.TamanhoVar = 4;
                return true;
            }
            else if (IdTokenAtual == TkDouble)
            {
                LerToken();
                tipo.Type = "double";
                tipo.TamanhoVar = 8;
                return true;
            }
            else if (IdTokenAtual == TkFloat)
            {
                LerToken();
                tipo.Type = "float";
                tipo.TamanhoVar = 4;
                return true;
            }
            else if (IdTokenAtual == TkLong)
            {
                LerToken();
                if (IdTokenAtual == TkLong)
                {
                    LerToken();
                    if (IdTokenAtual == TkInt)
                    {
                        tipo.Type = "long long int";
                        tipo.TamanhoVar = 8;
                        return true;
                    }
                }
                else
                {
                    tipo.Type = "long int";
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

        // Expression -> AssigmentExpression ExpressionRec
        // ExpressionRec -> , AssigmentExpression ExpressionRec | <vazio>

        // AssigmentExpression -> LogicalOrExpression AssigmentExpression

        // AssigmentExpression -> = AssigmentExpression 
        // AssigmentExpression -> *= AssigmentExpression 
        // AssigmentExpression -> /= AssigmentExpression       
        // AssigmentExpression -> += AssigmentExpression 
        // AssigmentExpression -> -= AssigmentExpression        
        // AssigmentExpression -> %= AssigmentExpression        

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
                LerToken();
                if (IdTokenAtual == TkAbreParentese)
                {
                    LerToken();
                    if (C(c))
                    {
                        if (IdTokenAtual == TkFechaParentese)
                        {
                            LerToken();

                            cmd1.Rotulo1 = GerarRotulo();
                            cmd1.Rotulo2 = GerarRotulo();
                            if (Statement(cmd1))
                            {
                                if (IdTokenAtual == TkElse)
                                {
                                    LerToken();

                                    if (Statement(cmd2))
                                    {
                                        statement.Cod += EscreverCodigo(c.Cod);
                                        statement.Cod += EscreverCodigoIfZ(c.Place, cmd1.Rotulo1); // goto else                                    
                                        statement.Cod += EscreverCodigo(cmd1.Cod);
                                        statement.Cod += EscreverCodigo("goto {0}", cmd1.Rotulo2);
                                        statement.Cod += EscreverRotulo(cmd1.Rotulo1);
                                        statement.Cod += EscreverCodigo(cmd2.Cod);
                                        statement.Cod += EscreverRotulo(cmd1.Rotulo2);
                                        return true;
                                    }
                                }
                                else
                                {
                                    statement.Cod += EscreverCodigo(c.Cod);
                                    statement.Cod += EscreverCodigoIfZ(c.Place, cmd1.Rotulo1); // goto else                                    
                                    statement.Cod += EscreverCodigo(cmd1.Cod);
                                    statement.Cod += EscreverRotulo(cmd1.Rotulo1);
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
                LerToken();
                cmd1.Rotulo1 = GerarRotulo();
                cmd1.Rotulo2 = GerarRotulo();
                if (Statement(cmd1))
                {
                    if (IdTokenAtual == TkWhile)
                    {
                        LerToken();
                        if (IdTokenAtual == TkAbreParentese)
                        {
                            LerToken();
                            if (C(c))
                            {
                                if (IdTokenAtual == TkFechaParentese)
                                {
                                    LerToken();
                                    statement.Cod += EscreverRotulo(cmd1.Rotulo1);
                                    statement.Cod += EscreverCodigo(cmd1.Cod);
                                    statement.Cod += EscreverCodigo(c.Cod);
                                    statement.Cod += EscreverCodigoIf(c.Place, cmd1.Rotulo1);
                                    statement.Cod += EscreverCodigo("goto {0}", cmd1.Rotulo2);
                                    statement.Cod += EscreverRotulo(cmd1.Rotulo2);
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
                LerToken();
                if (IdTokenAtual == TkAbreParentese)
                {
                    LerToken();
                    if (C(c))
                    {
                        if (IdTokenAtual == TkFechaParentese)
                        {
                            LerToken();

                            cmd1.Rotulo1 = GerarRotulo();
                            cmd1.Rotulo2 = GerarRotulo();
                            if (Statement(cmd1))
                            {
                                statement.Cod += EscreverRotulo(cmd1.Rotulo1);
                                statement.Cod += EscreverCodigo(c.Cod);
                                statement.Cod += EscreverCodigoIfZ(c.Place, cmd1.Rotulo2);
                                statement.Cod += EscreverCodigo(cmd1.Cod);
                                statement.Cod += EscreverCodigo("goto {0}", cmd1.Rotulo1);
                                statement.Cod += EscreverRotulo(cmd1.Rotulo2);
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

                statement.Cod += EscreverCodigo("goto {0}", statement.Rotulo2);
                LerToken();
                return true;

            }
            #endregion
            #region continue
            else if (IdTokenAtual == TkContinue)
            {
                if (string.IsNullOrEmpty(statement.Rotulo1))
                    return false;

                statement.Cod += EscreverCodigo("goto {0}", statement.Rotulo1);
                LerToken();
                return true;

            }
            #endregion
            else if (IdTokenAtual == TkId)
            {
                statement.Place = TokenAtual;
                LerToken();
                if (IdTokenAtual == TkAssignment)
                {
                    LerToken();
                    if (E(e))
                    {
                        statement.Cod += EscreverCodigo(e.Cod);
                        statement.Cod += EscreverCodigo(string.Empty, statement.Place, e.Place, string.Empty);
                        return true;
                    }
                }
            }
            else if (IdTokenAtual == TkAbreChaves)
            {
                LerToken();

                // propaga os labels 
                listacmd.Rotulo2 = statement.Rotulo2;
                listacmd.Rotulo1 = statement.Rotulo1;
                if (ListaCmd(listacmd))
                {
                    if (IdTokenAtual == TkFechaChaves)
                    {
                        LerToken();
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
            else if (SelectionStatement(selectionStatement))
            {
                statement.Cod = selectionStatement.Cod;
                return true;
            }
            else if (ExpressionStatement(expressionStatement))
            {
                statement.Cod = expressionStatement.Cod;
                return true;
            }
            return false;
        }

        public static bool CompoundStatement(Campo compoundStatement)
        {
            var blockItemList = new Campo();

            if (VerificarToken(TkAbreChaves))
            {
                LerToken();

                // propaga os labels 
                blockItemList.Rotulo2 = compoundStatement.Rotulo2;
                blockItemList.Rotulo1 = compoundStatement.Rotulo1;
                if (BlockItemList(blockItemList))
                {
                    if (VerificarToken(TkFechaChaves))
                    {
                        LerToken();
                        compoundStatement.Cod = blockItemList.Cod;
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
            var declaration = new Campo();
            var statement = new Campo();
            if (Declaration(declaration))
            {
                blockItem.Cod = EscreverCodigo(statement.Cod);
                if (Statement(statement))
                {
                    blockItem.Cod += EscreverCodigo(statement.Cod);
                    return true;
                }
            }
            return false;

        }

        public static bool SelectionStatement(Campo selectionStatement)
        {
            var c = new Campo();
            var statement1 = new Campo();
            var statement2 = new Campo();

            if (IdTokenAtual == TkIf)
            {
                LerToken();
                if (IdTokenAtual == TkAbreParentese)
                {
                    LerToken();
                    if (C(c))
                    {
                        if (IdTokenAtual == TkFechaParentese)
                        {
                            LerToken();

                            statement1.Rotulo1 = GerarRotulo();
                            statement1.Rotulo2 = GerarRotulo();
                            if (Statement(statement1))
                            {
                                if (IdTokenAtual == TkElse)
                                {
                                    LerToken();
                                    if (Statement(statement2))
                                    {
                                        selectionStatement.Cod += EscreverCodigo(c.Cod);
                                        selectionStatement.Cod += EscreverCodigoIfZ(c.Place, statement1.Rotulo1); // goto else                                    
                                        selectionStatement.Cod += EscreverCodigo(statement1.Cod);
                                        selectionStatement.Cod += EscreverCodigo("goto {0}", statement1.Rotulo2);
                                        selectionStatement.Cod += EscreverRotulo(statement1.Rotulo1);
                                        selectionStatement.Cod += EscreverCodigo(statement2.Cod);
                                        selectionStatement.Cod += EscreverRotulo(statement1.Rotulo2);
                                        return true;
                                    }
                                }
                                else
                                {
                                    selectionStatement.Cod += EscreverCodigo(c.Cod);
                                    selectionStatement.Cod += EscreverCodigoIfZ(c.Place, statement1.Rotulo1); // goto else                                    
                                    selectionStatement.Cod += EscreverCodigo(statement1.Cod);
                                    selectionStatement.Cod += EscreverRotulo(statement1.Rotulo1);
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
                    LerToken();
                    expressionStatement.Cod = expression.Cod;
                    return true;
                }
            }
            else if (IdTokenAtual == TkPontoVirgula)
            {
                LerToken();
                return true;
            }
            GerarExcessao(new[] { ";", "expressão" });
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
            if (VerificarToken(TkVirgula))
            {
                LerToken();
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

        public static bool AssignmentExpression(Campo assignmentExpression)
        {
            var logicalOrExpression = new Campo();
            var assignmentExpression1 = new Campo();

            if (LogicalOrExpression(logicalOrExpression))
            {
                assignmentExpression.Cod = logicalOrExpression.Cod;
                assignmentExpression.Place = logicalOrExpression.Place;

                if (VerificarToken(TkAssignment)) // identifica atribuicao
                {
                    LerToken();
                    if (AssignmentExpression(assignmentExpression1))
                    {
                        assignmentExpression.Cod += EscreverCodigo(assignmentExpression1.Cod);
                        assignmentExpression.Cod += EscreverCodigo(string.Empty, logicalOrExpression.Place, assignmentExpression1.Place, string.Empty);
                        return true;
                    }
                }
                else if (VerificarToken(TkMultiplicationAssignment)) // identifica atribuicao
                {
                    LerToken();
                    if (AssignmentExpression(assignmentExpression1))
                    {
                        assignmentExpression.Cod += EscreverCodigo(assignmentExpression1.Cod);
                        assignmentExpression.Cod += EscreverCodigo("*", logicalOrExpression.Place, logicalOrExpression.Place, assignmentExpression1.Place);
                        return true;
                    }
                }
                else if (VerificarToken(TkDivisionAssignment)) // identifica atribuicao
                {
                    LerToken();
                    if (AssignmentExpression(assignmentExpression1))
                    {
                        assignmentExpression.Cod += EscreverCodigo(assignmentExpression1.Cod);
                        assignmentExpression.Cod += EscreverCodigo("/", logicalOrExpression.Place, logicalOrExpression.Place, assignmentExpression1.Place);
                        return true;
                    }
                }
                else if (VerificarToken(TkAdditionAssignment)) // identifica atribuicao
                {
                    LerToken();
                    if (AssignmentExpression(assignmentExpression1))
                    {
                        assignmentExpression.Cod += EscreverCodigo(assignmentExpression1.Cod);
                        assignmentExpression.Cod += EscreverCodigo("+", logicalOrExpression.Place, logicalOrExpression.Place, assignmentExpression1.Place);
                        return true;
                    }
                }
                else if (VerificarToken(TkSubtractionAssignment)) // identifica atribuicao
                {
                    LerToken();
                    if (AssignmentExpression(assignmentExpression1))
                    {
                        assignmentExpression.Cod += EscreverCodigo(assignmentExpression1.Cod);
                        assignmentExpression.Cod += EscreverCodigo("-", logicalOrExpression.Place, logicalOrExpression.Place, assignmentExpression1.Place);
                        return true;
                    }
                }
                else if (VerificarToken(TkRemainderAssignment)) // identifica atribuicao
                {
                    LerToken();
                    if (AssignmentExpression(assignmentExpression1))
                    {
                        assignmentExpression.Cod += EscreverCodigo(assignmentExpression1.Cod);
                        assignmentExpression.Cod += EscreverCodigo("%", logicalOrExpression.Place, logicalOrExpression.Place, assignmentExpression1.Place);
                        return true;
                    }
                }
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
            if (VerificarToken(TkLogicalOr))
            {
                LerToken();
                if (LogicalAndExpression(logicalAndExpression))
                {
                    logicalOrExpressionRec1H.Place = GerarTemp();
                    logicalOrExpressionRec1H.Cod += EscreverCodigo(logicalOrExpressionRecH.Cod);
                    logicalOrExpressionRec1H.Cod += EscreverCodigo(logicalAndExpression.Cod);
                    logicalOrExpressionRec1H.Cod += EscreverCodigo("||", logicalOrExpressionRec1H.Place, logicalOrExpressionRecH.Place, logicalAndExpression.Place);
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
            if (VerificarToken(TkLogicalAnd))
            {
                LerToken();
                if (InclusiveOrExpression(inclusiveOrExpression))
                {
                    logicalAndExpressionRec1H.Place = GerarTemp();
                    logicalAndExpressionRec1H.Cod += EscreverCodigo(logicalAndExpressionRecH.Cod);
                    logicalAndExpressionRec1H.Cod += EscreverCodigo(inclusiveOrExpression.Cod);
                    logicalAndExpressionRec1H.Cod += EscreverCodigo("&&", logicalAndExpressionRec1H.Place, logicalAndExpressionRecH.Place, inclusiveOrExpression.Place);
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
            if (VerificarToken(TkOr))
            {
                LerToken();
                if (ExclusiveOrExpression(exclusiveOrExpression))
                {
                    inclusiveOrExpressionRec1H.Place = GerarTemp();
                    inclusiveOrExpressionRec1H.Cod += EscreverCodigo(inclusiveOrExpressionRecH.Cod);
                    inclusiveOrExpressionRec1H.Cod += EscreverCodigo(exclusiveOrExpression.Cod);
                    inclusiveOrExpressionRec1H.Cod += EscreverCodigo("|", inclusiveOrExpressionRec1H.Place, inclusiveOrExpressionRecH.Place, exclusiveOrExpression.Place);
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
            if (VerificarToken(TkExclusiveOr))
            {
                LerToken();
                if (AndExpression(andExpression))
                {
                    exclusiveOrExpressionRec1H.Place = GerarTemp();
                    exclusiveOrExpressionRec1H.Cod += EscreverCodigo(exclusiveOrExpressionRecH.Cod);
                    exclusiveOrExpressionRec1H.Cod += EscreverCodigo(andExpression.Cod);
                    exclusiveOrExpressionRec1H.Cod += EscreverCodigo("^", exclusiveOrExpressionRec1H.Place, exclusiveOrExpressionRecH.Place, andExpression.Place);
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
            if (VerificarToken(TkAnd))
            {
                LerToken();
                if (EqualityExpression(equalityExpression))
                {
                    andExpressionRec1H.Place = GerarTemp();
                    andExpressionRec1H.Cod += EscreverCodigo(andExpressionRecH.Cod);
                    andExpressionRec1H.Cod += EscreverCodigo(equalityExpression.Cod);
                    andExpressionRec1H.Cod += EscreverCodigo("&", andExpressionRec1H.Place, andExpressionRecH.Place, equalityExpression.Place);
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
            if (VerificarToken(TkIgual))
            {
                LerToken();
                if (RelationalExpression(relationalExpression))
                {
                    equalityExpressionRec1H.Place = GerarTemp();
                    equalityExpressionRec1H.Cod += EscreverCodigo(equalityExpressionRecH.Cod);
                    equalityExpressionRec1H.Cod += EscreverCodigo(relationalExpression.Cod);
                    equalityExpressionRec1H.Cod += EscreverCodigo("==", equalityExpressionRec1H.Place, equalityExpressionRecH.Place, relationalExpression.Place);
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
                LerToken();
                if (RelationalExpression(relationalExpression))
                {
                    equalityExpressionRec1H.Place = GerarTemp();
                    equalityExpressionRec1H.Cod += EscreverCodigo(equalityExpressionRecH.Cod);
                    equalityExpressionRec1H.Cod += EscreverCodigo(relationalExpression.Cod);
                    equalityExpressionRec1H.Cod += EscreverCodigo("!=", equalityExpressionRec1H.Place, equalityExpressionRecH.Place, relationalExpression.Place);
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
            if (VerificarToken(TkMenor))
            {
                LerToken();
                if (AddictiveExpression(addictiveExpression))
                {
                    relationalExpressionRec1H.Place = GerarTemp();
                    relationalExpressionRec1H.Cod += EscreverCodigo(relationalExpressionRecH.Cod);
                    relationalExpressionRec1H.Cod += EscreverCodigo(addictiveExpression.Cod);
                    relationalExpressionRec1H.Cod += EscreverCodigo("<", relationalExpressionRec1H.Place, relationalExpressionRecH.Place, addictiveExpression.Place);
                    if (RelationalExpressionRec(relationalExpressionRec1H, relationalExpressionRec1S))
                    {
                        relationalExpressionRecS.Cod = relationalExpressionRec1S.Cod;
                        relationalExpressionRecS.Place = relationalExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else if (VerificarToken(TkMaior))
            {
                LerToken();
                if (AddictiveExpression(addictiveExpression))
                {
                    relationalExpressionRec1H.Place = GerarTemp();
                    relationalExpressionRec1H.Cod += EscreverCodigo(relationalExpressionRecH.Cod);
                    relationalExpressionRec1H.Cod += EscreverCodigo(addictiveExpression.Cod);
                    relationalExpressionRec1H.Cod += EscreverCodigo(">", relationalExpressionRec1H.Place, relationalExpressionRecH.Place, addictiveExpression.Place);
                    if (RelationalExpressionRec(relationalExpressionRec1H, relationalExpressionRec1S))
                    {
                        relationalExpressionRecS.Cod = relationalExpressionRec1S.Cod;
                        relationalExpressionRecS.Place = relationalExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else if (VerificarToken(TkMenorIgual))
            {
                LerToken();
                if (AddictiveExpression(addictiveExpression))
                {
                    relationalExpressionRec1H.Place = GerarTemp();
                    relationalExpressionRec1H.Cod += EscreverCodigo(relationalExpressionRecH.Cod);
                    relationalExpressionRec1H.Cod += EscreverCodigo(addictiveExpression.Cod);
                    relationalExpressionRec1H.Cod += EscreverCodigo("<=", relationalExpressionRec1H.Place, relationalExpressionRecH.Place, addictiveExpression.Place);
                    if (RelationalExpressionRec(relationalExpressionRec1H, relationalExpressionRec1S))
                    {
                        relationalExpressionRecS.Cod = relationalExpressionRec1S.Cod;
                        relationalExpressionRecS.Place = relationalExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else if (VerificarToken(TkMaiorIgual))
            {
                LerToken();
                if (AddictiveExpression(addictiveExpression))
                {
                    relationalExpressionRec1H.Place = GerarTemp();
                    relationalExpressionRec1H.Cod += EscreverCodigo(relationalExpressionRecH.Cod);
                    relationalExpressionRec1H.Cod += EscreverCodigo(addictiveExpression.Cod);
                    relationalExpressionRec1H.Cod += EscreverCodigo(">=", relationalExpressionRec1H.Place, relationalExpressionRecH.Place, addictiveExpression.Place);
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
            if (VerificarToken(TkAddiction))
            {
                LerToken();
                if (MultiplicativeExpression(multiplicativeExpression))
                {
                    addictiveExpressionRec1H.Place = GerarTemp();
                    addictiveExpressionRec1H.Cod += EscreverCodigo(addictiveExpressionRecH.Cod);
                    addictiveExpressionRec1H.Cod += EscreverCodigo(multiplicativeExpression.Cod);
                    addictiveExpressionRec1H.Cod += EscreverCodigo("+", addictiveExpressionRec1H.Place, addictiveExpressionRecH.Place, multiplicativeExpression.Place);
                    if (AddictiveExpressionRec(addictiveExpressionRec1H, addictiveExpressionRec1S))
                    {
                        addictiveExpressionRecS.Cod = addictiveExpressionRec1S.Cod;
                        addictiveExpressionRecS.Place = addictiveExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else if (VerificarToken(TkSubtraction))
            {
                LerToken();
                if (MultiplicativeExpression(multiplicativeExpression))
                {
                    addictiveExpressionRec1H.Place = GerarTemp();
                    addictiveExpressionRec1H.Cod += EscreverCodigo(addictiveExpressionRecH.Cod);
                    addictiveExpressionRec1H.Cod += EscreverCodigo(multiplicativeExpression.Cod);
                    addictiveExpressionRec1H.Cod += EscreverCodigo("-", addictiveExpressionRec1H.Place, addictiveExpressionRecH.Place, multiplicativeExpression.Place);
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

            if (VerificarToken(TkMultiplication))
            {
                LerToken();
                if (PrimaryExpression(primaryExpression))
                {
                    multiplicativeExpressionRec1H.Place = GerarTemp();
                    multiplicativeExpressionRec1H.Cod += EscreverCodigo(multiplicativeExpressionRecH.Cod);
                    multiplicativeExpressionRec1H.Cod += EscreverCodigo(primaryExpression.Cod);
                    multiplicativeExpressionRec1H.Cod += EscreverCodigo("*", multiplicativeExpressionRec1H.Place, multiplicativeExpressionRecH.Place, primaryExpression.Place);
                    if (MultiplicativeExpressionRec(multiplicativeExpressionRec1H, multiplicativeExpressionRec1S))
                    {
                        multiplicativeExpressionRecS.Cod = multiplicativeExpressionRec1S.Cod;
                        multiplicativeExpressionRecS.Place = multiplicativeExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else if (VerificarToken(TkDivision))
            {
                LerToken();
                if (PrimaryExpression(primaryExpression))
                {
                    multiplicativeExpressionRec1H.Place = GerarTemp();
                    multiplicativeExpressionRec1H.Cod += EscreverCodigo(multiplicativeExpressionRecH.Cod);
                    multiplicativeExpressionRec1H.Cod += EscreverCodigo(primaryExpression.Cod);
                    multiplicativeExpressionRec1H.Cod += EscreverCodigo("/", multiplicativeExpressionRec1H.Place, multiplicativeExpressionRecH.Place, primaryExpression.Place);
                    if (MultiplicativeExpressionRec(multiplicativeExpressionRec1H, multiplicativeExpressionRec1S))
                    {
                        multiplicativeExpressionRecS.Cod = multiplicativeExpressionRec1S.Cod;
                        multiplicativeExpressionRecS.Place = multiplicativeExpressionRec1S.Place;
                        return true;
                    }
                }
                return false;
            }
            else if (VerificarToken(TkRemainder))
            {
                LerToken();
                if (PrimaryExpression(primaryExpression))
                {
                    multiplicativeExpressionRec1H.Place = GerarTemp();
                    multiplicativeExpressionRec1H.Cod += EscreverCodigo(multiplicativeExpressionRecH.Cod);
                    multiplicativeExpressionRec1H.Cod += EscreverCodigo(primaryExpression.Cod);
                    multiplicativeExpressionRec1H.Cod += EscreverCodigo("%", multiplicativeExpressionRec1H.Place, multiplicativeExpressionRecH.Place, primaryExpression.Place);
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
            else if (IdTokenAtual == TkDoublePlus)
            {
                LerToken();
                if (UnaryExpression(unaryExpression1))
                {
                    unaryExpression.Cod = unaryExpression1.Cod;
                    return true;
                }
            }
            else if (IdTokenAtual == TkDoubleMinus)
            {
                LerToken();
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
                if (IdTokenAtual == TkDoublePlus)
                {
                    LerToken();
                    if (UnaryExpression(unaryExpression1))
                    {
                        postFixExpression.Cod += unaryExpression1.Cod;
                        return true;
                    }
                }
                else if (IdTokenAtual == TkDoubleMinus)
                {
                    LerToken();
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
            if (VerificarToken(TkConst))
            {
                primaryExpression.Place = TokenAtual;
                LerToken();
                return true;
            }
            else if (VerificarToken(TkId))
            {
                primaryExpression.Place = TokenAtual;
                LerToken();
                return true;
            }
            else if (VerificarToken(TkAbreParentese))
            {
                LerToken();
                if (Expression(expression))
                {
                    if (IdTokenAtual == TkFechaParentese)
                    {
                        LerToken();
                        primaryExpression.Cod = expression.Cod;
                        primaryExpression.Place = expression.Place;
                        return true;
                    }
                    GerarExcessao(new[] { ")" });
                }
            }
            GerarExcessao(new[] { "constante", "identificador", "(" });
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
                    LerToken();

                    // propaga os labels 
                    listacont.Rotulo2 = cmd.Rotulo2;
                    listacont.Rotulo1 = cmd.Rotulo1;
                    if (ListaCont(listacont))
                    {
                        listacmd.Cod += EscreverCodigo(cmd.Cod);
                        listacmd.Cod += EscreverCodigo(listacont.Cod);
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
                    LerToken();

                    // propaga os labels 
                    listacont1.Rotulo2 = cmd.Rotulo2;
                    listacont1.Rotulo1 = cmd.Rotulo1;
                    if (ListaCont(listacont1))
                    {
                        listacont.Cod += EscreverCodigo(cmd.Cod);
                        listacont.Cod += EscreverCodigo(listacont1.Cod);
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
                LerToken();
                if (E(e))
                {
                    rb1h.Place = GerarTemp();
                    rb1h.Cod += EscreverCodigo(rbh.Cod);
                    rb1h.Cod += EscreverCodigo(e.Cod);
                    rb1h.Cod += EscreverCodigo("==", rb1h.Place, rbh.Place, e.Place);
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
                LerToken();
                if (E(e))
                {
                    rb1h.Place = GerarTemp();
                    rb1h.Cod += EscreverCodigo(rbh.Cod);
                    rb1h.Cod += EscreverCodigo(e.Cod);
                    rb1h.Cod += EscreverCodigo("!=", rb1h.Place, rbh.Place, e.Place);
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
                LerToken();
                if (E(e))
                {
                    rb1h.Place = GerarTemp();
                    rb1h.Cod += EscreverCodigo(rbh.Cod);
                    rb1h.Cod += EscreverCodigo(e.Cod);
                    rb1h.Cod += EscreverCodigo(">=", rb1h.Place, rbh.Place, e.Place);
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
                LerToken();
                if (E(e))
                {
                    rb1h.Place = GerarTemp();
                    rb1h.Cod += EscreverCodigo(rbh.Cod);
                    rb1h.Cod += EscreverCodigo(e.Cod);
                    rb1h.Cod += EscreverCodigo(">", rb1h.Place, rbh.Place, e.Place);
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
                LerToken();
                if (E(e))
                {
                    rb1h.Place = GerarTemp();
                    rb1h.Cod += EscreverCodigo(rbh.Cod);
                    rb1h.Cod += EscreverCodigo(e.Cod);
                    rb1h.Cod += EscreverCodigo("<=", rb1h.Place, rbh.Place, e.Place);
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
                LerToken();
                if (E(e))
                {

                    rb1h.Place = GerarTemp();
                    rb1h.Cod += EscreverCodigo(rbh.Cod);
                    rb1h.Cod += EscreverCodigo(e.Cod);
                    rb1h.Cod += EscreverCodigo("<", rb1h.Place, rbh.Place, e.Place);
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

            if (IdTokenAtual == TkAddiction)
            {
                LerToken();
                if (T(t))
                {
                    r1h.Place = GerarTemp();
                    r1h.Cod += EscreverCodigo(rh.Cod);
                    r1h.Cod += EscreverCodigo(t.Cod);
                    r1h.Cod += EscreverCodigo("+", r1h.Place, rh.Place, t.Place);
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
            else if (IdTokenAtual == TkSubtraction)
            {
                LerToken();
                if (T(t))
                {
                    r1h.Place = GerarTemp();
                    r1h.Cod += EscreverCodigo(rh.Cod);
                    r1h.Cod += EscreverCodigo(t.Cod);
                    r1h.Cod += EscreverCodigo("-", r1h.Place, rh.Place, t.Place);
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

            if (IdTokenAtual == TkMultiplication)
            {
                LerToken();
                if (F(f))
                {
                    ra1h.Place = GerarTemp();
                    ra1h.Cod += EscreverCodigo(rah.Cod);
                    ra1h.Cod += EscreverCodigo(f.Cod);
                    ra1h.Cod += EscreverCodigo("*", ra1h.Place, rah.Place, f.Place);
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
            else if (IdTokenAtual == TkDivision)
            {
                LerToken();
                if (F(f))
                {
                    ra1h.Place = GerarTemp();
                    ra1h.Cod += EscreverCodigo(rah.Cod);
                    ra1h.Cod += EscreverCodigo(f.Cod);
                    ra1h.Cod += EscreverCodigo("/", ra1h.Place, rah.Place, f.Place);
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
                LerToken();
                return true;
            }
            else if (IdTokenAtual == TkAbreParentese)
            {
                LerToken();
                if (E(e))
                {
                    if (IdTokenAtual == TkFechaParentese)
                    {
                        LerToken();
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
                LerToken();
                return true;
            }
            return false;
        }

    }
}

