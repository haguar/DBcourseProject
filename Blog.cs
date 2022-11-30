using System;
using System.Windows.Forms;
using System.Drawing;
using System.Data;

public partial class Blog : Form {
    private FlowLayoutPanel flowPanel;
    private Button buttonAdd;
    private Label messageBoard;
    private ComboBox blogSelectionBox;
    private ComboBox commentBox;
    private Label commentText;
    private TextBox commentTextBox;
    private Button buttonPostComment;
    private DatabaseHandler database;
    private DataTable dt = new DataTable();
    private DataGridView dataGridView1 = new DataGridView();
    //phase 3 features
    private Button blogList;
    private Label usernameBlogListLabel;
    private TextBox userBlogList;
    private Button topCommenter;
    private Button followedBy;
    private TextBox followedByX;
    private TextBox followedByY;
    private Button neverPosted;
    private Button noNegatives;
    private Button commonHobby;

    public Blog(DatabaseHandler database) {
        this.database = database;
        createGUI();
    }
    private void blogListOnClick(object sender, EventArgs e) {
        string usernameToGetBlogsFor = userBlogList.Text;
        database.getBlogsFromUser(usernameToGetBlogsFor);
    }
    private void topCommenterOnClick(object sender, EventArgs e) { }
    private void followedByOnClick(object sender, EventArgs e) { }
    private void neverPostedOnClick(object sender, EventArgs e) { }
    private void noNegativesOnClick(object sender, EventArgs e) { }
    private void commonHobbyOnClick(object sender, EventArgs e) { }

    private void createGUI()
    {

        buttonAdd = new Button();
        buttonAdd.Text = "Add Blog";
        buttonAdd.AutoSize = true;
        buttonAdd.Click += new System.EventHandler(addBlogOnClick);

        Text = "Blog";
        ClientSize = new Size(800, 450);
        flowPanel = new FlowLayoutPanel();

        flowPanel.Dock = DockStyle.Fill;
        flowPanel.BorderStyle = BorderStyle.FixedSingle;

        messageBoard = new Label();
        messageBoard.AutoSize = true;

        blogSelectionBox = new ComboBox();
        blogSelectionBox.Text = "Select a Blog";
        blogSelectionBox.Items.AddRange(database.getBlogs());
        //add the blogs created on the dropdown

        commentBox = new ComboBox();
        commentBox.Text = "Add a Comment";
        string[] installs = new string[] { "positive", "negative"};
        commentBox.Items.AddRange(installs);

        commentText = new Label();
        commentText.Text = "Comment Description:";
        commentText.AutoSize = true;
        commentTextBox = new TextBox();

        buttonPostComment = new Button();
        buttonPostComment.Text = "Post";
        buttonPostComment.AutoSize = true;
        buttonPostComment.Click += new System.EventHandler(postCommentOnClick);

        //fill out columns of the datagridview
        dt.Columns.Add("Username", typeof(string));
        dt.Columns.Add("Blogs", typeof(string));
        dt.Columns.Add("Author", typeof(string));

        dt.Rows.Add(new object[] { "Book A", DateTime.Parse("1/1/2016"), "Author A" });
        dt.Rows.Add(new object[] { "Book B", DateTime.Parse("1/2/2016"), "Author B" });
        dt.Rows.Add(new object[] { "Book C", DateTime.Parse("1/3/2016"), "Author C" });
        dataGridView1.DataSource = dt;
        dataGridView1.Size = new Size(500, 250);

        blogList = new Button();
        blogList.Text = "List Blogs";
        blogList.AutoSize = true;
        blogList.Click += new System.EventHandler(blogListOnClick);
        usernameBlogListLabel = new Label();
        usernameBlogListLabel.Text = "Username: ";
        usernameBlogListLabel.AutoSize = true;
        userBlogList = new TextBox();

        topCommenter = new Button();
        topCommenter.Text = "Show Top Commenter";
        topCommenter.AutoSize = true;
        topCommenter.Click += new System.EventHandler(topCommenterOnClick);

        followedBy = new Button();
        followedBy.Text = "Follows";
        followedBy.AutoSize = true;
        followedBy.Click += new System.EventHandler(followedByOnClick);

        neverPosted = new Button();
        neverPosted.Text = "Users Who Haven't Posted";
        neverPosted.AutoSize = true;
        neverPosted.Click += new System.EventHandler(neverPostedOnClick);

        noNegatives = new Button();
        noNegatives.Text = "Users with no Negative Blog comments";
        noNegatives.AutoSize = true;
        noNegatives.Click += new System.EventHandler(noNegativesOnClick);

        commonHobby = new Button();
        commonHobby.Text = "Show Users with Common Hobby";
        commonHobby.AutoSize = true;
        commonHobby.Click += new System.EventHandler(commonHobbyOnClick);

        flowPanel.Controls.Add(buttonAdd);
        flowPanel.Controls.Add(blogSelectionBox);
        flowPanel.Controls.Add(commentBox);
        flowPanel.Controls.Add(commentText);
        flowPanel.Controls.Add(commentTextBox);
        flowPanel.Controls.Add(buttonPostComment);
        //add phase 3 buttons to flowpanel
        flowPanel.Controls.Add(dataGridView1);
        flowPanel.Controls.Add(blogList);
        flowPanel.Controls.Add(usernameBlogListLabel);
        flowPanel.Controls.Add(userBlogList);
        flowPanel.Controls.Add(topCommenter);
        flowPanel.Controls.Add(followedBy);
        flowPanel.Controls.Add(neverPosted);
        flowPanel.Controls.Add(noNegatives);
        flowPanel.Controls.Add(commonHobby);

        flowPanel.Controls.Add(messageBoard);
        Controls.Add(flowPanel);
        CenterToScreen();
    }

