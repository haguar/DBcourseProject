using System;
using System.Collections.Generic; 
using Npgsql;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Linq;

public class DatabaseHandler {    
    private string userName;
        public void createAccount(string username, string password, string firstName, 
        string lastName, string email, Label messageBoard) {
            using(NpgsqlConnection con = GetConnection()) {    
                string query = $"insert into userinfo(username,password,firstName,lastName,email)values('{username}','{password}','{firstName}','{lastName}','{email}')";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = -1;
                try {
                    n = cmd.ExecuteNonQuery();
                } catch (PostgresException e) {
                    con.Close();
                    messageBoard.Text = "Duplicate Username/Emails, use a different one";
                }
                if(n==1) {
                    messageBoard.Text = "Account Created";
                }
                con.Close();
            }
        }

        public void TestConnection() {
            using(NpgsqlConnection con = GetConnection()) {
                con.Open();
                if(con.State==ConnectionState.Open) {
                    Console.WriteLine("Connected");
                }
            }
        }

        private NpgsqlConnection GetConnection() {
            return new NpgsqlConnection(@"Server=localhost;Port=5432;User Id=postgres; Password=comp586;Database=projdb");
        }

        public bool checkLogin(string tempUsername, string password) {
            using(NpgsqlConnection con = GetConnection()) {
                string query = $"Select * From userinfo";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read()) {
                    if(tempUsername == (string)reader[0]) {
                        if(password == (string)reader[1]) {
                            con.Close();
                            userName = tempUsername;
                            return true;
                        } else {
                            con.Close();
                            return false;
                        }
                    }
                }
                con.Close();
            }
            return false;
        }

        public void resetDatabase() {
            using(NpgsqlConnection con = GetConnection()) {
                string query = $"TRUNCATE TABLE blogs, blogstags, comments, userinfo, follows, hobbies RESTART IDENTITY";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        //methods for part 2
        public void createBlog(string subject, string description, string tags, Label messageBoard) {

            DateTime dt = DateTime.Today;
            //checks if blogs have been posted twice already today
            using(NpgsqlConnection con = GetConnection()) {
                string query = $"Select * From blogs";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                int counter = 0;
                while (reader.Read()) {
                    if(userName == (string)reader[4]) {
                        if(dt.ToString() == reader[3].ToString()) {
                            counter++;
                        }
                    }
                    if(counter == 2) {
                        messageBoard.Text = "Already posted 2 blogs today";
                        Console.WriteLine("Already posted twice");
                        con.Close();
                        return;
                    }
                }
                con.Close();
            }

            //adds blog
            using(NpgsqlConnection con = GetConnection()) {
                string query = $"insert into blogs(subject,description,pdate,created_by)values('{subject}','{description}','{dt}','{userName}')";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();
                if(n==1) {
                    messageBoard.Text = "Blog Posted";
                    Console.WriteLine("Blog posted");

                }
                con.Close();
            }

            using(NpgsqlConnection con = GetConnection()) {
                string query = $"insert into blogstags(tag)values('{tags}')";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();
                if(n==1) {
                    Console.WriteLine("Tags Posted");
                }
                con.Close();
            }
        }
        
        public void createComments(string sentiment, string description, int blogid, Label messageBoard) {
            DateTime dt = DateTime.Today;
            
            //checks conditionals for posting a comment
            using(NpgsqlConnection con = GetConnection()) {
                //checks if user is trying to post on their own blog
                string query = $"Select * From blogs WHERE blogid = {blogid}";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if(reader.HasRows) {
                    reader.Read();
                    if((string)reader[4] == userName) {
                        messageBoard.Text = "Can't post on your own blog";
                        Console.WriteLine("Can't post on your own blog");
                        con.Close();
                        return;
                    }
                }
                con.Close();
                query = $"Select * From comments";
                cmd = new NpgsqlCommand(query, con);
                con.Open();
                reader = cmd.ExecuteReader();
                int counter = 0;
                while (reader.Read()) {
                    //checks if you've posted a comment on this blog already
                    if(userName == (string)reader[5]) {
                        if(blogid == (int)reader[4]) {
                            messageBoard.Text = "Already posted a comment on this blog";
                            Console.WriteLine("Already posted on this");
                            con.Close();
                            return;
                        }
                    }                    

                    //checks if you've posted 3 comments already
                    if(userName == (string)reader[5]) {
                        if(dt.ToString() == reader[3].ToString()) {
                            counter++;
                        }
                    }
                    if(counter == 3) {
                        messageBoard.Text = "Already posted 3 comments today";
                        Console.WriteLine("Already posted 3 times today");
                        con.Close();
                        return;
                    }
                }
                con.Close();
            }

            //adds comment
            using(NpgsqlConnection con = GetConnection()) {
                string query = $"insert into comments(sentiment,description,cdate,blogid,posted_by)values('{sentiment}','{description}','{dt}','{blogid}','{userName}')";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();
                if(n==1) {
                    messageBoard.Text = "Comment Posted";
                    Console.WriteLine("Comment posted");
                }
                con.Close();
            }
        }

        public string[] getBlogs() {
            ArrayList blogList = new ArrayList();
            using(NpgsqlConnection con = GetConnection()) {
                string query = $"SELECT * From blogs";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    blogList.Add(reader[0].ToString());
                }
                con.Close();
            }
            string[] returnable = (string[])blogList.ToArray(typeof(string));
            return returnable;
        }

        //phase 3
        public void listOfPositiveBlogs(string user, Label messageBoard) {
            ArrayList blogIds = new ArrayList();

            //gets all the blog ids of blogs posted by user X
            using(NpgsqlConnection con = GetConnection()) {
                string query = $"SELECT * From blogs WHERE created_by = {user}";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    blogIds.Add(reader[0].ToString());
                }
                con.Close();
            }

            //checks if all comments for each blog prev found is positive
            string blogIdMessage = "The Following Are All Positive Blog Ids: ";
            using(NpgsqlConnection con = GetConnection()) {
                bool allPos = true;
                for(int i = 0; i < blogIds.Count; i++) {
                    allPos = true;
                    string query = $"SELECT * From comments WHERE blogid = {blogIds[i]}";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                    con.Open();
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read()) {
                        if(reader[1].ToString() == "negative") {
                            allPos = false;
                        }
                    }
                    if(allPos) {
                        blogIdMessage += blogIds[i];
                    }
                    con.Close();
                }
            }
            messageBoard.Text = blogIdMessage;
        }

        //gets user(s) with most number of comments
        public void mostNumberOfComments(Label messageBoard) {
            ArrayList mostComments = new ArrayList();
            Dictionary<string, int> commentsTracker = new Dictionary<string, int>();
            using(NpgsqlConnection con = GetConnection()) {
                string query = $"SELECT * From comments";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    commentsTracker.Add(reader[5].ToString(), commentsTracker[reader[5].ToString()] + 1);
                }
                con.Close();
            }
            int max = -1;
            foreach(var key in commentsTracker) {
                if(key.Value > max) {
                    mostComments.Clear();
                    mostComments.Add(key);
                    max = key.Value;
                } else if(key.Value == max) {
                    mostComments.Add(key);
                }
            }

            string message = "User(s) With the Most Comments: ";
            for(int i = 0; i < mostComments.Count; i++) {
                message += mostComments[i] + ", ";
            }
            messageBoard.Text = message;
        }

        //4
        public void neverPostedBlog(Label messageBoard) {
            //username
            ArrayList listOfUsers = new ArrayList();
            using(NpgsqlConnection con = GetConnection()) {
                string query = $"SELECT * From userinfo";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    listOfUsers.Add(reader[0]);    
                }
                con.Close();
            }
            for(int i = 0; i < listOfUsers.Count; i++) {
                using(NpgsqlConnection con = GetConnection()) {
                    string query = $"SELECT * From blogs WHERE created_by = {listOfUsers[i]}";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                    con.Open();
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    if(reader.HasRows) {
                        listOfUsers.Remove(listOfUsers[i]);
                    }
                    con.Close();
                }
            }

            string message = "Users Who've Never Posted A Blog: ";
            for(int i = 0; i < listOfUsers.Count; i++) {
                message += listOfUsers[i] + ", ";
            }
            messageBoard.Text = message;
        }

        //All blogs are positive Comments 5
        public void listOfUsersWithAllPositiveComments(Label messageBoard) {
            //grab all users

            //for each user, grab an arraylist with all of their blogids

            //store it in a dictionary

            //traverse through each arraylist, if no negatives are found, include it in returnable
            ArrayList users = new ArrayList();
            using(NpgsqlConnection con = GetConnection()) {
                string query = $"SELECT * From userinfo";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    users.Add(reader[0].ToString());
                }
                con.Close();
            }

            Dictionary<string, object> userBlogsList = new Dictionary<string, object>();
            for(int i = 0; i < users.Count; i++) {
                using(NpgsqlConnection con = GetConnection()) {
                    ArrayList temp = new ArrayList();
                    string query = $"SELECT * From blogs WHERE created_by = {users[i]}";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                    con.Open();
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read()) {
                        temp.Add(reader[0].ToString());
                    }
                    con.Close();
                    userBlogsList.Add(users[i].ToString(),temp);
                }
            }

            ArrayList answer = new ArrayList();
            foreach(var user in userBlogsList) {
                ArrayList temp = (ArrayList)user.Value;
                for(int i = 0; i < temp.Count; i++) {
                    bool isAllPos = true;
                    using(NpgsqlConnection con = GetConnection()) {
                        string query = $"SELECT * From comments WHERE blogid = {temp[i]}";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                        con.Open();
                        NpgsqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read()) {
                            if(reader[1].ToString() == "negative") {
                                isAllPos = false;
                            }
                        }
                        con.Close();
                    }
                    if(isAllPos) {
                        answer.Add(user.Key);
                    }
                }
            }
            string message = "Users Who've Never Had A Negative Comment: ";
            for(int i = 0; i < answer.Count; i++) {
                message += answer[i] + ", ";
            }
            messageBoard.Text = message;
        }

        //All users followed by both 
        public void followedByBoth(string userOne, string userTwo, Label messageBoard) {
            ArrayList listOfUsers = new ArrayList();
            using(NpgsqlConnection con = GetConnection()) {
                string query = $"SELECT leadername From follows WHERE followername = {userOne}";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    listOfUsers.Add(reader[0].ToString());
                }
                con.Close();
            }
            for(int i = 0; i < listOfUsers.Count; i++) {
                using(NpgsqlConnection con = GetConnection()) {
                    string query = $"SELECT * From follows WHERE leadername = {listOfUsers[i]} AND followername = {userTwo}";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                    con.Open();
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    if(!(reader.HasRows)) {
                        listOfUsers.RemoveAt(i);
                        i--;
                    }
                con.Close();
                }
            }
            
            string message = $"Users Who're Followed By Both {userOne} and {userTwo}: ";
            for(int i = 0; i < listOfUsers.Count; i++) {
                message += listOfUsers[i] + ", ";
            }
            messageBoard.Text = message;
        }

        //returns the pair of users with the same hobby, and the hobby 6
        public void pairOfSameHobby(Label messageBoard) {
            //get all users

            //store all hobbies of each user in an arraylist

            //store everything in a dictionary

            //brute force solution, nested for loop with inner loop starting at i + 1

            //store answers in an arraylist, with user1 at index 0, user2 at index 1, hobby at 2
            ArrayList users = new ArrayList();
            using(NpgsqlConnection con = GetConnection()) {
                string query = $"SELECT * From userinfo";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    users.Add(reader[0].ToString());
                }
                con.Close();
            }

            Dictionary<string, object> userHobbyList = new Dictionary<string, object>();
            for(int i = 0; i < users.Count; i++) {
                using(NpgsqlConnection con = GetConnection()) {
                    ArrayList temp = new ArrayList();
                    string query = $"SELECT * From hobbies WHERE username = {users[i]}";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                    con.Open();
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read()) {
                        temp.Add(reader[1].ToString());
                    }
                    con.Close();
                    userHobbyList.Add(users[i].ToString(),temp);
                }
            }

            ArrayList answer = new ArrayList();
            for(int i = 0; i < userHobbyList.Count; i++) {
                ArrayList tempOne = (ArrayList)userHobbyList.ElementAt(i).Value;
                for(int j = i + 1; j < userHobbyList.Count; j++) {
                    ArrayList tempTwo = (ArrayList)userHobbyList.ElementAt(j).Value;
                    //have 2 arraylists, need to compare them for similar hobbies
                    for(int z = 0; z < tempOne.Count; z++) {
                        if(tempTwo.Contains(tempOne[z])) {
                            answer.Add(userHobbyList.ElementAt(i).Key);
                            answer.Add(userHobbyList.ElementAt(j).Key);
                            answer.Add(tempOne[z]);
                        }
                    }
                }
            }
            string message = "Users who have similar hobbies: ";
            for(int i = 0; i < answer.Count; i+=3) {
                message += answer[i] + ", ";
                message += answer[i+1] + ", ";
                message += answer[i+2] += "; ";
            }
            messageBoard.Text = message;
        }
}