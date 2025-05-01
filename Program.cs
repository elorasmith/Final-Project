// Elora Smith, Final Project, 4/27/2025

using System.Diagnostics;
using System.Runtime.InteropServices;

// TESTING ////////////////////////////////////////////////////////////////////////////////////////

Debug.Assert(ChooseGraphScale(1000, 0) == 100);
Debug.Assert(ChooseGraphScale(10_000, 0) == 1000);
Debug.Assert(ChooseGraphScale(100_000, 0) == 10_000);
Debug.Assert(ChooseGraphScale(0, 10_000) != 100);

// MAIN CODE BODY ////////////////////////////////////////////////////////////////////////////////////////

Console.BackgroundColor = ConsoleColor.Black;
Dictionary<int, List<string>> Incomes = new();
Dictionary<int, List<string>> Expenses = new();
decimal incomeTotal = 0;
decimal expenseTotal = 0;

Console.Clear();
Console.WriteLine("Welcome to your Business Budget Planner! \nYou can input your incomes and expenses and then will see a graph representing your net income. \n(This can be for either a month or a what-if scenario) \n\nPlease enter the number of the option you want: \n");
string menu = @"1- Enter Data for Month or What-if Scenario
2- Load Most Recent Entry
3- Exit";
Console.WriteLine(menu);
string? input = Console.ReadLine();
switch (input)
{
    case "1":
        {
            Console.Clear();
            int incomeID = 1;
            int expenseID = 1;
            Console.WriteLine("Enter income: Press enter to input income or any other key to continue to expenses.");
            while (Console.ReadKey(false).Key == ConsoleKey.Enter)
            {
                Console.Clear();
                EnterIncome(ref Incomes, ref incomeID);
                Console.WriteLine("Press enter to input another income or any other key to continue to expenses.");
            }
            Console.Clear();

            Console.WriteLine("Enter expense: Press enter to input expense or any other key to continue to bar graph.");
            while(Console.ReadKey(false).Key == ConsoleKey.Enter)
            {
                Console.Clear();
                EnterExpense(ref Expenses, ref expenseID);
                Console.WriteLine("Press enter to input another expense or any other key to continue to bar graph.");
            }
            Console.Clear();

            decimal net = NetIncome(ref Incomes, ref Expenses, ref incomeTotal, ref expenseTotal);
            Console.WriteLine($"Net Income is ${net}");

            DisplayGraph(incomeTotal, expenseTotal, net);

            Console.WriteLine("What name should this month or what-if scenario be saved under?");
            string? fileName = Console.ReadLine();
            SaveIncomes(fileName, Incomes);
            SaveExpenses(fileName, Expenses);
            File.WriteAllText("FileName", $"{fileName}");
        }
        break;
    
    case "2":
        {
            Console.Clear();
            string fileName = "";
            LoadData(fileName, ref Incomes, ref Expenses);
            Console.WriteLine("Loading previous file...\n");
            Console.WriteLine($"{fileName} Incomes: ");
            foreach (int key in Incomes.Keys)
                Console.WriteLine($"{key} - Type: {Incomes[key][0]}, Amount: ${Incomes[key][1]}, Description: {Incomes[key][2]}");
            Console.WriteLine();
            Console.WriteLine($"{fileName} Expenses: ");
            foreach (int key in Expenses.Keys)
                Console.WriteLine($"{key} - Type: {Expenses[key][0]}, Amount: ${Expenses[key][1]}, Description: {Expenses[key][2]}");
            Console.WriteLine();
            
            decimal net = NetIncome(ref Incomes, ref Expenses, ref incomeTotal, ref expenseTotal);
            DisplayGraph(incomeTotal, expenseTotal, net);
        }
        break;
    
    case "3":
        Console.Clear();
        Environment.Exit(0);
        break;

    default:
        Console.WriteLine("Choice unregconized. Please start over.");
        break;
}


// METHODS ////////////////////////////////////////////////////////////////////////////////////////


static void EnterIncome(ref Dictionary<int, List<string>> Incomes, ref int incomeID)
{
    Console.WriteLine("Please enter the income type: ");
    string? type = Console.ReadLine();
    Console.WriteLine("Enter amount: ");
    string? amount = Console.ReadLine();
    decimal decimalAmount;
    while (!decimal.TryParse(amount, out decimalAmount) || decimalAmount < 0)
    {
        Console.WriteLine("Invalid amount. Please enter in monetary format, eg. 100.00. No negatives.");
        amount = Console.ReadLine();
    }
    Console.WriteLine("Enter description: ");
    string? description = Console.ReadLine();
    Incomes.Add(incomeID, new List<string> {$"{type}", $"{amount}", $"{description}"});
    incomeID++;
}


static void EnterExpense(ref Dictionary<int, List<string>> Expenses, ref int expenseID)
{
    Console.WriteLine("Please enter the expense type: ");
    string? type = Console.ReadLine();
    Console.WriteLine("Enter amount: ");
    string? amount = Console.ReadLine();
    decimal decimalAmount;
    while (!decimal.TryParse(amount, out decimalAmount) || decimalAmount < 0)
    {
        Console.WriteLine("Invalid amount. Please enter in monetary format, eg. 100.00. No negatives.");
        amount = Console.ReadLine();
    }
    Console.WriteLine("Enter description: ");
    string? description = Console.ReadLine();
    Expenses.Add(expenseID, new List<string> {$"{type}", $"{amount}", $"{description}"});
    expenseID++;
}


