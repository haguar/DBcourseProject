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
                string query = $"TRUNCATE TABLE blogs, blogstags, comments, userinfo RESTART IDENTITY";
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
}