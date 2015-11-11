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

            Mayers mayers = new Mayers(codeText);
            Mayers.MayersMetrix result = mayers.GetValueMayersMetrix();

            Console.WriteLine("Mayers metrix = [{0} , {1}]", result.CyclomaticNumber, result.CyclomaticNumber + result.PredicateComplecity);
            Console.ReadLine();
        }
    }
}
