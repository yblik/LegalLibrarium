using Microsoft.Data.Sqlite;
using System.Data;
using System.Windows.Forms;

public static class LookupLoader
{
    // =========================
    // GENERIC TABLE LOADER
    // =========================
    public static DataTable LoadTable(string sql)
    {
        using var conn = new SqliteConnection(DbConfig.ConnectionString);
        conn.Open(); // ← YOU WERE MISSING THIS!

        using var cmd = new SqliteCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        var table = new DataTable();

        // Create columns from result set
        for (int i = 0; i < reader.FieldCount; i++)
        {
            table.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
        }

        // Fill rows with data
        while (reader.Read())
        {
            var row = table.NewRow();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[i] = reader.GetValue(i);
            }
            table.Rows.Add(row);
        }

        return table;
    }

    // =========================
    // MULTI-SELECT DROPDOWN
    // =========================
    public static void LoadMultiSelect(
        string sql,
        MultiSelectDropdown dropdown)
    {
        dropdown.Load(LoadTable(sql));
    }

    // =========================
    // STANDARD COMBOBOX
    // =========================
    public static void LoadCombo(
        string sql,
        ComboBox combo)
    {
        combo.Items.Clear();
        using var conn = new SqliteConnection(DbConfig.ConnectionString);
        conn.Open();
        using var cmd = new SqliteCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
            combo.Items.Add(reader.GetString(0));

        if (combo.Items.Count > 0)
            combo.SelectedIndex = 0;
    }

    // =========================
    // EVIDENCE COMBO
    // =========================
    public static void LoadEvidence(ComboBox combo)
    {
        combo.Items.Clear();
        using var conn = new SqliteConnection(DbConfig.ConnectionString);
        conn.Open();
        using var cmd = new SqliteCommand(
            "SELECT id, title FROM Evidence ORDER BY id",
            conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            combo.Items.Add(new EvidenceItem(
                reader.GetInt32(0),
                reader.GetString(1)));
        }

        combo.DisplayMember = nameof(EvidenceItem.Title);
        combo.ValueMember = nameof(EvidenceItem.Id);

        if (combo.Items.Count > 0)
            combo.SelectedIndex = 0;
    }
}