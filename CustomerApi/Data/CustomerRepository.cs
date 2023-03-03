using CustomerApi.Models;
using CustomerApi.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Data;

public class CustomerRepository : IRepository<Customer>
{
    private readonly CustomerApiContext db;
    public CustomerRepository(CustomerApiContext context)
    {
        db = context;
    }
    
    public IEnumerable<Customer> GetAll()
    {
        return db.Customers.ToList();
    }

    public Customer Get(int id)
    {
        return db.Customers.FirstOrDefault(c => c.Id == id);
    }

    public Customer Add(Customer entity)
    {
        var newCustomer = db.Customers.Add(entity).Entity;
        db.SaveChanges();
        return newCustomer;
    }

    public void Edit(Customer entity)
    {
        db.Entry(entity).State = EntityState.Modified;
        db.SaveChanges();
    }

    public void Remove(int id)
    {
        var customer = db.Customers.FirstOrDefault(c => c.id == id);
        db.Customers.Remove(order);
        db.SaveChanges();
    }
}