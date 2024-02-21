using System;
using System.Collections.Generic;
using System.Linq;
using Task3.Entities;
using ClosedXML.Excel;
using System.IO;
using System.Text;

namespace Task3
{
    internal class DataProcessor : IDataProcessor
    {
        private readonly string _filePath;
        private List<Customer> customers;
        private List<Order> orders;
        private List<Product> products;
        public DataProcessor(string filePath)
        {
            _filePath = filePath;
        }
        public bool FileExists()
        {
            return File.Exists(_filePath);
        }
        public void LoadData()
        {
            if (customers == null)
                LoadCustomerData();

            if (products == null)
                LoadProductData();

            if (orders == null)
                LoadOrderData();
        }
        void LoadCustomerData()
        {
            customers = new List<Customer>();
            using (var workbook = new XLWorkbook(_filePath))
            {
                customers = workbook.Worksheet("Клиенты").RowsUsed()
                    .Skip(1)
                    .Select(row => new Customer
                    {
                        Id = row.Cell("A").GetValue<int>(),
                        NameSurname = row.Cell("D").GetString(),
                        Company = row.Cell("B").GetString(),
                        Address = row.Cell("C").GetString()
                    }).ToList();
            }
        }
        void LoadProductData()
        {
            products = new List<Product>();
            using (var workbook = new XLWorkbook(_filePath))
            {
                products = workbook.Worksheet("Товары").RowsUsed()
                    .Skip(1)
                    .Select(row => new Product
                    {
                        Id = row.Cell("A").GetValue<int>(),
                        Name = row.Cell("B").GetString(),
                        MeasurementUnit = row.Cell("C").GetString(),
                        Price = row.Cell("D").GetValue<decimal>()
                    }).ToList();
            }
        }
        void LoadOrderData()
        {
            orders = new List<Order>();
            using (var workbook = new XLWorkbook(_filePath))
            {
                orders = workbook.Worksheet("Заявки").RowsUsed()
                    .Where(row => !row.IsEmpty())
                    .Skip(1)
                    .Select(row => new Order
                    {
                        Id = row.Cell("A").GetValue<int>(),
                        ProductId = row.Cell("B").GetValue<int>(),
                        CustomerId = row.Cell("C").GetValue<int>(),
                        OrderNumber = row.Cell("D").GetValue<int>(),
                        Quantity = row.Cell("E").GetValue<int>(),
                        OrderDate = row.Cell("F").GetDateTime()
                    }).ToList();
            }
        }
        public void DisplayCustomerOrdersByProduct(string productName)
        {
            var query = from order in orders
                        join product in products on order.ProductId equals product.Id
                        where product.Name == productName
                        join customer in customers on order.CustomerId equals customer.Id
                        select new
                        {
                            CustomerName = customer.NameSurname,
                            Quantity = order.Quantity,
                            Price = product.Price * order.Quantity,
                            OrderDate = order.OrderDate
                        };
            DisplayMessage($"Клиенты, которые заказали {productName}:");
            foreach (var result in query)
            {
                DisplayMessage($"ФИО: {result.CustomerName}, количество: {result.Quantity}, " +
                    $"цена: {result.Price}, дата заказа: {result.OrderDate.ToShortDateString()}");
            }
        }
        public void DisplayGoldenCustomer(int year, int month)
        {
            var goldenCustomer = orders
                .Where(order => order.OrderDate.Year == year && order.OrderDate.Month == month)
                .GroupBy(order => order.CustomerId)
                .OrderByDescending(group => group.Sum(order => order.Quantity * products.First(p => p.Id == order.ProductId).Price))
                .Select(group => new
                {
                    Customer = customers.First(c => c.Id == group.Key).NameSurname,
                    TotalAmount = group.Sum(order => order.Quantity * products.First(p => p.Id == order.ProductId).Price)
                })
                .FirstOrDefault();
            DisplayMessage($"Золотой клиент за {month}/{year}: {goldenCustomer?.Customer}, общая сумма: {goldenCustomer?.TotalAmount}");
        }
        public void UpdatePersonInfo(string organization, string newPersonNameSurname)
        {
            var customer = customers.FirstOrDefault(c => c.Company == organization);
            if (customer != null)
            {
                customer.NameSurname = newPersonNameSurname;
                using (var workbook = new XLWorkbook(_filePath))
                {
                    var worksheet = workbook.Worksheet("Клиенты");
                    var cell = worksheet.CellsUsed(c => c.GetString() == organization).FirstOrDefault();
                    if (cell != null)
                    {
                        var newContactPersonCell = worksheet.Cell(cell.Address.RowNumber, cell.Address.ColumnNumber + 3);
                        newContactPersonCell.Value = newPersonNameSurname;
                        workbook.SaveAs(_filePath);
                        DisplayMessage($"Контактное лицо организации {organization} изменено на {newPersonNameSurname}.");
                    }
                }
            }
            else
            {
                DisplayMessage($"Организация {organization} не найдена.");
            }
        }
        private void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
