using System.Windows.Forms; //formatter

public abstract class EntryFormBase : Form
{
    protected int Y = 20;
    protected const int RowHeight = 40;

    protected int NextRow()
    {
        int current = Y;
        Y += RowHeight;
        return current;
    }

    protected abstract void BuildUI();
    protected abstract void LoadData();
    protected abstract bool ValidateForm();
    protected abstract void Save();
    protected abstract void ResetForm();

    protected void AddAndContinue()
    {
        if (!ValidateForm())
            return;

        Save();
        MessageBox.Show("Entry added.");
        ResetForm();
    }
}
