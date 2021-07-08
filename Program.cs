using CsvHelper;
using System;
using System.Collections.Generic;
using static System.Console;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class Transaction
{
    public DateTime date {get; set;}
    public int amount { get; set; }
    public string category { get; set; }
}
class Program
{
    static DateTime now = DateTime.Today;
    static string FILE = "statement.csv";
    static List<Transaction> records = new List<Transaction>();
    static void welcome()
    {
        WriteLine();
        WriteLine("Hello & Welcome to the Personal Finance Tool");
        WriteLine("Type \"help\" to view commands or \"end\" to close program.");
        WriteLine();
    }

    static void help_text()
    {
        WriteLine();
        WriteLine("Type \"[date] [amount] [category]\" : to enter a transaction.");
        WriteLine("For example: 50 indicates an income and -50 indicates an expense.");
        WriteLine("Not Entering a year defaults it to the current year.");
        WriteLine("Type \"balance\" : to check current balance.");
        WriteLine("Type \"view [month]\" : to view all transactions. Passing with no filters will show all transactions.");
        WriteLine("Type \"delete [date] [amount] [category]\" : to delete transaction.");
        WriteLine("Type \"edit\" and follow instructions : to edit a transaction.");
        WriteLine("Type \"end\" to close program");
        WriteLine();
    }

