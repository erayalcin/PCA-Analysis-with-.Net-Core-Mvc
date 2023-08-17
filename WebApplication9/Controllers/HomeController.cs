using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Accord.Statistics.Analysis;

public class HomeController : Controller
{
    public IActionResult UploadCSV()
    {
        return View();
    }

   

    [HttpPost]
    public IActionResult PerformPCA(IFormFile csvFile)
    {
        using (var streamReader = new StreamReader(csvFile.OpenReadStream()))
        {
            List<double[]> records = new List<double[]>();
            List<string> columnNames = new List<string>(); // Sütun isimlerini tutacak liste
            int columnCount = -1;

            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                var values = line.Split(',');

                if (columnCount == -1)
                {
                    columnCount = values.Length;
                    columnNames.AddRange(values); // İlk satırdaki değerleri sütun isimleri olarak ekliyoruz
                }
                else if (values.Length != columnCount)
                {
                    
                }

                var record = new double[columnCount];

                for (int i = 0; i < columnCount; i++)
                {
                    if (double.TryParse(values[i], NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                    {
                        record[i] = value;
                    }
                    else
                    {
                        
                    }
                }

                records.Add(record);
            }

            double[][] rawData = records.ToArray();

            PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis();
            pca.Learn(rawData);

            double[][] pcaResult = pca.Transform(rawData);

            int numComponents = pca.Components.Count();
            int numInstances = pcaResult.GetLength(0);
            double[][] reducedData = new double[numInstances][];

            for (int i = 0; i < numInstances; i++)
            {
                reducedData[i] = pcaResult[i].Take(numComponents).ToArray();
            }

            ViewBag.ReducedData = reducedData;
            ViewBag.ColumnNames = columnNames; // Sütun isimlerini ViewBag'e ekliyoruz
            ViewBag.records = records;
            return View("PcaResult");
        }
    }
}
