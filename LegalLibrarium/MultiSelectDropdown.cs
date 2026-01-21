using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

public sealed class MultiSelectDropdown
{
    private readonly TextBox search;
    private readonly CheckedListBox list;
    private readonly Form host;
    private readonly Dictionary<string, int> map = new();
    private DataTable sourceData;

    public MultiSelectDropdown(Form host, string label, int top)
    {
        this.host = host;

        host.Controls.Add(new Label
        {
            Left = 20,
            Top = top,
            Text = label
        });

        search = new TextBox
        {
            Left = 150,
            Top = top - 4,
            Width = 600,
            PlaceholderText = "Type to search or click to select..."
        };

        list = new CheckedListBox
        {
            Left = 150,
            Top = top + 22,
            Width = 600,
            Height = 120,
            Visible = false
        };

        search.Click += (_, _) => list.Visible = !list.Visible;
        search.TextChanged += Search_TextChanged;
        search.GotFocus += (_, _) => list.Visible = true;
        list.ItemCheck += (_, _) => host.BeginInvoke(UpdateDisplay);

        host.Controls.Add(search);
        host.Controls.Add(list);
    }

    public void Load(DataTable table)
    {
        sourceData = table;
        map.Clear();
        PopulateList("");
    }

    private void Search_TextChanged(object sender, EventArgs e)
    {
        // If user is typing (not just showing selections), filter the list
        if (search.Focused && !IsShowingSelections())
        {
            PopulateList(search.Text);
            list.Visible = true;
        }
    }

    private void PopulateList(string filter)
    {
        var checkedItems = list.CheckedItems.Cast<string>().ToHashSet();

        list.Items.Clear();
        map.Clear();

        if (sourceData == null) return;

        foreach (DataRow row in sourceData.Rows)
        {
            string name = row["name"].ToString();
            int id = Convert.ToInt32(row["id"]);

            // Filter by search text
            if (!string.IsNullOrWhiteSpace(filter) &&
                !name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                continue;

            map[name] = id;
            int index = list.Items.Add(name);

            // Restore checked state
            if (checkedItems.Contains(name))
                list.SetItemChecked(index, true);
        }
    }

    private void UpdateDisplay()
    {
        var selected = list.CheckedItems.Cast<string>().ToList();

        if (selected.Count == 0)
        {
            search.Clear();
        }
        else
        {
            search.Text = string.Join(", ", selected);
        }
    }

    private bool IsShowingSelections()
    {
        // Check if the current text matches the selection display format
        return search.Text.Contains(", ") || list.CheckedItems.Count > 0;
    }

    public void Clear()
    {
        // Uncheck all items
        for (int i = 0; i < list.Items.Count; i++)
            list.SetItemChecked(i, false);

        // Clear search and reload full list
        search.Clear();
        if (sourceData != null)
            PopulateList("");
    }

    public IReadOnlyList<int> SelectedIds =>
        list.CheckedItems
            .Cast<string>()
            .Select(x => map[x])
            .ToList();

    public bool HasSelection => list.CheckedItems.Count > 0;
}