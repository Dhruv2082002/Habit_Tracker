using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace project_cs_acad_2
{
    internal class Program
    {

        static public (string,int) Get_vals()
        {
            string input_date;
            int input_glasses;
            Console.WriteLine("Enter the date(dd-mm-yy): ");
            input_date = Console.ReadLine();
            Console.WriteLine("Enter the amount of glasses: ");
            input_glasses = Convert.ToInt32(Console.ReadLine());

            return (input_date,input_glasses);  
        }

        static void Main(string[] args)
        {


            // Connection string
            string databaseFile = $@"{AppDomain.CurrentDomain.BaseDirectory}habit - tracker.db";

            if (!System.IO.File.Exists(databaseFile))
            {
                // Create a new SQLite database file
                SQLiteConnection.CreateFile(databaseFile);
            }

            string connectionString = $"Data Source={databaseFile}";

            // Create a new connection
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            //Console.WriteLine(connectionString);


            // Open the connection
            connection.Open();

            var tablecmd = connection.CreateCommand();

            tablecmd.CommandText = @"CREATE TABLE IF NOT EXISTS water_habit(

                                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Date TEXT,
                                    Glasses INTEGER)";


            tablecmd.ExecuteNonQuery();
            bool App_on = true;
            bool menu_show = true;
            string userInput = "";

            while (App_on)
            {
                if (menu_show)
                {
                    // Perform database operations here
                    Console.WriteLine("what would you like to do?\n");
                    Console.WriteLine("V - view logs");
                    Console.WriteLine("I - insert an entry");
                    Console.WriteLine("U - update an entry");
                    Console.WriteLine("D - delete an entry");
                    Console.WriteLine("Q - quit the app");

                    userInput = Console.ReadLine();

                }
                while (!(new string[] { "V", "I", "U", "D" , "Q" }.Contains(userInput)))
                {
                    Console.WriteLine("Enter valid operation");
                    userInput = Console.ReadLine();
                }

                Console.Clear();
                menu_show = false;  

                switch (userInput.Trim().ToUpper())
                {

                    case "V":

                        tablecmd.CommandText = "SELECT Date,Glasses FROM water_habit";
                        SQLiteDataReader reader = tablecmd.ExecuteReader();

                        Console.WriteLine("Date\t\tGlasses");
                        Console.WriteLine("-------------------------");

                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["Date"]}\t\t {reader["Glasses"]}");
                        }

                        reader.Close();

                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                        menu_show= true;
                        Console.Clear();

                        break;

                    case "I":

                        Console.WriteLine("Enter data to be Inserted: ");
                        var result_insert = Get_vals();
                        tablecmd.CommandText = $"INSERT INTO water_habit(Date,Glasses) VALUES('{result_insert.Item1}',{result_insert.Item2})";

                        tablecmd.ExecuteNonQuery();
                        menu_show = true;
                        Console.Clear();

                        break;

                    case "U":

                        Console.WriteLine("Enter data to be updated");
                        var result_update = Get_vals();

                        tablecmd.CommandText = $"UPDATE water_habit SET glasses = {result_update.Item2} WHERE date = {result_update.Item1}";
                        tablecmd.ExecuteNonQuery();
                        menu_show = true;
                        Console.Clear();      


                        break;

                    case "D":
                        Console.WriteLine("Which entry to delete: ");
                        string del_date = Console.ReadLine();
                        tablecmd.CommandText = $"DELETE FROM water_habit WHERE date = '{del_date}'";
                        tablecmd.ExecuteNonQuery();
                        menu_show = true;
                        Console.Clear();    
                        break;

                    case "Q":
                        App_on = false;
                        break;
                    default:
                        break;
                }

            }

            // Close the connection when done
            connection.Close();

        }
    }
}
