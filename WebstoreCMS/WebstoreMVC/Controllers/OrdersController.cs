using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinanceLibrary;
using WebstoreMVC.Data;
using Microsoft.AspNetCore.Authorization;
using WebstoreMVC.Models;
using System.Security.Claims;

namespace WebstoreMVC.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly FinanceDbContext _context;

        public OrdersController(FinanceDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public ActionResult Index(int pageNumber = 1)
        {
            var model = new OrderListViewModel();
            model.PageNumber = pageNumber;

            if (User.IsInRole("StoreOwner") || User.IsInRole("Admin"))
            {
                model.ListItems = _context.Orders
                               .OrderBy(o => o.OrderNumber)
                               .Skip(model.PageSize * (model.PageNumber - 1))
                               .Take(model.PageSize)
                               .Select(o => new OrderListItemViewModel()
                               {
                                   Id = o.Id,
                                   OrderNumber = o.OrderNumber,
                                   OrderedProducts = o.OrderedProducts,
                                   TotalPrice = o.TotalPrice,
                                   StatusDescription = o.StatusDescription
                               }).ToList();
            }
            else
            {
                model.ListItems = _context.Orders
                               .Where(o => o.OwnerId == this.User.FindFirstValue(ClaimTypes.NameIdentifier))
                               .OrderBy(o => o.OrderNumber)
                               .Skip(model.PageSize * (model.PageNumber - 1))
                               .Take(model.PageSize)
                               .Select(o => new OrderListItemViewModel()
                               {
                                   Id = o.Id,
                                   OrderNumber = o.OrderNumber,
                                   OrderedProducts = o.OrderedProducts,
                                   TotalPrice = o.TotalPrice,
                                   StatusDescription = o.StatusDescription
                               }).ToList();
            }
            model.PageCount = (int)Math.Ceiling(_context.Orders.Count() / (double)model.PageSize);
            model.AllowEdit = User.Identity.IsAuthenticated;

            return View(model);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        private IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderNumber,OrderedProducts,Status,TotalPrice,StatusDescription")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5   //ONLY StoreOwner & Admin
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderNumber,OrderedProducts,Status,TotalPrice,StatusDescription")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            return View(order);
        }
        
        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        public async Task<IActionResult> CancelOrder(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Cancel();
            _context.Update(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
