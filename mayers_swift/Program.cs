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
            //var result = mayers.GetValueMayersMetrix();
            mayers.GetValueMayersMetrix();
            //Console.WriteLine("Mayers metrix = [{0} , {1}]", result.CyclomaticNumber, result.CyclomaticNumber + result.PredicateComplecity);
            Console.ReadLine();
        }
    }
}
