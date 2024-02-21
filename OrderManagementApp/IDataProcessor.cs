using System;
namespace Task3
{
    internal interface IDataProcessor
    {
        void LoadData();
        void DisplayCustomerOrdersByProduct(string productName);
        void UpdatePersonInfo(string organization, string newPersonNameSurname);
        void DisplayGoldenCustomer(int year, int month);
    }
}
