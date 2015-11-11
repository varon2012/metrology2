using System.IO;
using System.Text.RegularExpressions;

namespace mayers_swift
{
    class File
    {
        private StreamReader _file;
        private bool _isMultiLineComment = false;
        public void FileOpen()
        {
            _file = new StreamReader("text.txt");                       
        }

        public string GetSourceText()
        {
            string codeText = "";
            while (!_file.EndOfStream)
            {
                string sourceTextLine = _file.ReadLine();
                sourceTextLine = CheckForComments(sourceTextLine);
                sourceTextLine = DeleteLiteralConstants(sourceTextLine);
                if (sourceTextLine.Length != 0)
                    sourceTextLine += " "; 
                codeText += sourceTextLine;
            }
            _file.Close();
            return codeText;
        }

        private string DeleteLiteralConstants(string textLine)
        {
            string pattern = "\"([^\"]*)\"";
            string replacement = "\"\"";
            textLine = Regex.Replace(textLine, pattern, replacement);
            return textLine;
        }

        private string CheckForComments(string textLine)
        {
            CheckForMultiLineComments(textLine);
            if (_isMultiLineComment)
                textLine = DeleteMultiLineComents(textLine);
            textLine = DeleteOneLineComment(textLine);
            return textLine;
        }

        private void CheckForMultiLineComments(string textLine)
        {
            if (_isMultiLineComment)
                return;

            if (textLine.IndexOf("/*") != -1)
            {
                _isMultiLineComment = true;
            }
            else
            {
                _isMultiLineComment = false;
            }
        }

        private string DeleteMultiLineComents(string textLine)
        {
            string newLine;
            int startPosition = textLine.IndexOf("/*");
            if (startPosition == -1)
                startPosition = 0;

            int finishedPosition = textLine.IndexOf("*/");
            if (finishedPosition == -1)
            {
                finishedPosition = textLine.Length;
            }
            else
            {
                _isMultiLineComment = false;
                finishedPosition += 2;
            }
            newLine = textLine.Remove(startPosition, finishedPosition - startPosition);
            return newLine;
        }

        private string DeleteOneLineComment(string textLine)
        {
            string newLine = textLine;

            for (int i = 0; i < textLine.Length - 1; i++)
            {
                if ((textLine[i] == '/') && (textLine[i + 1] == '/'))
                {
                    newLine = textLine.Remove(i);
                    break;
                }
            }

            return newLine;
        }
    }
}
