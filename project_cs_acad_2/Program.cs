using System;
using System.Data.SQLite;
using System.Linq;

namespace project_cs_acad_2
{
    internal class Program
    {
        private static string databaseFile = $@"{AppDomain.CurrentDomain.BaseDirectory}habit-tracker.db";
        private static string connectionString = $"Data Source={databaseFile}";

        static void Main(string[] args)
        {
            if (!System.IO.File.Exists(databaseFile))
            {
                SQLiteConnection.CreateFile(databaseFile);
                InitializeDatabase();
            }

            bool appOn = true;
            bool menuShow = true;
            string userInput = "";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                while (appOn)
                {
                    if (menuShow)
                    {
                        ShowMenu();
                        userInput = Console.ReadLine();
                    }

                    while (!IsValidOperation(userInput))
                    {
                        Console.WriteLine("Enter a valid operation");
                        userInput = Console.ReadLine();
                    }

                    Console.Clear();
                    menuShow = false;

                    switch (userInput.Trim().ToUpper())
                    {
                        case "V":
                            ViewLogs(tableCmd);
                            menuShow = true;
                            Console.Clear();
                            break;
                        case "I":
                            InsertEntry(tableCmd);
                            menuShow = true;
                            Console.Clear();
                            break;
                        case "U":
                            UpdateEntry(tableCmd);
                            menuShow = true;
                            Console.Clear();
                            break;
                        case "D":
                            DeleteEntry(tableCmd);
                            menuShow = true;
                            Console.Clear();
                            break;
                        case "Q":
                            appOn = false;
                            break;
                        default:
                            break;
                    }
                }

                connection.Close();
            }
        }

        private static void InitializeDatabase()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS water_habit(
                                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            Date TEXT,
                                            Glasses INTEGER)";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        private static void ShowMenu()
        {
            Console.WriteLine("What would you like to do?\n");
            Console.WriteLine("V - View logs");
            Console.WriteLine("I - Insert an entry");
            Console.WriteLine("U - Update an entry");
            Console.WriteLine("D - Delete an entry");
            Console.WriteLine("Q - Quit the app");
        }

        private static bool IsValidOperation(string operation)
        {
            return new string[] { "V", "I", "U", "D", "Q" }.Contains(operation.Trim().ToUpper());
        }

        private static void ViewLogs(SQLiteCommand tableCmd)
        {
            tableCmd.CommandText = "SELECT Date, Glasses FROM water_habit";
            using (SQLiteDataReader reader = tableCmd.ExecuteReader())
            {
                Console.WriteLine("Date\t\tGlasses");
                Console.WriteLine("-------------------------");

                while (reader.Read())
                {
                    string date = reader["Date"].ToString();
                    string glasses = reader["Glasses"].ToString();

                    Console.WriteLine($"{date.PadRight(12)}{glasses}");
                }

                reader.Close();
            }

            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }

        private static void InsertEntry(SQLiteCommand tableCmd)
        {
            Console.WriteLine("Enter data to be inserted: ");
            var resultInsert = GetVals();

            tableCmd.CommandText = "INSERT INTO water_habit(Date, Glasses) VALUES(@Date, @Glasses)";
            tableCmd.Parameters.AddWithValue("@Date", resultInsert.Item1);
            tableCmd.Parameters.AddWithValue("@Glasses", resultInsert.Item2);
            tableCmd.ExecuteNonQuery();
        }

        private static void UpdateEntry(SQLiteCommand tableCmd)
        {
            Console.WriteLine("Enter data to be updated");
            var resultUpdate = GetVals();

            tableCmd.CommandText = "UPDATE water_habit SET Glasses = @Glasses WHERE Date = @Date";
            tableCmd.Parameters.AddWithValue("@Glasses", resultUpdate.Item2);
            tableCmd.Parameters.AddWithValue("@Date", resultUpdate.Item1);
            tableCmd.ExecuteNonQuery();
        }

        private static void DeleteEntry(SQLiteCommand tableCmd)
        {
            Console.WriteLine("Which entry to delete: ");
            string delDate = Console.ReadLine();

            tableCmd.CommandText = "DELETE FROM water_habit WHERE Date = @Date";
            tableCmd.Parameters.AddWithValue("@Date", delDate);
            tableCmd.ExecuteNonQuery();
        }

        static public (string, int) GetVals()
        {
            string inputDate;
            int inputGlasses;
            Console.WriteLine("Enter the date (dd-mm-yy): ");
            inputDate = Console.ReadLine();
            Console.WriteLine("Enter the amount of glasses: ");
            inputGlasses = Convert.ToInt32(Console.ReadLine());

            return (inputDate, inputGlasses);
        }
    }
}
