using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using CinguettioDataService.Data;
using CinguettioDataService.Models;
using System.ServiceModel.Activation;

namespace CinguettioDataService
{
    public class CinguettioDataService : ICinguettioDataService
    {
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public bool CreateUser(string userName, string password, string firstName, string lastName, string email, string latitude, string longitude)
        {
            try
            {
                decimal? latitudeDecimal = null;
                if (!string.IsNullOrEmpty(latitude))
                {
                    latitudeDecimal = decimal.Parse(latitude, System.Globalization.CultureInfo.InvariantCulture);
                }

                decimal? longitudeDecimal = null;
                if (!string.IsNullOrEmpty(longitude))
                {
                    longitudeDecimal = decimal.Parse(longitude, System.Globalization.CultureInfo.InvariantCulture);
                }

                CinguettioDBEntities context = new CinguettioDBEntities();
                context.AddToUsers(new User()
                {
                    UserName = userName,
                    Password = password,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Latitude = latitudeDecimal,
                    Longitude = longitudeDecimal
                });
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public Models.UserModel AuthenticateUser(string userName, string password)
        {
            CinguettioDBEntities context = new CinguettioDBEntities();
            User user = context.Users.FirstOrDefault(u => u.UserName == userName && u.Password == password);

            if (user == null)
            {
                return null;
            }
            return new Models.UserModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public void UpdateUserPosition(int userId, decimal latitude, decimal longitude)
        {
            CinguettioDBEntities context = new CinguettioDBEntities();
            User user = context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return;
            }

            user.Latitude = latitude;
            user.Longitude = longitude;

            context.SaveChanges();
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public void UpdateUserProfile(int userId, string firstName, string lastName, string email)
        {
            CinguettioDBEntities context = new CinguettioDBEntities();
            User user = context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(firstName) && firstName != user.FirstName)
            {
                user.FirstName = firstName;
            }

            if (!string.IsNullOrEmpty(lastName) && lastName != user.LastName)
            {
                user.LastName = lastName;
            }

            if (!string.IsNullOrEmpty(email) && email != user.Email)
            {
                user.Email = email;
            }

            context.SaveChanges();
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public IEnumerable<Models.PostModel> GetLatestPosts(int from, int to)
        {
            CinguettioDBEntities context = new CinguettioDBEntities();

            return context.Posts.Include("User")
                .OrderByDescending(p => p.DateCreated).Skip(from).Take(to - from + 1)
                .Select(p => new PostModel()
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    DateCreated = p.DateCreated,
                    User = new UserModel()
                    {
                        Id = p.User.Id,
                        FirstName = p.User.FirstName,
                        LastName = p.User.LastName,
                        UserName = p.User.UserName,
                        Email = p.User.Email
                    }
                });
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public IEnumerable<Models.PostModel> GetLatestPostsForUser(int userId, int from, int to)
        {
            CinguettioDBEntities context = new CinguettioDBEntities();

            return context.Posts.Include("User")
                .Where(p => p.UserId == userId).OrderByDescending(p => p.DateCreated).Skip(from).Take(to - from + 1)
                .Select(p => new PostModel()
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    DateCreated = p.DateCreated,
                    User = new UserModel()
                    {
                        Id = p.User.Id,
                        FirstName = p.User.FirstName,
                        LastName = p.User.LastName,
                        UserName = p.User.UserName,
                        Email = p.User.Email
                    }
                });
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public IEnumerable<Models.UserModelWithPosition> GetUsersInArea(decimal latitudeFrom, decimal latitudeTo, decimal longitudeFrom, decimal longitudeTo, bool friendsOnly, int userId)
        {
            CinguettioDBEntities context = new CinguettioDBEntities();
            IQueryable<User> users = context.Users;
            if (friendsOnly)
            {
                users = users.Where(u => u.HalfFriends.Any(usr => usr.Id == userId));
            }

            return users.Where(u =>u.Longitude != null && u.Latitude != null &&
                u.Latitude >= latitudeFrom && u.Latitude <= latitudeTo &&
                u.Longitude >= longitudeFrom && u.Longitude <= longitudeTo)
                .Select(u => new UserModelWithPosition()
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    UserName = u.UserName,
                    Email = u.Email,
                    Latitude = u.Latitude.Value,
                    Longitude = u.Longitude.Value
                });
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public IEnumerable<Models.UserModel> GetFriends(int userId, int from, int to)
        {
            CinguettioDBEntities context = new CinguettioDBEntities();
            User user = context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            return user.Friends.OrderBy(u => u.UserName).Skip(from).Take(to - from + 1)
                .Select(u => new UserModel()
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    UserName = u.UserName,
                    Email = u.Email
                });
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public int GetFriendsCount(int userId)
        {
            CinguettioDBEntities context = new CinguettioDBEntities();
            User user = context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return 0;
            }

            return user.Friends.Count();
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public int GetPostsCount(int userId)
        {
            CinguettioDBEntities context = new CinguettioDBEntities();
            User user = context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return 0;
            }

            return user.Posts.Count;
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public int GetAllPostsCount()
        {
            CinguettioDBEntities context = new CinguettioDBEntities();

            return context.Posts.Count();
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public bool AddFriend(int userId, int friendId)
        {
            try
            {
                CinguettioDBEntities context = new CinguettioDBEntities();
                User user = context.Users.FirstOrDefault(u => u.Id == userId);

                if (user == null)
                {
                    return false;
                }

                User friend = context.Users.FirstOrDefault(u => u.Id == friendId);

                if (friend == null)
                {
                    return false;
                }

                user.Friends.Add(friend);
                context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public bool RemoveFriend(int userId, int friendId)
        {
            try
            {
                CinguettioDBEntities context = new CinguettioDBEntities();
                User user = context.Users.FirstOrDefault(u => u.Id == userId);

                if (user == null)
                {
                    return false;
                }

                User friend = user.Friends.FirstOrDefault(u => u.Id == friendId);

                if (friend == null)
                {
                    return false;
                }

                user.Friends.Remove(friend);
                context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public bool AreFriends(int userId, int friendId)
        {
            CinguettioDBEntities context = new CinguettioDBEntities();

            return context.Users.Any(u => u.Id == userId && u.Friends.Any(f => f.Id == friendId));
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public IEnumerable<Models.UserModel> SearchUsers(string data)
        {
            CinguettioDBEntities context = new CinguettioDBEntities();

            return context.Users.Where(u => u.FirstName.StartsWith(data) || u.Email.StartsWith(data)
                || u.UserName.StartsWith(data) || u.LastName.StartsWith(data))
                .Select(u => new UserModel()
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    UserName = u.UserName,
                    Email = u.Email
                });
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public void CreatePost(int userId, string title, string content)
        {
            CinguettioDBEntities context = new CinguettioDBEntities();

            context.AddToPosts(new Post()
            {
                UserId = userId,
                Title = title,
                Content = content,
                DateCreated = DateTime.UtcNow
            });

            context.SaveChanges();
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public Models.PostModel GetPost(int postId)
        {
            CinguettioDBEntities context = new CinguettioDBEntities();

            Post post = context.Posts.Include("User").FirstOrDefault(p => p.Id == postId);
            if (post == null)
            {
                return null;
            }

            return new PostModel()
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                DateCreated = post.DateCreated,
                User = new UserModel()
                {
                    Id = post.User.Id,
                    FirstName = post.User.FirstName,
                    LastName = post.User.LastName,
                    UserName = post.User.UserName,
                    Email = post.User.Email
                }
            };
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public Models.UserModel GetUser(int userId)
        {
            CinguettioDBEntities context = new CinguettioDBEntities();

            User user = context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return null;
            }

            return new UserModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public UserModelWithPosition GetUserWithPosition(int userId)
        {
            CinguettioDBEntities context = new CinguettioDBEntities();

            User user = context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return null;
            }

            if (!user.Latitude.HasValue || !user.Longitude.HasValue)
            {
                return null;
            }

            return new UserModelWithPosition()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                Latitude = user.Latitude.Value,
                Longitude = user.Longitude.Value
            };
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public void UpdatePost(int postId, string title, string content)
        {
            CinguettioDBEntities context = new CinguettioDBEntities();

            Post post = context.Posts.FirstOrDefault(p => p.Id == postId);
            if (post == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(title) && post.Title != title)
            {
                post.Title = title;
            }

            if (!string.IsNullOrEmpty(content) && post.Content != content)
            {
                post.Content = content;
            }

            context.SaveChanges();
        }
    }
}
