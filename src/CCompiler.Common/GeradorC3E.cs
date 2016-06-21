using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;

namespace CCompiler.Common
{
    public static class GeradorC3E
    {

        public static int IdTokenAtual { get; set; }
        public static string TokenAtual { get; set; }
        public static int LinhaTokenAtual { get; set; }
        public static int ColunaTokenAtual { get; set; }
        public static List<string> TokensAguardados { get; set; }
        public static List<Exception> Exceptions { get; set; }
        public static Escopo EscopoGlobal { get; set; }
        public static Escopo EscopoAtual { get; set; }

        public static void Inicializar(string exp)
        {
            TempContador = 0;
            RotuloContador = 0;
            Pos = 0;
            Linha = 1;
            Coluna = 1;
            EscopoGlobal = new Escopo(null);
            EscopoAtual = EscopoGlobal;
            TokensAguardados = new List<string>();
            Exceptions = new List<Exception>();
            Exp = exp.ToCharArray();
            LerToken();
        }

        public static int MemoriaBase { get; set; }

        #region Lexico

        private static readonly List<string> Reservadas = new List<string>() { "int", "float", "char", "struct", "while", "break", "continue", "short", "double", "long", "do", "if", "else", "void", "for", "switch", "case", "default", "return" };
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
        private const int TkVoid = 13;
        private const int TkFor = 14;
        private const int TkSwitch = 15;
        private const int TkCase = 16;
        private const int TkDefault = 17;
        private const int TkReturn = 18;

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
        private const int TkAspasSimples = 137;

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
                    if (c == '\'')
                    {
                        LerChar();
                        IdTokenAtual = TkAspasSimples;
                        break;
                    }
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
                        continue;
                    }
                    else if (c >= 'a' && c <= 'z' || c == '_')
                    {
                        c = LerChar();
                        estado = 1;
                        continue;
                    }
                    else if (c == null || c == '\n' || c == '\r' || c == '\t' || c == ' ')
                    {
                        c = LerChar();
                        continue;
                    }
                    GerarExcessao(new[] { "identificador", "palavra reservada" });
                    break;
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

