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
                string query = $"TRUNCATE TABLE blogs, blogstags, comments, userinfo, follows, hobbies RESTART IDENTITY; INSERT INTO userinfo VALUES ('batman','1234','bat','bat','nananana@batman.com'),('bob','12345','bob','bob','bobthatsme@yahoo.com'),('catlover','abcd','cat','cat','catlover@whiskers.com'),('doglover','efds','dog','dog','doglover@bark.net'),('jdoe','25478','joe','jod','jane@doe.com'),('jsmith','1111','john','smith','jsmith@gmail.com'),('matty','2222','mat','mat','matty@csun.edu'),('notbob','5555','not','bob','stopcallingmebob@yahoo.com'),('pacman','9999','pacman','pacman','pacman@gmail.com'),('scooby','8888','scoby','scoby','scooby@doo.net'); ";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();
                con.Close();
            }
            using (NpgsqlConnection con = GetConnection())
            {
                string query = $"INSERT INTO blogs (subject, description, pdate, created_by) VALUES ('Hello World','Hey everyone, this is my first blog. Hello world and all who inhabit it!','2020-03-15','jsmith'),('I love cats!','Catsare amazing. They\''re awesome, and fuzzy, and cute. Who DOESN\''T love cats?','2020-03-17','catlover'),('Dogs are the best.','So I saw a post the other day talking about cats. Now, I love cats. They\''re great. But here\''s the thing: dogs are just the best, okay? There\''s no question about it. That is all.','2020-03-19','doglover'),('I am the night.','To all you lowly criminals out there, this isa warning to know I am watching. I am justice. I am righteousness. I am the NIGHT.','2020-03-24','batman'),('Waka waka','waka waka waka waka waka waka waka waka waka waka waka waka waka waka waka waka','2020-03-31','pacman'),('Who is this Bob guy?','Decided to start tracking down this mysterious human known as \''Bob.\'' Who is Bob? What does he do?','2020-04-02','notbob'),('Re: I love cats.','A reader recently reachtto me about my last post.','2020-04-04','catlover'),('Scooby Dooby Doo!','The search for scooby snacks: Where did they go? I know this whole quarantine thing is affecting businesses, but aren\''t scooby snacks counted as an essential service?','2020-04-05','scooby'),('Bob Update','Dearreaders, I know you have been waiting anxiously for an update on Bob, but there isnot much to share so far.He appears to have little to no online presence.','2020-04-06','notbob'),('Lizard People.','What are your guys\'' thoughts on them ? I, forone, welcome out reptitlian overlords.','2020-04-12','jdoe'); ";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();
                con.Close();
            }
            using (NpgsqlConnection con = GetConnection())
            {
                string query = $"INSERT INTO comments (sentiment, description, cdate, blogid, posted_by) VALUES ('negative','Cats are cool and all, but dogs are where it\''s at.','2020-03-17',2,'doglover'),('negative','What\''s all the hypeabout? Cats are clearly superior.','2020-03-20',3,'catlover'),('positive', 'Nice.', '2020-03-20', 4, 'scooby'),('positive', 'Who IS Bob? I can\''twait to find out.','2020-04-02',6,'jsmith'),('negative','I guess cat people justdon\''t know what they\''re missing.','2020-04-05',7,'doglover'),('positive','Thisis totally unrelated, but I just wanted to say I am a HUGE fan of yours.I loveyour work!','2020-04-05',8,'doglover'),('positive','Have you checked out Dog -Mart ? They\''ve got everything.','2020-04-06',8,'matty'),('negative','I was hopingthere\''d be more of an update. Sorry, Bob.','2020-04-07',9,'jsmith'),('positive', 'I think they\''re all secretly cats.Open your eyes, sheeple!','2020-04-13',10,'doglover'),('negative','Who ? Me ? Multimillionaire philanthropist ofArkham ? A lizard person ? Nope.Nothing to see here!','2020-04-15',10,'batman');";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();
                con.Close();
            }
            using(NpgsqlConnection con = GetConnection())
            {
                string query = $"INSERT INTO blogstags (tag) VALUES ('hello world'),('animals'),('cats'),('animals'),('dogs'),('crime'),('justice'),('cartoon'),('waka'),('bob');";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();
                con.Close();
            }
            using (NpgsqlConnection con = GetConnection())
            {
                string query = $"INSERT INTO follows (leadername, followername) VALUES ('jsmith','bob'),('batman','catlover'),('doglover', 'catlover'),('pacman', 'catlover'),('catlover', 'doglover'),('jsmith', 'jdoe'),('bob', 'notbob'),('jdoe', 'notbob'),('batman', 'pacman'),('scooby', 'pacman'),('doglover', 'scooby'),('pacman', 'scooby'); ";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();
                con.Close();
            }
            using (NpgsqlConnection con = GetConnection())
            {
                string query = $"INSERT INTO hobbies (username, hobby) VALUES ('batman','movie'),('bob','movie'),('catlover', 'movie'),('doglover', 'hiking'),('jdoe', 'dancing'),('jdoe', 'movie'),('jsmith', 'hiking'),('matty', 'bowling'),('notbob', 'calligraphy'),('pacman', 'dancing'),('pacman', 'movie'),('scooby', 'cooking');";
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
                string query = $"SELECT subject FROM blogs WHERE blogid IN (SELECT comments.blogid from comments INNER JOIN blogs ON blogs.blogid = comments.blogid WHERE comments.blogid NOT IN (SELECT blogid from comments WHERE sentiment = 'negative')) AND created_by = '{username}';";
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
                string query = $"SELECT leadername FROM follows WHERE leadername IN (SELECT leadername FROM follows WHERE followername = '{user1}' INTERSECT SELECT leadername FROM follows WHERE followername = '{user2}')";
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
                string query = $"SELECT created_by FROM blogs WHERE created_by NOT IN (SELECT created_by FROM blogs WHERE blogid IN (SELECT blogid FROM comments WHERE sentiment = 'negative'))"; //insert correct SELECT statement here
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