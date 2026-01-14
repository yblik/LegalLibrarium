using Microsoft.Data.Sqlite;
using System;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

public class ClaimEntryForm : Form
{
    private TextBox txtPoint;
    private ComboBox comboType;
    private ComboBox comboCategory;
    private ComboBox comboLegislation;
    private ComboBox comboRespondent;
    private TrackBar trackEvidence;
    private Label lblEvidenceValue;
    private Button btnAdd;

    private const string ConnectionString = "Data Source=case_timeline.db";

    public ClaimEntryForm()
    {
        Text = "Claim Entry";
        Width = 500;
        Height = 400;
        StartPosition = FormStartPosition.CenterScreen;

        BuildUI();
        LoadDropdowns();
    }

    private void BuildUI()
    {
        var lblPoint = new Label { Left = 20, Top = 20, Text = "Point:", Width = 80 };
        txtPoint = new TextBox { Left = 110, Top = 18, Width = 340 };

        var lblType = new Label { Left = 20, Top = 60, Text = "Type:", Width = 80 };
        comboType = new ComboBox { Left = 110, Top = 58, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

        var lblCategory = new Label { Left = 20, Top = 100, Text = "Category:", Width = 80 };
        comboCategory = new ComboBox { Left = 110, Top = 98, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

        var lblLegislation = new Label { Left = 20, Top = 140, Text = "Legislation:", Width = 80 };
        comboLegislation = new ComboBox { Left = 110, Top = 138, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

        var lblRespondent = new Label { Left = 20, Top = 180, Text = "Respondent:", Width = 80 };
        comboRespondent = new ComboBox { Left = 110, Top = 178, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

        var lblEvidence = new Label { Left = 20, Top = 220, Text = "Evidence rating:", Width = 100 };
        trackEvidence = new TrackBar
        {
            Left = 130,
            Top = 215,
            Width = 200,
            Minimum = 0,
            Maximum = 10,
            TickFrequency = 1,
            Value = 5
        };
        trackEvidence.Scroll += (s, e) => lblEvidenceValue.Text = trackEvidence.Value.ToString();
        lblEvidenceValue = new Label { Left = 340, Top = 220, Width = 50, Text = trackEvidence.Value.ToString() };

        btnAdd = new Button
        {
            Left = 20,
            Top = 270,
            Width = 120,
            Text = "Add Claim"
        };
        btnAdd.Click += BtnAdd_Click;

        Controls.Add(lblPoint);
        Controls.Add(txtPoint);
        Controls.Add(lblType);
        Controls.Add(comboType);
        Controls.Add(lblCategory);
        Controls.Add(comboCategory);
        Controls.Add(lblLegislation);
        Controls.Add(comboLegislation);
        Controls.Add(lblRespondent);
        Controls.Add(comboRespondent);
        Controls.Add(lblEvidence);
        Controls.Add(trackEvidence);
        Controls.Add(lblEvidenceValue);
        Controls.Add(btnAdd);
    }

    private void LoadDropdowns()
    {
        LoadCombo("SELECT name FROM Types ORDER BY name", comboType);
        LoadCombo("SELECT name FROM Categories ORDER BY name", comboCategory);
        LoadCombo("SELECT name FROM Legislation ORDER BY name", comboLegislation);
        LoadCombo("SELECT name FROM Respondents ORDER BY name", comboRespondent);
    }

    private void LoadCombo(string sql, ComboBox combo)
    {
        combo.Items.Clear();

        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        using var cmd = new SqliteCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            combo.Items.Add(reader.GetString(0));
        }

        if (combo.Items.Count > 0)
            combo.SelectedIndex = 0;
    }

    private void BtnAdd_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtPoint.Text))
        {
            MessageBox.Show("Point is required.");
            return;
        }

        if (comboType.SelectedItem == null ||
            comboCategory.SelectedItem == null ||
            comboLegislation.SelectedItem == null ||
            comboRespondent.SelectedItem == null)
        {
            MessageBox.Show("All dropdowns must have a selection.");
            return;
        }

        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Claims 
                (point, type, category, legislation, respondent, evidence_rating)
            VALUES 
                ($p, $t, $c, $l, $r, $e);
        ";

        cmd.Parameters.AddWithValue("$p", txtPoint.Text.Trim());
        cmd.Parameters.AddWithValue("$t", comboType.SelectedItem.ToString());
        cmd.Parameters.AddWithValue("$c", comboCategory.SelectedItem.ToString());
        cmd.Parameters.AddWithValue("$l", comboLegislation.SelectedItem.ToString());
        cmd.Parameters.AddWithValue("$r", comboRespondent.SelectedItem.ToString());
        cmd.Parameters.AddWithValue("$e", trackEvidence.Value);

        cmd.ExecuteNonQuery();

        MessageBox.Show("Claim added to database.");

        txtPoint.Clear();
        trackEvidence.Value = 5;
        lblEvidenceValue.Text = "5";
    }
}
