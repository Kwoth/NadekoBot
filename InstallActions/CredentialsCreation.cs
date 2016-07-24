using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace InstallActions
{
    [RunInstaller(true)]
    public partial class CredentialsCreation : System.Configuration.Install.Installer
    {
        public CredentialsCreation()
        {
            InitializeComponent();
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            string ownerId = "";
            string botClientId = "";
            string botUserId = "";
            string token = "";
            //var dir = Environment.SpecialFolder.DesktopDirectory;
            string path = System.AppDomain.CurrentDomain.BaseDirectory;

            try
            {
                path = Context.Parameters["targetdir"] ?? "";
                ownerId = Context.Parameters["userid"] ?? "";
                botClientId = Context.Parameters["clientid"] ?? "";
                botUserId = Context.Parameters["botuserid"] ?? "";
                token = Context.Parameters["token"] ?? "";

            }
            catch (Exception)
            {
                File.WriteAllText(path + Path.DirectorySeparatorChar + "log.txt", "could not parse parameters");
            }

            ulong userid = 0;
            ulong botId = 0;

            ulong.TryParse(ownerId, out userid);
            ulong.TryParse(botUserId, out botId);
            Credentials creds = new Credentials()
            {
                OwnerIds = new[] { userid },
                Token = token,
                BotId = botId,
                ClientId = botClientId
            };

            File.WriteAllText(path + Path.DirectorySeparatorChar +"credentials.json", JsonConvert.SerializeObject(creds, Formatting.Indented));
        }
    }
}
