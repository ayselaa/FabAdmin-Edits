using Fab.Areas.FabAdmin.Helpers;
using Fab.Areas.FabAdmin.ViewModels.Gorunus;
using Fab.Data;
using Fab.Models.CategoriesFolder;
using Fab.Models.Tetbiq;
using Fab.ViewModels.ProductVM;
using FabAdmin.ViewModels.Subcategory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing.Printing;

namespace Fab.Controllers
{
    public class ProductController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(int categoryid, int subctgid, int applicationid, int appearanceid, int charid, int page=1, int pageSize=10)
        {

            ViewBag.Applicate = applicationid;
            ViewBag.Appear = appearanceid;
            ViewBag.Charid = charid;
            ViewBag.Categoryid = categoryid;
            ViewBag.SubCategoryid = subctgid;

            var lang = Request.Cookies["selectedLanguage"];
            if (string.IsNullOrEmpty(lang))
            {
                lang = "az";
            }
            else
            {
                lang = lang.ToLower();
            }
         
            var allproduct = _context.Products.Select(x => new GetallProduct
            {
                Id = x.Id,
                Name = x.Translates.FirstOrDefault(x => x.LangCode == lang).Name,
                Size = x.Translates.FirstOrDefault(x => x.LangCode == lang).Size,
                Image = x.Images.FirstOrDefault(x => x.IsMain == true).Image,
                CategoryId = x.CategoryId,
                SubCategoryId = x.SubcategoryId,
                ApplicationId = x.ApplicationFieldId,
                AppearanceId = x.AppearanceFieldId,
                CharacteristicsId = x.CharacteristicId,
                CharacteristicsName = x.Characteristic.Translates.FirstOrDefault(x => x.LangCode == lang).Name,
                ApplicationName = x.ApplicationField.Translates.FirstOrDefault(x => x.LangCode == lang).Name,
                AppearanceName = x.AppearanceField.Translates.FirstOrDefault(x => x.LangCode == lang).Name,
            }).AsNoTracking()
                .AsQueryable();               



            var allchars = _context.Characteristics.Select(x => new CharVM()
            {
                Id = x.Id,
                CategoryId = x.Category.Id,
                Name = x.Translates.FirstOrDefault(x => x.LangCode == lang).Name
            }).ToList();
            var allapplication = _context.Characteristics.Select(x => new ApplicationVm()
            {
                Id = x.Id,
                CategoryId = x.Category.Id,
                Name = x.Translates.FirstOrDefault(x => x.LangCode == lang).Name
            }).ToList();
            var allapperance = _context.Characteristics.Select(x => new ApperanceVm()
            {

                Id = x.Id,
                CategoryId = x.Category.Id,
                Name = x.Translates.FirstOrDefault(x => x.LangCode == lang).Name
            }).ToList();

            if (categoryid != 0)
            {
                allproduct = allproduct.Where(x => x.CategoryId == categoryid);
                allchars = allchars.Where(x => x.CategoryId == categoryid).ToList();
                allapperance = allapperance.Where(x => x.CategoryId == categoryid).ToList();
                allapplication = allapplication.Where(x => x.CategoryId == categoryid).ToList();
            }

            if (subctgid != 0)
            {
                allproduct = allproduct.Where(x => x.SubCategoryId == subctgid);
            }
            if (appearanceid != 0)
            {
                allproduct = allproduct.Where(x => x.AppearanceId == appearanceid);
            }
            if (charid != 0)
            {
                allproduct = allproduct.Where(x => x.CharacteristicsId == charid);
            }
            if (applicationid != 0)
            {
                allproduct = allproduct.Where(x => x.ApplicationId == applicationid);
            }
            var ctg = _context.Categories.Select(x => new GetCategory()
            {
                Id = x.Id,
                Name = x.Translates.FirstOrDefault(x => x.LangCode == lang).Name,
                Subcategories = x.Subcategories.Select(y => new Getsubcategory()
                {
                    Name = y.Translates.FirstOrDefault(u => u.LangCode == lang).Name,
                    Id = y.Id,
                    CategoryId = x.Id
                }).ToList(),
            }).ToList();
            var allsubctg = new GetallCategory
            {


                Categories = ctg,
                Apperance = allapperance,
                Applicat = allapplication,
                Chars = allchars

            };

