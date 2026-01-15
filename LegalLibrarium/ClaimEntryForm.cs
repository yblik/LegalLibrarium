using Microsoft.Data.Sqlite;
using System;
using System.Linq;
using System.Windows.Forms;

public class ClaimEntryForm : Form
{
    private TextBox txtPoint;

    private CheckedListBox listTypes;
    private CheckedListBox listCategories;
    private ComboBox comboLegislation;
    private CheckedListBox listRespondents;

    private TrackBar trackEvidence;
    private Label lblEvidenceValue;
    private Button btnAdd;

    private ComboBox comboEvidence;
    private NumericUpDown numEvidencePage;
    private TextBox txtEvidenceLocation;

    private const string ConnectionString = "Data Source=case_timeline.db";

    public ClaimEntryForm()
    {
        Text = "Claim Entry";
        Width = 600;
        Height = 700;
        StartPosition = FormStartPosition.CenterScreen;

        BuildUI();
        LoadDropdowns();
    }

    private void BuildUI()
    {
        var lblPoint = new Label { Left = 20, Top = 20, Text = "Point:", Width = 120 };
        txtPoint = new TextBox { Left = 150, Top = 18, Width = 400 };

        var lblType = new Label { Left = 20, Top = 60, Text = "Types:", Width = 120 };
        listTypes = new CheckedListBox { Left = 150, Top = 58, Width = 200, Height = 100 };

        var lblCategory = new Label { Left = 20, Top = 170, Text = "Categories:", Width = 120 };
        listCategories = new CheckedListBox { Left = 150, Top = 168, Width = 200, Height = 100 };

        var lblLegislation = new Label { Left = 20, Top = 280, Text = "Legislation:", Width = 120 };
        comboLegislation = new ComboBox { Left = 150, Top = 278, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

        var lblRespondent = new Label { Left = 20, Top = 320, Text = "Respondents:", Width = 120 };
        listRespondents = new CheckedListBox { Left = 150, Top = 318, Width = 200, Height = 100 };

        var lblEvidence = new Label { Left = 20, Top = 430, Text = "Evidence rating:", Width = 120 };

        var lblEvidenceItem = new Label { Left = 20, Top = 470, Text = "Evidence item:", Width = 120 };
        comboEvidence = new ComboBox { Left = 150, Top = 468, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };

        var lblEvidencePage = new Label { Left = 20, Top = 510, Text = "Page number:", Width = 120 };
        numEvidencePage = new NumericUpDown { Left = 150, Top = 508, Width = 80, Minimum = 0, Maximum = 9999 };

        var lblEvidenceLocation = new Label { Left = 20, Top = 550, Text = "Location text:", Width = 120 };
        txtEvidenceLocation = new TextBox { Left = 150, Top = 548, Width = 300 };

        trackEvidence = new TrackBar
        {
            Left = 150,
            Top = 425,
            Width = 200,
            Minimum = 0,
            Maximum = 10,
            TickFrequency = 1,
            Value = 5
        };
        trackEvidence.Scroll += (s, e) => lblEvidenceValue.Text = trackEvidence.Value.ToString();
        lblEvidenceValue = new Label { Left = 360, Top = 430, Width = 50, Text = "5" };


        btnAdd = new Button
        {
            Left = 20,
            Top = 620, //lower (from 500)
            Width = 150,
            Text = "Add Claim"
        };
        btnAdd.Click += BtnAdd_Click;

        Controls.Add(lblPoint);
        Controls.Add(txtPoint);
        Controls.Add(lblType);
        Controls.Add(listTypes);
        Controls.Add(lblCategory);
        Controls.Add(listCategories);
        Controls.Add(lblLegislation);
        Controls.Add(comboLegislation);
        Controls.Add(lblRespondent);
        Controls.Add(listRespondents);
        Controls.Add(lblEvidence);
        Controls.Add(trackEvidence);
        Controls.Add(lblEvidenceValue);
        Controls.Add(btnAdd);
        Controls.Add(lblEvidenceItem);
        Controls.Add(comboEvidence);
        Controls.Add(lblEvidencePage);
        Controls.Add(numEvidencePage);
        Controls.Add(lblEvidenceLocation);
        Controls.Add(txtEvidenceLocation);
    }

    private void LoadDropdowns()
    {
        LoadCheckedList("SELECT id, name FROM Types ORDER BY id", listTypes);
        LoadCheckedList("SELECT id, name FROM Categories ORDER BY id", listCategories);
        LoadCheckedList("SELECT id, name FROM Respondents ORDER BY id", listRespondents);

        LoadCombo("SELECT name FROM Legislation ORDER BY name", comboLegislation);
        LoadEvidence();
    }

    private void LoadEvidence()
    {
        comboEvidence.Items.Clear();

        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        using var cmd = new SqliteCommand("SELECT id, title FROM Evidence ORDER BY id", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string title = reader.GetString(1);

            comboEvidence.Items.Add(new { Id = id, Title = title });
        }

        comboEvidence.DisplayMember = "Title";
        comboEvidence.ValueMember = "Id";
    }
    private void LoadCheckedList(string sql, CheckedListBox list)
    {
        list.Items.Clear();

        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        using var cmd = new SqliteCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            if (reader.IsDBNull(0) || reader.IsDBNull(1))
                continue; // skip bad rows

            string id = reader.GetString(0);
            string name = reader.GetString(1);

            list.Items.Add($"{id} - {name}");
        }
    }

