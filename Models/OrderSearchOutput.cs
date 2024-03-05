using _20T1020637.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _20T1020637.Web.Models
{
    public class OrderSearchOutput : PaginationSearchOutput
    {
        /// <summary>
        /// Danh sách đơn hàng
        /// </summary>
        public List<Order> Data { get; set; }

        public int EmployeeID { get; set; } = 0;
        public int ShipperID { get; set; } = 0;
        public int Status { get; set; }
    }
}