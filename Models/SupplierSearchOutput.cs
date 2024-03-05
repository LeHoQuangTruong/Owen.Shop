using _20T1020637.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _20T1020637.Web.Models
{
    /// <summary>
    /// Kết quả tìm kiếm nhà cung cấp dưới dạng phân trang
    /// </summary>
    public class SupplierSearchOutput : PaginationSearchOutput
    {
        /// <summary>
        /// Danh sách nhà cung cấp ; Thuộc tính
        /// </summary>
        public List<Supplier> Data { get; set; }

    }
}