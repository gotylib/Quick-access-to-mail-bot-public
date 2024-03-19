using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Core;

namespace TelegramBot.Infrastructure.DateBase
{
    public class DBController
    {
        public void UpdateUser(ref ApplicationContext db, ref MailUser user, ref string login, ref string password)
        {
            user.mail = login;
            user.password = password;
            db.SaveChanges();
        }

        public void CreateUser(ref ApplicationContext db, ref long userId, ref string login, ref string password)
        {
            MailUser newUser = new MailUser { Id = userId, mail = login, password = password };
            db.Users.Add(newUser);
            db.SaveChanges();
        }

        public void DeleteUser(ref ApplicationContext db, ref MailUser user)
        {
            db.Users.Remove(user);
            db.SaveChanges();
        }
    }
}
