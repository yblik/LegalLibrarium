using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

public sealed class MultiSelectDropdown
{
    private readonly TextBox display;
    private readonly CheckedListBox list;
    private readonly Form host;

    private readonly Dictionary<string, int> map = new();

    public MultiSelectDropdown(Form host, string label, int top)
    {
        this.host = host;

        host.Controls.Add(new Label
        {
            Left = 20,
            Top = top,
            Text = label
        });

        display = new TextBox
        {
            Left = 150,
            Top = top - 4,
            Width = 400,
            ReadOnly = true
        };

        list = new CheckedListBox
        {
            Left = 150,
            Top = top + 22,
            Width = 400,
            Height = 120,
            Visible = false
        };

        display.Click += (_, _) => list.Visible = !list.Visible;
        list.ItemCheck += (_, _) => host.BeginInvoke(UpdateText);

        host.Controls.Add(display);
        host.Controls.Add(list);
    }

    public void Load(DataTable table)
    {
        map.Clear();
        list.Items.Clear();

        foreach (DataRow row in table.Rows)
        {
            string name = row["name"].ToString();
            int id = Convert.ToInt32(row["id"]);

            map[name] = id;
            list.Items.Add(name);
        }
    }

    private void UpdateText()
    {
        display.Text = string.Join(", ",
            list.CheckedItems.Cast<string>());
    }

    public IReadOnlyList<int> SelectedIds =>
        list.CheckedItems
            .Cast<string>()
            .Select(x => map[x])
            .ToList();

    public bool HasSelection => list.CheckedItems.Count > 0;
}
