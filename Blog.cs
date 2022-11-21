using System;
using System.Windows.Forms;
using System.Drawing;

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
    public Blog(DatabaseHandler database) {
        this.database = database;
        createGUI();
    }

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

        flowPanel.Controls.Add(buttonAdd);
        flowPanel.Controls.Add(blogSelectionBox);
        flowPanel.Controls.Add(commentBox);
        flowPanel.Controls.Add(commentText);
        flowPanel.Controls.Add(commentTextBox);
        flowPanel.Controls.Add(buttonPostComment);

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