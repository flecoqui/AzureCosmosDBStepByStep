namespace AppServiceCosmosDB.Controllers
{
    using System;
    using System.Threading.Tasks;
    using AppServiceCosmosDB.DataService;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    public class EmployeeController : Controller
    {
        private readonly CosmosDBService _cosmosDbService;
        public EmployeeController(CosmosDBService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _cosmosDbService.GetEmployeesAsync());
        }

        [ActionName("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind("id,employeeId,firstName,lastName,address,zipCode,city,country")] Employee item)
        {
            if (ModelState.IsValid)
            {
                item.id = Guid.NewGuid().ToString();
                await _cosmosDbService.AddEmployeeAsync(item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("id,employeeId,firstName,lastName,address,zipCode,city,country")] Employee item)
        {
            if (ModelState.IsValid)
            {
                await _cosmosDbService.UpdateEmployeeAsync(item);
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

            Employee item = await _cosmosDbService.GetEmployeeAsync(id);
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

            Employee item = await _cosmosDbService.GetEmployeeAsync(id);
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
            await _cosmosDbService.DeleteEmployeeAsync(id);
            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            return View(await _cosmosDbService.GetEmployeeAsync(id));
        }
    }
}