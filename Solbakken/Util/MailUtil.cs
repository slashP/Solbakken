using System.Net.Mail;
using Typesafe.Mailgun;

namespace Solbakken.Util
{
    public class MailUtil
    {
        private const string MyEmailAddress = "perkristianhelland@gmail.com";

        public static void SendRegisteredEmail(string emailAddress, string username)
        {

            var client = new MailgunClient("app692.mailgun.org", "key-63fneqd63xpee86sxtd07u3ofsnmzm30");
            var mail = new MailMessage {From = new MailAddress(MyEmailAddress)};
            mail.ReplyToList.Add(new MailAddress(MyEmailAddress));
            mail.To.Add(new MailAddress(emailAddress));
            mail.Body = string.Format(@"Du er nå registrert på Solbakken og kan laste opp bilder som andre kan se.
                          Siden finner du på http://solbakken.apphb.com. Ditt brukernavn er: {0} 
                            \n\nIkke la bildene støve ned på datamaskinen - vis dem fram!", username);
            mail.Subject = "Solbakken - Gratulerer, du er nå registrert på Solbakken og kan dele bilder med familien!";
            client.SendMail(mail);
        }

        public static void SendNewPassword(string password, string email, string resetLinkUrl)
        {
            var client = new MailgunClient("app692.mailgun.org", "key-63fneqd63xpee86sxtd07u3ofsnmzm30");
            var mail = new MailMessage { From = new MailAddress(MyEmailAddress) };
            mail.ReplyToList.Add(new MailAddress(MyEmailAddress));
            mail.To.Add(new MailAddress(email));
            mail.Body = string.Format(@"Passordet ditt har blitt resatt. Det nye passordet er {0}. Etter du har logget inn kan du endre det ved å gå hit: {1}", password, resetLinkUrl);
            mail.Subject = "Solbakken - Ditt passord har blitt resatt.";
            client.SendMail(mail);
        }
    }
}