using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

public sealed class MultiSelectCombo
{
    public ComboBox Combo { get; }

    private readonly List<string> _originalOrder = new();
    private readonly List<string> _selected = new();

    public bool HasSelection => _selected.Count > 0;

    public MultiSelectCombo(Control parent, string labelText, int top)
    {
        parent.Controls.Add(new Label
        {
            Left = 20,
            Top = top,
            Text = labelText,
            AutoSize = true
        });

        Combo = new ComboBox
        {
            Left = 150,
            Top = top - 2,
            Width = 500,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        Combo.SelectionChangeCommitted += OnSelect;
        parent.Controls.Add(Combo);
    }

    /* called by LookupLoader */
    public void LoadItems(IEnumerable<string> items, string placeholder)
    {
        _originalOrder.Clear();
        _originalOrder.AddRange(items);

        _selected.Clear();

        Combo.Items.Clear();
        Combo.Items.Add(placeholder);

        foreach (var item in _originalOrder)
            Combo.Items.Add(item);

        Combo.SelectedIndex = 0;
    }

    /* used by ClaimEntryForm.Save() */
    public string ToCodeString()
    {
        // A = first item, B = second, etc
        return string.Concat(
            _selected
                .Select(v => (char)('A' + _originalOrder.IndexOf(v)))
                .OrderBy(c => c)
        );
    }

    private void OnSelect(object? sender, EventArgs e)
    {
        if (Combo.SelectedIndex <= 0)
            return;

        string value = Combo.SelectedItem!.ToString()!;

        if (_selected.Contains(value))
            return;

        _selected.Add(value);

        // remove from possible future selection
        Combo.Items.Remove(value);

        Combo.SelectedIndex = 0;
    }
}
