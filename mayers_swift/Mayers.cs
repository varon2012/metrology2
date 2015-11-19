using System;
using System.Text.RegularExpressions;

namespace mayers_swift
{
    class Mayers
    {
        struct FunctionInfo  // структура, хранящая имя и код функции
        {
            public string FunctionName;
            public string FunctionCode;

            public FunctionInfo(string functionName, string functionCode)
            {
                FunctionName = functionName;
                FunctionCode = functionCode;
            }
        }

        public struct MayersMetrix
        {
            public int CyclomaticNumber;
            public int PredicateComplecity;
        }

        private const int NumberFunctions = 1;
        private string _codeText;
        private FunctionInfo[] _arrayFunction = new FunctionInfo[NumberFunctions];
        
        public Mayers(string codeText)
        {
            this._codeText = codeText;
        }
        
        //public MayersMetrix GetValueMayersMetrix()
        public void GetValueMayersMetrix()
        {
            GetFunctionsNameAndCode();
          //  GetMainFunction();
            for (int i = 0; i < _arrayFunction.Length; i++)
            {
                var result = GetSubFunctionsValue(_arrayFunction[i].FunctionName, _arrayFunction[i].FunctionCode);
                result.CyclomaticNumber++;
                PrintResult(_arrayFunction[i].FunctionName, result);
            }
        }

        private void PrintResult(string functionName ,MayersMetrix result)
        {
            Console.WriteLine("");
            Console.WriteLine("function {0} = [{1} , {2}]", functionName, result.CyclomaticNumber, result.CyclomaticNumber + result.PredicateComplecity);
        }

        private void GetFunctionsNameAndCode()
        {
            string patternFunctionCode = @"(func\s(\w+)\([^\)]*\)[^{]*{)";
            Regex regexFunctionCode = new Regex(patternFunctionCode);
            Match matchFunctionCode = regexFunctionCode.Match(_codeText);

            int numberMatches = 0;
            while (matchFunctionCode.Success)
            {
                Array.Resize(ref _arrayFunction, numberMatches + 1);
                FunctionInfo elem = new FunctionInfo(matchFunctionCode.Groups[2].Value, GetFunctionCode(matchFunctionCode.Groups[1].Value, matchFunctionCode.Index));
                _arrayFunction[numberMatches] = elem;
                matchFunctionCode = matchFunctionCode.NextMatch();
                numberMatches++;
            }
        }

        private string GetFunctionCode(string functionName, int indexStart)
        {
            int startposition = functionName.Length + indexStart;
            int numberOpenBrackets = 1, numberCloseBrackets = 0;
            int i = startposition;
            while (numberOpenBrackets != numberCloseBrackets)
            {
                if (_codeText[i] == '{')
                    numberOpenBrackets++;
                if (_codeText[i] == '}')
                    numberCloseBrackets++;
                i++;
            }
            return _codeText.Substring(startposition, i - startposition);            
        }
/*
        private void GetMainFunction()
        {
            int mainStartPosition = codeText.LastIndexOf("}", StringComparison.Ordinal);
            string mainFunctionName = "main";
            char temp = '0';
            while (GetFunctionCode(mainFunctionName) != null)
            {
                mainFunctionName = mainFunctionName + temp;
                temp++;
            }

            string mainFunctionCode = codeText.Substring(mainStartPosition);
            FunctionInfo elem = new FunctionInfo(mainFunctionName, mainFunctionCode);
            Array.Resize(ref _arrayFunction, _arrayFunction.Length + 1);
            _arrayFunction[_arrayFunction.Length - 1] = elem;
        }
*/
        private MayersMetrix GetSubFunctionsValue(string mainFunctionName, string mainFunctionCode)
        {
            MayersMetrix result;
            result.CyclomaticNumber = result.PredicateComplecity = 0;
            for (int i = 0; i < _arrayFunction.Length; i++) 
            {
                string functionName = _arrayFunction[i].FunctionName;

                if (functionName != mainFunctionName)
                {
                    string pattern = @functionName + @"\([^\)]*\)";
                    Regex regex = new Regex(pattern);
                    Match match = regex.Match(mainFunctionCode);

                    while (match.Success)
                    {
                        string code = GetFunctionCode(functionName);
                        if (code != null)
                        {
                            var temp = GetSubFunctionsValue(functionName, code);
                            result.CyclomaticNumber += temp.CyclomaticNumber;
                            result.PredicateComplecity += temp.PredicateComplecity;
                        }
                        match = match.NextMatch();
                    }
                }
            }
            result.CyclomaticNumber += GetFunctionCodeValue(mainFunctionCode);
            result.PredicateComplecity += GetFunctionComplexityNumber(mainFunctionCode);
            return result;
        }

        private int GetFunctionCodeValue(string code)// находим цикломатическое число данной функции
        {
            int result = 0;
            string pattern = @"if|for|while|\?[^:]+:";
            Regex regex  = new Regex(pattern);
            Match match = regex.Match(code);
            while (match.Success)
            {
                if (match.Success)
                {
                    result++;
                }
                match = match.NextMatch();
            }
            result +=GetSwitchValue(code); // значение веток switch-ей
            return result;
        }

        private int GetSwitchValue(string code)
        {

            int result = 0;
            string pattern = @"switch\w*";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(code);
            if (match.Success)
            {
                result += GetNumberOperators(@"case", code);
                result += GetNumberOperators(@"default", code);
                return --result;
            }
            return 0;
        }

        private int GetNumberOperators(string pattern, string code)
        {
            int result = 0;
            Regex regex = new Regex(pattern);
            Match match = regex.Match(code);
            while (match.Success)
            {
                result++;
                match = match.NextMatch();
            }
            return result;
        }

        private string GetFunctionCode(string functionName)
        {
            for (int i = 0; i < _arrayFunction.Length; i++)
            {
                if (_arrayFunction[i].FunctionName == functionName)
                {
                    return _arrayFunction[i].FunctionCode;
                }
            }
            return null;
        }

        private int GetFunctionComplexityNumber(string functionCode)  // сложность предикатов данной функции
        {
            int result = 0;
            result += GetOperatorComplexityNumber(functionCode, @" if (.*?){");
            result += GetOperatorComplexityNumber(functionCode, @" while (.*?){");
            result += GetOperatorComplexityNumber(functionCode, @" for.*?;(.*?);");
            return result;
        }

        private int GetOperatorComplexityNumber(string functionCode, string pattern) // нахождение сложности для разных операторов
        {
            int result = 0;
            Regex regex = new Regex(pattern);
            Match match = regex.Match(functionCode);
            while (match.Success)
            {
                result += CountComplexityNumber(match.Value);
                match = match.NextMatch();
            }
            return result;
        }

        private int CountComplexityNumber(string condition) // собственно нахождение сложности выражения 
        {
            int result = 0;
            string pattern = @"==|!=|>|<|!.|&&|\|\|";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(condition);
            while (match.Success)
            {
                result++;
                match = match.NextMatch();
            }
            if (result == 0)
                return 0;
            return --result;
        }
    }
}