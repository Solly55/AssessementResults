using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace NinetyOne.FileProcessor.TopScores
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Retrieve the file path from the app.config file
                string filePath = ConfigurationManager.AppSettings["FilePath"];

                // Read the CSV data from the file
                string csvData = File.ReadAllText(filePath);
         
                // Parse the CSV data
                List<(string, string, int)> personScores = ParseCSVData(csvData);

                // Find the maximum score
                int maxScore = 0;
                foreach (var person in personScores)
                {
                    if (person.Item3 > maxScore)
                    {
                        maxScore = person.Item3;
                    }
                }

                // Find the top scorers that matches the maximum score
                var topScorers = new List<(string, string)>();
                foreach (var person in personScores)
                {
                    if (person.Item3 == maxScore)
                    {
                        topScorers.Add((person.Item1, person.Item2));
                    }
                }

                // Sort the results by the scorer name
                topScorers.Sort();

                // Writing the top scorers and their scores to the standard output
                
                foreach (var scorer in topScorers)
                {
                    Console.WriteLine($"{scorer.Item1} {scorer.Item2}");
                }
                Console.WriteLine($"Score: {maxScore}");
                Console.Read();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"The file was not found: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }

        }

        // CSV parsing method
        static List<(string, string, int)> ParseCSVData(string csvData)
        {
            List<(string, string, int)> result = new List<(string, string, int)>();

            string[] lines = csvData.Split('\n');

            for (int i = 1; i < lines.Length; i++) // ignore first row as it contains headers
            {
                string[] values = SplitCSVLine(lines[i]);
                string firstName = values[0];
                string secondName = values[1];
                int score = int.Parse(values[2]);
                result.Add((firstName, secondName, score));
            }

            return result;
        }

        // Custom method to split a CSV line
        static string[] SplitCSVLine(string line)
        {
            List<string> values = new List<string>();
            int start = 0;
            bool inQuote = false;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == ',' && !inQuote)
                {
                    values.Add(line.Substring(start, i - start));
                    start = i + 1;
                }
                else if (line[i] == '"')
                {
                    inQuote = !inQuote;
                }
            }
            values.Add(line.Substring(start));
            return values.ToArray();
        }
    }
}
