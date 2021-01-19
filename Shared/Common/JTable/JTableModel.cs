using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Shared.Common.JTable
{
    public class Order
    {
        public int Column { get; set; }
        public string Dir { get; set; }
    }

    public class Search
    {
        public string Value { get; set; }
        public string Regex { get; set; }
    }

    public class Column
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public Search Search { get; set; }
    }

    public class JTableModel : SortModel
    {
        public int Draw { get; set; }
        public List<Column> Columns { get; set; }
        public List<Order> Order { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public Search Search { get; set; }

        public string QueryOrderBy
        {
            get
            {
                try
                {
                    var lstSort = new List<string>();
                    foreach (var item in GetSortedColumns())
                    {
                        var sdir = "";
                        sdir = item.Direction.ToString().Equals(SortingDirection.Ascending.ToString()) ? "" : "DESC";
                        lstSort.Add($"{item.PropertyName} {sdir}");
                    }

                    return string.Join(",", lstSort);
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        public int CurrentPage
        {
            get
            {
                var d = Start / (double) Length;
                return (int) Math.Ceiling(d) + 1;
            }
        }

        public IEnumerable<SortedColumn> GetSortedColumns()
        {
            if (!Order.Any()) return new ReadOnlyCollection<SortedColumn>(new List<SortedColumn>());

            var sortedColumns = Order.Select(item =>
                new SortedColumn(
                    string.IsNullOrEmpty(Columns[item.Column].Name)
                        ? Columns[item.Column].Data
                        : Columns[item.Column].Name, item.Dir)).ToList();

            return sortedColumns.AsReadOnly();
        }
    }

    public class SortedColumn
    {
        private const string Ascending = "asc";

        public SortedColumn(string propertyName, string sortingDirection)
        {
            PropertyName = propertyName;
            Direction = sortingDirection.Equals(Ascending) ? SortingDirection.Ascending : SortingDirection.Descending;
        }

        public string PropertyName { get; }
        public SortingDirection Direction { get; }

        public override int GetHashCode()
        {
            var directionHashCode = Direction.GetHashCode();
            return PropertyName != null ? PropertyName.GetHashCode() + directionHashCode : directionHashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (GetType() != obj.GetType()) return false;
            var other = (SortedColumn) obj;
            if (other.Direction != Direction) return false;
            return other.PropertyName == PropertyName;
        }
    }

    public enum SortingDirection
    {
        Ascending,
        Descending
    }
}