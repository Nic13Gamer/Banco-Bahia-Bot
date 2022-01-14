using System;
using System.Collections.Generic;
using System.Linq;

namespace BancoBahiaBot
{
    public static class UserHandler
    {
        static readonly List<User> users = new();

        public static User GetUser(ulong id) => CreateUser(id);

        public static User CreateUser(ulong id)
        {
            User user = users.Where(x => x.id == id.ToString()).FirstOrDefault();
            if (user != default) return user;

            User newUser = new
                (
                    id: id.ToString(),
                    money: 0,
                    lastDaily: DateTime.Now.AddDays(-1),
                    banned: false,
                    stocks: Array.Empty<UserStock>()
                );

            users.Add(newUser);
            return newUser;
        }

        public static User[] GetUsers() => users.ToArray();
    }

    public class User
    {
        public User(string id, int money, DateTime lastDaily, bool banned, UserStock[] stocks)
        {
            this.id = id;
            this.money = money;
            this.lastDaily = lastDaily;
            this.banned = banned;

            this.stocks = stocks;
        }

        public string id;
        public int money;
        public DateTime lastDaily;
        public bool banned;

        //public UserProperty[] properties;
        //public UserItem[] items;
        public UserStock[] stocks;
    }
}
