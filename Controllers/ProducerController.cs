using eTickets.Data;
using eTickets.Data.Services;
using eTickets.Data.Static;
using eTickets.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eTickets.Controllers
{
    [Authorize(Roles =UserRoles.Admin)]
    public class ProducerController : Controller
    {
        private readonly IProducerService _service;

        public ProducerController(IProducerService service)
        {
            _service = service;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var data=await _service.GetAllAsync();  
            return View(data);
        }

      public async Task<IActionResult> Details(int id)
        {
            var producerDetails=await _service.GetByIdAsync(id);
            if(producerDetails == null)  return View("NotFound");
            return View(producerDetails);
            
        }

        public IActionResult create()
        {
            return View();  
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("profilepictureURl,fullName,Bio")] Producer producer)
        {
            ModelState.Remove("Movies");
            if (!ModelState.IsValid)
            {
                return View(producer);
            }
            await _service.AddAsync(producer);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var producerdetails=await _service.GetByIdAsync(id);
            if(producerdetails == null) return View("NotFound");
            return View(producerdetails);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id,[Bind("Id,profilepictureURl,fullName,Bio")] Producer producer)
        {
            ModelState.Remove("Movies");
            if (!ModelState.IsValid)
            {
                return View(producer);
            }
            if (id == producer.Id)
            {
                await _service.UpdateAsync(id, producer);
                return RedirectToAction(nameof(Index));
            }
            return View(producer);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var Details = await _service.GetByIdAsync(id);

            if (Details == null) return View("NotFound");
            return View(Details);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Details = await _service.GetByIdAsync(id);
            if (Details == null) return View("NotFound");

            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
