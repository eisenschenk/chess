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
        private Rendermode RenderMode;


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
            return
                 RenderMode == Rendermode.error
                ? Text("Wrong Username/Password!", Styles.AbortBtn & Styles.MP4, () => RenderMode = Rendermode.@default)
                : Div(
                    Input(Username, s => Username = s),
                    Input(Password, s => Password = s).WithPassword(),
                    Text("login ", Styles.Btn, () =>
                    {
                        string hashsalt = AccountProjection.Accounts.Where(x => x.Username == Username).FirstOrDefault()?.Password;
                        if (PasswordHelper.IsPasswordMatch(Password, hashsalt) && hashsalt != default)
                        {
                            Account.Commands.LoginAccount(User(hashsalt).ID, Username, Password, hashsalt);
                            rootController.AccountEntry = User(hashsalt);
                        }
                        else
                            RenderMode = Rendermode.error;
                    }),
                    Text("back", Styles.Btn & Styles.MP4, () => LoginSelected = false)
                );
        }

        private VNode RenderRegisterAccount()
        {

            return
                RenderMode == Rendermode.error
                ? Text("Username taken!", Styles.AbortBtn & Styles.MP4, () => RenderMode = Rendermode.@default)
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
                            RenderMode = Rendermode.error;
                    }),
                    Text("back", Styles.Btn & Styles.MP4, () => RegisterSelected = false)
                );
        }

        private AccountEntry User(string hashsalt) => AccountProjection.Accounts.Where(p => p.Password == hashsalt && p.Username == Username).SingleOrDefault();

        private enum Rendermode
        {
            @default,
            error
        }
    }
}
