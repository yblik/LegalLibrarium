using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public sealed class MultiSelectDropdown
{
    private readonly TextBox search;
    private readonly Button btnClearSearch;
    private readonly CheckedListBox list;
    private readonly Form host;
    private readonly Dictionary<string, int> map = new();
    private DataTable sourceData;
    private bool isShowingSelections = false;

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
            Width = 560,
            PlaceholderText = "Type to search or click to select..."
        };

        // Clear/Reset button
        btnClearSearch = new Button
        {
            Left = 715,
            Top = top - 4,
            Width = 35,
            Height = search.Height,
            Text = "↻",
            Font = new Font(search.Font.FontFamily, 10, FontStyle.Bold)
        };
        btnClearSearch.Click += (_, _) => ResetSearch();

        list = new CheckedListBox
        {
            Left = 150,
            Top = top + 22,
            Width = 600,
            Height = 120,
            Visible = false
        };

        search.Click += Search_Click;
        search.TextChanged += Search_TextChanged;
        search.Enter += (_, _) =>
        {
            if (isShowingSelections)
                ResetSearch();
        };
        list.ItemCheck += (_, _) => host.BeginInvoke(UpdateDisplay);

        host.Controls.Add(search);
        host.Controls.Add(btnClearSearch);
        host.Controls.Add(list);
    }

    private void Search_Click(object sender, EventArgs e)
    {
        if (isShowingSelections)
            ResetSearch();
        else
            list.Visible = !list.Visible;
    }

    private void ResetSearch()
    {
        isShowingSelections = false;
        search.Clear();
        search.ForeColor = SystemColors.WindowText;
        PopulateList("");
        list.Visible = true;
        search.Focus();
    }

    public void Load(DataTable table)
    {
        sourceData = table;
        map.Clear();
        PopulateList("");
    }

    private void Search_TextChanged(object sender, EventArgs e)
    {
        // Only filter if we're in search mode (not showing selections)
        if (!isShowingSelections && search.Focused)
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
            isShowingSelections = false;
            search.Clear();
            search.ForeColor = SystemColors.WindowText;
        }
        else
        {
            isShowingSelections = true;
            search.Text = string.Join(", ", selected);
            search.ForeColor = SystemColors.GrayText;
        }
    }

    public void Clear()
    {
        // Uncheck all items
        for (int i = 0; i < list.Items.Count; i++)
            list.SetItemChecked(i, false);

        // Reset to search mode
        isShowingSelections = false;
        search.Clear();
        search.ForeColor = SystemColors.WindowText;

        // Reload full list
        if (sourceData != null)
            PopulateList("");
    }

    public IReadOnlyList<int> SelectedIds =>
        list.CheckedItems
            .Cast<string>()
            .Select(x => map.ContainsKey(x) ? map[x] : 0)
            .Where(id => id != 0)
            .ToList();

    public bool HasSelection => list.CheckedItems.Count > 0;
}