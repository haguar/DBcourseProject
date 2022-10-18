using System;
using System.Collections.Generic; 
using Npgsql;
using System.Data;

class DatabaseHandler {    

        public void createAccount(string username, string password, string firstName, 
        string lastName, string email) {
            using(NpgsqlConnection con = GetConnection()) {    
                string query = $"insert into userinfo(username,password,firstName,lastName,email)values('{username}','{password}','{firstName}','{lastName}','{email}')";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = -1;
                try {
                    n = cmd.ExecuteNonQuery();
                } catch (PostgresException e) {
                    con.Close();
                    Console.WriteLine("Duplicate Username/Emails, use a different one");
                }
                if(n==1) {
                    Console.WriteLine("Account Created");
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

        public bool checkLogin(string username, string password) {
            using(NpgsqlConnection con = GetConnection()) {
                string query = $"Select * From userinfo";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read()) {
                    if(username == (string)reader[0]) {
                        if(password == (string)reader[1]) {
                            con.Close();
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

        public void destroyDatabase() {
            using(NpgsqlConnection con = GetConnection()) {
                string query = $"DROP TABLE IF EXISTS userinfo";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();
                if(n==1) {
                    Console.WriteLine("Database Destroyed");
                }
                con.Close();
            }
        }

        public void createDatabase() {
            using(NpgsqlConnection con = GetConnection()) {
                string query = $"CREATE TABLE userinfo(userName varchar(50) primary key, password varchar(50), firstName varchar(50), lastName varchar(50), email varchar(50) UNIQUE)";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();
                if(n==1) {
                    Console.WriteLine("Database Created");
                }
                con.Close();
            }
        }
            
        
}