﻿using SteamAccountManager.Application.Steam.Service;
using SteamAccountManager.Application.Steam.UseCase;
using SteamAccountManager.AvaloniaUI.Common;
using SteamAccountManager.AvaloniaUI.Mappers;
using SteamAccountManager.AvaloniaUI.Models;
using SteamAccountManager.AvaloniaUI.ViewModels.Commands;
using SteamAccountManager.Domain.Steam.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SteamAccountManager.AvaloniaUI.ViewModels
{
    // TODO: looks ridicilous, I should refactor all of this but I don't feel bored enough yet
    internal class AccountSwitcherViewModel
    {
        private readonly IGetAccountsWithDetailsUseCase _getAccountsUseCase;
        private readonly SwitchAccountUseCase _switchAccountUseCase;
        private readonly AccountMapper _accountMapper;
        private readonly ILocalNotificationService _notificationService;

        public AdvancedObservableCollection<Account> Accounts { get; private set; }
        public ICommand ProfileClickedCommand { get; }
        public ICommand RefreshAccountsCommand { get; }
        public ICommand ShowInfoCommand { get; }
        public ICommand AddAccountCommand { get; }
        public Account? SelectedAccount { get; set; }


        public AccountSwitcherViewModel
        (
            IGetAccountsWithDetailsUseCase getAccountsUseCase,
            SwitchAccountUseCase switchAccountUseCase,
            AccountMapper accountMapper,
            IAccountStorageObservable accountStorageObserver,
            ILocalNotificationService notificationService
        )
        {
            _getAccountsUseCase = getAccountsUseCase;
            _switchAccountUseCase = switchAccountUseCase;
            _accountMapper = accountMapper;
            _notificationService = notificationService;

            Accounts = new AdvancedObservableCollection<Account>();
            ProfileClickedCommand = new ProfileClickedCommand();
            RefreshAccountsCommand = new QuickCommand(LoadAccounts);
            ShowInfoCommand = new QuickCommand(ShowInfo);
            AddAccountCommand = new QuickCommand(AddAccount);
            accountStorageObserver.Subscribe(Accounts_Changed);

            LoadAccounts();
        }

        private void Accounts_Changed(List<Domain.Steam.Model.Account>? accounts)
        {
            LoadAccounts();
        }

        private async void AddAccount()
        {
            await _switchAccountUseCase.Execute(string.Empty);
        }

        private IEnumerable<Account> SortAccounts(Account[] accounts) => accounts.OrderByDescending(x => x.Rank.Level)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.IsVacBanned);

        private async Task<IEnumerable<Account>> GetAccounts()
        {
            var steamAccounts = await _getAccountsUseCase.Execute();
            var accounts = await Task.WhenAll(steamAccounts.ConvertAll(x => _accountMapper.FromSteamAccount(x)));
            return SortAccounts(accounts);
        }

        public async void LoadAccounts()
        {
            Accounts.SetItems((await GetAccounts()).ToList());
        }

        public async void OnAccountSelected(Account selectedAccount)
        {
            await _switchAccountUseCase.Execute(selectedAccount.Name);
            SelectedAccount = selectedAccount;
            _notificationService.Send
            (
                new Notification
                (
                    selectedAccount.Name,
                    selectedAccount.Username,
                    selectedAccount.ProfilePictureUrl
                )
            );
        }

        public void ShowInfo()
        {
            if(System.OperatingSystem.IsWindows())
            {
                System.Diagnostics.Process.Start("explorer", "https://github.com/sahin-a/SteamAccountManager/");
            }
            else 
            {
                System.Diagnostics.Process.Start("xdg-open", "https://github.com/sahin-a/SteamAccountManager/");
            }
        }
    }
}
