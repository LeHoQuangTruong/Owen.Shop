using _20T1020637.BusinessLayers;
using _20T1020637.DomainModels;
using _20T1020637.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace _20T1020637.Web.Controllers
{
    [Authorize]
    [RoutePrefix("product")]
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 10;
        private const string PRODUCT_SEARCH = "SearchProductCondition";
        private const string ERROR_MESSAGE = "ErrorMessage";

        /// <summary>
        /// Tìm kiếm, hiển thị mặt hàng dưới dạng phân trang
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ProductSearchInput condition = Session[PRODUCT_SEARCH] as ProductSearchInput;
            if (condition == null)
            {
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0
                };
            }
            return View(condition);
        }

        /// <summary>
        /// Tìm kiếm, hiển thị mặt hàng dưới dạng phân trang
        /// </summary>
        /// <returns></returns>
        public ActionResult Search(ProductSearchInput condition, int page = 1)
        { 
            int rowCount = 0;
            ViewBag.Page = page;
            var data = ProductDataService.ListProducts(
                        condition.Page,
                        condition.PageSize,
                        condition.SearchValue,
                        condition.CategoryID,
                        condition.SupplierID,
                        out rowCount
                        );
            var result = new ProductSearchOutput()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue,
                CategoryID = condition.CategoryID,
                SupplierID = condition.SupplierID,
                RowCount = rowCount,
                Data = data
            };
            Session[PRODUCT_SEARCH] = condition;
            return View(result);
        }
        /// <summary>
        /// Tạo mặt hàng mới
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var data = new Product()
            {
                ProductID = 0
            };
            ViewBag.Title = "Bổ sung mặt hàng mới";
            return View(data);
        }
        /// <summary>
        /// Uoload Image
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string UploadFile(HttpPostedFileBase file)
        {
            try
            {
                if (file != null)
                {
                    string path = Server.MapPath("~/Uploads/Product");
                    // lay file Name upload photo
                    string fileName = $"{DateTime.Now.Ticks}_{file.FileName}";
                    //duong dan upload
                    string filePath = System.IO.Path.Combine(path, fileName);
                    file.SaveAs(filePath);
                    return $"/Uploads/Product/{fileName}";
                }
                return "";
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// Cập nhật thông tin mặt hàng, 
        /// Hiển thị danh sách ảnh và thuộc tính của mặt hàng, điều hướng đến các chức năng
        /// quản lý ảnh và thuộc tính của mặt hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>        
        public ActionResult Edit(int id = 0)
        {
            if (id <= 0)
                return RedirectToAction("Index");
            var data = ProductDataService.GetProduct(id);
            if (data.CategoryID == 0 || data.ProductID == 0)
            {
                TempData[ERROR_MESSAGE] = "Vui lòng chọn loại hàng hoặc nhà cung cấp!";
            }
            var model = new ProductEditModel()
            {
                ProductID = data.ProductID,
                ProductName = data.ProductName,
                SupplierID = data.SupplierID,
                CategoryID = data.CategoryID,
                Photo = data.Photo,
                Unit = data.Unit,
                Price = data.Price,
                Attributes = ProductDataService.ListAttributes(id),
                Photos = ProductDataService.ListPhotos(id)
            };


            return View(model);
        }

        /// <summary>
        /// Lưu sản phẩm
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Product data, string unPrice, HttpPostedFileBase Photo)
        {
            try
            {
                Decimal? p = Converter.StringToDecimal(unPrice);
                if (p == null)
                    ModelState.AddModelError("Price", $"Giá trị {unPrice} không hợp lệ hoặc không được để trống!");
                else
                    data.Price = p.Value;

                if (string.IsNullOrWhiteSpace(data.ProductName))
                    ModelState.AddModelError("ProductName", "Tên mặt hàng không được để trống!");
                if (string.IsNullOrWhiteSpace(data.Unit))
                    ModelState.AddModelError("Unit", "Vui lòng nhập đơn vị tính!");
                if (data.CategoryID == 0)
                    ModelState.AddModelError("CategoryID", "Vui lòng chọn loại hàng!");
                if (data.SupplierID == 0)
                    ModelState.AddModelError("SupplierID", "Vui lòng chọn nhà cung cấp!");
                if (string.IsNullOrWhiteSpace(data.Photo))
                    ModelState.AddModelError("Photo", "Vui lòng thêm ảnh");
                var model = new ProductEditModel()
                {
                    ProductID = data.ProductID,
                    ProductName = data.ProductName,
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Photo = data.Photo,
                    Unit = data.Unit,
                    Price = data.Price,
                    Attributes = ProductDataService.ListAttributes(data.ProductID),
                    Photos = ProductDataService.ListPhotos(data.ProductID)
                };

                //Nếu lỗi ở các rules ở trên thì return view
                if (!ModelState.IsValid)
                {
                    if (data.ProductID == 0)
                        return View("Create", data);
                    else
                        return View("Edit", model);
                }

                string imageFile = UploadFile(Photo);
                if (imageFile != "")
                    data.Photo = imageFile;

                if (data.ProductID == 0)
                {
                    ProductDataService.AddProduct(data);
                }
                else
                {
                    ProductDataService.UpdateProduct(data);
                }
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                //Ghi lại log lỗi
                return Content("Có lỗi xảy ra.Vui lòng thử lại sau!" + ex.Message);
            }

        }


        /// <summary>
        /// Lưu các thuộc tính của mặt  hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveAttribute(ProductAttribute data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.AttributeName))
                    ModelState.AddModelError("AttributeName", "Tên thuộc tính không được để trống!");
                if (string.IsNullOrWhiteSpace(data.AttributeValue))
                    ModelState.AddModelError("AttributeValue", "Giá trị thuộc tính không được để trống!");
                if (string.IsNullOrWhiteSpace(data.DisplayOrder.ToString()))
                {
                    ModelState.AddModelError("DisplayOrder", "Vui lòng thêm thứ tự hiển thị ảnh!");
                }
                else if (data.DisplayOrder < 1)
                {
                    ModelState.AddModelError("DisplayOrder", "Thứ tự hiển thị phải là một số tự nhiên không âm");
                }
                else
                {
                    bool isUsedDisplayOrder = false;
                    // kiểm tra nếu DisplayOrder giống nhau
                    List<ProductAttribute> productPhotos = ProductDataService.ListAttributes(data.ProductID);
                    foreach (ProductAttribute item in productPhotos)
                    {
                        if (item.DisplayOrder == data.DisplayOrder && item.AttributeID != data.AttributeID)
                        {
                            isUsedDisplayOrder = true;
                            break;
                        }
                    }
                    if (isUsedDisplayOrder)
                    {
                        ModelState.AddModelError("DisplayOrder", $"Thứ tự hiển thị {data.DisplayOrder} đã được dùng");
                    }
                }

                if (!ModelState.IsValid)
                {
                    return View("Attribute", data);
                }


                if (data.AttributeID == 0)
                {
                    ProductDataService.AddAttribute(data);
                }
                else
                {
                    ProductDataService.UpdateAttribute(data);
                }

                return RedirectToAction($"Edit/{data.ProductID}");
            }
            catch (Exception ex)
            {
                //Ghi lại log lỗi
                return Content("Có lỗi xảy ra.Vui lòng thử lại sau!" + ex.Message);
            }

        }

        /// <summary>
        /// Lưu ảnh của mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <param name="uploadPhoto"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult SavePhoto(ProductPhoto data, HttpPostedFileBase uploadPhoto)
        {
            try
            {
                string imageFile = UploadFile(uploadPhoto);
                if (imageFile != "")
                    data.Photo = imageFile;
                if (string.IsNullOrWhiteSpace(data.Photo))
                    ModelState.AddModelError("Photo", "Vui lòng thêm ảnh!");
                if (string.IsNullOrWhiteSpace(data.Description))
                    ModelState.AddModelError("Description", "Mô tả ảnh không được để trống");
                if (string.IsNullOrWhiteSpace(data.IsHidden.ToString()))
                    ModelState.AddModelError("IsHidden", "Thêm kiểu hiển thị!");
                if (string.IsNullOrWhiteSpace(data.DisplayOrder.ToString()))
                {
                    ModelState.AddModelError("DisplayOrder", "Vui lòng thêm thứ tự hiển thị ảnh!");
                }
                else if (data.DisplayOrder < 1)
                {
                    ModelState.AddModelError("DisplayOrder", "Thứ tự hiển thị phải là một số tự nhiên không âm");
                }
                else
                {
                    bool isUsedDisplayOrder = false;
                    // kiểm tra nếu DisplayOrder giống nhau
                    List<ProductPhoto> productPhotos = ProductDataService.ListPhotos(data.ProductID);
                    foreach (ProductPhoto item in productPhotos)
                    {
                        if (item.DisplayOrder == data.DisplayOrder && item.PhotoID != data.PhotoID)
                        {
                            isUsedDisplayOrder = true;
                            break;
                        }
                    }
                    if (isUsedDisplayOrder)
                    {
                        ModelState.AddModelError("DisplayOrder", $"Thứ tự hiển thị {data.DisplayOrder} đã được dùng");
                    }
                }
                if (!ModelState.IsValid)
                {
                    return View("Photo", data);
                }


                if (data.PhotoID == 0)
                {
                    ProductDataService.AddPhoto(data);
                }
                else
                {
                    ProductDataService.UpdatePhoto(data);
                }
                return RedirectToAction($"Edit/{data.ProductID}");
            }
            catch (Exception ex)
            {
                //Ghi lại log lỗi
                return Content("Có lỗi xảy ra.Vui lòng thử lại sau!" + ex.Message);
            }

        }

        /// <summary>
        /// Xóa mặt hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>        
        public ActionResult Delete(int id = 0)
        {
            if (id <= 0)
                return RedirectToAction("Index");
            if (Request.HttpMethod == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var data = ProductDataService.GetProduct(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }

        /// <summary>
        /// Các chức năng quản lý ảnh của mặt hàng
        /// </summary>
        /// <param name="method"></param>
        /// <param name="productID"></param>
        /// <param name="photoID"></param>
        /// <returns></returns>
        [Route("photo/{method?}/{productID?}/{photoID?}")]
        public ActionResult Photo(string method = "add", int productID = 0, long photoID = 0)
        {
            switch (method)
            {
                case "add":
                    ProductPhoto data = new ProductPhoto() { ProductID = productID, DisplayOrder = 0 };
                    ViewBag.Title = "Bổ sung ảnh";
                    return View(data);
                case "edit":
                    ProductPhoto dataPhoto = ProductDataService.GetPhoto(photoID);
                    ViewBag.Title = "Thay đổi ảnh";
                    return View(dataPhoto);

                case "delete":
                    ProductDataService.DeletePhoto(photoID);
                    return RedirectToAction($"Edit/{productID}");
                default:
                    return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Các chức năng quản lý thuộc tính của mặt hàng
        /// </summary>
        /// <param name="method"></param>
        /// <param name="productID"></param>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        [Route("attribute/{method?}/{productID}/{attributeID?}")]
        public ActionResult Attribute(string method = "add", int productID = 0, long attributeID = 0)
        {
            switch (method)
            {
                case "add":
                    ProductAttribute data = new ProductAttribute() { ProductID = productID, DisplayOrder = 0 };
                    ViewBag.Title = "Bổ sung thuộc tính";
                    return View(data);
                case "edit":
                    ProductAttribute dataAttribute = ProductDataService.GetAttribute(attributeID);
                    ViewBag.Title = "Thay đổi thuộc tính";
                    return View(dataAttribute);
                case "delete":
                    ProductDataService.DeleteAttribute(attributeID);
                    return RedirectToAction($"Edit/{productID}");
                default:
                    return RedirectToAction("Index");
            }
        }
    }
}