static decimal NetIncome(ref Dictionary<int, List<string>> Incomes, ref Dictionary<int, List<string>> Expenses, ref decimal incomeTotal, ref decimal expenseTotal)
{
    foreach (int ID in Incomes.Keys)
    {
        decimal amount = Convert.ToDecimal(Incomes[ID][1]);
        incomeTotal += amount;
    }
    foreach (int ID in Expenses.Keys)
    {
        decimal amount = Convert.ToDecimal(Expenses[ID][1]);
        expenseTotal += amount;
    }
    decimal net = incomeTotal - expenseTotal;
    if (net < 0) return 0;
    else return net;
}


static void DisplayGraph(decimal incomeTotal, decimal expenseTotal, decimal net)
{
    int intIncomes = Convert.ToInt32(incomeTotal);
    int intExpenses = Convert.ToInt32(expenseTotal);
    int intNet = intIncomes - intExpenses;
    int scale = ChooseGraphScale(incomeTotal, expenseTotal);
    if (intNet < 0) intNet = 0;

    // Bar for Income Total
    int CursorTop = 25;
    Console.SetCursorPosition(5, CursorTop);
    Console.BackgroundColor = ConsoleColor.Blue;
    int cursorLimit = CursorTop - intIncomes/scale;
    while (CursorTop > cursorLimit)
    {
        Console.SetCursorPosition(5, CursorTop);
        Console.WriteLine("            ");
        CursorTop--;
    }
    Console.BackgroundColor = ConsoleColor.Black;
    Console.SetCursorPosition(5, cursorLimit);
    Console.WriteLine($"${incomeTotal}");
    Console.SetCursorPosition(5, 26);
    Console.WriteLine("Income Total");

    // Bar for Expense Total
    CursorTop = 25;
    Console.SetCursorPosition(20, CursorTop);
    Console.BackgroundColor = ConsoleColor.Red;
    cursorLimit = CursorTop - intExpenses/scale;
    while (CursorTop > cursorLimit)
    {
        Console.SetCursorPosition(20, CursorTop);
        Console.WriteLine("            ");
        CursorTop--;
    }
    Console.BackgroundColor = ConsoleColor.Black;
    Console.SetCursorPosition(20, cursorLimit);
    Console.WriteLine($"${expenseTotal}");
    Console.SetCursorPosition(20, 26);
    Console.WriteLine("Expense Total");

    // Bar for Net Income
    CursorTop = 25;
    Console.SetCursorPosition(35, CursorTop);
    Console.BackgroundColor = ConsoleColor.Green;
    cursorLimit = CursorTop - intNet/scale;
    while (CursorTop > cursorLimit)
    {
        Console.SetCursorPosition(35, CursorTop);
        Console.WriteLine("            ");
        CursorTop--;
    }
    Console.BackgroundColor = ConsoleColor.Black;
    Console.SetCursorPosition(35, cursorLimit);
    Console.WriteLine($"${net}");
    Console.SetCursorPosition(35, 26);
    Console.WriteLine("NET Income");
    Console.SetCursorPosition(0, 28);
}


static int ChooseGraphScale(decimal incomeTotal, decimal expenseTotal)
{
    int scale;
    if (incomeTotal > 0 && incomeTotal < 2000 || expenseTotal > 0 && expenseTotal < 2000) scale = 100;
    else if (incomeTotal >= 2000 && incomeTotal < 20_000 || expenseTotal > 2000 && expenseTotal < 20_000) scale = 1000;
    else if (incomeTotal >= 20_000 && incomeTotal < 200_000 || expenseTotal > 20_000 && expenseTotal < 200_000) scale = 10_000;
    else scale = 100_000;
    return scale;
}


static void SaveIncomes(string? fileName, Dictionary<int, List<string>> Incomes) //transations = either Incomes or Expenses
{
    string[] temp = Incomes.Select(kvp => $"{kvp.Key},{string.Join(",", kvp.Value)}").ToArray();
    File.WriteAllLines($"{fileName}.Incomes", temp);
}

static void SaveExpenses(string? fileName, Dictionary<int, List<string>> Expenses)
{
    string[] temp = Expenses.Select(kvp => $"{kvp.Key},{string.Join(",", kvp.Value)}").ToArray();
    File.WriteAllLines($"{fileName}.Expenses", temp);
}


static void LoadData(string fileName, ref Dictionary<int, List<string>> Incomes, ref Dictionary<int, List<string>> Expenses)
{
    fileName = File.ReadAllText("FileName");

    // LOAD INCOMES
    string[] temp = File.ReadAllLines($"{fileName}.Incomes");
    foreach (string item in temp)
    {
        string[] parts = item.Split(",");
        int key = int.Parse(parts[0]);
        List<string> values = parts.Skip(1).ToList();
        Incomes[key] = values;
    }

    // LOAD EXPENSES
    temp = File.ReadAllLines($"{fileName}.Expenses");
    foreach (string item in temp)
    {
        string[] parts = item.Split(",");
        int key = int.Parse(parts[0]);
        var values = parts.Skip(1).ToList();
        Expenses[key] = values;
    }
}
