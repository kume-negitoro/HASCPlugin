using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;

using Plugin;

namespace HASCPlugin
{
    public class DataRowCreator
    {
        private string filepath;
        private IPluginHost host;
        private TextFieldParser parser;

        private List<double> timestampList = new List<double>();
        private List<double> xAxisList = new List<double>();
        private List<double> yAxisList = new List<double>();
        private List<double> zAxisList = new List<double>();

        public DataRowCreator(IPluginHost host, string filepath)
        {
            this.host = host;
            this.filepath = filepath;

            ReadCSV();
            AddRows();
        }

        private void ReadCSV()
        {
            var encoding = Encoding.GetEncoding("UTF-8");
            parser = new TextFieldParser(filepath, encoding);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.HasFieldsEnclosedInQuotes = true;
            parser.TrimWhiteSpace = true;

            using(parser)
            {
                while(!parser.EndOfData)
                {
                    string[] row = parser.ReadFields();
                    if(row.Length != 4)
                    {
                        throw new Exception("Field count mismatch.");
                    }

                    timestampList.Add(Double.Parse(row[0]));
                    xAxisList.Add(Double.Parse(row[1]));
                    yAxisList.Add(Double.Parse(row[2]));
                    zAxisList.Add(Double.Parse(row[3]));
                }
            }
        }

        private void AddRows()
        {
            var filename = Path.GetFileName(filepath);
            host.DataRows.Add(new HASCDataRow(host, filename + "_x", timestampList, xAxisList));
            host.DataRows.Add(new HASCDataRow(host, filename + "_y", timestampList, yAxisList));
            host.DataRows.Add(new HASCDataRow(host, filename + "_z", timestampList, zAxisList));
        }
    }
}