    private void addBlogOnClick(object sender, EventArgs e)
    {
        addPopup blog = new addPopup(database);
        blog.ShowDialog();
    }

    private void postCommentOnClick(object sender, EventArgs e)
    {
        string blogPickedId = blogSelectionBox.GetItemText(blogSelectionBox.SelectedItem);
        string commentType = commentBox.GetItemText(commentBox.SelectedItem);
        string commentDescription = commentTextBox.Text;

        if(blogPickedId.Length == 0 || commentType.Length == 0 || commentDescription.Length == 0) {
            messageBoard.Text = "Empty Fields Not Allowed";
            return;
        }
        int blogid = Int32.Parse(blogPickedId);
        database.createComments(commentType, commentDescription, blogid, messageBoard);
    }
}

public partial class addPopup : Form
{
    private FlowLayoutPanel flowPanel;
    private Label subjectText;
    private TextBox subjectTextBox;
    private Label descriptionText;
    private TextBox descriptionTextBox;
    private Label tagsText;
    private TextBox tagsTextBox;
    private Button buttonPostBlog;
    private Label messageBoard;
    private ComboBox commentBox;
    private DatabaseHandler database;
    public addPopup(DatabaseHandler database)
    {
        this.database = database;
        createGUI();
    }

    private void createGUI()
    {
        Text = "Create your Blog";
        ClientSize = new Size(600, 250);
        flowPanel = new FlowLayoutPanel();

        flowPanel.Dock = DockStyle.Left;
        //flowPanel.BorderStyle = BorderStyle.FixedSingle;

        subjectText = new Label();
        subjectText.Text = "Subject:";
        subjectTextBox = new TextBox();
        //subjectTextBox.AcceptsReturn = true;
        //subjectTextBox.AcceptsTab = true;
        //subjectTextBox.Multiline = true;
        //subjectTextBox.AutoSize = true;
        //subjectTextBox.ScrollBars = ScrollBars.Vertical;

        descriptionText = new Label();
        descriptionText.Text = "Description:";
        descriptionTextBox = new TextBox();

        tagsText = new Label();
        tagsText.Text = "Tags:";
        tagsTextBox = new TextBox();

        buttonPostBlog = new Button();
        buttonPostBlog.Text = "Post";
        buttonPostBlog.AutoSize = true;
        buttonPostBlog.Click += new System.EventHandler(postBlogOnClick);

        messageBoard = new Label();
        messageBoard.AutoSize = true;

        flowPanel.Controls.Add(subjectText);
        flowPanel.Controls.Add(subjectTextBox);
        flowPanel.Controls.Add(descriptionText);
        flowPanel.Controls.Add(descriptionTextBox);
        flowPanel.Controls.Add(tagsText);
        flowPanel.Controls.Add(tagsTextBox);
        flowPanel.Controls.Add(buttonPostBlog);

        flowPanel.Controls.Add(messageBoard);
        Controls.Add(flowPanel);
        CenterToScreen();
    }

    private void postBlogOnClick(object sender, EventArgs e)
    {
        string subject = subjectTextBox.Text;
        string description = descriptionTextBox.Text;
        string tags = tagsTextBox.Text;

        if(subject.Length == 0 || description.Length == 0 || tags.Length == 0) {
            messageBoard.Text = "Empty Fields Not Allowed";
            return;
        }
        database.createBlog(subject, description, tags, messageBoard);
    }
}