    static void invalid_text()
    {
        WriteLine("invalid input, type \"help\" to view commands");
    }
    static void make_csv()
    {
        using (var writer = new StreamWriter(FILE))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<Transaction>();
            csv.NextRecord();
        }
    }

    static List<Transaction> read_csv()
    {
        using (var reader = new StreamReader(FILE))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            List<Transaction> list = csv.GetRecords<Transaction>().ToList();
            return list;
        }
    }

    static void write_to_csv(List<Transaction> list)
    {
        using (var writer = new StreamWriter(FILE))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(list);
            csv.NextRecord();
        }
    }

    static void add_item(string s)
    {
        string[] words = s.Split(null);
        int amt = int.Parse(words[1]);
        records.Add(new Transaction {date = DateTime.Parse(words[0]), 
                                amount = amt, category = words[2]});
    }

    static void view_transactions(string s)
    {
        string[] w = s.Split(null);
        string filter = null;
        if (w.Length > 2)
        {
            invalid_text();
            return;
        }
        else if (w.Length == 2)
        {
            filter = w[1];
            if (!Regex.IsMatch(filter, @"(\d\d|\d)", RegexOptions.IgnoreCase))
            {
                invalid_text();
                return;
            }
        }
        if (records.Count > 0)
        {
            WriteLine();
            bool start = true; //check if header needs to be printed
            bool entered = false; //check if atleast one entry present;
            var orderedList = records.OrderBy(x => x.date).ToList();
            foreach(Transaction t in orderedList)
            {
                bool output = false;
                string type = (t.amount >= 0)? "income ":"expense";
                if (filter == null)
                    output = true;
                else if (t.date.Month == int.Parse(filter))
                    output = true;
                if (output)
                {
                    if (start)
                    {
                        WriteLine("Type       Date           Amount        Details");
                        start = false;
                    }
                    entered = true;
                    Write($"{type}    {t.date.ToShortDateString()}    ");
                    Write("{0,7}",$"{t.amount:n0}");
                    WriteLine($"         {t.category}");
                }
            }
            if (!entered)
                WriteLine("No transactions to display");
        }
        else
            WriteLine("No transactions to display");
        WriteLine();
    }

    static void delete_item(string s)
    {
        string[] w = s.Split(null);
        Transaction item = new Transaction();
        DateTime filter = DateTime.Parse(w[1]);
        bool exists = false;
        foreach (Transaction t in records)
        {
            if (t.date == filter && t.amount == int.Parse(w[2]) && t.category == w[3])
            {
                exists = true;
                item = t;
                break;
            }
        }
        if (exists)
        {
            records.Remove(item);
            WriteLine($"Deleted {item.date.ToShortDateString()} {item.amount} {item.category}");
        }
        else
            WriteLine("No such transaction exists.");
    }

    static void edit_date(Transaction item)
    {
        WriteLine("Enter the new date");
        string new_data = ReadLine().Trim();
        while (true)
        {
            if(new_data == "")
            {
                WriteLine("Exiting editing process. Returning to main program.");
                return;
            }        
            else if ((Regex.IsMatch(new_data, @"(\d\d|\d)/(\d\d|\d) *", RegexOptions.IgnoreCase) 
            || Regex.IsMatch(new_data, @"(\d\d|\d)/(\d\d|\d)/\d\d\d\d *", RegexOptions.IgnoreCase)))
            {
                DateTime new_date = DateTime.Parse(new_data);
                records.Remove(item);
                item.date = new_date;
                break;
            }
            else
            {
                WriteLine("Wrong date format. Please re-enter.");
                new_data = ReadLine().Trim();
            }
        }
    }

    static void edit_amount(Transaction item)
    {
        WriteLine("Enter the new amount");
        string new_data = ReadLine().Trim();
        while (true)
        {
            if (Regex.IsMatch(new_data, @"-?\d+ *", RegexOptions.IgnoreCase))
            {
                records.Remove(item);
                item.amount = int.Parse(new_data);
                break;
            }
            else if(new_data == "")
            {
                WriteLine("Exiting editing process. Returning to main program.");
                return;
            }
            else
            {
                WriteLine("Wrong amount format. Amount can have only numbers. Please re-enter.");
                new_data = ReadLine().Trim();
            }
        }
    }
    static void edit_category(Transaction item)
    {
        WriteLine("Enter the new category");
        string new_data = ReadLine().Trim();
        if(new_data == "")
        {
            WriteLine("Exiting editing process. Returning to main program.");
            return;
        }
        records.Remove(item);
        item.category = (new_data);
    }
    static void edit()
    {
        WriteLine("Enter [DD/MM], amount and category of the transaction you wish to edit.");
        WriteLine("Hit enter twice to cancel editing and return to main program.");
        WriteLine("For example: 1/7 100 pizza");
        string w = ReadLine().Trim();
        bool exists = false;
        
        if (w == "")
        {
            WriteLine("Exited edit process");
            return;
        }
        else if (!(Regex.IsMatch(w, @"(\d\d|\d)/(\d\d|\d) *-?\d* *[a-z]", RegexOptions.IgnoreCase) 
        || Regex.IsMatch(w, @"(\d\d|\d)/(\d\d|\d)/\d\d\d\d *-?\d* *[a-z]", RegexOptions.IgnoreCase)))
        {
            WriteLine("Wrong format. Exiting edit process. Please start again.");
            return;
        }

        string[] arr = w.Split();
        DateTime filter = DateTime.Parse(arr[0]);
        Transaction item = new Transaction();
        foreach (Transaction t in records)
        {
            if (t.date == filter && t.amount == int.Parse(arr[1]) && t.category == arr[2])
            {
                exists = true;
                item = t;
                break;
            }
        }
        if (!exists)
        {
            WriteLine("No such transaction exists. Exiting edit process. Please start again.");
            return;
        }
        WriteLine("Do you wish to edit date, amount or category?");
        WriteLine("For example, type \"date\" to edit the date.");
        string s = ReadLine().Trim();
        bool tryagain = true;
        while(tryagain)
        {
            tryagain = false;
            if (s == "")
            {
                WriteLine("Exiting editing process. Returning to main program.");
                return;
            }
            if (s == "date")
                edit_date(item);
            else if (s == "amount")
                edit_amount(item);                
            else if (s == "category")
                edit_category(item);
            else
            {
                WriteLine("Wrong format. Please enter again.");
                s = ReadLine().Trim();
                tryagain = true;
            }
        }
        records.Add(item);
        WriteLine("Edited");
        WriteLine();
    }

    static void view_balance()
    {
        Dictionary<string, int> balance_dict =new Dictionary<string, int>();
        balance_dict.Add("Total", 0);
        foreach (Transaction B in records)
        {
            balance_dict["Total"] += B.amount;
            if (balance_dict.ContainsKey(B.category))
                balance_dict[B.category] += B.amount;
            else
                balance_dict[B.category] = B.amount;

        }
        foreach( KeyValuePair<string, int> kvp in balance_dict)
        {
            WriteLine(kvp.Key + ": "+ kvp.Value);
        }
    }

    static void Main(string[] args)
    {

        if (!File.Exists(FILE))
            make_csv();
        records = read_csv();
        welcome();

        while (true)
        {
            string s = ReadLine().Trim();
            if (s == null || s == "end")
            {
                write_to_csv(records);
                break;
            }
            else if (s == "help")
                help_text();
            else if (s == "edit")
                edit();
            else if (Regex.IsMatch(s, @"delete *(\d\d|\d)/(\d\d|\d) *-?\d+ *[a-z]", 
                    RegexOptions.IgnoreCase) || Regex.IsMatch(s, @"delete *(\d\d|\d)/(\d\d|\d)/\d\d\d\d *-?\d* *[a-z]", 
                    RegexOptions.IgnoreCase))
                delete_item(s);
            else if (Regex.IsMatch(s, "balance", RegexOptions.IgnoreCase))
                view_balance();
            else if (Regex.IsMatch(s, "view", RegexOptions.IgnoreCase))
                view_transactions(s);
            else if (Regex.IsMatch(s, @"(\d\d|\d)/(\d\d|\d) *-?\d+ *[a-z]", 
                    RegexOptions.IgnoreCase) || Regex.IsMatch(s, @"(\d\d|\d)/(\d\d|\d)/\d\d\d\d *-?\d+ *[a-z]", 
                    RegexOptions.IgnoreCase))
                add_item(s);
            else
                invalid_text();
        }
    }
}
