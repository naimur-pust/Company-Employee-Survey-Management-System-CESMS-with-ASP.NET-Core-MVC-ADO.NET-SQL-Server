using Microsoft.AspNetCore.Mvc;
using SmartHRIS.Models;
using SmartHRIS.Services;

namespace SmartHRIS.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _service;
        public EmployeeController(IEmployeeService service)
        {
            _service = service;
        }

        // Dashboard / View All Employees
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Employee> data = await _service.GetAllAsync();
            
            ViewBag.Shabuj = data;
            return View(data);
        }

        // Create (GET)
        [HttpGet]
        public IActionResult Create() => View();

        // Create (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var id = await _service.CreateAsync(model);
                TempData["ok"] = $"Employee saved (Id={id})";
                @ViewBag.ok = $"Employee saved (Id={id})";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) 
            {
                ModelState.AddModelError("", "Email or Phone Number already exists!");
                return View(model);
            }
        }

        // Update Search (GET)
        [HttpGet]
        public IActionResult UpdateSearch() => View();

        // Update Search (POST) -> Show Edit Form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSearch(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                ModelState.AddModelError("", "Please enter a phone number");
                return View();
            }

            var data = await _service.GetByPhoneAsync(phone);
            if (data == null)
            {
                ModelState.AddModelError("", "No record found for this number");
                return View();
            }
            return View("Edit", data);
        }

        // Edit (POST) -> Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Employee model)
        {
            if (!ModelState.IsValid) return View(model);

            var rows = await _service.UpdateByPhoneAsync(model);
            if (rows == 0)
            {
                ModelState.AddModelError("", "Update failed. Number not found.");
                return View(model);
            }

            TempData["ok"] = "Employee Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        // Delete Search (GET)
        [HttpGet]
        public IActionResult DeleteSearch() => View();

        // Delete Search (POST) -> Show Confirm Page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSearch(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                ModelState.AddModelError("", "Please enter a phone number");
                return View();
            }

            var data = await _service.GetByPhoneAsync(phone);
            if (data == null)
            {
                ModelState.AddModelError("", "No record found for this number");
                return View();
            }
            return View("DeleteConfirm", data);
        }

        // Delete (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string phone)
        {
            var rows = await _service.DeleteByPhoneAsync(phone);
            TempData["ok"] = rows > 0 ? "Deleted Successfully" : "Delete failed";
            return RedirectToAction(nameof(Index));
        }
    }
}
