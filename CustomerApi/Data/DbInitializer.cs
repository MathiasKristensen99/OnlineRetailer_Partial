using CustomerApi.Models;

namespace CustomerApi.Data;

public class DbInitializer : IDbInitializer
{
    public void Initialize(CustomerApiContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        List<Customer> customers = new List<Customer>
        {
            new Customer
            {
                Name = "Test Bobsen",
                BillingAddress = "Hejvej 123",
                Email = "Hejmail123@mail.dk",
                CreditStanding = 100.0,
                Phone = "12345678",
                ShippingAddress = "Hejvej 123"
            },
            new Customer
            {
                Name = "Test Bobsen",
                BillingAddress = "Hejvej 123",
                Email = "Hejmail123@mail.dk",
                CreditStanding = 100.0,
                Phone = "12345678",
                ShippingAddress = "Hejvej 123"
            },
            new Customer
            {
                Name = "Test Bobsen",
                BillingAddress = "Hejvej 123",
                Email = "Hejmail123@mail.dk",
                CreditStanding = 100.0,
                Phone = "12345678",
                ShippingAddress = "Hejvej 123"
            }
        };
        context.Customers.AddRange(customers);
        context.SaveChanges();
    }
}