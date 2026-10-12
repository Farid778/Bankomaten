
namespace Bankomaten
{
    internal class Program
    {
        // Stores the usernames of all five users.
        static string[] usernames =
            { "user1", "user2", "user3", "user4", "user5" };

        // Stores the PIN codes for all five users.
        static string[] passwords =
            { "1234", "2345", "3456", "4567", "5678" };

        // Stores the number of accounts each user has.
        static int[] accountCounts = { 1, 2, 3, 1, 2 };

        // Stores account names in a two-dimensional array.
        // Each row represents a user, and each column represents an account.
        static string[,] accountsName =
        {
            { "Allkonto",   "",            "" },
            { "Allkonto",   "Sparkonto",   "" },
            { "Allkonto",   "Sparkonto1",   "Sparkonto2" },
            { "Allkonto",   "",            "" },
            { "Allkonto",   "Sparkonto",   "" }
        };

        // Stores account balances in a two-dimensional array.
        // Empty account positions have a balance of zero.
        static decimal[,] accountsBalance =
        {
            { 10000.00m,     0m,          0m },
            { 2300.55m,   4500.45m,        0m },
            { 10000.00m, 15000.00m, 30000.00m },
            { 1900.00m,      0m,          0m },
            { 7000.00m,  15000.00m,        0m }
        };

        // Starts the program and handles login and logout.
        static void Main(string[] args)
        {
            Console.WriteLine("Välkommen till Banken!");

            while (true)
            {
                if (Login(out int userIn))
                {
                    Menu(userIn);
                }
                else
                {
                    Console.WriteLine(
                        "För många försök. Programmet avslutas.");
                    return;
                }
            }
        }

        // Handles login and allows a maximum of three attempts.
        static bool Login(out int userIn)
        {
            userIn = -1;
            int attempts = 0;

            while (attempts < 3)
            {
                Console.Write("Ange ditt användarnamn: ");
                string username = Console.ReadLine() ?? "";

                Console.Write("Ange din PIN-kod: ");
                string password = Console.ReadLine() ?? "";

                // Check the username and PIN against all users.
                for (int i = 0; i < usernames.Length; i++)
                {
                    if (usernames[i] == username &&
                        passwords[i] == password)
                    {
                        userIn = i;
                        Console.WriteLine("Inloggningen lyckades!");
                        return true;
                    }
                }

                attempts++;

                Console.WriteLine(
                    $"Fel användarnamn eller PIN-kod. " +
                    $"Försök kvar: {3 - attempts}");
            }

            return false;
        }

        // Displays the menu and handles the user's choices.
        static void Menu(int userIn)
        {
            while (true)
            {
                Console.WriteLine(
                    "\nVad vill du göra?\n" +
                    "1. Se dina konton och saldo\n" +
                    "2. Överföring mellan konton\n" +
                    "3. Ta ut pengar\n" +
                    "4. Logga ut");

                if (!int.TryParse(Console.ReadLine(), out int userChoice))
                {
                    Console.WriteLine("Ogiltigt val, försök igen!");
                }
                else
                {
                    switch (userChoice)
                    {
                        case 1:
                            Accounts(userIn);
                            break;

                        case 2:
                            Transfer(userIn);
                            break;

                        case 3:
                            Withdraw(userIn);
                            break;

                        case 4:
                            Console.WriteLine("Du har loggat ut.");
                            return;

                        default:
                            Console.WriteLine(
                                "Ogiltigt val, välj mellan 1 och 4.");
                            break;
                    }
                }

                Console.WriteLine(
                    "\nTryck på Enter för att komma till huvudmenyn.");
                Console.ReadLine();
            }
        }

        // Displays the logged-in user's accounts and balances.
        static void Accounts(int userIn)
        {
            Console.WriteLine("\nDina konton och saldon:");

            for (int i = 0; i < accountCounts[userIn]; i++)
            {
                Console.WriteLine(
                    $"{i + 1}. {accountsName[userIn, i]}: " +
                    $"{accountsBalance[userIn, i]:C}");
            }
        }

