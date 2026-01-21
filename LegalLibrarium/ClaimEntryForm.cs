using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.Windows.Forms;

public sealed class ClaimEntryForm : EntryFormBase
{
    private TextBox txtPoint;

    private MultiSelectDropdown categories;
    private MultiSelectDropdown respondents;

    private ComboBox comboLegislation;
    private ComboBox comboEvidence;

    private TrackBar trackEvidence;
    private Label lblEvidenceValue;

    private NumericUpDown numEvidencePage;
    private TextBox txtEvidenceLocation;

    private Button btnAdd;

    public ClaimEntryForm()
    {
        Text = "Claim Entry";
        Width = 700;
        Height = 700; // Increased to accommodate dropdowns
        StartPosition = FormStartPosition.CenterScreen;

        BuildUI();
        LoadData();
    }

    protected override void BuildUI()
    {
        Controls.Add(new Label { Left = 20, Top = NextRow(), Text = "Point:" });
        txtPoint = new TextBox { Left = 150, Top = Y - 22, Width = 500 };
        Controls.Add(txtPoint);

        categories = new MultiSelectDropdown(this, "Categories:", NextRow());

        // Add extra spacing for dropdown
        NextRow(); NextRow(); NextRow(); NextRow();

        respondents = new MultiSelectDropdown(this, "Respondents:", NextRow());

        // Add extra spacing for dropdown
        NextRow(); NextRow(); NextRow(); NextRow();

        Controls.Add(new Label { Left = 20, Top = NextRow(), Text = "Legislation:" });
        comboLegislation = new ComboBox
        {
            Left = 150,
            Top = Y - 22,
            Width = 300,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        Controls.Add(comboLegislation);

        Controls.Add(new Label { Left = 20, Top = NextRow(), Text = "Evidence rating:" });
        trackEvidence = new TrackBar
        {
            Left = 150,
            Top = Y - 30,
            Width = 200,
            Minimum = 0,
            Maximum = 10,
            Value = 5
        };
        trackEvidence.Scroll += (_, _) =>
            lblEvidenceValue.Text = trackEvidence.Value.ToString();

        Controls.Add(trackEvidence);

        lblEvidenceValue = new Label
        {
            Left = 360,
            Top = Y - 24,
            Text = "5"
        };
        Controls.Add(lblEvidenceValue);

        Controls.Add(new Label { Left = 20, Top = NextRow(), Text = "Evidence item:" });
        comboEvidence = new ComboBox
        {
            Left = 150,
            Top = Y - 22,
            Width = 400,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        Controls.Add(comboEvidence);

        Controls.Add(new Label { Left = 20, Top = NextRow(), Text = "Page number:" });
        numEvidencePage = new NumericUpDown
        {
            Left = 150,
            Top = Y - 22,
            Width = 80
        };
        Controls.Add(numEvidencePage);

        Controls.Add(new Label { Left = 20, Top = NextRow(), Text = "Location text:" });
        txtEvidenceLocation = new TextBox
        {
            Left = 150,
            Top = Y - 22,
            Width = 400
        };
        Controls.Add(txtEvidenceLocation);

        btnAdd = new Button
        {
            Left = 20,
            Top = NextRow(),
            Width = 150,
            Text = "Add Claim"
        };
        btnAdd.Click += (_, _) => AddAndContinue();
        Controls.Add(btnAdd);
    }

    protected override void LoadData()
    {
        categories.Load(
            LookupLoader.LoadTable("SELECT id, name FROM Categories ORDER BY name"));

        respondents.Load(
            LookupLoader.LoadTable("SELECT id, name FROM Respondents ORDER BY name"));

        LookupLoader.LoadCombo(
            "SELECT name FROM Legislation ORDER BY name",
            comboLegislation);

        LookupLoader.LoadEvidence(comboEvidence);
    }

    protected override bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(txtPoint.Text))
        {
            MessageBox.Show("Point text is required.");
            return false;
        }

        if (!categories.HasSelection || !respondents.HasSelection)
        {
            MessageBox.Show("At least one Category and Respondent is required.");
            return false;
        }

        if (comboLegislation.SelectedIndex < 0)
        {
            MessageBox.Show("Legislation must be selected.");
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
            INSERT INTO Claims
            (point, category, legislation, respondent,
             evidence_rating, evidence_id, evidence_page, evidence_location_text)
            VALUES
            ($p, $c, $l, $r, $e, $eid, $ep, $el);";

        cmd.Parameters.AddWithValue("$p", txtPoint.Text.Trim());
        cmd.Parameters.AddWithValue(
            "$c",
            string.Join(",", categories.SelectedIds));
        cmd.Parameters.AddWithValue("$l", comboLegislation.SelectedItem?.ToString() ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue(
            "$r",
            string.Join(",", respondents.SelectedIds));

        cmd.Parameters.AddWithValue("$e", trackEvidence.Value);
        cmd.Parameters.AddWithValue(
            "$eid",
            comboEvidence.SelectedItem is EvidenceItem ev ? (object)ev.Id : DBNull.Value);
        cmd.Parameters.AddWithValue("$ep", numEvidencePage.Value);
        cmd.Parameters.AddWithValue("$el", txtEvidenceLocation.Text.Trim());

        cmd.ExecuteNonQuery();
    }

    protected override void ResetForm()
    {
        txtPoint.Clear();
        txtEvidenceLocation.Clear();
        numEvidencePage.Value = 0;

        trackEvidence.Value = 5;
        lblEvidenceValue.Text = "5";

        categories.Clear();
        respondents.Clear();

        LoadData();
    }
}