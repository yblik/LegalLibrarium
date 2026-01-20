using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

public sealed class MultiSelectCombo
{
    private readonly ComboBox combo;
    private readonly List<(int Id, string Name)> available = new();
    private readonly List<(int Id, string Name)> selected = new();

    public bool HasSelection => selected.Count > 0;

    public MultiSelectCombo(Form parent, string label, int top)
    {
        parent.Controls.Add(new Label
        {
            Left = 20,
            Top = top,
            Text = label
        });

        combo = new ComboBox
        {
            Left = 150,
            Top = top - 22,
            Width = 400,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        combo.SelectedIndexChanged += OnSelected;
        parent.Controls.Add(combo);
    }

    public void Load(IEnumerable<(int Id, string Name)> values, string placeholder)
    {
        available.Clear();
        selected.Clear();
        available.AddRange(values);
        RefreshCombo(placeholder);
    }

    public void Reset()
    {
        // Move all selected items back to available
        available.AddRange(selected);
        selected.Clear();
        RefreshCombo("-- select category --");
    }

    public void LoadItems(IEnumerable<string> items, string placeholder)
    {
        Load(items.Select((name, idx) => (Id: idx, Name: name)), placeholder);
    }

    private void OnSelected(object sender, EventArgs e)
    {
        if (combo.SelectedItem is not string text)
            return;

        var match = available.FirstOrDefault(v => v.Name == text);
        if (match == default)
            return;

        available.Remove(match);
        selected.Add(match);
        RefreshCombo("-- select another --");
    }

    private void RefreshCombo(string placeholder)
    {
        combo.BeginUpdate();
        combo.Items.Clear();

        // selected items at the TOP
        foreach (var s in selected)
            combo.Items.Add($"✔ {s.Name}");

        if (combo.Items.Count > 0)
            combo.Items.Add("──────────");

        foreach (var a in available)
            combo.Items.Add(a.Name);

        if (combo.Items.Count == 0)
            combo.Items.Add(placeholder);

        combo.SelectedIndex = -1;
        combo.EndUpdate();
    }

    public string ToCodeString()
    {
        if (selected.Count == 0)
            return string.Empty;

        return string.Concat(
            selected
                .OrderBy(s => s.Id)
                .Select(s => IdToAlpha(s.Id))
        );
    }

    private static string IdToAlpha(int id)
    {
        // ID 1 = A, ID 2 = B, ... ID 26 = Z
        // ID 0 would be invalid for alphabetic encoding
        if (id < 1 || id > 26)
            throw new ArgumentOutOfRangeException(nameof(id), "ID must be between 1 and 26 for alphabetic encoding");

        return ((char)('A' + id - 1)).ToString();
    }
}