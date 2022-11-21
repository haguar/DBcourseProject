using System;
using System.Windows.Forms;
using System.Drawing;

public partial class GUIHandler : Form {
    private FlowLayoutPanel flowPanel;
    private Label text;
    private TextBox textBox;
    private Label loginPasswordLabel;
    private TextBox loginPasswordBox;
    private Button button;
    private Label createUsernameText;
    private TextBox createUsernameBox;
    private Label createPasswordLabel;
    private TextBox createPasswordBox;
    private Label confirmPasswordLabel;
    private TextBox confirmPasswordBox;
    private Label createEmailLabel;
    private TextBox createEmailBox;
    private Label createFirstNameLabel;
    private TextBox createFirstNameBox;
    private Label createLastNameLabel;
    private TextBox createLastNameBox;
    private Button button2;
    private Button button3;
    private Label messageBoard;
    private DatabaseHandler database;
    public GUIHandler(DatabaseHandler database) {
        this.database = database;
        createGUI();
    }

    private void createGUI()
        {
            Text = "Comp440 Project";
            ClientSize = new Size(800, 450);
            flowPanel = new FlowLayoutPanel();

            flowPanel.Dock = DockStyle.Fill;
            flowPanel.BorderStyle = BorderStyle.FixedSingle;

            text = new Label();
            text.Text = "Username ";
            textBox = new TextBox();
            loginPasswordLabel = new Label();
            loginPasswordLabel.Text = "Password ";
            loginPasswordBox = new TextBox();

            button = new Button();
            button.Text = "Login";
            button.AutoSize = true;
            button.Click += new System.EventHandler(loginAccountOnClick);            

            createUsernameText = new Label();
            createUsernameText.Text = "Username ";
            createUsernameBox = new TextBox();
            createPasswordLabel = new Label();
            createPasswordLabel.Text = "Password ";
            createPasswordBox = new TextBox();
            confirmPasswordLabel = new Label();
            confirmPasswordLabel.Text = "Confirm Password ";
            confirmPasswordLabel.AutoSize = true;
            confirmPasswordBox = new TextBox();
            createEmailLabel = new Label();
            createEmailLabel.Text = "Email ";
            createEmailLabel.AutoSize = true;
            createEmailBox = new TextBox();
            createFirstNameLabel = new Label();
            createFirstNameLabel.Text = "First Name";
            createFirstNameBox = new TextBox();
            createLastNameLabel = new Label();
            createLastNameLabel.Text = "Last Name";
            createLastNameBox = new TextBox();

            button2 = new Button();
            button2.Text = "Create Account";
            button2.AutoSize = true;
            button2.Click += new System.EventHandler(createAccountOnClick);

            button3 = new Button();
            button3.Text = "Re-Initalize Database";
            button3.AutoSize = true;
            button3.Click += new System.EventHandler(reCreateDatabaseOnClick);
            //button3.Location = new Point(300, 300);
            
            messageBoard = new Label();
            messageBoard.AutoSize = true;

            flowPanel.Controls.Add(text);
            flowPanel.Controls.Add(textBox);
            flowPanel.Controls.Add(loginPasswordLabel);
            flowPanel.Controls.Add(loginPasswordBox);
            flowPanel.Controls.Add(button);

            flowPanel.Controls.Add(createUsernameText);
            flowPanel.Controls.Add(createUsernameBox);
            flowPanel.Controls.Add(createPasswordLabel);
            flowPanel.Controls.Add(createPasswordBox);
            flowPanel.Controls.Add(confirmPasswordLabel);
            flowPanel.Controls.Add(confirmPasswordBox);
            flowPanel.Controls.Add(createEmailLabel);
            flowPanel.Controls.Add(createEmailBox);
            flowPanel.Controls.Add(createFirstNameLabel);
            flowPanel.Controls.Add(createFirstNameBox);
            flowPanel.Controls.Add(createLastNameLabel);
            flowPanel.Controls.Add(createLastNameBox);
            flowPanel.Controls.Add(button2);
            flowPanel.Controls.Add(button3);
            flowPanel.Controls.Add(messageBoard);
            Controls.Add(flowPanel);
            CenterToScreen();
        }

        private void loginAccountOnClick(object sender, EventArgs e) {
            string username = textBox.Text;
            string password = loginPasswordBox.Text;
            if(username.Length == 0 || password.Length == 0) {
                messageBoard.Text = "Empty Fields Not Allowed";
                return;
            }
            if(database.checkLogin(username, password) == true) {
                messageBoard.Text = "Login Successful";
                Blog blog = new Blog(database);
                Hide();
                blog.Show();
            } else {
                messageBoard.Text = "Login Fail";
            }        
        }

        private void createAccountOnClick(object sender, EventArgs e) {
            string username = createUsernameBox.Text;
            string password = createPasswordBox.Text;
            string confirmedPassword = confirmPasswordBox.Text;
            string firstName = createFirstNameBox.Text;
            string lastName = createLastNameBox.Text;
            string email = createEmailBox.Text;
            if(username.Length == 0 || password.Length == 0 || confirmedPassword.Length == 0 || 
            firstName.Length == 0 || lastName.Length == 0 || email.Length == 0) {
                messageBoard.Text = "Empty Fields Not Allowed";
                return;
            }
            if(password != confirmedPassword) {
                messageBoard.Text = "Passwords do not match";
                return;
            }
            database.createAccount(username, password, firstName, lastName, email, messageBoard);
        }

        private void reCreateDatabaseOnClick(object sender, EventArgs e) {
            database.destroyDatabase();
            database.createDatabase();
            messageBoard.Text = "Database Recreated";
        }
}