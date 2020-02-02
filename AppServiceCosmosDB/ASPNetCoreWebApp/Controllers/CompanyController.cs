namespace AppServiceCosmosDB.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AppServiceCosmosDB.DataService;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    public class CompanyController : Controller
    {
        private readonly CosmosDBService _cosmosDbService;
        public CompanyController(CosmosDBService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Company> list = await _cosmosDbService.GetCompaniesAsync();
            return View(list);
        }

        [ActionName("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind("id,companyId,name,address,zipCode,city,country")] Company item)
        {
            if (ModelState.IsValid)
            {
                item.id = Guid.NewGuid().ToString();
                await _cosmosDbService.AddCompanyAsync(item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("id,companyId,name,address,zipCode,city,country")] Company item)
        {
            if (ModelState.IsValid)
            {
                await _cosmosDbService.UpdateCompanyAsync(item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Company item = await _cosmosDbService.GetCompanyAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Company item = await _cosmosDbService.GetCompanyAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind("id")] string id)
        {
            await _cosmosDbService.DeleteCompanyAsync(id);
            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            return View(await _cosmosDbService.GetCompanyAsync(id));
        }
    }
}