using Grpc.Net.Client;
using GrpcCustomersService;
using Microsoft.AspNetCore.Mvc;
namespace Iepan_Flaviu_Lab2.Controllers
{
    public class CustomersGrpcController : Controller
    {
        private readonly GrpcChannel channel;
        public CustomersGrpcController()
        {
//a se modifica portul corespunzator (vezi in proiectul GrpcCustomerService-> Properties->launchSettings.json)
            channel = GrpcChannel.ForAddress("http://localhost:5244");
        }
        [HttpGet]
        public IActionResult Index()
        {
            var client = new CustomerService.CustomerServiceClient(channel);
            CustomerList cust = client.GetAll(new Empty());
            return View(cust);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var client = new
                CustomerService.CustomerServiceClient(channel);
                var createdCustomer = client.Insert(customer);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var client = new CustomerService.CustomerServiceClient(channel);
            Customer customer = client.Get(new CustomerId() { Id = (int)id });
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var client = new CustomerService.CustomerServiceClient(channel);
            Empty response = client.Delete(new CustomerId()
            {
                Id = id
            });
            return RedirectToAction(nameof(Index));
        }


        // GET: CustomersGrpc/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = new CustomerService.CustomerServiceClient(channel);
 
            Customer customer = client.Get(new CustomerId() { Id = (int)id });

            if (customer == null || customer.CustomerId == 0) 
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: CustomersGrpc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Customer customer)
        {

            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var client = new CustomerService.CustomerServiceClient(channel);

                    client.Update(customer);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Eroare la actualizare: " + ex.Message);
                }
            }
            return View(customer);
        }
    }
}