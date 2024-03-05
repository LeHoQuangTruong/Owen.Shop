using _20T1020637.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _20T1020637.Web.Models
{
    /// <summary>
    /// Tìm kiếm dưới dạng phân trang
    /// </summary>
    public class ShipperSearchOutput : PaginationSearchOutput
    {
        /// <summary>
        /// Danh sách nhân viên giao hàng
        /// </summary>
        public List<Shipper> Data { get; set; }
    }
}