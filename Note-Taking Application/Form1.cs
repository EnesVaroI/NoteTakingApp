using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Note_Taking_Application
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadNotes();
        }

        private void LoadNotes()
        {
            if (Directory.Exists(filePath))
            {
                string[] fileNames = Directory.GetFiles(filePath);

                foreach (string file in fileNames)
                    using (StreamReader reader = new StreamReader(file))
                    {
                        row_no++;
                        
                        table.Rows.Add(row_no, Path.GetFileName(file).Substring(0, Path.GetFileName(file).Length - 4), reader.ReadToEnd());
                    }
            }
        }

        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Notes");

        int row_no = 0;

        private void button_new_Click(object sender, EventArgs e)
        {
            textBox_name.Text = string.Empty;
            textBox_text.Text = string.Empty;
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            if (textBox_name.Text != "")
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    if (table.Rows[i].Cells[1].Value.ToString() == textBox_name.Text)
                    {
                        _ = MessageBox.Show("This name already exists.");
                        return;
                    }
                }

                row_no++;

                table.Rows.Add(row_no, textBox_name.Text, textBox_text.Text);

                File.WriteAllText($@"{filePath}\{textBox_name.Text}.txt", textBox_text.Text);

                textBox_name.Clear();
                textBox_text.Clear();
            }

            else MessageBox.Show("Please include a title in your note.");
        }

        private void button_edit_Click(object sender, EventArgs e)
        {
            int i;
            if (table.CurrentCell != null)
                i = table.CurrentCell.RowIndex;
            else return;

            for (int j = 0; j < table.Rows.Count; j++)
            {
                if (j != i && table.Rows[j].Cells[1].Value.ToString() == textBox_name.Text)
                {
                    MessageBox.Show("This name already exists.");
                    return;
                }
            }

            string oldFilePath = $@"{filePath}\{table.Rows[i].Cells[1].Value.ToString()}.txt";
            string newFilePath = Path.Combine(Path.GetDirectoryName(oldFilePath), $"{textBox_name.Text}.txt");
            File.Move(oldFilePath, newFilePath);

            table.Rows[i].Cells[1].Value = textBox_name.Text;
            table.Rows[i].Cells[2].Value = textBox_text.Text;

            using (StreamWriter writer = new StreamWriter($@"{filePath}\{textBox_name.Text}.txt", false, Encoding.Unicode))
            {
                writer.Write(textBox_text.Text);
            }

            textBox_name.Clear();
            textBox_text.Clear();
        }

        private void button_view_Click(object sender, EventArgs e)
        {
            int i;
            if (table.CurrentCell != null)
                i = table.CurrentCell.RowIndex;
            else return;

            textBox_name.Text = table.Rows[i].Cells[1].Value.ToString();
            textBox_text.Text = table.Rows[i].Cells[2].Value.ToString();
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            int i;
            if (table.CurrentCell != null)
                i = table.CurrentCell.RowIndex;
            else return;

            File.Delete($@"{filePath}\{table.Rows[i].Cells[1].Value.ToString()}.txt");

            table.Rows.RemoveAt(i);

            row_no--;

            for (; i < row_no; i++) table.Rows[i].Cells[0].Value = i + 1;
        }
    }
}