    private void LoadCombo(string sql, ComboBox combo)
    {
        combo.Items.Clear();

        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        using var cmd = new SqliteCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
            combo.Items.Add(reader.GetString(0));

        if (combo.Items.Count > 0)
            combo.SelectedIndex = 0;
    }

    private string BuildMultiSelectString(CheckedListBox list)
    {
        return string.Concat(
            list.CheckedItems
                .Cast<string>()
                .Select(item => item.Substring(0, 1)) // extract CHAR(1)
                .OrderBy(c => c)
        );
    }

    private void BtnAdd_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtPoint.Text))
        {
            MessageBox.Show("Point is required.");
            return;
        }

        if (comboLegislation.SelectedItem == null)
        {
            MessageBox.Show("Legislation must be selected.");
            return;
        }

        string typeStr = BuildMultiSelectString(listTypes);
        string categoryStr = BuildMultiSelectString(listCategories);
        string respondentStr = BuildMultiSelectString(listRespondents);

        if (typeStr.Length == 0 || categoryStr.Length == 0 || respondentStr.Length == 0)
        {
            MessageBox.Show("You must select at least one Type, Category, and Respondent.");
            return;
        }

        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Claims 
                (point, type, category, legislation, respondent, evidence_rating,
                 evidence_id, evidence_page, evidence_location_text)
            VALUES 
                ($p, $t, $c, $l, $r, $e,
                 $eid, $ep, $el);
        ";

        cmd.Parameters.AddWithValue("$p", txtPoint.Text.Trim());
        cmd.Parameters.AddWithValue("$t", typeStr);
        cmd.Parameters.AddWithValue("$c", categoryStr);
        cmd.Parameters.AddWithValue("$l", comboLegislation.SelectedItem.ToString());
        cmd.Parameters.AddWithValue("$r", respondentStr);
        cmd.Parameters.AddWithValue("$e", trackEvidence.Value);

        cmd.Parameters.AddWithValue("$eid", comboEvidence.SelectedItem == null
            ? DBNull.Value
            : (comboEvidence.SelectedItem as dynamic).Id);

        cmd.Parameters.AddWithValue("$ep", numEvidencePage.Value);
        cmd.Parameters.AddWithValue("$el", txtEvidenceLocation.Text.Trim());


        cmd.ExecuteNonQuery();

        MessageBox.Show("Claim added to database.");

        txtPoint.Clear();
        trackEvidence.Value = 5;
        lblEvidenceValue.Text = "5";

        foreach (int i in listTypes.CheckedIndices) listTypes.SetItemChecked(i, false);
        foreach (int i in listCategories.CheckedIndices) listCategories.SetItemChecked(i, false);
        foreach (int i in listRespondents.CheckedIndices) listRespondents.SetItemChecked(i, false);
    }
}