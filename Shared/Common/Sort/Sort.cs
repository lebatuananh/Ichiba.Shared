using System.Collections.Generic;

namespace Shared.Common.Sort
{
    public class Sort
    {
        public const string SORT_DIRECTION_ASC = "ASC";
        public const string SORT_DIRECTION_DESC = "DESC";

        public readonly IList<string> SortDirections = new List<string>
            {
                SORT_DIRECTION_ASC,
                SORT_DIRECTION_DESC
            }
            .AsReadOnly();

        private string sortDirection;

        public string SortBy { get; set; }

        public string SortDirection
        {
            get => sortDirection;
            set => sortDirection = ValidateSortDirectionAndDefault(value);
        }

        public string SortExpression => $"{SortBy} {SortDirection}";

        public bool ValidateSortDirection(string sortDirection)
        {
            if (string.IsNullOrWhiteSpace(sortDirection)) return false;

            sortDirection = sortDirection.ToUpper();

            return SortDirections.Contains(sortDirection);
        }

        public string ValidateSortDirectionAndDefault(string sortDirection)
        {
            var isSortDirection = ValidateSortDirection(sortDirection);

            if (isSortDirection) return sortDirection;

            return SORT_DIRECTION_ASC;
        }
    }
}