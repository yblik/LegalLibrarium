using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

public sealed class MultiSelectCombo
{
    public ComboBox Combo { get; }
    public List<string> Selected { get; } = new();

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

        Combo.SelectionChangeCommitted += HandleSelect;
        parent.Controls.Add(Combo);
    }

    private void HandleSelect(object? sender, EventArgs e)
    {
        if (Combo.SelectedIndex <= 0) return;

        string value = Combo.SelectedItem!.ToString()!;
        Selected.Add(value);
        Combo.Items.Remove(value);
        Combo.Text = string.Join(", ", Selected);
        Combo.SelectedIndex = 0;
    }

    public void Reset(string placeholder)
    {
        Selected.Clear();
        Combo.Items.Clear();
        Combo.Items.Add(placeholder);
        Combo.SelectedIndex = 0;
    }

    public string ToCodeString()
    {
        return string.Concat(
            Selected
                .Select(v => v.Substring(0, 1))
                .OrderBy(c => c)
        );
    }

    public bool HasSelection => Selected.Count > 0;
}
