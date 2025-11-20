using System;
using System.Data;
using System.IO;

namespace wtrans2cnrps   // ⚠️ même namespace que MainForm
{
    public static class CsvProcessor
    {
        public static DataTable ReadCsv(string path, char separator = ';')
        {
            var dt = new DataTable();

            using var reader = new StreamReader(path);
            bool headerRead = false;

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var values = line.Split(separator);

                if (!headerRead)
                {
                    // La première ligne contient les noms de colonnes
                    foreach (var col in values)
                        dt.Columns.Add(col.Trim());
                    headerRead = true;
                }
                else
                {
                    dt.Rows.Add(values);
                }
            }

            return dt;
        }
    }
}
