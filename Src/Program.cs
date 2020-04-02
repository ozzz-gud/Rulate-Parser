using CommandLine;
using Src.Models;
using Src.Services;
using System;

namespace Src
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<ParseOptions, UpdateOptions>(args)
                .MapResult(
                (ParseOptions opts) => Parse(opts), 
                (UpdateOptions opts) => UpdateData(opts),
                errs => 1);

        }

        [Verb("parse")]
        class ParseOptions
        {
            [Option('i',"input", Required = false, HelpText = "Файл куда сохранялись данные в прошлый раз. Для продолжения его заполнения.")]
            public string LastOutputFile { get; set; }

            [Option('o', "output", Required = true, HelpText = "Файл для сохранения данных в JSON")]
            public string OutputFile { get; set; }

            [Option('t', "temp", Required = false, HelpText = "Temp file")]
            public string TempFile { get; set; }

            [Option("firstPage", Required =true, HelpText = "Страница с которой начнется парсинг")]
            public int FirstPageNumber { get; set; }

            [Option("lastPage", Required = true, HelpText = "Страница на которой закончится парсинг")]
            public int LastPageNumber { get; set; }
        }
        static int Parse(ParseOptions opts)
        {
            Services.Parser parser = new Services.Parser(opts.LastOutputFile, opts.TempFile);
            var parsedTitles = parser.Parse(opts.FirstPageNumber, opts.LastPageNumber).Result;
            Exporter.Save(parsedTitles, opts.OutputFile);
            Console.WriteLine("ALL_PARSED");
            return 0;
        }

        [Verb("updateData")]
        class UpdateOptions
        {
            [Option('i', "input", Required = true, HelpText = "Файл для обновления модели")]
            public string InputFile { get; set; }
        }
        static int UpdateData(UpdateOptions opts)
        {
            if(!string.IsNullOrEmpty(opts.InputFile))
            {
                var data = Exporter.Load<Title>(opts.InputFile);
                Exporter.Save(data, opts.InputFile);
                return 0;
            }
            return 1;
        }
    }
}
