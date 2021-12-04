using System;
using Autofac;
using SteamAccountManager.Domain.Steam.Service;

namespace SteamAccountManager.Console.Menu
{
    public class MainMenu : IMenu   
    {
        private readonly ISteamService _steamService;

        public MainMenu()
        {
            _steamService = Program.Container.Resolve<ISteamService>();

            Show();
        }
        
        public void Show()
        {
            ShowAccountSelection();
        }

        private void ShowAccountSelection()
        {
            System.Console.Clear();

            var steamAccounts = _steamService.GetAccounts().Result;
            
            for (int i = 0; i < steamAccounts.Count; i++)
            {
                var account = steamAccounts[i];
                System.Console.WriteLine($"{i}. [Valid: {account.IsLoginTokenValid}] {account.AccountName}");
            }

            System.Console.WriteLine("Enter Number to log in account, Habibi!!");

            string? accountSelection = System.Console.ReadLine();
        
            if (Int32.TryParse(accountSelection, out int accountIndex))
            {
                var selectedAccount = steamAccounts[accountIndex];
                System.Console.WriteLine($"Selected Account: {selectedAccount.AccountName}");
                
                _steamService.SwitchAccount(selectedAccount);
            }

            OnAccountSelected();
        }

        private void OnAccountSelected()
        {
            ShowAccountSelection();
        }
    }
}