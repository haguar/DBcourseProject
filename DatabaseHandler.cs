using System;
using System.Collections.Generic; 
using Npgsql;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

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
            return new NpgsqlConnection(@"Server=localhost;Port=5432;User Id=postgres; Password=barrow9;Database=postgres");
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
        public string[] getBlogsFromUser(string username)
        {
            ArrayList blogList = new ArrayList();
            using (NpgsqlConnection con = GetConnection())
            {
                string query = $"SELECT description from comments where sentiment = 'positive' AND posted_by = '{ username}'";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    blogList.Add(reader[0].ToString());
                }
                con.Close();
            }
            string[] returnable = (string[])blogList.ToArray(typeof(string));
            return returnable;
        }
        public string[] getFollowedBy(string user1, string user2) {
            ArrayList blogList = new ArrayList();
            using (NpgsqlConnection con = GetConnection())
            {
                string query = $"SELECT leadername FROM follows WHERE leadername IN (SELECT leadername FROM follows WHERE followername = '{user1}' UNION SELECT leadername FROM follows WHERE followername = '{user2}')";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    blogList.Add(reader[0].ToString());
                }
                con.Close();
            }
            blogList.RemoveAt(0);
            string[] returnable = (string[])blogList.ToArray(typeof(string));
            return returnable;
        }
        public string[] getTopCommenter()
        {
            ArrayList blogList = new ArrayList();
            using (NpgsqlConnection con = GetConnection())
            {
                string query = $"SELECT posted_by from comments GROUP BY posted_by having COUNT(posted_by) = (SELECT MAX(count) FROM (SELECT posted_by, COUNT(posted_by)  from comments GROUP BY posted_by) as mycount)";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    blogList.Add(reader[0].ToString());
                }
                con.Close();
            }
            string[] returnable = (string[])blogList.ToArray(typeof(string));
            return returnable;
        }
        public string[] getNeverPosted()
        {
            ArrayList blogList = new ArrayList();
            using (NpgsqlConnection con = GetConnection())
            {
                string query = $"SELECT username from userinfo LEFT JOIN blogs ON userinfo.username = blogs.created_by WHERE blogs.blogid is NULL"; //insert correct SELECT statement here
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    blogList.Add(reader[0].ToString());
                }
                con.Close();
            }
            string[] returnable = (string[])blogList.ToArray(typeof(string));
            return returnable;
        }
        public string[] getNoNegatives()
        {
            ArrayList blogList = new ArrayList();
            using (NpgsqlConnection con = GetConnection())
            {
                string query = $"SELECT created_by FROM blogs WHERE blogid NOT IN (SELECT blogid FROM comments WHERE sentiment = 'negative') AND blogid NOT IN (SELECT blogid from comments)"; //insert correct SELECT statement here
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    blogList.Add(reader[0].ToString());
                }
                con.Close();
            }
            string[] returnable = (string[])blogList.ToArray(typeof(string));
            return returnable;
        }
        public string[] getCommonHobbies()                      //this function needs to return a 3-part-tuple
        {
            ArrayList blogList = new ArrayList();
            using (NpgsqlConnection con = GetConnection())
            {
                string query = $"SELECT hobby, username from hobbies where hobby IN (SELECT hobby from hobbies group by hobby having COUNT(hobby) > 1) order by hobby";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (!blogList.Contains(reader[0].ToString())) { 
                        blogList.Add("^");
                        blogList.Add(reader[0].ToString()); ; 
                    }
                    blogList.Add(reader[1].ToString());
                }
                while (reader.Read()) { blogList.Add(reader[0].ToString()); }
                con.Close();
            }
            string[] returnable = (string[])blogList.ToArray(typeof(string));
            return returnable;
        }
}