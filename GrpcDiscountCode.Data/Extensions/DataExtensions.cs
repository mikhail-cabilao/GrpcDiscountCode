using GrpcDiscountCode.Data.Models;
using System.Data;

namespace GrpcDiscountCode.Data.Extensions
{
    public static class DataExtensions
    {
        public static DataTable ToDataTable(this List<DiscountCode> discountCodes)
        {
            var table = new DataTable();
            table.Columns.Add("Code", typeof(string));
            table.Columns.Add("CreatedDate", typeof(DateTime));

            foreach (var c in discountCodes)
            {
                table.Rows.Add(c.Id, c.Code, c.CreatedDate);
            }

            return table;
        }
    }
}
