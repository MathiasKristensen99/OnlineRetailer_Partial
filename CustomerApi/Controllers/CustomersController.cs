using CustomerApi.Data;
using CustomerApi.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CustomerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IRepository<Customer> _repository;

        public CustomersController(IRepository<Customer> repository)
        {
            _repository = repository;
        }
        
        // GET: api/<CustomerController>
        [HttpGet]
        public IEnumerable<Customer> GetAll()
        {
            return _repository.GetAll();
        }

        // GET api/<CustomerController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var customer = _repository.Get(id);
            if (customer == null)
            {
                return NotFound();
            }

            return new ObjectResult(customer);
        }

        // POST api/<CustomerController>
        [HttpPost]
        public IActionResult Post([FromBody]Customer customer)
        {
            if (customer == null)
            {
                return BadRequest();
            }

            var newCustomer = _repository.Add(customer);
            return new ObjectResult(newCustomer);
        }

        // PUT api/<CustomerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]Customer customer)
        {
            _repository.Edit(new Customer
            {
                Id = id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                BillingAddress = customer.BillingAddress,
                ShippingAddress = customer.ShippingAddress,
                CreditStanding = customer.CreditStanding
            });
        }

        // DELETE api/<CustomerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _repository.Remove(id);
        }
    }
}
