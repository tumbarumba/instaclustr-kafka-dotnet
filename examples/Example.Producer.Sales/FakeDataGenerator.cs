using Example.Domain.Sales;
using System;

namespace Example.Producer.Sales
{
    public class FakeDataGenerator
    {
        public Customer FakeCustomer()
        {
            return new Customer() {
                customerId = Guid.NewGuid().ToString(),
                name = new PersonName() {
                    givenName = Faker.Name.FirstName(),
                    familyName = Faker.Name.LastName()
                },
                email = Faker.User.Email(),
                location = new Location() {
                    city = Faker.Address.USCity(),
                    state = Faker.Address.State(),
                    country = Faker.Address.Country()
                },
                company = new Company() {
                    companyId = Guid.NewGuid().ToString(),
                    name = String.Join(" ", Faker.Lorem.Words(3))
                }
            };
        }
    }
}