            return string.Format("\t" + format, args);
        }

        public static string EscreverCodigo(string comando, string atribuido, string op1, string op2)
        {
            return string.Format("\t{0} := {1} {2} {3}\n", atribuido, op1, comando, op2);
        }

        public static string EscreverCodigoIf(string op1, string operador, string op2, string label)
        {
            return string.Format("\tif {0}{1}{2} goto {3}\n", op1, operador, op2, label);
        }

        public static string EscreverCodigoIf(string expressao, string gotoLabel)
        {
            return string.Format("\tif {0}=1 goto {1}\n", expressao, gotoLabel);
        }

        public static string EscreverCodigoIfZ(string expressao, string gotoLabel)
        {
            return string.Format("\tif {0}=0 goto {1}\n", expressao, gotoLabel);
        }

        private static bool VerificarToken(int token)
        {
            return token == IdTokenAtual;
        }

        private static void GerarExcessao(string[] esperava)
        {
            Exceptions.Add(
                new Exception(string.Format("({0},{1}) Esperava {2} e encontrou \"{3}\"", LinhaTokenAtual,
                    ColunaTokenAtual, esperava.Aggregate((a, b) => a + ", " + b), TokenAtual)));
        }

        private static void GerarExcessaoParams(string proc)
        {
            Exceptions.Add(
                new Exception(string.Format("({0},{1}) Quantidade de parametos incorretos para o procedimento {2}", LinhaTokenAtual,
                    ColunaTokenAtual, proc)));
        }

        private static void GerarExcessaoTipoDup(string nome)
        {
            Exceptions.Add(
                new Exception(string.Format("({0},{1}) O identificador \"{2}\" já esta declarado.", LinhaTokenAtual,
                    ColunaTokenAtual, nome)));
        }

        private static void GerarExcessaoVarNaoDecl(string nome)
        {
            Exceptions.Add(
                new Exception(string.Format("({0},{1}) O identificador \"{2}\" não esta declarado.", LinhaTokenAtual,
                    ColunaTokenAtual, nome)));
        }

        #endregion

        private static bool VerificarVariavelTab(string nome)
        {
            if (!VerificarVariavelTab(nome, EscopoAtual))
            {
                GerarExcessaoVarNaoDecl(nome);
                return false;
            }
            return true;
        }

        private static bool VerificarVariavelTab(string nome, Escopo referencia)
        {
            if (referencia.Variaveis.Any(x => x.Id == nome))
                return true;

            if (referencia.Ref == null) return false;

            return VerificarVariavelTab(nome, referencia.Ref);
        }

        private static Variavel BuscarVariavelTab(string nome, Escopo referencia)
        {
            if (referencia.Variaveis.Any(x => x.Id == nome))
                return referencia.Variaveis.First(x => x.Id == nome);

            if (referencia.Ref == null) return null;

            return BuscarVariavelTab(nome, referencia.Ref);
        }

        private static bool AdicionarVariavelTab(string nome, string tipo, Escopo referencia = null)
        {
            if (EscopoAtual.Variaveis.Any(x => x.Id == nome))
            {
                GerarExcessaoTipoDup(nome);
                return false;
            }

            EscopoAtual.Variaveis.Add(new Variavel() { Id = nome, Ref = referencia, Tipo = tipo });
            return true;
        }

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

        // ----------------------------------------------------
        // ExternalDeclaration -> Declaration DeclarationRec

        // Declaration -> TypeSpecifier Declarator (ParametersList) { FunctionStatement }
        // Declaration -> TypeSpecifier Declarator InitDeclarator DeclaratorListRec;

        // DeclarationRec -> Declaration DeclarationRec
        // DeclarationRec -> <vazio>

        // DeclaratorListRec -> , Declarator InitDeclarator DeclaratorListRec
        // DeclaratorListRec -> <vazio>

        // InitDeclarator -> = AssignmentExpression
        // InitDeclarator -> <vazio>

        // Declarator -> id 
        // ######## Declarator -> ( Expression ) // retirar

        // TypeSpecifier -> void
        // TypeSpecifier -> char // nao implementado
        // TypeSpecifier -> short
        // TypeSpecifier -> int
        // TypeSpecifier -> long
        // TypeSpecifier -> long int 
        // TypeSpecifier -> long long int
        // TypeSpecifier -> float
        // TypeSpecifier -> double

        // ParametersList -> TypeSpecifier Declarator InitDeclarator ParametersListRec
        // ParametersList -> <vazio>
        // ParametersListRec -> , TypeSpecifier Declarator InitDeclarator ParametersListRec
        // ParametersListRec -> <vazio>        

        public static bool ExternalDeclaration(Campo externalDeclaration)
        {
            var externalDeclaration1 = new Campo();
            var declaration = new Campo();
            if (Declaration(declaration))
            {
                externalDeclaration.Cod += EscreverCodigo("goto main");
                externalDeclaration.Cod += EscreverCodigo(declaration.Cod);
                if (DeclarationRec(externalDeclaration1))
                {
                    externalDeclaration.Cod += EscreverCodigo(externalDeclaration1.Cod);                    
                }                
            }

            if (!VerificarVariavelTab("main")) return false;
            if (Exceptions.Count > 0) return false;

            return true;
        }

        public static bool Declaration(Campo declaration)
        {
            var typeSpecifier = new Campo();
            var declarator = new Campo();
            var initDeclarator = new Campo();
            var declaratorListRecH = new Campo();
            var declaratorListRecS = new Campo();
            var parametersList = new Campo();
            var functionStatement = new Campo();

            if (TypeSpecifier(typeSpecifier))
            {
                declarator.Tipo = typeSpecifier.Tipo;
                if (Declarator(declarator))
                {
                    initDeclarator.Place = declarator.Place;
                    if (VerificarToken(TkAbreParentese))
                    {
                        // se achou funcao é porque é um novo escopo a partir dai
                        var scp = new Escopo(EscopoAtual);
                        EscopoAtual = scp;

                        try
                        {
                            LerToken();
                            if (ParametersList(parametersList))
                            {
                                var proc = BuscarVariavelTab(declarator.Place, EscopoAtual);
                                if (typeSpecifier.Tipo != "void")
                                    proc.Return = new Variavel() { Tipo = typeSpecifier.Tipo };

                                proc.Params.AddRange(EscopoAtual.Variaveis); // adiciona todas as variaveis do escopo

                                if (VerificarToken(TkFechaParentese))
                                {
                                    LerToken();
                                    if (VerificarToken(TkAbreChaves))
                                    {
                                        LerToken();
                                        if (FunctionStatement(functionStatement))
                                        {
                                            if (VerificarToken(TkFechaChaves))
                                            {
                                                LerToken();
                                                declaration.Cod += "\n";
                                                declaration.Cod += EscreverRotulo(declarator.Place);
                                                declaration.Cod += EscreverCodigo(declarator.Cod);
                                                declaration.Cod += EscreverCodigo(parametersList.Cod);
                                                declaration.Cod += EscreverCodigo(functionStatement.Cod);
                                                declaration.Cod += "\n";
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                        finally // no finally faz voltar o escopo
                        {
                            EscopoAtual = EscopoAtual.Ref;
                        }
                        return false;
                    }
                    else if (InitDeclarator(initDeclarator)) // aqui eh declaração de variavel
                    {
                        declaratorListRecH.Tipo = typeSpecifier.Tipo;
                        if (DeclaratorListRec(declaratorListRecH, declaratorListRecS))
                        {
                            if (VerificarToken(TkPontoVirgula))
                            {
                                LerToken();
                                declaration.Cod += EscreverCodigo(declarator.Cod);
                                declaration.Cod += EscreverCodigo(initDeclarator.Cod);
                                declaration.Cod += EscreverCodigo(declaratorListRecS.Cod);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static bool DeclarationRec(Campo declarationRec)
        {
            var externalDeclaration1 = new Campo();
            var declaration = new Campo();
            if (Declaration(declaration))
            {
                declarationRec.Cod += EscreverCodigo(declaration.Cod);
                if (DeclarationRec(externalDeclaration1))
                {
                    declarationRec.Cod += EscreverCodigo(externalDeclaration1.Cod);
                }
            }
            return true;
        }

        public static bool TypeSpecifier(Campo typeSpecifier)
        {
            if (VerificarToken(TkVoid))
            {
                LerToken();
                typeSpecifier.Tipo = "void";
                return true;
            }
            /*if (VerificarToken(TkChar))
            {
                LerToken();
                typeSpecifier.Tipo = "char";
                return true;
            }*/
            if (VerificarToken(TkShort))
            {
                LerToken();
                typeSpecifier.Tipo = "short";
                return true;
            }
            if (VerificarToken(TkInt))
            {
                LerToken();
                typeSpecifier.Tipo = "int";
                return true;
            }
            if (VerificarToken(TkLong))
            {
                LerToken();
                typeSpecifier.Tipo = "long";
                if (VerificarToken(TkLong))
                {
                    LerToken();
                    typeSpecifier.Tipo += " long";
                    if (VerificarToken(TkInt))
                    {
                        LerToken();
                        typeSpecifier.Tipo += " int";
                    }
                }
                else if (VerificarToken(TkInt))
                {
                    LerToken();
                    typeSpecifier.Tipo += " int";
                }
                return true;
            }
            if (VerificarToken(TkFloat))
            {
                LerToken();
                typeSpecifier.Tipo = "float";
                return true;
            }
            if (VerificarToken(TkDouble))
            {
                LerToken();
                typeSpecifier.Tipo = "double";
                return true;
            }
            return false;
        }

        public static bool DeclaratorListRec(Campo declaratorListRecH, Campo declaratorListRecS)
        {
            var initDeclarator = new Campo();
            var declarator = new Campo();
            var declaratorListRec1H = new Campo();
            var declaratorListRec1S = new Campo();
            if (VerificarToken(TkVirgula))
            {
                LerToken();
                declarator.Tipo = declaratorListRecH.Tipo;
                if (Declarator(declarator))
                {
                    initDeclarator.Place = declarator.Place;
                    initDeclarator.Tipo = declaratorListRecH.Tipo;
                    if (InitDeclarator(initDeclarator))
                    {
                        declaratorListRec1H.Tipo = declaratorListRecH.Tipo;
                        declaratorListRec1H.Cod += EscreverCodigo(initDeclarator.Cod);
                        declaratorListRec1H.Cod += EscreverCodigo(declaratorListRecH.Cod);
                        if (DeclaratorListRec(declaratorListRec1H, declaratorListRec1S))
                        {
                            declaratorListRecS.Cod = declaratorListRec1S.Cod;
                            return true;
                        }
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
            var assignmentExpression = new Campo();
            if (VerificarToken(TkAssignment))
            {
                LerToken();

                assignmentExpression.EhDeclarador = true;

                if (AssignmentExpression(assignmentExpression))
                {
                    initDeclarator.Cod += EscreverCodigo(assignmentExpression.Cod);
                    initDeclarator.Cod += EscreverCodigo(string.Empty, initDeclarator.Place, assignmentExpression.Place, string.Empty);
                    return true;
                }
                return false;
            }
            return true;
        }

        public static bool Declarator(Campo declarator)
        {
            if (VerificarToken(TkId))
            {
                declarator.Place = TokenAtual;
                if (!AdicionarVariavelTab(declarator.Place, declarator.Tipo)) return false;

                LerToken();
                return true;
            }
            GerarExcessao(new[] { "identificador" });
            return false;
        }

        public static bool ParametersList(Campo parametersList)
        {
            var typeSpecifier = new Campo();
            var declarator = new Campo();
            var initDeclarator = new Campo();
            var parametersListRec = new Campo();
            if (TypeSpecifier(typeSpecifier))
            {
                declarator.Tipo = typeSpecifier.Tipo;
                if (Declarator(declarator))
                {
                    initDeclarator.Place = declarator.Place;
                    initDeclarator.Tipo = typeSpecifier.Tipo;
                    if (InitDeclarator(initDeclarator))
                    {
                        if (ParametersListRec(parametersListRec))
                        {
                            parametersList.Cod = initDeclarator.Cod;
                            parametersList.Cod += EscreverCodigo("pop {0}", declarator.Place);
                            parametersList.Cod += EscreverCodigo(parametersListRec.Cod);
                            return true;
                        }
                    }
                }
                return false;
            }
            return true;
        }

        public static bool ParametersListRec(Campo parametersListRec)
        {
            var typeSpecifier = new Campo();
            var initDeclarator = new Campo();
            var declarator = new Campo();
            var parametersListRec1 = new Campo();
            if (VerificarToken(TkVirgula))
            {
                LerToken();
                if (TypeSpecifier(typeSpecifier))
                {
                    declarator.Tipo = typeSpecifier.Tipo;
                    if (Declarator(declarator))
                    {
                        initDeclarator.Place = declarator.Place;
                        initDeclarator.Tipo = typeSpecifier.Tipo;
                        if (InitDeclarator(initDeclarator))
                        {
                            if (ParametersListRec(parametersListRec1))
                            {
                                parametersListRec.Cod = initDeclarator.Cod;
                                parametersListRec.Cod += EscreverCodigo("pop {0}", declarator.Place);
                                parametersListRec.Cod += EscreverCodigo(parametersListRec1.Cod);
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            return true;
        }

        // ----------------------------------------------------
        // Statement -> CompoundStatement | ExpressionStatement | SelectionStatement | IterationStatement | JumpStatement

        // SelectionStatement -> if (Expression) Statement  
        // SelectionStatement -> if (Expression) Statement else Statement    
        // SelectionStatement -> switch (Expression) SwitchStatement

        // LabeledStatement -> id : Statement
        // LabeledStatement -> case cte : Statement

        // SwitchStatement -> default : Statement SwitchStatement
        // SwitchStatement -> LabeledStatement SwitchStatement
        // SwitchStatement -> Statement SwitchStatement
        // SwitchStatement -> <vazio>

        // IterationStatement -> for (Declaration; Expression; Expression) Statement 
        // IterationStatement -> for (Expression; Expression; Expression) Statement 
        // IterationStatement -> do Statement while (Expression);
        // IterationStatement -> while (Expression) Statement;

        // JumpStatement -> break;
        // JumpStatement -> continue;
        // JumpStatement -> return Expression;
        // JumpStatement -> return;

        // FunctionStatement -> BlockItemList
        // FunctionStatement -> <vazio>        

        // CompoundStatement -> { BlockItemList } 
        // CompoundStatement -> <vazio>        

        // BlockItemList -> BlockItem | BlockItemList BlockItem

        // BlockItem -> Declaration 
        // BlockItem -> Statement
        // BlockItem -> Declaration Statement

        // ExpressionStatement -> Expression;
        // ExpressionStatement -> ;

        // Expression -> AssigmentExpression ExpressionRec
        // ExpressionRec -> , AssigmentExpression ExpressionRec 
        // ExpressionRec -> <vazio>

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

        // InclusiveOrExpression -> AndExpression InclusiveOrExpressionRec
        // InclusiveOrExpressionRec -> | AndExpression InclusiveOrExpressionRec
        // InclusiveOrExpressionRec -> <vazio>       

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

        // PrimaryExpression -> id
        // PrimaryExpression -> id (ArgumentList)
        // PrimaryExpression -> cte
        // PrimaryExpression -> (Expression)

        // ArgumentList -> AssignmentExpression ArgumentListRec
        // ArgumentList -> <vazio>
        // ArgumentListRec -> , AssignmentExpression ArgumentListRec
        // ArgumentListRec -> <vazio>


        // ----------------------------------------------------       
        public static bool Statement(Campo statement)
        {
            var compoundStatement = new Campo();
            var expressionStatement = new Campo();
            var selectionStatement = new Campo();
            var iterationStatement = new Campo();
            var jumpStatement = new Campo();

            jumpStatement.Rotulo1 = statement.Rotulo1;
            jumpStatement.Rotulo2 = statement.Rotulo2;

            compoundStatement.Rotulo1 = statement.Rotulo1;
            compoundStatement.Rotulo2 = statement.Rotulo2;

            selectionStatement.Rotulo1 = statement.Rotulo1;
            selectionStatement.Rotulo2 = statement.Rotulo2;

            iterationStatement.Rotulo1 = statement.Rotulo1;
            iterationStatement.Rotulo2 = statement.Rotulo2;

            if (CompoundStatement(compoundStatement))
            {
                statement.Cod = compoundStatement.Cod;
                return true;
            }
            else if (IterationStatement(iterationStatement))
            {
                statement.Cod = iterationStatement.Cod;
                return true;
            }
            else if (SelectionStatement(selectionStatement))
            {
                statement.Cod = selectionStatement.Cod;
                return true;
            }
            else if (JumpStatement(jumpStatement))
            {
                statement.Cod = jumpStatement.Cod;
                return true;
            }
            else if (ExpressionStatement(expressionStatement))
            {
                statement.Cod = expressionStatement.Cod;
                return true;
            }
            return false;
        }

        public static bool FunctionStatement(Campo functionStatement)
        {
            var blockItemList = new Campo();

            // propaga os labels 
            blockItemList.Rotulo2 = functionStatement.Rotulo2;
            blockItemList.Rotulo1 = functionStatement.Rotulo1;
            if (BlockItemList(blockItemList))
            {
                functionStatement.Cod = blockItemList.Cod;
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

                var scp = new Escopo(EscopoAtual);
                EscopoAtual = scp;

                try
                {
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
                        GerarExcessao(new[] { "}" });
                        return false;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    EscopoAtual = EscopoAtual.Ref;
                }
            }
            return false;
        }

        public static bool BlockItemList(Campo blockItemList)
        {
            var blockItem = new Campo();
            var blockItemList1 = new Campo();

            // propaga os labels 
            blockItem.Rotulo2 = blockItemList.Rotulo2;
            blockItem.Rotulo1 = blockItemList.Rotulo1;
            if (BlockItem(blockItem))
            {
                blockItemList1.Rotulo2 = blockItem.Rotulo2;
                blockItemList1.Rotulo1 = blockItem.Rotulo1;
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
            statement.Rotulo1 = blockItem.Rotulo1;
            statement.Rotulo2 = blockItem.Rotulo2;

            if (Declaration(declaration))
            {
                blockItem.Cod = EscreverCodigo(declaration.Cod);
                if (Statement(statement))
                {
                    blockItem.Cod += EscreverCodigo(statement.Cod);
                    return true;
                }
                return true;
            }
            else if (Statement(statement))
            {
                blockItem.Cod += EscreverCodigo(statement.Cod);
                return true;
            }
            return false;

        }

        public static bool IterationStatement(Campo iterationStatement)
        {
            var expression = new Campo();
            var expression2 = new Campo();
            var expression3 = new Campo();
            var statement = new Campo();
            var declaration = new Campo();

            if (VerificarToken(TkFor))
            {
                LerToken();
                if (VerificarToken(TkAbreParentese))
                {
                    LerToken();
                    if (Declaration(declaration))
                    {
                        if (Expression(expression2))
                        {
                            if (VerificarToken(TkPontoVirgula))
                            {
                                LerToken();
                                if (Expression(expression3))
                                {
                                    if (VerificarToken(TkFechaParentese))
                                    {
                                        LerToken();
                                        statement.Rotulo1 = GerarRotulo();
                                        statement.Rotulo2 = GerarRotulo();
                                        if (Statement(statement))
                                        {
                                            iterationStatement.Cod += EscreverCodigo(declaration.Cod);
                                            iterationStatement.Cod += EscreverRotulo(statement.Rotulo1);

                                            iterationStatement.Cod += EscreverCodigo(expression2.Cod);
                                            iterationStatement.Cod += EscreverCodigoIfZ(expression2.Place, statement.Rotulo2);

                                            iterationStatement.Cod += EscreverCodigo(statement.Cod);
                                            iterationStatement.Cod += EscreverCodigo(expression3.Cod);
                                            iterationStatement.Cod += EscreverCodigo("goto {0}", statement.Rotulo1);
                                            iterationStatement.Cod += EscreverRotulo(statement.Rotulo2);
                                            return true;
                                        }
                                        GerarExcessao(new[] { "if", "while", "do", "for", "switch", "atribuição" });
                                        return false;
                                    }
                                    GerarExcessao(new[] { ")" });
                                    return false;
                                }
                                GerarExcessao(new[] { "expressao" });
                                return false;
                            }
                            GerarExcessao(new[] { ";" });
                            return false;
                        }
                        GerarExcessao(new[] { "expressao" });
                        return false;
                    }
                    else if (Expression(expression))
                    {
                        if (VerificarToken(TkPontoVirgula))
                        {
                            LerToken();
                            if (Expression(expression2))
                            {
                                if (VerificarToken(TkPontoVirgula))
                                {
                                    LerToken();
                                    if (Expression(expression3))
                                    {
                                        if (VerificarToken(TkFechaParentese))
                                        {
                                            LerToken();
                                            if (Statement(statement))
                                            {
                                                iterationStatement.Cod += EscreverCodigo(expression.Cod);
                                                iterationStatement.Cod += EscreverRotulo(statement.Rotulo1);

                                                iterationStatement.Cod += EscreverCodigo(expression2.Cod);
                                                iterationStatement.Cod += EscreverCodigoIfZ(expression2.Place, statement.Rotulo2);

                                                iterationStatement.Cod += EscreverCodigo(statement.Cod);
                                                iterationStatement.Cod += EscreverCodigo(expression3.Cod);
                                                iterationStatement.Cod += EscreverCodigo("goto {0}", statement.Rotulo1);
                                                iterationStatement.Cod += EscreverRotulo(statement.Rotulo2);
                                                return true;

                                            }
                                            GerarExcessao(new[] { "if", "while", "do", "for", "switch", "atribuição" });
                                            return false;
                                        }
                                        GerarExcessao(new[] { ")" });
                                        return false;
                                    }
                                    GerarExcessao(new[] { "expressao" });
                                    return false;
                                }
                                GerarExcessao(new[] { ";" });
                                return false;
                            }
                            GerarExcessao(new[] { "expressao" });
                            return false;
                        }
                        GerarExcessao(new[] { ";" });
                        return false;
                    }
                    GerarExcessao(new[] { "expressao", "declaracao" });
                    return false;
                }
                GerarExcessao(new[] { "(" });
                return false;
            }
            else if (VerificarToken(TkDo))
            {
                LerToken();
                statement.Rotulo1 = GerarRotulo();
                statement.Rotulo2 = GerarRotulo();
                if (Statement(statement))
                {
                    if (VerificarToken(TkWhile))
                    {
                        LerToken();
                        if (VerificarToken(TkAbreParentese))
                        {
                            LerToken();
                            if (Expression(expression))
                            {
                                if (VerificarToken(TkFechaParentese))
                                {
                                    LerToken();
                                    if (VerificarToken(TkPontoVirgula))
                                    {
                                        LerToken();
                                        iterationStatement.Cod += EscreverRotulo(statement.Rotulo1);
                                        iterationStatement.Cod += EscreverCodigo(statement.Cod);
                                        iterationStatement.Cod += EscreverCodigo(expression.Cod);
                                        iterationStatement.Cod += EscreverCodigoIf(expression.Place, statement.Rotulo1);
                                        iterationStatement.Cod += EscreverCodigo("goto {0}", statement.Rotulo2);
                                        iterationStatement.Cod += EscreverRotulo(statement.Rotulo2);
                                        return true;
                                    }
                                    GerarExcessao(new[] { ";" });
                                    return false;
                                }
                                GerarExcessao(new[] { ")" });
                                return false;
                            }
                            GerarExcessao(new[] { "expressao" });
                            return false;
                        }
                        GerarExcessao(new[] { "(" });
                        return false;
                    }
                    GerarExcessao(new[] { "while" });
                    return false;
                }
                GerarExcessao(new[] { "if", "while", "do", "for", "switch", "atribuição" });
                return false;
            }
            else if (VerificarToken(TkWhile))
            {
                LerToken();
                if (VerificarToken(TkAbreParentese))
                {
                    LerToken();
                    if (Expression(expression))
                    {
                        if (VerificarToken(TkFechaParentese))
                        {
                            LerToken();

                            statement.Rotulo1 = GerarRotulo();
                            statement.Rotulo2 = GerarRotulo();
                            if (Statement(statement))
                            {
                                iterationStatement.Cod += EscreverRotulo(statement.Rotulo1);
                                iterationStatement.Cod += EscreverCodigo(expression.Cod);
                                iterationStatement.Cod += EscreverCodigoIfZ(expression.Place, statement.Rotulo2);
                                iterationStatement.Cod += EscreverCodigo(statement.Cod);
                                iterationStatement.Cod += EscreverCodigo("goto {0}", statement.Rotulo1);
                                iterationStatement.Cod += EscreverRotulo(statement.Rotulo2);
                                return true;
                            }
                            GerarExcessao(new[] { "if", "while", "do", "for", "switch", "atribuição", ";", "expressao" });
                            return false;
                        }
                        GerarExcessao(new[] { ")" });
                        return false;
                    }
                    GerarExcessao(new[] { "expressao" });
                    return false;
                }
                GerarExcessao(new[] { "(" });
                return false;
            }
            return false;
        }

        public static bool SelectionStatement(Campo selectionStatement)
        {
            var expression = new Campo();
            var statement1 = new Campo();
            var statement2 = new Campo();
            var switchStatement = new Campo();

            if (VerificarToken(TkIf))
            {
                LerToken();
                if (VerificarToken(TkAbreParentese))
                {
                    LerToken();
                    if (Expression(expression))
                    {
                        if (VerificarToken(TkFechaParentese))
                        {
                            LerToken();
                            statement1.Rotulo1 = selectionStatement.Rotulo1; // aqui vai ser rotulo do while
                            statement1.Rotulo2 = selectionStatement.Rotulo2;
                            if (Statement(statement1))
                            {
                                statement1.Rotulo1 = GerarRotulo();
                                statement1.Rotulo2 = GerarRotulo();
                                if (VerificarToken(TkElse))
                                {
                                    LerToken();
                                    if (Statement(statement2))
                                    {
                                        selectionStatement.Cod += EscreverCodigo(expression.Cod);
                                        selectionStatement.Cod += EscreverCodigoIfZ(expression.Place, statement1.Rotulo1); // goto else                                    
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
                                    selectionStatement.Cod += EscreverCodigo(expression.Cod);
                                    selectionStatement.Cod += EscreverCodigoIfZ(expression.Place, statement1.Rotulo1); // goto else                                    
                                    selectionStatement.Cod += EscreverCodigo(statement1.Cod);
                                    selectionStatement.Cod += EscreverRotulo(statement1.Rotulo1);
                                    return true;
                                }
                            }
                            GerarExcessao(new[] { "if", "while", "do", "for", "switch", "atribuição", ";", "expressao" });
                            return false;
                        }
                        GerarExcessao(new[] { ")" });
                        return false;
                    }
                    GerarExcessao(new[] { "expressao" });
                    return false;
                }
                GerarExcessao(new[] { "(" });
                return false;
            }
            else if (VerificarToken(TkSwitch))
            {
                LerToken();
                if (VerificarToken(TkAbreParentese))
                {
                    LerToken();
                    if (Expression(expression))
                    {
                        if (VerificarToken(TkFechaParentese))
                        {
                            LerToken();

                            if (VerificarToken(TkAbreChaves))
                            {
                                LerToken();
                                switchStatement.Rotulo2 = GerarRotulo(); // rotulo para o fim 
                                switchStatement.Place = expression.Place; // Esse vai ser a comparação nos testes dos cases
                                if (SwitchStatement(switchStatement))
                                {
                                    if (VerificarToken(TkFechaChaves))
                                    {
                                        LerToken();

                                        selectionStatement.Cod += EscreverCodigo(expression.Cod);
                                        selectionStatement.Cod += EscreverCodigo(switchStatement.Cod); // aqui vai ter os ifs
                                        if (!string.IsNullOrEmpty(switchStatement.RotuloDefault))
                                            selectionStatement.Cod += EscreverCodigo("goto {0}", switchStatement.RotuloDefault);
                                        selectionStatement.Cod += EscreverCodigo(switchStatement.CodTestesSwitch); // aqui vai ter os ifs
                                        selectionStatement.Cod += EscreverRotulo(switchStatement.Rotulo2);
                                        return true;
                                    }
                                    GerarExcessao(new[] { "}" });
                                    return false;
                                }
                                GerarExcessao(new[] { "constante", "case", "default" });
                                return false;
                            }
                            GerarExcessao(new[] { "{" });
                            return false;
                        }
                        GerarExcessao(new[] { ")" });
                        return false;
                    }
                    GerarExcessao(new[] { "expressao" });
                    return false;
                }
                GerarExcessao(new[] { "(" });
                return false;
            }
            return false;
        }

        public static bool SwitchStatement(Campo switchStatement)
        {
            var statement = new Campo();
            var labeledStatement = new Campo();
            var switchStatement1 = new Campo();

            labeledStatement.Rotulo1 = switchStatement.Rotulo1;
            labeledStatement.Rotulo2 = switchStatement.Rotulo2;
            statement.Rotulo1 = switchStatement.Rotulo1;
            statement.Rotulo2 = switchStatement.Rotulo2;
            switchStatement1.Rotulo1 = switchStatement.Rotulo1;
            switchStatement1.Rotulo2 = switchStatement.Rotulo2;
            if (LabeledStatement(labeledStatement))
            {
                switchStatement.Cod += EscreverCodigoIf(switchStatement.Place, "==", labeledStatement.Place, labeledStatement.Rotulo1);
                switchStatement1.Place = switchStatement.Place;
                switchStatement1.CodTestesSwitch += labeledStatement.CodTestesSwitch;
                if (SwitchStatement(switchStatement1))
                {
                    switchStatement.RotuloDefault = switchStatement1.RotuloDefault;
                    switchStatement.Cod += EscreverCodigo(switchStatement1.Cod);
                    switchStatement.CodTestesSwitch += EscreverCodigo(switchStatement1.CodTestesSwitch);
                }
                return true;
            }
            else if (Statement(statement))
            {
                switchStatement.CodTestesSwitch += statement.Cod;
                switchStatement1.Place = switchStatement.Place;
                if (SwitchStatement(switchStatement1))
                {
                    switchStatement.RotuloDefault = switchStatement1.RotuloDefault;
                    switchStatement.Cod += EscreverCodigo(switchStatement1.Cod);
                    switchStatement.CodTestesSwitch += EscreverCodigo(switchStatement1.CodTestesSwitch);
                }
                return true;
            }
            else if (VerificarToken(TkDefault))
            {
                LerToken();
                if (!string.IsNullOrEmpty(switchStatement.RotuloDefault))
                {
                    GerarExcessao(new[] { "case", "id" });
                    return false;
                }

                if (VerificarToken(TkDoisPontos))
                {
                    LerToken();
                    if (Statement(statement))
                    {
                        switchStatement.RotuloDefault = GerarRotulo();
                        switchStatement1.Place = switchStatement.Place;
                        if (SwitchStatement(switchStatement1))
                        {
                            switchStatement.Cod += EscreverCodigo(switchStatement1.Cod);
                            switchStatement.CodTestesSwitch += EscreverCodigo(switchStatement1.CodTestesSwitch);
                        }

                        switchStatement.CodTestesSwitch += EscreverRotulo(switchStatement.RotuloDefault);
                        switchStatement.CodTestesSwitch += EscreverCodigo(statement.Cod);
                        return true;
                    }
                    GerarExcessao(new[] { "if", "while", "do", "for", "switch", "atribuição", ";", "expressao" });
                    return false;
                }
                GerarExcessao(new[] { ":" });
                return false;
            }
            return true;
        }

        public static bool LabeledStatement(Campo labeledStatement)
        {
            var statement = new Campo();

            if (VerificarToken(TkId))
            {
                if (!VerificarVariavelTab(TokenAtual)) return false;

                LerToken();
                if (VerificarToken(TkDoisPontos))
                {
                    LerToken();
                    statement.Rotulo1 = labeledStatement.Rotulo1;
                    statement.Rotulo2 = labeledStatement.Rotulo2;
                    if (Statement(statement))
                    {
                        labeledStatement.Rotulo1 = GerarRotulo();
                        labeledStatement.CodTestesSwitch += EscreverRotulo(labeledStatement.Rotulo1);
                        labeledStatement.CodTestesSwitch += EscreverCodigo(statement.Cod);
                        return true;
                    }
                    GerarExcessao(new[] { "if", "while", "do", "for", "switch", "atribuição", ";", "expressao" });
                    return false;
                }
                GerarExcessao(new[] { ":" });
                return false;
            }
            else if (VerificarToken(TkCase))
            {
                LerToken();
                if (VerificarToken(TkConst))
                {
                    labeledStatement.Place = TokenAtual;
                    LerToken();
                    if (VerificarToken(TkDoisPontos))
                    {
                        LerToken();
                        statement.Rotulo1 = labeledStatement.Rotulo1;
                        statement.Rotulo2 = labeledStatement.Rotulo2;
                        if (Statement(statement))
                        {
                            labeledStatement.Rotulo1 = GerarRotulo();
                            labeledStatement.CodTestesSwitch += EscreverRotulo(labeledStatement.Rotulo1);
                            labeledStatement.CodTestesSwitch += EscreverCodigo(statement.Cod);
                            return true;
                        }
                        GerarExcessao(new[] { "if", "while", "do", "for", "switch", "atribuição", ";", "expressao" });
                        return false;
                    }
                    GerarExcessao(new[] { ":" });
                    return false;
                }
                GerarExcessao(new[] { "constante" });
                return false;
            }
            return false;
        }

        public static bool JumpStatement(Campo jumpStatement)
        {

            if (VerificarToken(TkBreak))
            {
                if (string.IsNullOrEmpty(jumpStatement.Rotulo2))
                    return false;

                jumpStatement.Cod += EscreverCodigo("goto {0}", jumpStatement.Rotulo2);
                LerToken();
                return true;

            }
            else if (VerificarToken(TkContinue))
            {
                if (string.IsNullOrEmpty(jumpStatement.Rotulo1))
                    return false;

                jumpStatement.Cod += EscreverCodigo("goto {0}", jumpStatement.Rotulo1);
                LerToken();
                return true;

            }
            else if (VerificarToken(TkReturn))
            {
                LerToken();
                var expression = new Campo();
                if (Expression(expression))
                {
                    jumpStatement.Cod = expression.Cod;
                    jumpStatement.Cod += EscreverCodigo("push {0}", expression.Place);
                }
                jumpStatement.Cod += EscreverCodigo("goto {0}", "BP"); // aqui vai implementacao do BP
                return true;
            }
            return false;
        }

        public static bool ExpressionStatement(Campo expressionStatement)
        {
            var expression = new Campo();
            if (IdTokenAtual == TkPontoVirgula)
            {
                LerToken();
                return true;
            }
            else if (Expression(expression))
            {
                if (IdTokenAtual == TkPontoVirgula)
                {
                    LerToken();
                    expressionStatement.Cod = expression.Cod;
                    return true;
                }
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
                GerarExcessao(new[] { "expressao" });
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

            logicalOrExpression.EhDeclarador = assignmentExpression.EhDeclarador;
            assignmentExpression1.EhDeclarador = assignmentExpression.EhDeclarador;
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
                    GerarExcessao(new[] { "expressao" });
                    return false;

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
                    GerarExcessao(new[] { "expressao" });
                    return false;
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
                    GerarExcessao(new[] { "expressao" });
                    return false;
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
                    GerarExcessao(new[] { "expressao" });
                    return false;
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
                    GerarExcessao(new[] { "expressao" });
                    return false;
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
                    GerarExcessao(new[] { "expressao" });
                    return false;
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

            logicalAndExpression.EhDeclarador = logicalOrExpression.EhDeclarador;
            if (LogicalAndExpression(logicalAndExpression))
            {
                logicalOrExpressionRecH.EhDeclarador = logicalOrExpression.EhDeclarador;
                logicalOrExpressionRecH.Cod = logicalAndExpression.Cod;
                logicalOrExpressionRecH.Place = logicalAndExpression.Place;
                if (LogicalOrExpressionRec(logicalOrExpressionRecH, logicalOrExpressionRecS))
                {
                    logicalOrExpression.Cod = logicalOrExpressionRecS.Cod;
                    logicalOrExpression.Place = logicalOrExpressionRecS.Place;
                    logicalOrExpression.EhDeclarador = logicalOrExpressionRecS.EhDeclarador;
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

                logicalAndExpression.EhDeclarador = logicalOrExpressionRecH.EhDeclarador;
                if (LogicalAndExpression(logicalAndExpression))
                {
                    var lblFim = GerarRotulo();
                    var lblAvalSegExpressao = GerarRotulo();
                    logicalOrExpressionRec1H.Place = GerarTemp();
                    logicalOrExpressionRec1H.Cod += EscreverCodigo(logicalOrExpressionRecH.Cod);

                    logicalOrExpressionRec1H.Cod += EscreverCodigoIf(logicalOrExpressionRecH.Place, "=", "0", lblAvalSegExpressao);
                    logicalOrExpressionRec1H.Cod += EscreverCodigo(string.Empty, logicalAndExpression.Place, "1", string.Empty);
                    logicalOrExpressionRec1H.Cod += EscreverCodigo("goto {0}", lblFim);

                    logicalOrExpressionRec1H.Cod += EscreverRotulo(lblAvalSegExpressao);
                    logicalOrExpressionRec1H.Cod += EscreverCodigo(logicalAndExpression.Cod);

                    logicalOrExpressionRec1H.Cod += EscreverRotulo(lblFim);
                    logicalOrExpressionRec1H.Cod += EscreverCodigo("or", logicalOrExpressionRec1H.Place, logicalOrExpressionRecH.Place, logicalAndExpression.Place);

                    if (LogicalOrExpressionRec(logicalOrExpressionRec1H, logicalOrExpressionRec1S))
                    {
                        logicalOrExpressionRecS.Cod = logicalOrExpressionRec1S.Cod;
                        logicalOrExpressionRecS.Place = logicalOrExpressionRec1S.Place;
                        return true;
                    }
                }
                GerarExcessao(new[] { "expressao" });
                return false;
            }
            else
            {
                logicalOrExpressionRecS.Cod = logicalOrExpressionRecH.Cod;
                logicalOrExpressionRecS.Place = logicalOrExpressionRecH.Place;
                logicalOrExpressionRecS.EhDeclarador = logicalOrExpressionRecH.EhDeclarador;
                return true;
            }
        }

        public static bool LogicalAndExpression(Campo logicalAndExpression)
        {
            var inclusiveOrExpression = new Campo();
            var logicalAndExpressionRecH = new Campo();
            var logicalAndExpressionRecS = new Campo();
            inclusiveOrExpression.EhDeclarador = logicalAndExpression.EhDeclarador;
            if (InclusiveOrExpression(inclusiveOrExpression))
            {
                logicalAndExpressionRecH.Cod = inclusiveOrExpression.Cod;
                logicalAndExpressionRecH.Place = inclusiveOrExpression.Place;
                logicalAndExpressionRecH.EhDeclarador = inclusiveOrExpression.EhDeclarador;
                if (LogicalAndExpressionRec(logicalAndExpressionRecH, logicalAndExpressionRecS))
                {
                    logicalAndExpression.Cod = logicalAndExpressionRecS.Cod;
                    logicalAndExpression.Place = logicalAndExpressionRecS.Place;
                    logicalAndExpression.EhDeclarador = logicalAndExpressionRecS.EhDeclarador;
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
                inclusiveOrExpression.EhDeclarador = logicalAndExpressionRecH.EhDeclarador;
                if (InclusiveOrExpression(inclusiveOrExpression))
                {
                    var lblFim = GerarRotulo();
                    var lblAvalSegExpressao = GerarRotulo();
                    logicalAndExpressionRec1H.Place = GerarTemp();
                    logicalAndExpressionRec1H.Cod += EscreverCodigo(logicalAndExpressionRecH.Cod);

                    logicalAndExpressionRec1H.Cod += EscreverCodigoIf(logicalAndExpressionRecH.Place, "=", "1", lblAvalSegExpressao);
                    logicalAndExpressionRec1H.Cod += EscreverCodigo(string.Empty, inclusiveOrExpression.Place, "0", string.Empty);
                    logicalAndExpressionRec1H.Cod += EscreverCodigo("goto {0}", lblFim);

                    logicalAndExpressionRec1H.Cod += EscreverRotulo(lblAvalSegExpressao);
                    logicalAndExpressionRec1H.Cod += EscreverCodigo(inclusiveOrExpression.Cod);

                    logicalAndExpressionRec1H.Cod += EscreverRotulo(lblFim);
                    logicalAndExpressionRec1H.Cod += EscreverCodigo("and", logicalAndExpressionRec1H.Place, logicalAndExpressionRecH.Place, inclusiveOrExpression.Place);

                    if (LogicalAndExpressionRec(logicalAndExpressionRec1H, logicalAndExpressionRec1S))
                    {
                        logicalAndExpressionRecS.Cod = logicalAndExpressionRec1S.Cod;
                        logicalAndExpressionRecS.Place = logicalAndExpressionRec1S.Place;
                        logicalAndExpressionRecS.EhDeclarador = logicalAndExpressionRec1S.EhDeclarador;
                        return true;
                    }
                }
                GerarExcessao(new[] { "expressao" });
                return false;
            }
            else
            {
                logicalAndExpressionRecS.Cod = logicalAndExpressionRecH.Cod;
                logicalAndExpressionRecS.Place = logicalAndExpressionRecH.Place;
                logicalAndExpressionRecS.EhDeclarador = logicalAndExpressionRecH.EhDeclarador;
                return true;
            }
        }

        public static bool InclusiveOrExpression(Campo inclusiveOrExpression)
        {
            var andOrExpression = new Campo();
            var inclusiveOrExpressionRecH = new Campo();
            var inclusiveOrExpressionRecS = new Campo();
            andOrExpression.EhDeclarador = inclusiveOrExpression.EhDeclarador;
            if (AndExpression(andOrExpression))
            {
                inclusiveOrExpressionRecH.Cod = andOrExpression.Cod;
                inclusiveOrExpressionRecH.Place = andOrExpression.Place;
                inclusiveOrExpressionRecH.EhDeclarador = andOrExpression.EhDeclarador;
                if (InclusiveOrExpressionRec(inclusiveOrExpressionRecH, inclusiveOrExpressionRecS))
                {
                    inclusiveOrExpression.Cod = inclusiveOrExpressionRecS.Cod;
                    inclusiveOrExpression.Place = inclusiveOrExpressionRecS.Place;
                    inclusiveOrExpression.EhDeclarador = inclusiveOrExpressionRecS.EhDeclarador;
                    return true;
                }
            }
            return false;
        }

        public static bool InclusiveOrExpressionRec(Campo inclusiveOrExpressionRecH, Campo inclusiveOrExpressionRecS)
        {
            var andOrExpression = new Campo();
            var inclusiveOrExpressionRec1H = new Campo();
            var inclusiveOrExpressionRec1S = new Campo();
            if (VerificarToken(TkOr))
            {
                LerToken();
                andOrExpression.EhDeclarador = inclusiveOrExpressionRecH.EhDeclarador;
                if (AndExpression(andOrExpression))
                {
                    inclusiveOrExpressionRec1H.Place = GerarTemp();
                    inclusiveOrExpressionRec1H.Cod += EscreverCodigo(inclusiveOrExpressionRecH.Cod);
                    inclusiveOrExpressionRec1H.Cod += EscreverCodigo(andOrExpression.Cod);
                    inclusiveOrExpressionRec1H.Cod += EscreverCodigo("or", inclusiveOrExpressionRec1H.Place, inclusiveOrExpressionRecH.Place, andOrExpression.Place);

                    inclusiveOrExpressionRec1H.EhDeclarador = inclusiveOrExpressionRecH.EhDeclarador;
                    if (InclusiveOrExpressionRec(inclusiveOrExpressionRec1H, inclusiveOrExpressionRec1S))
                    {
                        inclusiveOrExpressionRecS.Cod = inclusiveOrExpressionRec1S.Cod;
                        inclusiveOrExpressionRecS.Place = inclusiveOrExpressionRec1S.Place;
                        inclusiveOrExpressionRecS.EhDeclarador = inclusiveOrExpressionRec1S.EhDeclarador;
                        return true;
                    }
                }
                GerarExcessao(new[] { "expressao" });
                return false;
            }
            else
            {
                inclusiveOrExpressionRecS.Cod = inclusiveOrExpressionRecH.Cod;
                inclusiveOrExpressionRecS.Place = inclusiveOrExpressionRecH.Place;
                inclusiveOrExpressionRecS.EhDeclarador = inclusiveOrExpressionRecH.EhDeclarador;
                return true;
            }
        }

        public static bool AndExpression(Campo andExpression)
        {
            var equalityExpression = new Campo();
            var andExpressionRecH = new Campo();
            var andExpressionRecS = new Campo();
            equalityExpression.EhDeclarador = andExpression.EhDeclarador;
            if (EqualityExpression(equalityExpression))
            {
                andExpressionRecH.Cod = equalityExpression.Cod;
                andExpressionRecH.Place = equalityExpression.Place;
                andExpressionRecH.EhDeclarador = equalityExpression.EhDeclarador;
                if (AndExpressionRec(andExpressionRecH, andExpressionRecS))
                {
                    andExpression.Cod = andExpressionRecS.Cod;
                    andExpression.Place = andExpressionRecS.Place;
                    andExpression.EhDeclarador = andExpressionRecS.EhDeclarador;
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
                equalityExpression.EhDeclarador = andExpressionRecH.EhDeclarador;
                if (EqualityExpression(equalityExpression))
                {
                    andExpressionRec1H.Place = GerarTemp();
                    andExpressionRec1H.Cod += EscreverCodigo(andExpressionRecH.Cod);
                    andExpressionRec1H.Cod += EscreverCodigo(equalityExpression.Cod);
                    andExpressionRec1H.Cod += EscreverCodigo("and", andExpressionRec1H.Place, andExpressionRecH.Place, equalityExpression.Place);

                    andExpressionRec1H.EhDeclarador = andExpressionRecH.EhDeclarador;
                    if (AndExpressionRec(andExpressionRec1H, andExpressionRec1S))
                    {
                        andExpressionRecS.Cod = andExpressionRec1S.Cod;
                        andExpressionRecS.Place = andExpressionRec1S.Place;
                        andExpressionRecS.EhDeclarador = andExpressionRec1S.EhDeclarador;
                        return true;
                    }
                }
                GerarExcessao(new[] { "expressao" });
                return false;
            }
            else
            {
                andExpressionRecS.Cod = andExpressionRecH.Cod;
                andExpressionRecS.Place = andExpressionRecH.Place;
                andExpressionRecS.EhDeclarador = andExpressionRecH.EhDeclarador;
                return true;
            }
        }

        public static bool EqualityExpression(Campo equalityExpression)
        {
            var relationalExpression = new Campo();
            var equalityExpressionRecH = new Campo();
            var equalityExpressionRecS = new Campo();

            relationalExpression.EhDeclarador = equalityExpression.EhDeclarador;
            if (RelationalExpression(relationalExpression))
            {
                equalityExpressionRecH.Cod = relationalExpression.Cod;
                equalityExpressionRecH.Place = relationalExpression.Place;

                equalityExpressionRecH.EhDeclarador = equalityExpression.EhDeclarador;
                if (EqualityExpressionRec(equalityExpressionRecH, equalityExpressionRecS))
                {
                    equalityExpression.Cod = equalityExpressionRecS.Cod;
                    equalityExpression.Place = equalityExpressionRecS.Place;
                    equalityExpression.EhDeclarador = equalityExpressionRecS.EhDeclarador;
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
                relationalExpression.EhDeclarador = equalityExpressionRecH.EhDeclarador;
                if (RelationalExpression(relationalExpression))
                {
                    equalityExpressionRec1H.Place = GerarTemp();
                    equalityExpressionRec1H.Cod += EscreverCodigo(equalityExpressionRecH.Cod);
                    equalityExpressionRec1H.Cod += EscreverCodigo(relationalExpression.Cod);

                    var lblTrue = GerarRotulo();
                    var lblFim = GerarRotulo();

                    equalityExpressionRec1H.Cod += EscreverCodigoIf(equalityExpressionRecH.Place, "=", relationalExpression.Place, lblTrue);
                    equalityExpressionRec1H.Cod += EscreverCodigo(string.Empty, equalityExpressionRec1H.Place, "0", string.Empty);
                    equalityExpressionRec1H.Cod += EscreverCodigo("goto {0}", lblFim);
                    equalityExpressionRec1H.Cod += EscreverRotulo(lblTrue);
                    equalityExpressionRec1H.Cod += EscreverCodigo(string.Empty, equalityExpressionRec1H.Place, "1", string.Empty);
                    equalityExpressionRec1H.Cod += EscreverRotulo(lblFim);

                    equalityExpressionRec1H.EhDeclarador = equalityExpressionRecH.EhDeclarador;
                    if (EqualityExpressionRec(equalityExpressionRec1H, equalityExpressionRec1S))
                    {
                        equalityExpressionRecS.Cod = equalityExpressionRec1S.Cod;
                        equalityExpressionRecS.Place = equalityExpressionRec1S.Place;
                        equalityExpressionRecS.EhDeclarador = equalityExpressionRec1S.EhDeclarador;
                        return true;
                    }
                }
                GerarExcessao(new[] { "expressao" });
                return false;
            }
            else if (IdTokenAtual == TkDiferente)
            {
                LerToken();
                relationalExpression.EhDeclarador = equalityExpressionRecH.EhDeclarador;
                if (RelationalExpression(relationalExpression))
                {
                    equalityExpressionRec1H.Place = GerarTemp();
                    equalityExpressionRec1H.Cod += EscreverCodigo(equalityExpressionRecH.Cod);
                    equalityExpressionRec1H.Cod += EscreverCodigo(relationalExpression.Cod);

                    var lblTrue = GerarRotulo();
                    var lblFim = GerarRotulo();

                    equalityExpressionRec1H.Cod += EscreverCodigoIf(equalityExpressionRecH.Place, "!=", relationalExpression.Place, lblTrue);
                    equalityExpressionRec1H.Cod += EscreverCodigo(string.Empty, equalityExpressionRec1H.Place, "0", string.Empty);
                    equalityExpressionRec1H.Cod += EscreverCodigo("goto {0}", lblFim);
                    equalityExpressionRec1H.Cod += EscreverRotulo(lblTrue);
                    equalityExpressionRec1H.Cod += EscreverCodigo(string.Empty, equalityExpressionRec1H.Place, "1", string.Empty);
                    equalityExpressionRec1H.Cod += EscreverRotulo(lblFim);

                    equalityExpressionRec1H.EhDeclarador = equalityExpressionRecH.EhDeclarador;
                    if (EqualityExpressionRec(equalityExpressionRec1H, equalityExpressionRec1S))
                    {
                        equalityExpressionRecS.Cod = equalityExpressionRec1S.Cod;
                        equalityExpressionRecS.Place = equalityExpressionRec1S.Place;
                        equalityExpressionRecS.EhDeclarador = equalityExpressionRec1S.EhDeclarador;
                        return true;
                    }
                }
                GerarExcessao(new[] { "expressao" });
                return false;
            }
            else
            {
                equalityExpressionRecS.Cod = equalityExpressionRecH.Cod;
                equalityExpressionRecS.Place = equalityExpressionRecH.Place;
                equalityExpressionRecS.EhDeclarador = equalityExpressionRecH.EhDeclarador;
                return true;
            }
        }

        public static bool RelationalExpression(Campo relationalExpression)
        {
            var addictiveExpression = new Campo();
            var relationalExpressionRecH = new Campo();
            var relationalExpressionRecS = new Campo();
            addictiveExpression.EhDeclarador = relationalExpression.EhDeclarador;
            if (AddictiveExpression(addictiveExpression))
            {
                relationalExpressionRecH.Cod = addictiveExpression.Cod;
                relationalExpressionRecH.Place = addictiveExpression.Place;
                relationalExpressionRecH.EhDeclarador = addictiveExpression.EhDeclarador;
                if (RelationalExpressionRec(relationalExpressionRecH, relationalExpressionRecS))
                {
                    relationalExpression.Cod = relationalExpressionRecS.Cod;
                    relationalExpression.Place = relationalExpressionRecS.Place;
                    relationalExpression.EhDeclarador = relationalExpressionRecS.EhDeclarador;
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
                addictiveExpression.EhDeclarador = relationalExpressionRecH.EhDeclarador;
                if (AddictiveExpression(addictiveExpression))
                {
                    relationalExpressionRec1H.Place = GerarTemp();
                    relationalExpressionRec1H.Cod += EscreverCodigo(relationalExpressionRecH.Cod);
                    relationalExpressionRec1H.Cod += EscreverCodigo(addictiveExpression.Cod);

                    var lblTrue = GerarRotulo();
                    var lblFim = GerarRotulo();

                    relationalExpressionRec1H.Cod += EscreverCodigoIf(relationalExpressionRecH.Place, "<", addictiveExpression.Place, lblTrue);
                    relationalExpressionRec1H.Cod += EscreverCodigo(string.Empty, relationalExpressionRec1H.Place, "0", string.Empty);
                    relationalExpressionRec1H.Cod += EscreverCodigo("goto {0}", lblFim);
                    relationalExpressionRec1H.Cod += EscreverRotulo(lblTrue);
                    relationalExpressionRec1H.Cod += EscreverCodigo(string.Empty, relationalExpressionRec1H.Place, "1", string.Empty);
                    relationalExpressionRec1H.Cod += EscreverRotulo(lblFim);

                    relationalExpressionRec1H.EhDeclarador = relationalExpressionRecH.EhDeclarador;
                    if (RelationalExpressionRec(relationalExpressionRec1H, relationalExpressionRec1S))
                    {
                        relationalExpressionRecS.Cod = relationalExpressionRec1S.Cod;
                        relationalExpressionRecS.Place = relationalExpressionRec1S.Place;
                        relationalExpressionRecS.EhDeclarador = relationalExpressionRec1S.EhDeclarador;
                        return true;
                    }
                }
                GerarExcessao(new[] { "expressao" });
                return false;
            }
            else if (VerificarToken(TkMaior))
            {
                LerToken();
                addictiveExpression.EhDeclarador = relationalExpressionRecH.EhDeclarador;
                if (AddictiveExpression(addictiveExpression))
                {
                    relationalExpressionRec1H.Place = GerarTemp();
                    relationalExpressionRec1H.Cod += EscreverCodigo(relationalExpressionRecH.Cod);
                    relationalExpressionRec1H.Cod += EscreverCodigo(addictiveExpression.Cod);

                    var lblTrue = GerarRotulo();
                    var lblFim = GerarRotulo();

                    relationalExpressionRec1H.Cod += EscreverCodigoIf(relationalExpressionRecH.Place, ">", addictiveExpression.Place, lblTrue);
                    relationalExpressionRec1H.Cod += EscreverCodigo(string.Empty, relationalExpressionRec1H.Place, "0", string.Empty);
                    relationalExpressionRec1H.Cod += EscreverCodigo("goto {0}", lblFim);
                    relationalExpressionRec1H.Cod += EscreverRotulo(lblTrue);
                    relationalExpressionRec1H.Cod += EscreverCodigo(string.Empty, relationalExpressionRec1H.Place, "1", string.Empty);
                    relationalExpressionRec1H.Cod += EscreverRotulo(lblFim);

                    relationalExpressionRec1H.EhDeclarador = relationalExpressionRecH.EhDeclarador;
                    if (RelationalExpressionRec(relationalExpressionRec1H, relationalExpressionRec1S))
                    {
                        relationalExpressionRecS.Cod = relationalExpressionRec1S.Cod;
                        relationalExpressionRecS.Place = relationalExpressionRec1S.Place;
                        relationalExpressionRecS.EhDeclarador = relationalExpressionRec1S.EhDeclarador;
                        return true;
                    }
                }
                GerarExcessao(new[] { "expressao" });
                return false;
            }
            else if (VerificarToken(TkMenorIgual))
            {
                LerToken();
                addictiveExpression.EhDeclarador = relationalExpressionRecH.EhDeclarador;
                if (AddictiveExpression(addictiveExpression))
                {
                    relationalExpressionRec1H.Place = GerarTemp();
                    relationalExpressionRec1H.Cod += EscreverCodigo(relationalExpressionRecH.Cod);
                    relationalExpressionRec1H.Cod += EscreverCodigo(addictiveExpression.Cod);

                    var lblTrue = GerarRotulo();
                    var lblFim = GerarRotulo();

                    relationalExpressionRec1H.Cod += EscreverCodigoIf(relationalExpressionRecH.Place, "<=", addictiveExpression.Place, lblTrue);
                    relationalExpressionRec1H.Cod += EscreverCodigo(string.Empty, relationalExpressionRec1H.Place, "0", string.Empty);
                    relationalExpressionRec1H.Cod += EscreverCodigo("goto {0}", lblFim);
                    relationalExpressionRec1H.Cod += EscreverRotulo(lblTrue);
                    relationalExpressionRec1H.Cod += EscreverCodigo(string.Empty, relationalExpressionRec1H.Place, "1", string.Empty);
                    relationalExpressionRec1H.Cod += EscreverRotulo(lblFim);

                    relationalExpressionRec1H.EhDeclarador = relationalExpressionRecH.EhDeclarador;
                    if (RelationalExpressionRec(relationalExpressionRec1H, relationalExpressionRec1S))
                    {
                        relationalExpressionRecS.Cod = relationalExpressionRec1S.Cod;
                        relationalExpressionRecS.Place = relationalExpressionRec1S.Place;
                        relationalExpressionRecS.EhDeclarador = relationalExpressionRec1S.EhDeclarador;
                        return true;
                    }
                }
                GerarExcessao(new[] { "expressao" });
                return false;
            }
            else if (VerificarToken(TkMaiorIgual))
            {
                LerToken();
                addictiveExpression.EhDeclarador = relationalExpressionRecH.EhDeclarador;
                if (AddictiveExpression(addictiveExpression))
                {
                    relationalExpressionRec1H.Place = GerarTemp();
                    relationalExpressionRec1H.Cod += EscreverCodigo(relationalExpressionRecH.Cod);
                    relationalExpressionRec1H.Cod += EscreverCodigo(addictiveExpression.Cod);

                    var lblTrue = GerarRotulo();
                    var lblFim = GerarRotulo();

                    relationalExpressionRec1H.Cod += EscreverCodigoIf(relationalExpressionRecH.Place, ">=", addictiveExpression.Place, lblTrue);
                    relationalExpressionRec1H.Cod += EscreverCodigo(string.Empty, relationalExpressionRec1H.Place, "0", string.Empty);
                    relationalExpressionRec1H.Cod += EscreverCodigo("goto {0}", lblFim);
                    relationalExpressionRec1H.Cod += EscreverRotulo(lblTrue);
                    relationalExpressionRec1H.Cod += EscreverCodigo(string.Empty, relationalExpressionRec1H.Place, "1", string.Empty);
                    relationalExpressionRec1H.Cod += EscreverRotulo(lblFim);

                    relationalExpressionRec1H.EhDeclarador = relationalExpressionRecH.EhDeclarador;
                    if (RelationalExpressionRec(relationalExpressionRec1H, relationalExpressionRec1S))
                    {
                        relationalExpressionRecS.Cod = relationalExpressionRec1S.Cod;
                        relationalExpressionRecS.Place = relationalExpressionRec1S.Place;
                        relationalExpressionRecS.EhDeclarador = relationalExpressionRec1S.EhDeclarador;
                        return true;
                    }
                }
                GerarExcessao(new[] { "expressao" });
                return false;
            }
            else
            {
                relationalExpressionRecS.Cod = relationalExpressionRecH.Cod;
                relationalExpressionRecS.Place = relationalExpressionRecH.Place;
                relationalExpressionRecS.EhDeclarador = relationalExpressionRecH.EhDeclarador;
                return true;
            }
        }

        public static bool AddictiveExpression(Campo addictiveExpression)
        {
            var multiplicativeExpression = new Campo();
            var addictiveExpressionRecH = new Campo();
            var addictiveExpressionRecS = new Campo();
            multiplicativeExpression.EhDeclarador = addictiveExpression.EhDeclarador;
            if (MultiplicativeExpression(multiplicativeExpression))
            {
                addictiveExpressionRecH.Cod = multiplicativeExpression.Cod;
                addictiveExpressionRecH.Place = multiplicativeExpression.Place;
                addictiveExpressionRecH.EhDeclarador = multiplicativeExpression.EhDeclarador;
                if (AddictiveExpressionRec(addictiveExpressionRecH, addictiveExpressionRecS))
                {
                    addictiveExpression.Cod = addictiveExpressionRecS.Cod;
                    addictiveExpression.Place = addictiveExpressionRecS.Place;
                    addictiveExpression.EhDeclarador = addictiveExpressionRecS.EhDeclarador;
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
                multiplicativeExpression.EhDeclarador = addictiveExpressionRecH.EhDeclarador;
                if (MultiplicativeExpression(multiplicativeExpression))
                {
                    addictiveExpressionRec1H.Place = GerarTemp();
                    addictiveExpressionRec1H.Cod += EscreverCodigo(addictiveExpressionRecH.Cod);
                    addictiveExpressionRec1H.Cod += EscreverCodigo(multiplicativeExpression.Cod);
                    addictiveExpressionRec1H.Cod += EscreverCodigo("+", addictiveExpressionRec1H.Place, addictiveExpressionRecH.Place, multiplicativeExpression.Place);

                    addictiveExpressionRec1H.EhDeclarador = addictiveExpressionRecH.EhDeclarador;
                    if (AddictiveExpressionRec(addictiveExpressionRec1H, addictiveExpressionRec1S))
                    {
                        addictiveExpressionRecS.Cod = addictiveExpressionRec1S.Cod;
                        addictiveExpressionRecS.Place = addictiveExpressionRec1S.Place;
                        addictiveExpressionRecS.EhDeclarador = addictiveExpressionRec1S.EhDeclarador;
                        return true;
                    }
                }
                GerarExcessao(new[] { "expressao" });
                return false;
            }
            else if (VerificarToken(TkSubtraction))
            {
                LerToken();
                multiplicativeExpression.EhDeclarador = addictiveExpressionRecH.EhDeclarador;
                if (MultiplicativeExpression(multiplicativeExpression))
                {
                    addictiveExpressionRec1H.Place = GerarTemp();
                    addictiveExpressionRec1H.Cod += EscreverCodigo(addictiveExpressionRecH.Cod);
                    addictiveExpressionRec1H.Cod += EscreverCodigo(multiplicativeExpression.Cod);
                    addictiveExpressionRec1H.Cod += EscreverCodigo("-", addictiveExpressionRec1H.Place, addictiveExpressionRecH.Place, multiplicativeExpression.Place);

                    addictiveExpressionRec1H.EhDeclarador = addictiveExpressionRecH.EhDeclarador;
                    if (AddictiveExpressionRec(addictiveExpressionRec1H, addictiveExpressionRec1S))
                    {
                        addictiveExpressionRecS.Cod = addictiveExpressionRec1S.Cod;
                        addictiveExpressionRecS.Place = addictiveExpressionRec1S.Place;
                        addictiveExpressionRecS.EhDeclarador = addictiveExpressionRec1S.EhDeclarador;
                        return true;
                    }
                }
                GerarExcessao(new[] { "expressao" });
                return false;
            }
            else
            {
                addictiveExpressionRecS.Cod = addictiveExpressionRecH.Cod;
                addictiveExpressionRecS.Place = addictiveExpressionRecH.Place;
                addictiveExpressionRecS.EhDeclarador = addictiveExpressionRecH.EhDeclarador;
                return true;
            }
        }

        public static bool MultiplicativeExpression(Campo multiplicativeExpression)
        {
            var primaryExpression = new Campo();
            var multiplicativeExpressionRecH = new Campo();
            var multiplicativeExpressionRecS = new Campo();

            primaryExpression.EhDeclarador = multiplicativeExpression.EhDeclarador;
            if (PostFixExpression(primaryExpression))
            {
                multiplicativeExpressionRecH.Cod = primaryExpression.Cod;
                multiplicativeExpressionRecH.Place = primaryExpression.Place;
                multiplicativeExpressionRecH.EhDeclarador = primaryExpression.EhDeclarador;
                if (MultiplicativeExpressionRec(multiplicativeExpressionRecH, multiplicativeExpressionRecS))
                {
                    multiplicativeExpression.Cod = multiplicativeExpressionRecS.Cod;
                    multiplicativeExpression.Place = multiplicativeExpressionRecS.Place;
                    multiplicativeExpression.EhDeclarador = multiplicativeExpressionRecS.EhDeclarador;
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

                primaryExpression.EhDeclarador = multiplicativeExpressionRecH.EhDeclarador;
                if (PostFixExpression(primaryExpression))
                {
                    multiplicativeExpressionRec1H.Place = GerarTemp();
                    multiplicativeExpressionRec1H.Cod += EscreverCodigo(multiplicativeExpressionRecH.Cod);
                    multiplicativeExpressionRec1H.Cod += EscreverCodigo(primaryExpression.Cod);
                    multiplicativeExpressionRec1H.Cod += EscreverCodigo("*", multiplicativeExpressionRec1H.Place, multiplicativeExpressionRecH.Place, primaryExpression.Place);
                    multiplicativeExpressionRec1H.EhDeclarador = multiplicativeExpressionRecH.EhDeclarador;
                    if (MultiplicativeExpressionRec(multiplicativeExpressionRec1H, multiplicativeExpressionRec1S))
                    {
                        multiplicativeExpressionRecS.Cod = multiplicativeExpressionRec1S.Cod;
                        multiplicativeExpressionRecS.Place = multiplicativeExpressionRec1S.Place;
                        multiplicativeExpressionRecS.EhDeclarador = multiplicativeExpressionRec1S.EhDeclarador;
                        return true;
                    }
                }
                GerarExcessao(new[] { "expressao" });
                return false;
            }
            else if (VerificarToken(TkDivision))
            {
                LerToken();
                primaryExpression.EhDeclarador = multiplicativeExpressionRecH.EhDeclarador;
                if (PostFixExpression(primaryExpression))
                {
                    multiplicativeExpressionRec1H.Place = GerarTemp();
                    multiplicativeExpressionRec1H.Cod += EscreverCodigo(multiplicativeExpressionRecH.Cod);
                    multiplicativeExpressionRec1H.Cod += EscreverCodigo(primaryExpression.Cod);
                    multiplicativeExpressionRec1H.Cod += EscreverCodigo("/", multiplicativeExpressionRec1H.Place, multiplicativeExpressionRecH.Place, primaryExpression.Place);
                    multiplicativeExpressionRec1H.EhDeclarador = multiplicativeExpressionRecH.EhDeclarador;
                    if (MultiplicativeExpressionRec(multiplicativeExpressionRec1H, multiplicativeExpressionRec1S))
                    {
                        multiplicativeExpressionRecS.Cod = multiplicativeExpressionRec1S.Cod;
                        multiplicativeExpressionRecS.Place = multiplicativeExpressionRec1S.Place;
                        multiplicativeExpressionRecS.EhDeclarador = multiplicativeExpressionRec1S.EhDeclarador;
                        return true;
                    }
                }
                GerarExcessao(new[] { "expressao" });
                return false;
            }
            else if (VerificarToken(TkRemainder))
            {
                LerToken();
                primaryExpression.EhDeclarador = multiplicativeExpressionRecH.EhDeclarador;
                if (PostFixExpression(primaryExpression))
                {
                    multiplicativeExpressionRec1H.Place = GerarTemp();
                    multiplicativeExpressionRec1H.Cod += EscreverCodigo(multiplicativeExpressionRecH.Cod);
                    multiplicativeExpressionRec1H.Cod += EscreverCodigo(primaryExpression.Cod);
                    multiplicativeExpressionRec1H.Cod += EscreverCodigo("%", multiplicativeExpressionRec1H.Place, multiplicativeExpressionRecH.Place, primaryExpression.Place);
                    multiplicativeExpressionRec1H.EhDeclarador = multiplicativeExpressionRecH.EhDeclarador;
                    if (MultiplicativeExpressionRec(multiplicativeExpressionRec1H, multiplicativeExpressionRec1S))
                    {
                        multiplicativeExpressionRecS.Cod = multiplicativeExpressionRec1S.Cod;
                        multiplicativeExpressionRecS.Place = multiplicativeExpressionRec1S.Place;
                        multiplicativeExpressionRecS.EhDeclarador = multiplicativeExpressionRec1S.EhDeclarador;
                        return true;
                    }
                }
                GerarExcessao(new[] { "expressao" });
                return false;
            }
            else
            {
                multiplicativeExpressionRecS.Cod = multiplicativeExpressionRecH.Cod;
                multiplicativeExpressionRecS.Place = multiplicativeExpressionRecH.Place;
                multiplicativeExpressionRecS.EhDeclarador = multiplicativeExpressionRecH.EhDeclarador;
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
        public static bool PostFixExpression(Campo postFixExpression)
        {
            var primaryExpression = new Campo();
            primaryExpression.EhDeclarador = postFixExpression.EhDeclarador;
            if (PrimaryExpression(primaryExpression))
            {
                postFixExpression.Place = primaryExpression.Place;
                postFixExpression.Cod += primaryExpression.Cod;
                if (IdTokenAtual == TkDoublePlus)
                {
                    LerToken();
                    postFixExpression.Cod += EscreverCodigo("+", primaryExpression.Place, primaryExpression.Place, "1");
                    return true;
                }
                else if (IdTokenAtual == TkDoubleMinus)
                {
                    LerToken();
                    postFixExpression.Cod += EscreverCodigo("-", primaryExpression.Place, primaryExpression.Place, "1");
                    return true;
                }
                return true;
            }
            return false;
        }

        public static bool PrimaryExpression(Campo primaryExpression)
        {
            var expression = new Campo();
            /*var characterConstant = new Campo();
            if (CharacterConstant(characterConstant))
            {
                primaryExpression.Place = characterConstant.Place;
                primaryExpression.Cod = characterConstant.Cod;
                return true;
            }
            else */
            if (VerificarToken(TkConst))
            {
                primaryExpression.Place = GerarTemp();
                primaryExpression.Cod = EscreverCodigo(string.Empty, primaryExpression.Place, TokenAtual, string.Empty);
                LerToken();
                return true;
            }
            else if (VerificarToken(TkId))
            {
                primaryExpression.Place = TokenAtual;
                LerToken();

                if (VerificarToken(TkAbreParentese))
                {
                    LerToken();
                    var argumentList = new Campo();

                    var proc = BuscarVariavelTab(primaryExpression.Place, EscopoGlobal);
                    argumentList.Argumentos = proc.Params;
                    argumentList.Proc = proc.Id;
                    if (ArgumentList(argumentList))
                    {
                        if (VerificarToken(TkFechaParentese))
                        {
                            LerToken();
                            primaryExpression.Cod += EscreverCodigo(argumentList.Cod);
                            primaryExpression.Cod += EscreverCodigo("call {0}", primaryExpression.Place);
                            if (proc.Return != null)
                            {
                                primaryExpression.Place = GerarTemp();
                                primaryExpression.Cod += EscreverCodigo("pop {0}", primaryExpression.Place);
                            }


                            return true;
                        }
                    }
                    return false;
                }

                //if (primaryExpression.EhDeclarador)
                //{
                //    if (!AdicionarVariavelTab(primaryExpression.Place, primaryExpression.Tipo)) return false;
                //}
                //else
                //{
                    if (!VerificarVariavelTab(primaryExpression.Place)) return false;
                //}
                    

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
                    return false;
                }
            }
            return false;
        }

        public static bool ArgumentList(Campo argumentList)
        {
            var assignmentExpression = new Campo();
            var argumentListRecS = new Campo();
            var argumentListRecH = new Campo();
            if (AssignmentExpression(assignmentExpression))
            {
                if (argumentList.Argumentos.Count == 0)
                {
                    GerarExcessaoParams(argumentList.Proc);
                    return false;
                }

                argumentList.Argumentos.RemoveAt(0); // consome com o primeiro argumento 

                argumentListRecH.Proc = argumentList.Proc;
                argumentListRecH.Argumentos = argumentList.Argumentos;
                argumentListRecH.Cod = assignmentExpression.Cod;
                argumentListRecH.Place = assignmentExpression.Place;
                if (ArgumentListRec(argumentListRecH, argumentListRecS))
                {

                    argumentList.Place = argumentListRecS.Place;
                    argumentList.Cod = argumentListRecS.Cod;
                    argumentList.Cod += EscreverCodigo("push {0}", argumentList.Place);

                    return true;
                }
                return false;
            }
            return true;
        }

        public static bool ArgumentListRec(Campo argumentListRecH, Campo argumentListRecS)
        {
            var assignmentExpression = new Campo();
            var argumentListRec1H = new Campo();
            var argumentListRec1S = new Campo();
            if (VerificarToken(TkVirgula))
            {
                LerToken();
                if (AssignmentExpression(assignmentExpression))
                {
                    if (argumentListRecH.Argumentos.Count == 0)
                    {
                        GerarExcessaoParams(argumentListRecH.Proc);
                        return false;
                    }

                    argumentListRecH.Argumentos.RemoveAt(0); // consome com o primeiro argumento 

                    argumentListRec1H.Cod += argumentListRecH.Cod;
                    argumentListRec1H.Cod += assignmentExpression.Cod;
                    if (ArgumentListRec(argumentListRec1H, argumentListRec1S))
                    {
                        argumentListRecS.Place = argumentListRec1S.Place;
                        argumentListRecS.Cod = argumentListRec1S.Cod;
                        argumentListRecS.Cod += EscreverCodigo("param {0}", argumentListRecS.Place);

                        return true;
                    }
                    return false;
                }
                return false;
            }
            else
            {
                argumentListRecS.Cod = argumentListRecH.Cod;
                argumentListRecS.Place = argumentListRecH.Place;
                return true;
            }
        }

        public static bool CharacterConstant(Campo characterConstant)
        {
            if (VerificarToken(TkAspasSimples))
            {
                characterConstant.Cod = "\'";
                LerToken();
                if (VerificarToken(TkId))
                {
                    characterConstant.Cod += TokenAtual;
                    LerToken();
                    if (VerificarToken(TkAspasSimples))
                    {
                        characterConstant.Cod = "\'";
                        characterConstant.Place = GerarTemp();
                        characterConstant.Cod = EscreverCodigo(string.Empty, characterConstant.Place, characterConstant.Cod, string.Empty);
                        LerToken();
                        return true;
                    }
                }
                else if (VerificarToken(TkConst))
                {
                    characterConstant.Cod += TokenAtual;
                    LerToken();
                    if (VerificarToken(TkAspasSimples))
                    {
                        characterConstant.Cod = "\'";
                        characterConstant.Place = GerarTemp();
                        characterConstant.Cod = EscreverCodigo(string.Empty, characterConstant.Place, characterConstant.Cod, string.Empty);
                        LerToken();
                        return true;
                    }
                }
            }
            return false;
        }

    }
}

