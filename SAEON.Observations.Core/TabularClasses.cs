using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.Core
{
    public class RowData
    {
        public List<object> Values { get; private set; } = new List<object>();

        public RowData() { }

        public RowData(int numColumns) 
        {
            AddColumns(numColumns);
        }

        public void AddColumns(int number = 1)
        {
            for (int i = 1; i < number; i++)
            {
                Values.Add(null);
            }
        }
    }

    public class TabularData
    {
        public List<string> Columns { get; private set; } = new List<string>();
        public List<RowData> Rows { get; private set; } = new List<RowData>();

        public void AddColumn(string name)
        {
            Columns.Add(name);
            foreach (var row in Rows)
            {
                row.AddColumns();
            }
        }

        public RowData AddRow()
        {
            RowData row = new RowData();
            row.AddColumns(Columns.Count);
            return row;
        }

    }
}
