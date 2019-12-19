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


        public LoginController(AccountProjection accountProjection)
        {
            AccountProjection = accountProjection;
        }

        public VNode Render(GameboardController gameboardcontroller)
        {
            //gameboardcontroller.LoggedIn = true;
            return Div(
                Input(Username, s => Username = s),
                Input(Password, s => Password = s),
                Text("create Account", Styles.Btn, () => Account.Commands.RegisterAccount(ACL.ES.AggregateID<Account>.Create(), Username, Password)),

                Input(Username, s => Username = s),
                Input(Password, s => Password = s),
                Text("login ", Styles.Btn, () => Account.Commands.LoginAccount(User().ID, Username, Password, gameboardcontroller))
                );
        }

        private AccountEntry User() => AccountProjection.Accounts.Where(p => p.Password == Password && p.Username == Username).Single();

    }
}
