using System;
using System.Text;
using Task3;

class Program
{
    static void Main()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("************ Программа управления заказами ************");
        Console.Write("Введите путь до файла с данными: ");
        string filePath = Console.ReadLine();

        DataProcessor dataProcessor = new DataProcessor(filePath);

        if (dataProcessor.FileExists())
        {
            dataProcessor.LoadData();
            Console.WriteLine("\nВыберите команду:");
            Console.WriteLine("1. Поиск информации о клиентах по товару");
            Console.WriteLine("2. Изменение контактного лица клиента");
            Console.WriteLine("3. Определение золотого клиента");
            Console.WriteLine("4. Выход");

            int choice;
            if (int.TryParse(Console.ReadLine(), out choice))
            {
                switch (choice)
                {
                    case 1:
                        Console.Write("Введите наименование товара: ");
                        string productName = Console.ReadLine();
                        dataProcessor.DisplayCustomerOrdersByProduct(productName);
                        break;

                    case 2:
                        Console.Write("Введите название организации клиента: ");
                        string organizationName = Console.ReadLine();
                        Console.Write("Введите ФИО нового контактного лица: ");
                        string newContactPerson = Console.ReadLine();
                        dataProcessor.UpdatePersonInfo(organizationName, newContactPerson);
                        break;

                    case 3:
                        Console.Write("Введите год заказов: ");
                        int year;
                        if (int.TryParse(Console.ReadLine(), out year))
                        {
                            Console.Write("Введите месяц заказов: ");
                            int month;
                            if (int.TryParse(Console.ReadLine(), out month) && month >= 1 && month <= 12)
                            {
                                dataProcessor.DisplayGoldenCustomer(year, month);
                            }
                            else
                            {
                                Console.WriteLine("Некорректный месяц. Введите число от 1 до 12.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Некорректный год. Введите корректный год.");
                        }
                        break;

                    case 4:
                        Console.WriteLine("Программа завершена.");
                        return;

                    default:
                        Console.WriteLine("Некорректный ввод. Пожалуйста, выберите существующую команду.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Некорректный ввод. Введите число.");
            }
        }
        else
        {
            Console.WriteLine("Файл не найден.");
        }
    }
}