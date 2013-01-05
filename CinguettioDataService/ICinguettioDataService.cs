using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using CinguettioDataService.Models;

namespace CinguettioDataService
{
    [ServiceContract]
    public interface ICinguettioDataService
    {
        [OperationContract]
        bool CreateUser(string userName, string password, string firstName, string lastName, string email, string latitude, string longitude);

        [OperationContract]
        UserModel AuthenticateUser(string userName, string password);

        [OperationContract]
        void UpdateUserPosition(int userId, decimal latitude, decimal longitude);

        [OperationContract]
        void UpdateUserProfile(int userId, string firstName, string lastName, string email);

        [OperationContract]
        IEnumerable<PostModel> GetLatestPosts(int from, int to);

        [OperationContract]
        IEnumerable<PostModel> GetLatestPostsForUser(int userId, int from, int to);

        [OperationContract]
        IEnumerable<UserModelWithPosition> GetUsersInArea(decimal latitudeFrom, decimal latitudeTo, decimal longitudeFrom, decimal longitudeTo, bool friendsOnly, int userId);

        [OperationContract]
        IEnumerable<UserModel> GetFriends(int userId, int from, int to);

        [OperationContract]
        int GetFriendsCount(int userId);

        [OperationContract]
        int GetPostsCount(int userId);

        [OperationContract]
        int GetAllPostsCount();

        [OperationContract]
        bool AddFriend(int userId, int friendId);

        [OperationContract]
        bool RemoveFriend(int userId, int friendId);

        [OperationContract]
        bool AreFriends(int userId, int friendId);

        [OperationContract]
        IEnumerable<UserModel> SearchUsers(string data);

        [OperationContract]
        void CreatePost(int userId, string title, string content);

        [OperationContract]
        PostModel GetPost(int postId);

        [OperationContract]
        UserModel GetUser(int userId);

        [OperationContract]
        UserModelWithPosition GetUserWithPosition(int userId);

        [OperationContract]
        void UpdatePost(int postId, string title, string content);
    }
}
