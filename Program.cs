using System;
using System.Windows.Forms;

namespace DBcourseProject
{
    class Program
    {
        static DatabaseHandler database = new DatabaseHandler();
        //GUIHandler gui = new GUIHandler();
        static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.Run(new GUIHandler());
            //chooseLoginOrCreate();
        }
        // private static void chooseLoginOrCreate() {
        //     int option;
        //     while(true) {
        //         Console.WriteLine("\nSelect From the Following: ");
        //         Console.WriteLine("1) Login");
        //         Console.WriteLine("2) Create An Account");
        //         Console.WriteLine("3) Initialize Database");
        //         string loginOrCreate = Console.ReadLine();
        //         if(checkIfValidNumber(loginOrCreate, 1, 3)) {
        //             option = Int32.Parse(loginOrCreate);
        //             break;
        //         } else {
        //             continue;
        //         }
        //     }
        //     if(option == 3) {
        //         initializeDatabase();
        //         return;
        //     }
        //     Console.WriteLine("\nEnter Username: ");
        //     string username = Console.ReadLine();
        //     Console.WriteLine("\nEnter Password: ");
        //     string password = Console.ReadLine();
        //     if(option == 1) {
        //         loginAccount(username, password);
        //     } else if(option == 2) {
        //         createAccount(username, password);
        //     }
        // }

        // private static void loginAccount(string username, string password) {
        //     if(database.checkLogin(username, password) == true) {
        //         Console.WriteLine("Login Successful");
        //     } else {
        //         Console.WriteLine("Login Fail");
        //     }
        //     chooseLoginOrCreate();
        // }

        // private static void createAccount(string username, string password) {
        //     Console.WriteLine("Confirm password: ");
        //     string confirmedPassword = Console.ReadLine();
        //     if(password != confirmedPassword) {
        //         Console.WriteLine("\nPasswords do not match");
        //         chooseLoginOrCreate();
        //         return;
        //     }
        //     Console.WriteLine("What is your first name: ");
        //     string firstName = Console.ReadLine();
        //     Console.WriteLine("What is your last name: ");
        //     string lastName = Console.ReadLine();
        //     Console.WriteLine("What is your email: ");
        //     string email = Console.ReadLine();

        //     database.createAccount(username, password, firstName, lastName, email);
        //     chooseLoginOrCreate();
        // }

        // private static void initializeDatabase() {
        //     database.destroyDatabase();
        //     database.createDatabase();
        //     chooseLoginOrCreate();
        // }

        // //checks if string input is a number, as well as within the given range
        // private static bool checkIfValidNumber(string input, int low = -10000, int high = 10000) {
        //     int temp = -1;
        //     try {
        //         temp = Int32.Parse(input);
        //     } catch (FormatException e) {
        //         Console.WriteLine("\nPlease Enter a valid input");
        //         return false;
        //     }
        //     if(temp < low || temp > high) {
        //         Console.WriteLine("\nPlease Enter a valid input");
        //         return false;
        //     }
        //     return true;
        // }
    }
}
