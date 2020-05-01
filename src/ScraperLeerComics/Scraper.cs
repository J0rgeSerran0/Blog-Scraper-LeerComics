using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace ScraperLeerComics
{
    public class Scraper
    {
        private readonly Uri _uri;
        private readonly string _temporalPath = String.Empty;

        public Scraper(Uri uri, string temporalPath)
        {
            _uri = uri;
            _temporalPath = temporalPath;
        }

        public void ScrapToPdf(string pdfFilename)
        {
            // Get the Html Content of the comic
            var htmlContent = GetHtmlContent(_uri);

            // Load the Html Content to get the images
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            // Get all the images for the web page
            var images = GetElementsFromWebsite(doc, "//img[@src]", "src");
            // The images of a comic has an specific url identified by s1600
            images = images.Where(e => e.Contains("/s1600/") && (e.Contains(".jpg") ||
                                                                 e.Contains(".JPG") ||
                                                                 e.Contains(".jpeg"))).ToList();
            // Force to get dictinct image files and avoid possible repeated images
            images = images.Distinct().ToList();

            var imageFiles = DownloadAllImages(images);

            // Generate the pdf file
            PdfGenerator.GenerateFromImages(imageFiles, $"{Path.Combine(_temporalPath, pdfFilename)}");
        }

        private string GetHtmlContent(Uri uri)
        {
            var webRequest = WebRequest.Create(uri);

            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
                return reader.ReadToEnd();
        }

        private static List<String> GetElementsFromWebsite(HtmlDocument doc, string xPath, string attribute)
        {
            try
            {
                return doc
                    .DocumentNode
                    .SelectNodes(xPath)
                    .Select(node => node.Attributes[attribute].Value)
                    .ToList();
            }
            catch (Exception ex)
            {
                return new List<String>();
            }
        }

        private List<string> DownloadAllImages(List<string> images)
        {
            var temporalFileName = Guid.NewGuid().ToString("N");

            var position = 1;
            var imageFiles = new List<string>();

            // Download each image
            foreach (var image in images)
            {
                var urlImage = image;

                if (!image.StartsWith("http"))
                    urlImage = "https:" + urlImage;

                var imageFile = Path.Combine(_temporalPath, $"{temporalFileName}_{position.ToString("00#")}.jpg");

                DownloadRemoteImageFile(urlImage, imageFile);

                imageFiles.Add(imageFile);

                position += 1;
            }

            return imageFiles;
        }

        private void DownloadRemoteImageFile(string uri, string fileName)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            var response = (HttpWebResponse)request.GetResponse();

            if ((response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Moved ||
                response.StatusCode == HttpStatusCode.Redirect) &&
                response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {

                using (Stream inputStream = response.GetResponseStream())
                using (Stream outputStream = File.OpenWrite(fileName))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    do
                    {
                        bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                        outputStream.Write(buffer, 0, bytesRead);
                    } while (bytesRead != 0);
                }
            }
        }
    }
}