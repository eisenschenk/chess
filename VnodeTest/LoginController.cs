using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.BC.Account;
using static ACL.UI.React.DOM;


namespace VnodeTest
{
    public class LoginController
    {
        private AccountProjection AccountProjection;
        private string Username;
        private string Password;
        private bool LoginSelected;
        private bool RegisterSelected;
        private RenderMode CurrentRenderMode;


        public LoginController(AccountProjection accountProjection)
        {
            AccountProjection = accountProjection;
        }

        public VNode Render(RootController rootController)
        {
            VNode login = RenderLogin(rootController);
            VNode register = RenderRegisterAccount();
            VNode loginRegisterSelection = Div(
                Text("Register", Styles.Btn & Styles.MP4, () => RegisterSelected = true),
                Text("Login", Styles.Btn & Styles.MP4, () => LoginSelected = true)
            );
            return
                LoginSelected
                ? login
                : RegisterSelected
                    ? register
                    : loginRegisterSelection;
        }

        private VNode RenderLogin(RootController rootController)
        {
            if (CurrentRenderMode == RenderMode.error)
                return Text("Wrong Username/Password!", Styles.AbortBtn & Styles.MP4, () => CurrentRenderMode = RenderMode.@default);

            return Div(
                Input(Username, s => Username = s),
                Input(Password, s => Password = s).WithPassword(),
                Text("login ", Styles.Btn, () =>
                {
                    try
                    {
                        Account.Commands.LoginAccount(GetUser().ID, Password);
                    }
                    catch (ArgumentException)
                    {
                        CurrentRenderMode = RenderMode.error;
                    }
                    rootController.AccountEntry = GetUser();
                }),
                Text("back", Styles.Btn & Styles.MP4, () => LoginSelected = false)
            );
        }

        private VNode RenderRegisterAccount()
        {

            return
                CurrentRenderMode == RenderMode.error
                ? Text("Username taken!", Styles.AbortBtn & Styles.MP4, () => CurrentRenderMode = RenderMode.@default)
                : Div(
                    Input(Username, s => Username = s),
                    Input(Password, s => Password = s).WithPassword(),
                    Text("register Account", Styles.Btn, () =>
                    {
                        if (!AccountProjection.Accounts.Select(x => x.Username).Contains(Username))
                        {
                            Account.Commands.RegisterAccount(ACL.ES.AggregateID<Account>.Create(), Username, Password);
                            Username = string.Empty;
                            Password = string.Empty;
                            RegisterSelected = false;
                        }
                        else
                            CurrentRenderMode = RenderMode.error;
                    }),
                    Text("back", Styles.Btn & Styles.MP4, () => RegisterSelected = false)
                );
        }

        private AccountEntry GetUser() => AccountProjection.Accounts.Where(p => p.Username == Username).SingleOrDefault();

        private enum RenderMode
        {
            @default,
            error
        }
    }
}
