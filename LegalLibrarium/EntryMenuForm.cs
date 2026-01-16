using System;
using System.Collections.Generic;
using System.Windows.Forms;

public sealed class EntryMenuForm : Form
{
    private readonly Dictionary<string, Func<Form>> _entries;

    public EntryMenuForm()
    {
        Text = "Entry Menu";
        Width = 400;
        Height = 250;
        StartPosition = FormStartPosition.CenterScreen;

        _entries = new Dictionary<string, Func<Form>>
        {
            { "Add Claim", () => new ClaimEntryForm() },
            { "Add Evidence", () => new EvidenceEntryForm() }
        };

        BuildUI();
    }

    private void BuildUI()
    {
        int y = 30;

        foreach (var entry in _entries)
        {
            var btn = new Button
            {
                Left = 50,
                Top = y,
                Width = 280,
                Height = 40,
                Text = entry.Key
            };

            btn.Click += (_, _) =>
            {
                using var form = entry.Value();
                form.ShowDialog(this);
            };

            Controls.Add(btn);
            y += 50;
        }
    }
}

//to add a new one later: EntryMenuForm 