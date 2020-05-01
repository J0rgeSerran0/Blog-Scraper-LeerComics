using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.IO;

namespace ScraperLeerComics
{
    public class PdfGenerator
    {
        public static void GenerateFromImages(List<string> imageFiles, string pdfFilename, int width = 600)
        {
            // Generate the Pdf File
            using (var document = new PdfDocument())
            {
                var index = 0;

                foreach (var imageFile in imageFiles)
                {
                    var page = new PdfPage(document);

                    using (var xImage = XImage.FromFile(imageFile))
                    {
                        // Calculate new height to keep image ratio
                        var height = (int)(((double)width / (double)xImage.PixelWidth) * xImage.PixelHeight);

                        // Change PDF Page size to match image
                        page.Width = width;
                        page.Height = height;

                        var gfx = XGraphics.FromPdfPage(page);

                        gfx.DrawImage(xImage, 0, 0, width, height);
                    }

                    document.InsertPage(index, page);
                    index += 1;

                    var deleted = false;
                    while (!deleted)
                    {
                        try
                        {
                            File.Delete(imageFile);

                            deleted = true;
                        }
                        catch (Exception)
                        {
                        }
                    }

                    document.Save(pdfFilename);
                }
            }
        }
    }
}