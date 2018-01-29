using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace Suppliers_CRUD
{
    class Program
    {
        static void Main(string[] args)
        {
            //Main menu options
            char menuOption;
            do
            {
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("\n1. See all suppliers?");
                Console.WriteLine("2. Add a new supplier?");
                Console.WriteLine("3. UpDate a supplier?");
                Console.WriteLine("4. Delete a supplier?");
                Console.WriteLine("5. Would you like to exit program?");
                menuOption = Console.ReadKey().KeyChar;
                Console.Clear();
                //switch case for the main menu options
                switch(menuOption)
                {
                    //Display all the suppliers to user
                    case '1':
                        try
                        {
                            ReadAllSuppliers(); 
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine("Could not retrieve information from the data base.");
                        }
                        finally
                        {
                            Console.WriteLine("Press Enter to continue.");
                            Console.ReadLine();
                        }
                        break;
                        //Adding a new supplier
                    case '2':
                        try
                        {
                            AddNewSupplier();
                        }
                        catch (Exception exception) 
                        {
                            Console.WriteLine("Unable to add a new supplier at this time.");
                            Console.ReadLine();
                        }
                        finally
                        {
                            Console.WriteLine("Press Enter to continue.");
                            Console.ReadLine();
                        }
                        break;
                        //Update an existing supplier on the database
                    case '3':
                        try
                        {
                            UpDateSupplier();
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine("Unable to update a supplier at this time.");
                            Console.ReadLine();
                        }
                        finally
                        {
                            Console.WriteLine("Press Enter to continue.");
                            Console.ReadLine();
                        }
                        break;
                        //Deleting an existing supplier
                    case '4':
                        try
                        {
                            DeleteSupplier();
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine("Unable to delete supplier at this time.");
                            Console.ReadLine();
                        }
                        finally
                        {
                            Console.WriteLine("Press Enter to continue.");
                            Console.ReadLine();
                        }
                        break;
                        //Exit the menu
                    case '5':
                        Environment.Exit(0);
                        break;
                        //When the user uses inproper key
                    default:
                        Console.WriteLine("Invalid Option!");
                        Console.ReadLine();
                        break;
                }
                Console.Clear();
            }
            //this keeps the menu looping unles they press 5
            while (true);
        }

        private static void ReadAllSuppliers()
        {
            //this grabs the connection string from the config file
            string stringConnection = ConfigurationManager.ConnectionStrings["dataSource"].ConnectionString;
            //this defines the variables so they can be obtained throughout the try catch
            SqlConnection sqlConnection = null;
            SqlDataAdapter dataAdapter = null;
            DataTable suppliers = new DataTable();
            SqlCommand storedProcedure = null;

            try
            {
                //this obtains connection to the database
                sqlConnection = new SqlConnection(stringConnection);
                storedProcedure = new SqlCommand("OBTAIN_SUPPLIERS", sqlConnection);
                dataAdapter = new SqlDataAdapter(storedProcedure);
                sqlConnection.Open();
                dataAdapter.Fill(suppliers);

                //this goes through every row in the dataTable and displays the column value to the user 
                foreach (DataRow row in suppliers.Rows)
                {
                    int supplierId = (int)row["SupplierId"];
                    string companyName = row["CompanyName"].ToString();
                    string contactTitle = row["ContactTitle"].ToString();
                    string contactName = row["ContactName"].ToString();
                    string country = row["Country"].ToString();
                    string phone = row["Phone"].ToString();
                    Console.WriteLine("{0}: {1}", "SupplierId", supplierId);
                    Console.WriteLine("{0}: {1}", "CompanName", companyName);
                    Console.WriteLine("{0}: {1}", "ContactTitle & ContactName", contactTitle + " - " + contactName);
                    Console.WriteLine("{0}: {1}", "ContactName", contactName);
                    Console.WriteLine("{0}: {1}", "Country", country);
                    Console.WriteLine("{0}: {1}", "Phone", phone);
                    Console.WriteLine();
                }
            }
            // this catches any exception and logs it
            catch (Exception exception)
            {
                LogExceptions(exception, "ReadAllSupplier");
                throw exception;
            }
            finally
            {
                //this insures we were able assign a value to the sql connection before we try to close it
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
                if (dataAdapter != null)
                {
                    dataAdapter.Dispose();
                }

            }

        }

        private static void AddNewSupplier()
        {
            //This grabs the connection string from the config file
            string stringConnection = ConfigurationManager.ConnectionStrings["dataSource"].ConnectionString;
            //this obtains the variable so it can be used throughout the trycatch
            SqlConnection sqlConnection = null;
            SqlCommand storedProcedure = null;

            try
            {
                //this obtains the connection to the database
                sqlConnection = new SqlConnection(stringConnection);
                storedProcedure = new SqlCommand("ADD_NEW_SUPPLIER", sqlConnection);
                storedProcedure.CommandType = CommandType.StoredProcedure;

                Console.Write("Enter Company Name: ");
                string companyName = Console.ReadLine();

                Console.Write("Enter Contact Title: ");
                string contactTitle = Console.ReadLine();

                Console.Write("Enter Contact Name: ");
                string contactName = Console.ReadLine();

                Console.Write("Enter your Country: ");
                string country = Console.ReadLine();

                Console.Write("Enter your Phone Number: ");
                string phone = Console.ReadLine();

                //This defines the parameters the user will enter
                storedProcedure.Parameters.AddWithValue("@CompanyName", companyName);
                storedProcedure.Parameters.AddWithValue("@ContactTitle", contactTitle);
                storedProcedure.Parameters.AddWithValue("@ContactName", contactName);
                storedProcedure.Parameters.AddWithValue("@Country", country);
                storedProcedure.Parameters.AddWithValue("@Phone", phone);

                sqlConnection.Open();

                int rowsAffected = storedProcedure.ExecuteNonQuery();
                //if rows affected is greater than 0 then informs the user this was sucessfull
                if (rowsAffected > 0)
                {
                    Console.WriteLine("You successfully added a new supplier!");
                }
                else
                {
                    Console.WriteLine("Could not add new supplier!");
                }
            }
            //This catches the exception and logs it
            catch (Exception exception)
            {
                LogExceptions(exception, "AddNewSupplier");
                throw exception;
            }
            finally
            {
                //this insures we were able assign a value to the sql connection before we try to close it
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
        }

        private static void UpDateSupplier()
        {
            //this grabs the connection string from the config file
            string stringConnection = ConfigurationManager.ConnectionStrings["dataSource"].ConnectionString;
            //This obtains the variable so it can be used throughout the trycatch
            SqlConnection sqlConnection = null;
            SqlCommand storedProcedure = null;
            //This asks if user knows their supplier Id
            Console.WriteLine("Do you know your supplier ID? Y/N?");
            char supId = Console.ReadKey(true).KeyChar;
            //If they dont this reads back all the suppliers
            if (supId != 'Y' && supId != 'y')
            {
                ReadAllSuppliers();
            }

            try
            {
                //this obtains connection to database
                sqlConnection = new SqlConnection(stringConnection);
                Console.WriteLine("Connected");
                storedProcedure = new SqlCommand("UpDate_SUPPLIER", sqlConnection);
                storedProcedure.CommandType = CommandType.StoredProcedure;

                Console.WriteLine("Enter your supplier ID.: ");
                int supplierId = int.Parse(Console.ReadLine());

                Console.Write("Enter Company Name: ");
                string companyName = Console.ReadLine();

                Console.Write("Enter Contact Title: ");
                string contactTitle = Console.ReadLine();

                Console.Write("Enter Contact Name: ");
                string contactName = Console.ReadLine();

                Console.Write("Enter your Country: ");
                string country = Console.ReadLine();

                Console.Write("Enter your Phone Number: ");
                string phone = Console.ReadLine();
                //This defines the parameters the user will enter
                storedProcedure.Parameters.AddWithValue("SupplierId", supplierId);
                storedProcedure.Parameters.AddWithValue("@CompanyName", companyName);
                storedProcedure.Parameters.AddWithValue("@ContactTitle", contactTitle);
                storedProcedure.Parameters.AddWithValue("@ContactName", contactName);
                storedProcedure.Parameters.AddWithValue("@Country", country);
                storedProcedure.Parameters.AddWithValue("@Phone", phone);

                sqlConnection.Open();

                int rowsAffected = storedProcedure.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("You successfully updated a new supplier!");
                }
                else
                {
                    Console.WriteLine("Could not update supplier!");
                }

            }
            //This catches the exception and logs it
            catch (Exception exception)
            {
                LogExceptions(exception, "UpDateSupplier");
                throw exception;
            }
            finally
            {
                //this insures we were able assign a value to the sql connection before we try to close it
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            }
        }

        private static void DeleteSupplier()
        {
            //this grabs the connection string from the config file
            string stringConnection = ConfigurationManager.ConnectionStrings["dataSource"].ConnectionString;
            SqlConnection sqlConnection = null;
            SqlCommand storedProcedure = null;

            Console.WriteLine("Do you know your supplier ID? (y/n)");
            char supId = Console.ReadKey(true).KeyChar;
            //if user does not know Id this displays all suppliers
            if (supId != 'Y' && supId != 'y')
            {
                ReadAllSuppliers();
            }

            try
            {
                //this obtains connection to database
                sqlConnection = new SqlConnection(stringConnection);
                Console.WriteLine("Connected");
                storedProcedure = new SqlCommand("Delete_Supplier", sqlConnection);
                storedProcedure.CommandType = CommandType.StoredProcedure;

                Console.WriteLine("Enter your supplier ID.: ");
                int supplierId = int.Parse(Console.ReadLine());
                //questioning to confirm they want to delete
                Console.WriteLine("Are you sure you want to delete? y/n");
                char deleteConfirmation = Console.ReadKey(true).KeyChar;
                Console.Clear();

                if (deleteConfirmation == 'y')
                {
                    storedProcedure.Parameters.AddWithValue("@SuplierId", supplierId);

                    sqlConnection.Open();

                    int rowsAffected = storedProcedure.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("You successfully deleted your supplier!");
                    }
                    else
                    {
                        Console.WriteLine("Could not delete supplier!");
                    }
                }

            }
            //This catches the exception and logs it
            catch (Exception exception)
            {
                LogExceptions(exception, "DeleteSupplier");
                throw exception;
            }
            finally
            {
                //this insures we were able assign a value to the sql connection before we try to close it
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
        }

        private static void LogExceptions(Exception ex, string methodName)
        {
            //declaring writer as a streawriter type
            StreamWriter writer = null;
            //this is where this obtains the log file path from the config file
            string errorLog = ConfigurationManager.AppSettings.Get("ErrorLog");

            try
            {
                //This creates a srteam to the log file
                writer = new StreamWriter(errorLog, true);
                //This writes the error information to the log file 
                writer.WriteLine(DateTime.Now);
                writer.WriteLine("Level: Info");
                writer.WriteLine("Method Name: "+methodName);
                writer.WriteLine(ex.Message + "\n");
            }
            catch (Exception writerException)
            {
                throw writerException;
            }
            finally
            {
                //this insures we were able assign a value to the writer before we try to close it
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
            }
        }
    } 
}
