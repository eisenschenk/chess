using ACL.CSS;
using System;
using System.Collections.Generic;
using System.Text;

namespace VnodeTest
{
    public class Application : ACL.UI.Manager.Application
    {
        private object LockObject = new object();

        private AppContext _AppContext;
        public AppContext AppContext
        {
            get
            {
                // absicherung mit lock weil sonst bei gleichzeitigem mehrfachzugriff der kontext mehrfach versucht auf dieselbe datei zuzugreifen
                if (_AppContext == default)
                    lock (LockObject)
                        if (_AppContext == default)
                            _AppContext = new AppContext();
                return _AppContext;
            }
        }
        public Application(string[] args) : base(args) { }

        public override bool OnRun()
        {
            UseStyles = true;
            return true;
        }
        public override Rule[] GetCSSRules(string path) =>
    new Styles().GetRules();

        public override ACL.UI.Manager.ISimpleSession CreateSession(ACL.UI.Manager.SessionStartupInfo info)
        {
            var session = new Session(this, info);
            return session;
        }
    }

}
