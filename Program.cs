using System;
using static System.Console;
using System.Globalization;
using System.IO;
using CsvHelper;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Transaction
{
    public int amount { get; set; }
    public string date { get; set; }
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
        WriteLine("Type \"[DD/MM] [amount] [source]\" : to enter a transaction.");
        WriteLine("For example: 50 indicates an income and -50 indicates an expense.");
        WriteLine("Type \"balance [category]\" : to check current balance. Passing with no filters will show total balance.");
        WriteLine("Type \"view [MM]\" : to view all transactions. Passing with no filters will show total balance.");
        WriteLine("Type \"delete [DD/MM/YYYY] [category]\" : to delete transaction.");
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

    static void delete_item(string s)
    {
        string[] w = s.Split(null);
        Transaction item = new Transaction();
        string[] filter_date = w[1].Split("/");
        bool exists = false;
        foreach (Transaction t in records)
        {
            string[] date_list = t.date.Split("/");
            if (int.Parse(date_list[0]) == int.Parse(filter_date[0]) && int.Parse(date_list[1]) == int.Parse(date_list[1])
                && t.amount == int.Parse(w[2]) && t.category == w[3])
            {
                exists = true;
                item = t;
                break;
            }
        }
        if (exists)
        {
            records.Remove(item);
            WriteLine($"Deleted {item.amount} {item.date} {item.category}");
        }
        else
            WriteLine("No such transaction exists.");
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
            var orderedList = records.OrderBy(x => DateTime.Parse(x.date)).ToList();
            foreach(Transaction t in orderedList)
            {
                bool output = false;
                string type = (t.amount >= 0)? "income ":"expense";
                string[] date_list = t.date.Split("/");
                if (filter == null)
                    output = true;
                else if (int.Parse(date_list[1]) == int.Parse(filter))
                    output = true;
                if (output)
                {
                    if (start)
                    {
                        WriteLine("Type       Date           Amount        Details");
                        start = false;
                    }
                    entered = true;
                    Write($"{type}    {t.date}    ");
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

    static void add_item(string s)
    {
        string[] words = s.Split(null);
        int amt = int.Parse(words[1]);
        records.Add(new Transaction {date = (words[0]+"/"+now.ToString("yyyy")), 
                                amount = amt, category = words[2]});
    }
    
    static void edit()
    {
        WriteLine("Enter [DD/MM], amount and category of the transaction you wish to edit.");
        WriteLine("Hit enter twice to cancel editing and return to main program.");
        WriteLine("For example: 1/7 100 pizza");
        string w = ReadLine().Trim();
        string[] arr = w.Split();
        bool exists = false;
        string[] filter_date = arr[0].Split("/");
        if (String.IsNullOrEmpty(w))
        {
            WriteLine("Exited edit process");
            return;
        }
        else if (!Regex.IsMatch(w, @"(\d\d|\d)/(\d\d|\d) *-?\d* *[a-z]", RegexOptions.IgnoreCase))
        {
            WriteLine("Wrong format. Exiting edit process. Please start again.");
            return;
        }
        Transaction item = new Transaction();
        foreach (Transaction t in records)
        {
            string[] date_list = t.date.Split("/");
            if (int.Parse(date_list[0]) == int.Parse(filter_date[0]) && int.Parse(date_list[1]) == int.Parse(date_list[1])
                && t.amount == int.Parse(arr[1]) && t.category == arr[2])
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
        records.Remove(item);
        WriteLine("Do you wish to edit date, amount or category?");
        WriteLine("For example, type \"date\" to edit the date.");
        string s = ReadLine().Trim();
        if (String.IsNullOrEmpty(s))
            return;
        string new_data;
        if (s == "date")
        {
            WriteLine("Enter the new date");
            new_data = ReadLine().Trim();
            item.date = new_data+"/"+now.Year;
        }
        else if (s == "amount")
        {
            WriteLine("Enter the new amount");
            new_data = ReadLine().Trim();
            item.amount = int.Parse(new_data);
        }
        else if (s == "category")
        {
            WriteLine("Enter the new category");
            new_data = ReadLine().Trim();
            item.category = (new_data);
        }
        else
        {
            WriteLine("Wrong format. Exiting edit process. Please start again.");
        }
        records.Add(item);
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
            else if (Regex.IsMatch(s, @"delete *(\d\d|\d)/(\d\d|\d) *-?\d* *[a-z]", 
                    RegexOptions.IgnoreCase))
                delete_item(s);
            else if (Regex.IsMatch(s, "balance", RegexOptions.IgnoreCase))
                view_balance();
            else if (Regex.IsMatch(s, "view", RegexOptions.IgnoreCase))
                view_transactions(s);
            else if (Regex.IsMatch(s, @"(\d\d|\d)/(\d\d|\d) *-?\d* *[a-z]", 
                    RegexOptions.IgnoreCase))
                add_item(s);
            else
                invalid_text();
        }
    }
}
