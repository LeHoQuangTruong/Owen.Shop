using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _20T1020637.Web.Models
{
    /// <summary>
    /// Biểu diễn dữ liệu đầu vào để tìm kiếm, phân trang chung
    /// </summary>
    public class PaginationSearchInput
    {
        /// <summary>
        /// Trang cần hiển thị
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Số dòng cần hiển thị trên mỗi trang
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Giá trị cần tìm kiếm
        /// </summary>
        public string SearchValue { get; set; }
    }

    public class ProductSearchInput : PaginationSearchOutput
    {
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;

    }

    public class OrderSearchInput : PaginationSearchOutput
    {
        public int Status { get; set; } = 0;
        public int ShipperID { get; set; } = 0;
        public int EmployeeID { get; set; } = 0;


    }
}