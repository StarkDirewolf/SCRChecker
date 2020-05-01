using AODL.Document.Content.Tables;
using AODL.Document.Content.Text;
using AODL.Document.SpreadsheetDocuments;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR_Checker
{
    class SpreadsheetHandler
    {
        private const string FILE_PATH = @"C:\Users\Careway LINK\Desktop\simple.ods";
        private SpreadsheetDocument doc;
        private Table table;
        private int activeRow = 0;

        public SpreadsheetHandler()
        {
            doc = new SpreadsheetDocument();
            doc.New();

            table = new Table(doc, "Patients", "tablefirst");
        }

        // Adds a row to the spreadsheet with each string in the provided list being in a new column
        public void AddRow(List<string> strings)
        {
            Row row = new Row(table);

            for (int i = 0; i < strings.Count; i++)
            {
                //Create a new cell, without any extra styles 
                Cell cell = new Cell(table);
                cell.OfficeValueType = "string";

                //Add a paragraph to this cell
                Paragraph paragraph = ParagraphBuilder.CreateSpreadsheetParagraph(doc);

                //Add some text content
                paragraph.TextContent.Add(new SimpleText(doc, strings[i]));

                cell.Content.Add(paragraph);
                
                row.InsertCellAt(i, cell);
                
            }

            table.RowCollection.Insert(activeRow, row);
            activeRow++;
            
        }

        // Saves the table and spreadsheet file
        public void Save()
        {
            doc.TableCollection.Add(table);
            doc.SaveTo(FILE_PATH);
        }
        
        // Opens the file for the user to see
        public void OpenFile()
        {
            Process fileOpener = new Process();
            fileOpener.StartInfo.FileName = FILE_PATH;
            fileOpener.Start();
        }
    }
}
