using Microsoft.Data.Sqlite;
using System.Windows.Forms;

public static class LookupLoader
{
    public static void LoadMultiSelect(
        string sql,
        MultiSelectCombo multi,
        string placeholder)
    {
        multi.Reset(placeholder);

        using var conn = new SqliteConnection(DbConfig.ConnectionString);
        conn.Open();

        using var cmd = new SqliteCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
            multi.Combo.Items.Add($"{reader.GetInt32(0)} - {reader.GetString(1)}");
    }

    public static void LoadCombo(string sql, ComboBox combo)
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
            combo.Items.Add(new EvidenceItem(
                reader.GetInt32(0),
                reader.GetString(1)
            ));

        combo.DisplayMember = nameof(EvidenceItem.Title);
        combo.ValueMember = nameof(EvidenceItem.Id);
    }
}
