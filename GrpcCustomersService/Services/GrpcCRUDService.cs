using Grpc.Core;
using GrpcCustomersService;
using DataAccess = Iepan_Flaviu_Lab2.Data;
using ModelAccess = Iepan_Flaviu_Lab2.Models;
public class GrpcCRUDService : CustomerService.CustomerServiceBase
{

    private DataAccess.LibraryContext db = null;
    public GrpcCRUDService(DataAccess.LibraryContext db)
    {
        this.db = db;
    }
    public override Task<CustomerList> GetAll(Empty empty, ServerCallContext
   context)
    {

        CustomerList pl = new CustomerList();
        var query = from cust in db.Customer
                    select new Customer()
                    {
                        CustomerId = cust.CustomerID,
                        Name = cust.Name,
                        Adress = cust.Adress,
                        Birthdate = cust.BirthDate.ToString("yyyy-MM-dd")
                    };
        pl.Item.AddRange(query.ToArray());
        return Task.FromResult(pl);
    }
    public override Task<Empty> Insert(Customer requestData, ServerCallContext
   context)
    {
        db.Customer.Add(new ModelAccess.Customer
        {
            //CustomerID = requestData.CustomerId,
            Name = requestData.Name,
            Adress = requestData.Adress,
            BirthDate = DateTime.Parse(requestData.Birthdate)
        });
        db.SaveChanges();
        return Task.FromResult(new Empty());
    }

    public override Task<Customer> Get(CustomerId requestData, ServerCallContext context)
    {
        var data = db.Customer.Find(requestData.Id);

        
        if (data == null) return Task.FromResult(new Customer());

        Customer emp = new Customer()
        {
            CustomerId = data.CustomerID,
            Name = data.Name,
            Adress = data.Adress,

            Birthdate = data.BirthDate.ToString("yyyy-MM-dd")
        };
        return Task.FromResult(emp);
    }


    public override Task<Customer> Update(Customer requestData, ServerCallContext context)
    {

        var entity = db.Customer.Find(requestData.CustomerId);

        if (entity == null)
        {

            return Task.FromResult(new Customer());
        }


        entity.Name = requestData.Name;
        entity.Adress = requestData.Adress;


        if (DateTime.TryParse(requestData.Birthdate, out DateTime dateValue))
        {
            entity.BirthDate = dateValue;
        }

        db.SaveChanges();

        return Task.FromResult(requestData);
    }
}
