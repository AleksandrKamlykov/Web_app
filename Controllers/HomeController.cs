using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_app.DBContext;
using Web_app.Models;

namespace Web_app.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MainDBContext _context;
    private readonly BlobService _blobService;

    public HomeController(ILogger<HomeController> logger, MainDBContext context,BlobService blobService)
    {
        _logger = logger;
        _context = context;
        _blobService = blobService;
    }   

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
            // GET: Products
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        // GET: Products/Details/5
        public IActionResult Details(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile imageFile, [Bind("Name,Description,Price")] Product product)
        {
          
                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var stream = imageFile.OpenReadStream())
                    {
                        var imageUrl = await _blobService.UploadFileAsync(stream, imageFile.FileName);
                        product.ImageUrl = imageUrl;
                    }
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
            
            return View(product);
        }

        // GET: Products/Edit/5
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile imageFile, [Bind("Id,Name,Description,Price,ImageUrl")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                using (var stream = imageFile.OpenReadStream())
                {
                    var imageUrl = await _blobService.UploadFileAsync(stream, imageFile.FileName);
                    product.ImageUrl = imageUrl;
                }
            }
            else
            {
                product.ImageUrl = existingProduct.ImageUrl; // Retain the existing image URL
            }

            _context.Entry(existingProduct).CurrentValues.SetValues(product);
            _context.Entry(existingProduct).State = EntityState.Modified;

            try
            {
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

            return View(product);
        }
        // GET: Products/Delete/5
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
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
}