        // Transfers money between two accounts belonging to the same user.
        static void Transfer(int userIn)
        {
            int totalAccounts = accountCounts[userIn];

            if (totalAccounts < 2)
            {
                Console.WriteLine(
                    "Överföring är inte möjlig eftersom du har för få konton.");
                return;
            }

            Accounts(userIn);

            int fromAccount = SelectAccount(
                userIn, "Välj kontot du vill överföra pengar från:");

            int toAccount;

            // The destination must be a different account.
            while (true)
            {
                toAccount = SelectAccount(
                    userIn, "Välj kontot du vill överföra pengar till:");

                if (toAccount != fromAccount)
                {
                    break;
                }

                Console.WriteLine(
                    "Du kan inte överföra pengar till samma konto!");
            }

            decimal amount;

            // Check that the amount is positive and affordable.
            while (true)
            {
                Console.Write("Hur mycket vill du överföra? ");

                if (decimal.TryParse(
                    Console.ReadLine(), out amount) &&
                    amount > 0 &&
                    amount <= accountsBalance[userIn, fromAccount])
                {
                    break;
                }

                Console.WriteLine(
                    "Ogiltigt belopp eller otillräckligt saldo.");
            }

            // Update both account balances.
            accountsBalance[userIn, fromAccount] -= amount;
            accountsBalance[userIn, toAccount] += amount;

            Console.WriteLine("\nÖverföringen lyckades!");

            Console.WriteLine(
                $"{accountsName[userIn, fromAccount]}: " +
                $"{accountsBalance[userIn, fromAccount]:C}");

            Console.WriteLine(
                $"{accountsName[userIn, toAccount]}: " +
                $"{accountsBalance[userIn, toAccount]:C}");
        }

        // Lets the user select an existing account.
        // Returns the selected account's zero-based column index.
        static int SelectAccount(int userIn, string message)
        {
            while (true)
            {
                Console.WriteLine($"\n{message}");

                for (int i = 0; i < accountCounts[userIn]; i++)
                {
                    Console.WriteLine(
                        $"{i + 1}. {accountsName[userIn, i]}: " +
                        $"{accountsBalance[userIn, i]:C}");
                }

                if (int.TryParse(Console.ReadLine(), out int choice) &&
                    choice >= 1 &&
                    choice <= accountCounts[userIn])
                {
                    // Convert the menu number to an array index.
                    return choice - 1;
                }

                Console.WriteLine(
                    "Ogiltigt konto. Välj ett konto som finns.");
            }
        }

        // Handles cash withdrawals from the user's accounts.
        static void Withdraw(int userIn)
        {
            Accounts(userIn);

            int account = SelectAccount(
                userIn, "Välj kontot du vill ta ut pengar från:");

            decimal amount;

            // Check that the withdrawal amount is valid.
            while (true)
            {
                Console.Write("Hur mycket vill du ta ut? ");

                if (decimal.TryParse(
                    Console.ReadLine(), out amount) &&
                    amount > 0 &&
                    amount <= accountsBalance[userIn, account])
                {
                    break;
                }

                Console.WriteLine(
                    "Ogiltigt belopp eller otillräckligt saldo.");
            }

            // Require the user's PIN before completing the withdrawal.
            int attempts = 0;

            while (attempts < 3)
            {
                Console.Write("Ange din PIN-kod för att bekräfta uttaget: ");
                string userPin = Console.ReadLine() ?? "";

                if (userPin == passwords[userIn])
                {
                    accountsBalance[userIn, account] -= amount;

                    Console.WriteLine("\nUttaget lyckades!");
                    Console.WriteLine(
                        $"Nytt saldo på {accountsName[userIn, account]}: " +
                        $"{accountsBalance[userIn, account]:C}");

                    return;
                }

                attempts++;

                Console.WriteLine(
                    $"Fel PIN-kod. Försök kvar: {3 - attempts}");
            }

            Console.WriteLine(
                "Uttaget avbröts eftersom du angav fel PIN-kod tre gånger.");
        }
    }
}
