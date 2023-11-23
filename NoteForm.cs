using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace NoteTaking
{
    public partial class NoteForm : Form
    {
        DataTable _table;
        List<Note> _notes;

        public NoteForm()
        {
            InitializeComponent();

            this.FormClosing += NoteForm_FormClosing;
        }

        private void NoteForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveDataToJsonFile();
        }

        private void NoteForm_Load(object sender, EventArgs e)
        {
            _table = new DataTable();

            _table.Columns.Add("Title", typeof(String));
            _table.Columns.Add("Messeges", typeof(String));

            dataGridView1.DataSource = _table;

            dataGridView1.Columns["Messeges"].Visible = false;
            dataGridView1.Columns["Title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            LoadDataFromJsonFile();
        }

        private void SaveDataToJsonFile()
        {
            _notes = new List<Note>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    Note note = new Note
                    {
                        Title = row.Cells["Title"].Value.ToString(),
                        Messege = row.Cells["Messeges"].Value.ToString(),
                    };

                    _notes.Add(note);
                }
            }

            string json = JsonConvert.SerializeObject(_notes, Formatting.Indented);
            File.WriteAllText("storage.json", json);
        }

        private void LoadDataFromJsonFile()
        {
            string filePath = "storage.json";

            if(!File.Exists(filePath))
            {
                // File doesn't exist, create a new one with an empty array
                File.WriteAllText(filePath, "[]");
            }

            string json = File.ReadAllText(filePath);
            _notes = JsonConvert.DeserializeObject<List<Note>>(json);

            _table.Rows.Clear();

            foreach(Note note in _notes)
            {
                _table.Rows.Add(note.Title, note.Messege);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            txtTitle.Clear();
            txtMessage.Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(!(txtTitle.Text.Equals("") && txtMessage.Text.Equals("")))
            {
                _table.Rows.Add(txtTitle.Text, txtMessage.Text);

                txtTitle.Clear();
                txtMessage.Clear();
            }
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            if(dataGridView1.CurrentCell != null)
            {
                int index = dataGridView1.CurrentCell.RowIndex;

                if (index > -1)
                {
                    txtTitle.Text = _table.Rows[index].ItemArray[0].ToString();
                    txtMessage.Text = _table.Rows[index].ItemArray[1].ToString();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(dataGridView1.CurrentCell != null)
            {
                int index = dataGridView1.CurrentCell.RowIndex;
                _table.Rows[index].Delete();
            }
        }
    }
}