            var paginateTickets = await PaginatedList<GetallProduct>.CreateAsync(allproduct, page, pageSize);
            GetAllVM product = new GetAllVM()
            {
                Categories = allsubctg,
                Products = paginateTickets,
                LangCode= lang,
                //Applicat = allapplication,
                //Apperance = allapperance,
                //Chars = allchars
            };

            return View(product);
        }
        public async Task<IActionResult> Detail(int id)
        {
            if (id == 0) return BadRequest();

            var lang = Request.Cookies["selectedLanguage"];
            if (string.IsNullOrEmpty(lang))
            {
                lang = "az";
            }
            else
            {
                lang = lang.ToLower();
            }

            var product = await _context.Products.Include(m => m.Category).ThenInclude(m => m.Translates)
                .Include(m => m.Translates.Where(m => m.LangCode == lang))
                .Include(m => m.Images)
                .FirstOrDefaultAsync(m => m.Id == id);

            DetailVM detailedProducts = new()
            {
                Product = product,
                Others = await _context.Products.Where(m => m.CategoryId == product.CategoryId).Where(m => m.Id != id).Include(m => m.Translates.Where(m => m.LangCode == lang)).Include(m => m.Images).ToListAsync(),
                LangCode = lang
            };






            return View(detailedProducts);

        }
        //[HttpPost]
        //public async Task<IActionResult> GetAllSubCategory(int categoryId)
        //{
        //    var lang = Request.Cookies["selectedLanguage"];
        //    if (string.IsNullOrEmpty(lang))
        //    {
        //        lang = "az";
        //    }
        //    else
        //    {
        //        lang = lang.ToLower();
        //    }
        //    var allchars = _context.Characteristics.Select(x => new CharVM()
        //    {
        //        Id = x.Id,
        //        CategoryId = x.Category.Id,
        //        Name = x.Translates.FirstOrDefault(x => x.LangCode == lang).Name
        //    }).ToList();
        //    var allapplication = _context.Characteristics.Select(x => new ApplicationVm()
        //    {
        //        Id = x.Id,
        //        CategoryId = x.Category.Id,
        //        Name = x.Translates.FirstOrDefault(x => x.LangCode == lang).Name
        //    }).ToList();
        //    var allapperance = _context.Characteristics.Select(x => new ApperanceVm()
        //    {

        //        Id = x.Id,
        //        CategoryId = x.Category.Id,
        //        Name = x.Translates.FirstOrDefault(x => x.LangCode == lang).Name
        //    }).ToList();
        //    var ctg = _context.Categories.Select(x => new GetCategory()
        //    {
        //        Id = x.Id,
        //        Name = x.Translates.FirstOrDefault(x => x.LangCode == lang).Name,
        //        Subcategories = x.Subcategories.Select(y => new Getsubcategory()
        //        {
        //            Name = y.Translates.FirstOrDefault(u => u.LangCode == lang).Name,
        //            Id = y.Id,
        //            CategoryId = x.Id
        //        }).ToList(),
        //    }).ToList();
        //    var allcatg = new GetallCategory
        //    {
        //        Categories = ctg,
        //        Apperance = allapperance.Where(x => x.CategoryId == categoryId).ToList(),
        //        Applicat = allapplication.Where(x => x.CategoryId == categoryId).ToList(),
        //        Chars = allchars.Where(x => x.CategoryId == categoryId).ToList()

        //    };

        //    return PartialView("_FilterPPartial", allcatg);
        //}



        //public async Task<ActionResult> GetSubCategories(int categoryId)
        //{
        //    var subCategories = await _context.Subcategories
        //        .Where(x => x.CategoryId == categoryId)
        //        .Select(he => new SubcategoryDTO
        //        {
        //            Id = he.Id,
        //            Name = he.Translates.FirstOrDefault(het => het.LangCode == "en").Name
        //        })
        //        .ToListAsync();


        //    var exProp = await _context.ExProps
        //        .Where(x => x.CategoryId == categoryId)
        //        .Select(he => new ExPropDTO
        //        {
        //            Id = he.Id,
        //            Names = he.Translates.FirstOrDefault(het => het.LangCode == "en").Name
        //        })
        //        .ToListAsync();
	
	/////bisheyler	

        //    return Json(new { ExtraProperties = exProp, Subcategories = subCategories });
        //}
    }
}
