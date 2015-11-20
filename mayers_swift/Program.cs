using System;

namespace mayers_swift
{
    class Program
    {
        static void Main(string[] args)
        {
            File file = new File();
            file.FileOpen();
            string codeText = file.GetSourceText();

            var mayers = new Mayers(codeText);
            mayers.GetValueMayersMetrix();
            Console.ReadLine();
        }
    }
}
