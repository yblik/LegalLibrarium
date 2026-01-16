using Microsoft.Data.Sqlite;
using System.Windows.Forms;

public sealed class EvidenceEntryForm : EntryFormBase
{
    private TextBox txtTitle;
    private TextBox txtFilePath;
    private ComboBox comboType;
    private Button btnBrowse;
    private Button btnAdd;

    public EvidenceEntryForm()
    {
        Text = "Evidence Entry";
        Width = 600;
        Height = 400;
        StartPosition = FormStartPosition.CenterScreen;

        BuildUI();
        LoadData();
    }

    protected override void BuildUI()
    {
        Controls.Add(new Label { Left = 20, Top = NextRow(), Text = "Title:" });
        txtTitle = new TextBox { Left = 150, Top = Y - 22, Width = 380 };
        Controls.Add(txtTitle);

        Controls.Add(new Label { Left = 20, Top = NextRow(), Text = "File path:" });
        txtFilePath = new TextBox { Left = 150, Top = Y - 22, Width = 300 };
        Controls.Add(txtFilePath);

        btnBrowse = new Button
        {
            Left = 460,
            Top = Y - 24,
            Width = 70,
            Text = "Browse"
        };
        btnBrowse.Click += BrowseFile;
        Controls.Add(btnBrowse);

        Controls.Add(new Label { Left = 20, Top = NextRow(), Text = "Type:" });
        comboType = new ComboBox
        {
            Left = 150,
            Top = Y - 22,
            Width = 200,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        Controls.Add(comboType);

        btnAdd = new Button
        {
            Left = 20,
            Top = NextRow(),
            Width = 150,
            Text = "Add Evidence"
        };
        btnAdd.Click += (_, _) => AddAndContinue();
        Controls.Add(btnAdd);
    }

    protected override void LoadData()
    {
        comboType.Items.Clear();
        comboType.Items.AddRange(new[]
        {
            "bundle",
            "document",
            "pdf"
        });

        comboType.SelectedIndex = 0;
    }

    protected override bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(txtTitle.Text))
        {
            MessageBox.Show("Title is required.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtFilePath.Text))
        {
            MessageBox.Show("File path is required.");
            return false;
        }

        return true;
    }

    protected override void Save()
    {
        using var conn = new SqliteConnection(DbConfig.ConnectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Evidence (title, file_path, type)
            VALUES ($t, $f, $ty);";

        cmd.Parameters.AddWithValue("$t", txtTitle.Text.Trim());
        cmd.Parameters.AddWithValue("$f", txtFilePath.Text.Trim());
        cmd.Parameters.AddWithValue("$ty", comboType.SelectedItem.ToString());

        cmd.ExecuteNonQuery();
    }

    protected override void ResetForm()
    {
        txtTitle.Clear();
        txtFilePath.Clear();
        comboType.SelectedIndex = 0;
    }

    private void BrowseFile(object? sender, System.EventArgs e)
    {
        using var dlg = new OpenFileDialog();
        if (dlg.ShowDialog() == DialogResult.OK)
            txtFilePath.Text = dlg.FileName;
    }
}
