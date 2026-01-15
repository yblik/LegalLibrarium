using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

public class ClaimEntryForm : Form
{
    private TextBox txtPoint;

    private ComboBox comboTypes;
    private ComboBox comboCategories;
    private ComboBox comboRespondents;
    private ComboBox comboLegislation;
    private ComboBox comboEvidence;

    private readonly List<string> selectedTypes = new();
    private readonly List<string> selectedCategories = new();
    private readonly List<string> selectedRespondents = new();

    private TrackBar trackEvidence;
    private Label lblEvidenceValue;
    private Button btnAdd;

    private NumericUpDown numEvidencePage;
    private TextBox txtEvidenceLocation;

    private const string ConnectionString = "Data Source=case_timeline.db";

    public ClaimEntryForm()
    {
        Text = "Claim Entry";
        Width = 700;
        Height = 650;
        StartPosition = FormStartPosition.CenterScreen;

        BuildUI();
        LoadDropdowns();
    }

    private void BuildUI()
    {
        Controls.Add(new Label { Left = 20, Top = 20, Text = "Point:" });
        txtPoint = new TextBox { Left = 150, Top = 18, Width = 500 };
        Controls.Add(txtPoint);

        comboTypes = BuildMultiSelect("Types:", 60, selectedTypes);
        comboCategories = BuildMultiSelect("Categories:", 100, selectedCategories);
        comboRespondents = BuildMultiSelect("Respondents:", 140, selectedRespondents);

        Controls.Add(new Label { Left = 20, Top = 180, Text = "Legislation:" });
        comboLegislation = new ComboBox
        {
            Left = 150,
            Top = 178,
            Width = 300,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        Controls.Add(comboLegislation);

        Controls.Add(new Label { Left = 20, Top = 220, Text = "Evidence rating:" });
        trackEvidence = new TrackBar
        {
            Left = 150,
            Top = 215,
            Width = 200,
            Minimum = 0,
            Maximum = 10,
            Value = 5
        };
        trackEvidence.Scroll += (s, e) => lblEvidenceValue.Text = trackEvidence.Value.ToString();
        Controls.Add(trackEvidence);

        lblEvidenceValue = new Label { Left = 360, Top = 220, Text = "5" };
        Controls.Add(lblEvidenceValue);

        Controls.Add(new Label { Left = 20, Top = 260, Text = "Evidence item:" });
        comboEvidence = new ComboBox
        {
            Left = 150,
            Top = 258,
            Width = 400,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        Controls.Add(comboEvidence);

        Controls.Add(new Label { Left = 20, Top = 300, Text = "Page number:" });
        numEvidencePage = new NumericUpDown { Left = 150, Top = 298, Width = 80 };
        Controls.Add(numEvidencePage);

        Controls.Add(new Label { Left = 20, Top = 340, Text = "Location text:" });
        txtEvidenceLocation = new TextBox { Left = 150, Top = 338, Width = 400 };
        Controls.Add(txtEvidenceLocation);

        btnAdd = new Button
        {
            Left = 20,
            Top = 380,
            Width = 150,
            Text = "Add Claim"
        };
        btnAdd.Click += BtnAdd_Click;
        Controls.Add(btnAdd);
    }

    private ComboBox BuildMultiSelect(string label, int top, List<string> store)
    {
        Controls.Add(new Label { Left = 20, Top = top, Text = label });

        var combo = new ComboBox
        {
            Left = 150,
            Top = top - 2,
            Width = 500,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        combo.SelectionChangeCommitted += (s, e) =>
        {
            if (combo.SelectedIndex <= 0) return;

            string value = combo.SelectedItem.ToString();
            store.Add(value);
            combo.Items.Remove(value);
            combo.Text = string.Join(", ", store);
            combo.SelectedIndex = 0;
        };

        Controls.Add(combo);
        return combo;
    }

    private void LoadDropdowns()
    {
        LoadMultiSelectCombo("SELECT id, name FROM Types ORDER BY id", comboTypes, "-- select type --");
        LoadMultiSelectCombo("SELECT id, name FROM Categories ORDER BY id", comboCategories, "-- select category --");
        LoadMultiSelectCombo("SELECT id, name FROM Respondents ORDER BY id", comboRespondents, "-- select respondent --");

        LoadCombo("SELECT name FROM Legislation ORDER BY name", comboLegislation);
        LoadEvidence();
    }

    private void LoadMultiSelectCombo(string sql, ComboBox combo, string placeholder)
    {
        combo.Items.Clear();
        combo.Items.Add(placeholder);
        combo.SelectedIndex = 0;

        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        using var cmd = new SqliteCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
            combo.Items.Add($"{reader.GetString(0)} - {reader.GetString(1)}");
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

    private void LoadEvidence()
    {
        comboEvidence.Items.Clear();

        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        using var cmd = new SqliteCommand("SELECT id, title FROM Evidence ORDER BY id", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
            comboEvidence.Items.Add(new { Id = reader.GetInt32(0), Title = reader.GetString(1) });

        comboEvidence.DisplayMember = "Title";
        comboEvidence.ValueMember = "Id";
    }

    private string BuildMultiSelectString(List<string> values)
    {
        return string.Concat(values.Select(v => v.Substring(0, 1)).OrderBy(c => c));
    }

    private void BtnAdd_Click(object sender, EventArgs e)
    {
        if (!selectedTypes.Any() || !selectedCategories.Any() || !selectedRespondents.Any())
        {
            MessageBox.Show("Select at least one Type, Category, and Respondent.");
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
            ($p, $t, $c, $l, $r, $e, $eid, $ep, $el);";

        cmd.Parameters.AddWithValue("$p", txtPoint.Text.Trim());
        cmd.Parameters.AddWithValue("$t", BuildMultiSelectString(selectedTypes));
        cmd.Parameters.AddWithValue("$c", BuildMultiSelectString(selectedCategories));
        cmd.Parameters.AddWithValue("$l", comboLegislation.SelectedItem?.ToString());
        cmd.Parameters.AddWithValue("$r", BuildMultiSelectString(selectedRespondents));
        cmd.Parameters.AddWithValue("$e", trackEvidence.Value);
        cmd.Parameters.AddWithValue("$eid", comboEvidence.SelectedItem == null ? DBNull.Value : (comboEvidence.SelectedItem as dynamic).Id);
        cmd.Parameters.AddWithValue("$ep", numEvidencePage.Value);
        cmd.Parameters.AddWithValue("$el", txtEvidenceLocation.Text.Trim());

        cmd.ExecuteNonQuery();

        MessageBox.Show("Claim added.");
        Close();
    }
}
