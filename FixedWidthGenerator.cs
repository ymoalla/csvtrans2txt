using System;
using System.Data;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace wtrans2cnrps
{
    public static class FixedWidthGenerator
    {
        public static void Generate(DataTable csvData, string modelPath, string outputPath)
        {
            // 1. Charger et parser le modèle JSON
            string modelJson = File.ReadAllText(modelPath);
            var modelDoc = JsonDocument.Parse(modelJson);
            var fields = ParseModel(modelDoc);

            // 2. Valider que toutes les colonnes du modèle existent dans le CSV
            ValidateColumns(csvData, fields);

            // 3. Générer le fichier à largeurs fixes
            using (StreamWriter writer = new StreamWriter(outputPath, false, Encoding.UTF8))
            {
                foreach (DataRow row in csvData.Rows)
                {
                    string line = BuildFixedWidthLine(row, fields);
                    writer.WriteLine(line);
                }
            }
        }

        private static List<FieldDefinition> ParseModel(JsonDocument modelDoc)
        {
            var fields = new List<FieldDefinition>();

            // Parser la structure JSON
            var dataArray = modelDoc.RootElement.GetProperty("data");

            foreach (var item in dataArray.EnumerateArray())
            {
                string fieldName = item.GetProperty("NOM").GetString();
                string typeInfo = item.GetProperty("Type  ( longeur du champ)").GetString();

                // Parser le type : "NUMBER(10)" ou "VARCHAR(50)"
                var fieldDef = ParseTypeInfo(fieldName, typeInfo);
                fields.Add(fieldDef);
            }

            return fields;
        }

        private static FieldDefinition ParseTypeInfo(string fieldName, string typeInfo)
        {
            // Exemple : "NUMBER(10)" -> Type=NUMBER, Length=10
            var parts = typeInfo.Split('(');
            string dataType = parts[0].Trim();
            int length = int.Parse(parts[1].TrimEnd(')'));

            return new FieldDefinition
            {
                Name = fieldName,
                DataType = dataType,
                Length = length
            };
        }

        private static void ValidateColumns(DataTable csvData, List<FieldDefinition> fields)
        {
            var missingColumns = new List<string>();

            foreach (var field in fields)
            {
                if (!csvData.Columns.Contains(field.Name))
                {
                    missingColumns.Add(field.Name);
                }
            }

            if (missingColumns.Any())
            {
                throw new Exception($"Colonnes manquantes dans le CSV : {string.Join(", ", missingColumns)}");
            }
        }

        private static string BuildFixedWidthLine(DataRow row, List<FieldDefinition> fields)
        {
            var sb = new StringBuilder();

            foreach (var field in fields)
            {
                string value = row[field.Name]?.ToString() ?? "";
                string formattedValue = FormatField(value, field);
                sb.Append(formattedValue);
            }

            return sb.ToString();
        }

        private static string FormatField(string value, FieldDefinition field)
        {
            // Nettoyer la valeur
            value = value.Trim();

            if (field.DataType.Equals("NUMBER", StringComparison.OrdinalIgnoreCase))
            {
                // Pour les nombres : padding à gauche avec des zéros
                if (value.Length > field.Length)
                {
                    throw new Exception($"La valeur '{value}' dépasse la longueur autorisée ({field.Length}) pour le champ {field.Name}");
                }
                return value.PadLeft(field.Length, '0');
            }
            else if (field.DataType.Equals("VARCHAR", StringComparison.OrdinalIgnoreCase) || 
                     field.DataType.Equals("CHAR", StringComparison.OrdinalIgnoreCase))
            {
                // Pour les chaînes : padding à droite avec des espaces
                if (value.Length > field.Length)
                {
                    // Tronquer si trop long
                    return value.Substring(0, field.Length);
                }
                return value.PadRight(field.Length, ' ');
            }
            else if (field.DataType.Equals("DATE", StringComparison.OrdinalIgnoreCase) )
            {
                // Pour les chaînes : padding à droite avec des espaces
                if (value.Length > field.Length)
                {
                    // Tronquer si trop long
                    throw new Exception($"La valeur '{value}' dépasse la longueur autorisée ({field.Length}) pour le champ {field.Name}");
                }
                return value.Replace("/", "");
            }
            else
            {
                // Type par défaut : traiter comme VARCHAR
                if (value.Length > field.Length)
                {
                    return value.Substring(0, field.Length);
                }
                return value.PadRight(field.Length, ' ');
            }
        }

        private class FieldDefinition
        {
            public string Name { get; set; }
            public string DataType { get; set; }
            public int Length { get; set; }
        }
    }
}