using _20T1020637.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _20T1020637.Web.Models
{
    public class ProductSearchOutput : PaginationSearchOutput
    {
        /// <summary>
        /// Danh sách mặt hàng
        /// </summary>
        public List<Product> Data { get; set; }
        public int CategoryID { get; set; }
        public int SupplierID { get; set; }
    }
}