using Example.Domain.Sales;
using System;

namespace Example.Producer.Sales
{
    public class FakeDataGenerator
    {
        public Customer FakeCustomer()
        {
            long customerId = BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);
            long comapnyId = BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);

            return new Customer() {
                customerId = customerId,
                name = new PersonName() {
                    givenName = Faker.Name.FirstName(),
                    familyName = Faker.Name.LastName()
                },
                phoneNumber = Faker.Number.RandomNumber(10000000,99999999).ToString(),
                email = Faker.User.Email(),
                location = new Location() {
                    city = Faker.Address.USCity(),
                    state = Faker.Address.State(),
                    country = Faker.Address.Country()
                },
                company = new Company() {
                    companyId = comapnyId,
                    name = String.Join(" ", Faker.Lorem.Words())
                }
            };
        }
    }
}