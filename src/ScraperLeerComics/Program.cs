using System;
using System.IO;

namespace ScraperLeerComics
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Scraper Started... {DateTime.Now}");
            Console.WriteLine();

            Disclaimer();

            var uri = new Uri("https://leer-comics.blogspot.com/2020/05/magos-del-humor-194-mortadelo-y-filemon.html");
            var temporalPath = @"C:\temp\Scraper\LeerComics";
            var pdfFilename = "MyComic.pdf";

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\tConectando, descargando y generando documento...");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\t{uri}");


            try
            {
                var leerComicsScraper = new Scraper(uri, temporalPath);
                leerComicsScraper.ScrapToPdf(pdfFilename);

                Console.WriteLine($"\t{Path.Combine(temporalPath, pdfFilename)}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\t{ex.Message}");
                Console.WriteLine();
            }


            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Process finished! {DateTime.Now}");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Press any key to close");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void Disclaimer()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("**************************************************************************");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" ATENCIÓN - AVISO");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" ESTE ES UN PROYECTO EXPERIMENTAL");
            Console.WriteLine(" UNA VEZ GENERADO EL PDF, DEBERÁS ELIMINAR EL FICHERO GENERADO");
            Console.WriteLine(" EL AUTOR DE ESTE CÓDIGO NO SE HACE RESPONSABLE DEL MAL USO DE ESTE CÓDIGO");
            Console.WriteLine("**************************************************************************");
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}