using CustomerApi.Data;
using CustomerApi.Models;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Models;
using SharedModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CustomerApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IRepository<Customer> _repository;
        private readonly IConverter<Customer, CustomerDto> _customerConverter;

        public CustomersController(IRepository<Customer> repository, IConverter<Customer, CustomerDto> customerConverter)
        {
            _repository = repository;
            _customerConverter = customerConverter;
        }
        
        // GET: api/<CustomerController>
        [HttpGet]
        public IEnumerable<CustomerDto> GetAll()
        {
            var customerDtoList = new List<CustomerDto>();
            foreach (var customer in _repository.GetAll())
            {
                var customerDto = _customerConverter.ConvertModelToDto(customer);
                customerDtoList.Add(customerDto);
            }
            return customerDtoList;
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
            var customerDto = _customerConverter.ConvertModelToDto(customer);
            return new ObjectResult(customerDto);
        }

        // POST api/<CustomerController>
        [HttpPost]
        public IActionResult Post([FromBody]CustomerDto customerDto)
        {
            if (customerDto == null)
            {
                return BadRequest();
            }

            var customer = _customerConverter.ConvertDtoToModel(customerDto);
            var newCustomer = _repository.Add(customer);
            
            return new ObjectResult(newCustomer);
        }

        // PUT api/<CustomerController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]CustomerDto customerDto)
        {
            if (customerDto == null || customerDto.Id != id)
            {
                return BadRequest();
            }

            var modifiedCustomer = _repository.Get(id);

            if (modifiedCustomer == null)
            {
                return NotFound();
            }

            modifiedCustomer.Name = customerDto.Name;
            modifiedCustomer.Email = customerDto.Email;
            modifiedCustomer.Phone = customerDto.Phone;
            modifiedCustomer.BillingAddress = customerDto.BillingAddress;
            modifiedCustomer.ShippingAddress = customerDto.ShippingAddress;
            modifiedCustomer.GoodCreditStanding = customerDto.GoodCreditStanding;
            
            _repository.Edit(modifiedCustomer);
            return new NoContentResult();
        }

        // DELETE api/<CustomerController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_repository.Get(id) == null)
            {
                return NotFound();
            }
            _repository.Remove(id);
            return new NoContentResult();
        }
    }
}
