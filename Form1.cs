using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace StudentRegistrationForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnPickColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    lblSelectedColor.Text = $"Selected Color: {colorDialog.Color.Name}";
                    // Optionally change a preview background
                    lblSelectedColor.BackColor = colorDialog.Color;
                }
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Load image safely
                        using (var img = Image.FromFile(openFileDialog.FileName))
                        {
                            picStudent.Image = new Bitmap(img);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to load image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtName.Text = "";
            txtEmail.Text = "";
            txtPassword.Text = "";
            rdoMale.Checked = false;
            rdoFemale.Checked = false;
            rdoOther.Checked = false;
            cmbCountry.SelectedIndex = -1;
            dtpBirthdate.Value = DateTime.Now;
            lblSelectedColor.Text = "No Color Selected";
            lblSelectedColor.BackColor = SystemColors.Control;
            lblResult.Text = "";
            picStudent.Image = null;
            txtName.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Basic validation before saving
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please fill in at least Name and Email before saving!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string imageFilename = "student_picture.jpg";
            string country = cmbCountry.SelectedItem?.ToString() ?? "";
            string color = lblSelectedColor.Text.Replace("Selected Color: ", "");

            string[] lines = new string[]
            {
                txtName.Text,
                txtEmail.Text,
                txtPassword.Text,
                (rdoMale.Checked ? "Male" : rdoFemale.Checked ? "Female" : "Other"),
                dtpBirthdate.Value.ToShortDateString(),
                country,
                color,
                (picStudent.Image != null ? imageFilename : "NoImage")
            };

            try
            {
                File.WriteAllLines("student_data.txt", lines);

                if (picStudent.Image != null)
                {
                    picStudent.Image.Save(imageFilename);
                }

                MessageBox.Show("Data saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists("student_data.txt"))
                {
                    MessageBox.Show("No saved data found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string[] lines = File.ReadAllLines("student_data.txt");
                if (lines.Length < 8)
                {
                    MessageBox.Show("Saved data is incomplete or corrupted!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                txtName.Text = lines[0];
                txtEmail.Text = lines[1];
                txtPassword.Text = lines[2];

                if (lines[3] == "Male") rdoMale.Checked = true;
                else if (lines[3] == "Female") rdoFemale.Checked = true;
                else rdoOther.Checked = true;

                DateTime dt;
                if (DateTime.TryParse(lines[4], out dt)) dtpBirthdate.Value = dt;

                if (!string.IsNullOrEmpty(lines[5]))
                    cmbCountry.SelectedItem = lines[5];
                else
                    cmbCountry.SelectedIndex = -1;

                lblSelectedColor.Text = "Selected Color: " + lines[6];

                if (lines[7] == "student_picture.jpg" && File.Exists("student_picture.jpg"))
                {
                    try
                    {
                        using (var img = Image.FromFile("student_picture.jpg"))
                        {
                            picStudent.Image = new Bitmap(img);
                        }
                    }
                    catch
                    {
                        picStudent.Image = null;
                    }
                }
                else
                {
                    picStudent.Image = null;
                }

                MessageBox.Show("Data loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            // Validate Name
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Name is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            // Validate Email
            if (!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Enter a valid email address!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            // Validate Password
            if (txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            // Validate Gender
            if (!rdoMale.Checked && !rdoFemale.Checked && !rdoOther.Checked)
            {
                MessageBox.Show("Please select a gender!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate Country
            if (cmbCountry.SelectedItem == null)
            {
                MessageBox.Show("Please select a country!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCountry.Focus();
                return;
            }

            // Validate Color
            if (lblSelectedColor.Text == "No Color Selected")
            {
                MessageBox.Show("Please select your favorite color!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // All validations passed → display result
            string name = txtName.Text;
            string email = txtEmail.Text;
            string gender = rdoMale.Checked ? "Male" : rdoFemale.Checked ? "Female" : "Other";
            string birthdate = dtpBirthdate.Value.ToShortDateString();
            string country = cmbCountry.SelectedItem.ToString();
            string color = lblSelectedColor.Text.Replace("Selected Color: ", "");

            lblResult.Text = $"Name: {name}\nEmail: {email}\nGender: {gender}\nBirthdate: {birthdate}\nCountry: {country}\nFavorite Color: {color}";
        }

        #region Windows Form Designer generated code

        private System.ComponentModel.IContainer components = null;
        private Label lblName;
        private Label lblEmail;
        private Label lblPassword;
        private Label lblGender;
        private Label lblColor;
        private Label lblBirthdate;
        private Label lblCountry;
        private TextBox txtName;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private GroupBox grpGender;
        private RadioButton rdoMale;
        private RadioButton rdoFemale;
        private RadioButton rdoOther;
        private Button btnPickColor;
        private Label lblSelectedColor;
        private DateTimePicker dtpBirthdate;
        private ComboBox cmbCountry;
        private Button btnSubmit;
        private Label lblResult;
        private Button btnReset;
        private PictureBox picStudent;
        private Button btnUpload;
        private Button btnSave;
        private Button btnLoad;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblName = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblGender = new System.Windows.Forms.Label();
            this.lblColor = new System.Windows.Forms.Label();
            this.lblBirthdate = new System.Windows.Forms.Label();
            this.lblCountry = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.grpGender = new System.Windows.Forms.GroupBox();
            this.rdoOther = new System.Windows.Forms.RadioButton();
            this.rdoFemale = new System.Windows.Forms.RadioButton();
            this.rdoMale = new System.Windows.Forms.RadioButton();
            this.btnPickColor = new System.Windows.Forms.Button();
            this.lblSelectedColor = new System.Windows.Forms.Label();
            this.dtpBirthdate = new System.Windows.Forms.DateTimePicker();
            this.cmbCountry = new System.Windows.Forms.ComboBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.lblResult = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.picStudent = new System.Windows.Forms.PictureBox();
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.grpGender.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picStudent)).BeginInit();
            this.SuspendLayout();
            // 
            // Form settings
            // 
            this.Name = "frmRegistration";
            this.Text = "Student Registration Form";
            this.BackColor = System.Drawing.Color.LightBlue;
            this.ClientSize = new System.Drawing.Size(600, 500);

            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblName.Location = new System.Drawing.Point(30, 30);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(57, 20);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name:";

            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblEmail.Location = new System.Drawing.Point(30, 70);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(52, 20);
            this.lblEmail.TabIndex = 1;
            this.lblEmail.Text = "Email:";

            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblPassword.Location = new System.Drawing.Point(30, 110);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(86, 20);
            this.lblPassword.TabIndex = 2;
            this.lblPassword.Text = "Password:";

            // 
            // lblGender
            // 
            this.lblGender.AutoSize = true;
            this.lblGender.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblGender.Location = new System.Drawing.Point(30, 150);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(70, 20);
            this.lblGender.TabIndex = 3;
            this.lblGender.Text = "Gender:";

            // 
            // lblColor
            // 
            this.lblColor.AutoSize = true;
            this.lblColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblColor.Location = new System.Drawing.Point(30, 190);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(108, 20);
            this.lblColor.TabIndex = 4;
            this.lblColor.Text = "Favorite Color:";

            // 
            // lblBirthdate
            // 
            this.lblBirthdate.AutoSize = true;
            this.lblBirthdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblBirthdate.Location = new System.Drawing.Point(30, 230);
            this.lblBirthdate.Name = "lblBirthdate";
            this.lblBirthdate.Size = new System.Drawing.Size(82, 20);
            this.lblBirthdate.TabIndex = 5;
            this.lblBirthdate.Text = "Birthdate:";

            // 
            // lblCountry
            // 
            this.lblCountry.AutoSize = true;
            this.lblCountry.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblCountry.Location = new System.Drawing.Point(30, 270);
            this.lblCountry.Name = "lblCountry";
            this.lblCountry.Size = new System.Drawing.Size(123, 20);
            this.lblCountry.TabIndex = 6;
            this.lblCountry.Text = "Select Country:";

            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(150, 30);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(200, 25);
            this.txtName.TabIndex = 7;

            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(150, 70);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(200, 25);
            this.txtEmail.TabIndex = 8;

            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(150, 110);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(200, 25);
            this.txtPassword.TabIndex = 9;
            this.txtPassword.PasswordChar = '*';

            // 
            // grpGender
            // 
            this.grpGender.Location = new System.Drawing.Point(150, 140);
            this.grpGender.Name = "grpGender";
            this.grpGender.Size = new System.Drawing.Size(200, 50);
            this.grpGender.TabIndex = 10;
            this.grpGender.TabStop = false;
            this.grpGender.Text = "Gender";

            // 
            // rdoOther
            // 
            this.rdoOther.AutoSize = true;
            this.rdoOther.Location = new System.Drawing.Point(140, 20);
            this.rdoOther.Name = "rdoOther";
            this.rdoOther.Size = new System.Drawing.Size(51, 17);
            this.rdoOther.TabIndex = 2;
            this.rdoOther.TabStop = true;
            this.rdoOther.Text = "Other";
            this.rdoOther.UseVisualStyleBackColor = true;

            // 
            // rdoFemale
            // 
            this.rdoFemale.AutoSize = true;
            this.rdoFemale.Location = new System.Drawing.Point(70, 20);
            this.rdoFemale.Name = "rdoFemale";
            this.rdoFemale.Size = new System.Drawing.Size(59, 17);
            this.rdoFemale.TabIndex = 1;
            this.rdoFemale.TabStop = true;
            this.rdoFemale.Text = "Female";
            this.rdoFemale.UseVisualStyleBackColor = true;

            // 
            // rdoMale
            // 
            this.rdoMale.AutoSize = true;
            this.rdoMale.Location = new System.Drawing.Point(10, 20);
            this.rdoMale.Name = "rdoMale";
            this.rdoMale.Size = new System.Drawing.Size(48, 17);
            this.rdoMale.TabIndex = 0;
            this.rdoMale.TabStop = true;
            this.rdoMale.Text = "Male";
            this.rdoMale.UseVisualStyleBackColor = true;

            // Add radio buttons to group
            this.grpGender.Controls.Add(this.rdoMale);
            this.grpGender.Controls.Add(this.rdoFemale);
            this.grpGender.Controls.Add(this.rdoOther);

            // 
            // btnPickColor
            // 
            this.btnPickColor.Location = new System.Drawing.Point(150, 190);
            this.btnPickColor.Name = "btnPickColor";
            this.btnPickColor.Size = new System.Drawing.Size(100, 25);
            this.btnPickColor.TabIndex = 11;
            this.btnPickColor.Text = "Choose Color";
            this.btnPickColor.UseVisualStyleBackColor = true;
            this.btnPickColor.Click += new System.EventHandler(this.btnPickColor_Click);

            // 
            // lblSelectedColor
            // 
            this.lblSelectedColor.AutoSize = false;
            this.lblSelectedColor.Location = new System.Drawing.Point(300, 190);
            this.lblSelectedColor.Name = "lblSelectedColor";
            this.lblSelectedColor.Size = new System.Drawing.Size(200, 25);
            this.lblSelectedColor.TabIndex = 12;
            this.lblSelectedColor.Text = "No Color Selected";
            this.lblSelectedColor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // dtpBirthdate
            // 
            this.dtpBirthdate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpBirthdate.Location = new System.Drawing.Point(150, 230);
            this.dtpBirthdate.Name = "dtpBirthdate";
            this.dtpBirthdate.Size = new System.Drawing.Size(200, 25);
            this.dtpBirthdate.TabIndex = 13;

            // 
            // cmbCountry
            // 
            this.cmbCountry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCountry.FormattingEnabled = true;
            this.cmbCountry.Items.AddRange(new object[] {
            "Yemen",
            "Egypt",
            "Oman",
            "Qatar",
            "Palestine",
            "Syria"});
            this.cmbCountry.Location = new System.Drawing.Point(150, 270);
            this.cmbCountry.Name = "cmbCountry";
            this.cmbCountry.Size = new System.Drawing.Size(200, 21);
            this.cmbCountry.TabIndex = 14;

            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(150, 320);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(100, 30);
            this.btnSubmit.TabIndex = 15;
            this.btnSubmit.Text = "Register";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);

            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(260, 320);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(100, 30);
            this.btnReset.TabIndex = 16;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);

            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(260, 360);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.TabIndex = 18;
            this.btnSave.Text = "Save Data";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(380, 360);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(100, 30);
            this.btnLoad.TabIndex = 19;
            this.btnLoad.Text = "Load Data";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);

            // 
            // lblResult
            // 
            this.lblResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblResult.Location = new System.Drawing.Point(30, 400);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(540, 50);
            this.lblResult.TabIndex = 17;
            this.lblResult.Text = "";

            // 
            // picStudent
            // 
            this.picStudent.Location = new System.Drawing.Point(400, 30);
            this.picStudent.Name = "picStudent";
            this.picStudent.Size = new System.Drawing.Size(120, 120);
            this.picStudent.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picStudent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picStudent.TabIndex = 20;
            this.picStudent.TabStop = false;

            // 
            // btnUpload
            // 
            this.btnUpload.Location = new System.Drawing.Point(400, 160);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(120, 30);
            this.btnUpload.TabIndex = 21;
            this.btnUpload.Text = "Upload Picture";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);

            // 
            // Add controls to the form
            // 
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.lblGender);
            this.Controls.Add(this.lblColor);
            this.Controls.Add(this.lblBirthdate);
            this.Controls.Add(this.lblCountry);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.grpGender);
            this.Controls.Add(this.btnPickColor);
            this.Controls.Add(this.lblSelectedColor);
            this.Controls.Add(this.dtpBirthdate);
            this.Controls.Add(this.cmbCountry);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.picStudent);
            this.Controls.Add(this.btnUpload);

            // 
            // Finalize
            // 
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
