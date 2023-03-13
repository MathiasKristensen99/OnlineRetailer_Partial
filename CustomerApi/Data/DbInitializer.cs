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
                Name = "Test 1",
                BillingAddress = "Hejvej 123",
                Email = "Hejmail123@mail.dk",
                GoodCreditStanding = true,
                Phone = "12345678",
                ShippingAddress = "Hejvej 123"
            },
            new Customer
            {
                Name = "Test 2",
                BillingAddress = "Hejvej 123",
                Email = "Hejmail123@mail.dk",
                GoodCreditStanding = true,
                Phone = "12345678",
                ShippingAddress = "Hejvej 123"
            },
            new Customer
            {
                Name = "Test 3",
                BillingAddress = "Hejvej 123",
                Email = "Hejmail123@mail.dk",
                GoodCreditStanding = false,
                Phone = "12345678",
                ShippingAddress = "Hejvej 123"
            }
        };
        context.Customers.AddRange(customers);
        context.SaveChanges();
    }
}