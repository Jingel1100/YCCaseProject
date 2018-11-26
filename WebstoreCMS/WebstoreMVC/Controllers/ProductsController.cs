using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Claims;
using System.Threading.Tasks;
using DeliverySystemLibrary;
using FinanceLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductsManagementSystemLibrary;
using ShoppingCartLibrary;
using WebstoreMVC.Data;
using WebstoreMVC.Models;
using File = ProductsManagementSystemLibrary.File;

namespace WebstoreMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductManagementSystemDbContext _context;
        private readonly FinanceDbContext _financeDbContext;

        public ProductsController(ProductManagementSystemDbContext context, FinanceDbContext financeDbContext)
        {
            _context = context;
            _financeDbContext = financeDbContext;
        }

        // GET: Products
        public ActionResult Index(int pageNumber = 1)
        {
            var model = new ProductListViewModel();
            model.PageNumber = pageNumber;
            model.ListItems = _context.Products
                .OrderBy(p => p.Name)
                .Skip(model.PageSize * (model.PageNumber - 1))
                .Take(model.PageSize)
                .Select(p => new ProductListItemViewModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Image = p.Image,
                    Description = p.Description,
                    Stock = p.Stock,
                    Price = p.Price //p.Price.HasValue ? p.Price.Value.ToString() : "-"
                }).ToList();
            model.PageCount = (int)Math.Ceiling(_context.Products.Count() / (double)model.PageSize);
            model.AllowEdit = User.Identity.IsAuthenticated;

            return View(model);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var product = await _context.Products.Include("Image")
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Policy = "RequireStoreOwnerRole")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Policy = "RequireStoreOwnerRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Description,Stock,Image")] Product product, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        File fileDetails;
                        using (var reader = new BinaryReader(file.OpenReadStream()))
                        {
                            var fileContent = reader.ReadBytes((int)file.Length);
                            fileDetails = new File
                            {
                                FileName = file.FileName,
                                FileType = FileType.image,
                                Content = fileContent,
                                ContentType = file.ContentType
                            };
                        }
                        product.Image = fileDetails;
                        _context.Add(fileDetails);
                    }
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }


        // GET: Products/Edit/5
        [Authorize(Policy = "RequireStoreOwnerRole")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "RequireStoreOwnerRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Description,Stock")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Policy = "RequireStoreOwnerRole")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [Authorize(Policy = "RequireStoreOwnerRole")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Buy(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) { return NotFound(); }
            SaveItemToShoppingCart(product);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Add(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) { return NotFound(); }
            SaveItemToShoppingCart(product);
            return RedirectToAction(nameof(ShoppingCart));
        }

        //SHOPPINGCART METHODS
        public ShoppingCart GetShoppingCart()
        {
            //try to get shoppingcart from cache otherwise create it. 
            ShoppingCart shoppingCart = null;
            try
            {
                this.HttpContext.Session.TryGetValue("shoppingCart", out byte[] output);
                shoppingCart = (ShoppingCart)ByteArrayToObject(output);
            }
            catch (Exception)
            {
                shoppingCart = new ShoppingCart();
            }

            return shoppingCart;
        }

        public void SaveItemToShoppingCart(Product product)
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Add(product);
            shoppingCart.CalculateTotal();
            this.HttpContext.Session.Set("shoppingCart", ObjectToByteArray(shoppingCart));
        }

        //Method ShoppingCart to Order.
        public async Task<IActionResult> OrderShoppingCartItems()
        {
            var shoppingCart = GetShoppingCart();

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Order order = shoppingCart.ConfirmOrder(userId);

            _financeDbContext.Update(order);
            _financeDbContext.SaveChanges();

            foreach (var product in shoppingCart.Products)
            {
                _context.Update(product);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> RemoveItemShoppingCart(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var shoppingCart = GetShoppingCart();
            shoppingCart.Remove(product);
            shoppingCart.CalculateTotal();
            this.HttpContext.Session.Set("shoppingCart", ObjectToByteArray(shoppingCart));

            return RedirectToAction(nameof(ShoppingCart));
        }

        //Genenrate ShoppingCartView
        public ActionResult ShoppingCart()
        {
            var shoppingCart = GetShoppingCart();
            var model = new ShoppingCartListViewModel();
            model.Products = shoppingCart.Products.OrderBy(i => i.Name)
                .Select(i => new ProductListItemViewModel()
                {
                    Id = i.Id,
                    Name = i.Name,
                    Price = i.Price,
                    Stock = i.Stock,
                    Description = i.Description,
                    Quantity = i.Quantity.ToString(),
                    Subtotal = i.Subtotal
                }).ToList();
            model.Total = shoppingCart.Total;

            return View("ShoppingCart", model);
        }
        // Convert an object to a byte array
        private byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

        // Convert a byte array to an Object
        private Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);

            return obj;
        }
    }
    public class FileController : Controller
    {
        private readonly ProductManagementSystemDbContext _context;
        public FileController(ProductManagementSystemDbContext context)
        {
            _context = context;
        }

        public ActionResult Index(int id)
        {
            var fileRetrieve = _context.Files.Find(id);
            return File(fileRetrieve.Content, fileRetrieve.ContentType);
        }
    }
}

