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
            return Div(
                Input(Username, s => Username = s),
                Input(Password, s => Password = s),
                Text("login ", Styles.Btn, () => { Account.Commands.LoginAccount(User().ID, Username, Password); rootController.AccountEntry = User(); })
            );
        }

        private VNode RenderRegisterAccount()
        {
            return Div(
                Input(Username, s => Username = s),
                Input(Password, s => Password = s),
                Text("register Account", Styles.Btn, () =>
                {
                    Account.Commands.RegisterAccount(ACL.ES.AggregateID<Account>.Create(), Username, Password);
                    Username = string.Empty;
                    Password = string.Empty;
                    RegisterSelected = false;
                })
            );
        }

        private AccountEntry User() => AccountProjection.Accounts.Where(p => p.Password == Password && p.Username == Username).Single();

    }
}
