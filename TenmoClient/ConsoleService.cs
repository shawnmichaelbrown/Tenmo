using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    public class ConsoleService
    {
        private static readonly AuthService authService = new AuthService();

        public void Run()
        {

            while (true)
            {
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.WriteLine("3: Exit");
                Console.Write("Please choose an option: ");

                int loginRegister = -1;

                try
                {
                    if (!int.TryParse(Console.ReadLine(), out loginRegister))
                    {
                        Console.WriteLine("Invalid input. Please enter only a number.");
                    }

                    else if (loginRegister == 1)
                    {
                        LoginUser loginUser = PromptForLogin();
                        API_User user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
                            MenuSelection();
                        }
                    }

                    else if (loginRegister == 2)
                    {
                        LoginUser registerUser = PromptForLogin();
                        bool isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                        }
                    }

                    else if (loginRegister == 3)
                    {
                        Console.WriteLine("Goodbye!");
                        Environment.Exit(0);
                    }

                    else
                    {
                        Console.WriteLine("Invalid selection.");
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Error - " + ex.Message);
                }
            }
        }

        private void MenuSelection()
        {

            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers"); //view details through here
                Console.WriteLine("3: View your pending requests"); //ability to approve/reject through here
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: View list of users");
                Console.WriteLine("7: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                try
                {
                    if (!int.TryParse(Console.ReadLine(), out menuSelection))
                    {
                        Console.WriteLine("Invalid input. Please enter only a number.");
                    }
                    else if (menuSelection == 1)
                    {
                        ViewBalance();
                    }
                    else if (menuSelection == 2)
                    {
                        //todo view past transfers method
                        ListTransfers();
                    }
                    else if (menuSelection == 3)
                    {
                        //todo CODE CODE CODE
                    }
                    else if (menuSelection == 4)
                    {
                        SendTEBucks();
                    }
                    else if (menuSelection == 5)
                    {
                        //todo request TE bucks method
                    }
                    else if (menuSelection == 6)
                    {
                        ListUsers();
                    }
                    else if (menuSelection == 7)
                    {
                        Console.WriteLine("");
                        UserService.SetLogin(new API_User()); //wipe out previous login info
                        return; //return to register/login menu
                    }
                    else if (menuSelection == 0)
                    {
                        Console.WriteLine("Goodbye!");
                        Environment.Exit(0);
                    }

                    else
                    {
                        Console.WriteLine("Please try again");
                        Console.WriteLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error - " + ex.Message);
                    Console.WriteLine();
                }
            }
        }

        public int PromptForTransferID(string action)
        {
            Console.WriteLine("");
            Console.Write("Please enter transfer ID to " + action + " (0 to cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int auctionId))
            {
                Console.WriteLine("Invalid input. Only input a number.");
                return 0;
            }
            else
            {
                return auctionId;
            }
        }

        public LoginUser PromptForLogin()
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();
            string password = GetPasswordFromConsole("Password: ");

            LoginUser loginUser = new LoginUser
            {
                Username = username,
                Password = password
            };
            return loginUser;
        }

        private string GetPasswordFromConsole(string displayMessage)
        {
            string pass = "";
            Console.Write(displayMessage);
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Backspace Should Not Work
                if (!char.IsControl(key.KeyChar))
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Remove(pass.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine("");
            return pass;
        }

        public void ViewBalance()
        {
            int userId = UserService.GetUserId();
            double amount = authService.GetBalance(userId);

            Console.WriteLine("Your current balance: $" + amount);
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();

        }

        public double SetBalance()
        {
            int userId = UserService.GetUserId();
            double amount = authService.GetBalance(userId);
            return amount;
        }

        public void ListUsers()
        {
            List<API_User> users = authService.GetUsers();
            foreach (API_User item in users)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();

        }

        public void SendTEBucks()
        {
            double balance = SetBalance();
            int userId = UserService.GetUserId();
            double amountNum = 0D;
            Console.WriteLine("Here is everyone you can send TE Bucks to: ");
            List<API_User> users = authService.GetUsers();
            foreach (API_User item in users)
            {
                if (item.UserId != userId)
                {
                    Console.WriteLine(item);
                }
            }
            Console.WriteLine("Who would you like to send TE bucks to? (select their user id)");

            string transfer = Console.ReadLine();
            int transferId = Convert.ToInt32(transfer);
            bool done = false;
            while (!done)
            {

                Console.WriteLine("How much money would you like to send?");

                string amount = Console.ReadLine();

                try
                {
                    amountNum = Convert.ToDouble(amount);
                    if (amountNum < 0)
                    {
                        throw new InvalidOperationException("Negative numbers are not allowed.");
                    }
                    if (amountNum > balance)
                    {
                        throw new InvalidOperationException("You do not have enough money to make this transfer.");
                    }
                    done = true;
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine("Please enter a valid amount." + ex.Message);
                }
            }

            TransferMoney transferMoney = new TransferMoney(userId, transferId, amountNum);

            authService.TransferMoney(transferMoney, userId);

        }

        public void ListTransfers()
        {
            List<API_User> users = authService.GetUsers();
            int userId = UserService.GetUserId();
            List<TransferMoney> transfers = new List<TransferMoney>();
            transfers = authService.ViewTransfer(userId);
            foreach (TransferMoney item in transfers)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();

            Console.WriteLine("Which transaction would you like to view?");

            string trans = Console.ReadLine();

            int transNum = Convert.ToInt32(trans);

            List<TransferMoney> result = new List<TransferMoney>();

            result = authService.SingleTransfer(transNum);

            string usersName = "";
            string transfersName = "";

            foreach (TransferMoney item in result)
            {
                foreach (API_User name in users)
                {
                    if (item.UserId == name.UserId)
                    {
                        usersName = name.Username;
                    }
                    if (item.TransferToId == name.UserId)
                    {
                        transfersName = name.Username;
                    }

                }
            }

            foreach (TransferMoney item in result)
            {
                Console.WriteLine("Transfer ID: " + item.TransferId + " | Type: Send | Status: Approved | From: " + usersName + " | To: " + transfersName + " | $" + item.Amount);
            }
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();
        }
    }
}
