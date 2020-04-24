using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Dolotniy_postavka
{
    public partial class Form1 : Form
    {
       public bool controll_pass = false;
        public Form1()
        {
            InitializeComponent();
        }


        //створюю з'єднaння з базою данних
        SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\profitDB.mdf;Integrated Security=True;Connect Timeout=30");

        DataSet data = new DataSet();
        SqlDataAdapter dataAdapter = new SqlDataAdapter();


        //метод щоб очитистити таблицю
        public void Clear(DataGridView dataGridView)
        {
            while (dataGridView.Rows.Count > 1)
                for (int i = 0; i < dataGridView.Rows.Count - 1; i++)
                    dataGridView.Rows.Remove(dataGridView.Rows[i]);
        }
        //підключаюсь до бд
        public void connect()
        {
            try
            {
                connection.Open();

                if (connection.State == ConnectionState.Open)
                {
                    // MessageBox.Show("Зєднання встановленно!");
                }
            }
            catch (Exception)
            {
                if (connection.State != ConnectionState.Open)
                {
                    MessageBox.Show("Немає підключення з базою");
                }
            }
        }
        public int worker_ID;

        //получаю останнє id працівника і роблю інкремент
        public int getWorkerID()
        {
            string id;
            connect();
            try
            {
                SqlCommand com = new SqlCommand("Select MAX(Id) from  workers", connection);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    id = reader[0].ToString();

                    worker_ID = Convert.ToInt32(id);
                    worker_ID++;

                }
                reader.Close();
            }
            catch (SqlException)
            {
                MessageBox.Show("Сталсь помилка на сервері повторіть будь-ласка");


            }
            connection.Close();
            return worker_ID;

        }

        public int profit_ID;
        // те ж саме з Id прибутку

        public int getProfitID()
        {
            string id;
            connect();
            try
            {
                SqlCommand com = new SqlCommand("Select MAX(Id) from  profit", connection);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    id = reader[0].ToString();

                    profit_ID = Convert.ToInt32(id);
                    profit_ID++;

                }
                reader.Close();
            }
            catch (SqlException)
            {
                MessageBox.Show("Сталсь помилка на сервері повторіть будь-ласка");


            }
            connection.Close();
            return profit_ID;

        }
        public float total_salary = 0;
        //получаю і додаю всю зарплату
        public float getTotalSalary()
        {
            float salary;
            connect();
            try
            {
                SqlCommand com = new SqlCommand("Select salary from  workers", connection);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    salary = float.Parse(reader[0].ToString());

                    total_salary += salary;

                }
                reader.Close();
            }
            catch (SqlException)
            {
                MessageBox.Show("Сталсь помилка на сервері повторіть будь-ласка");


            }
            connection.Close();
            return total_salary;

        }
        // загружаю данні працівників в таблицю
        public void loadWorkers()
        {
            Clear(dataGridView1);
            SqlCommand com = new SqlCommand("Select * from workers", connection);
            SqlDataAdapter da = new SqlDataAdapter();

            DataSet ds = new DataSet();
            da.SelectCommand = com;
            da.Fill(ds, "workers");

            dataGridView1.DataSource = ds;
            dataGridView1.DataMember = "workers";

            dataGridView1.Columns["Id"].Width = 40;
            dataGridView1.Rows[0].Selected = true;

            dataGridView1.Columns["Id"].HeaderText = "№";
            dataGridView1.Columns["name"].HeaderText = "Імя";
            dataGridView1.Columns["surname"].HeaderText = "Прізвище";
            dataGridView1.Columns["salary"].HeaderText = "Заробітня плата";
            
            connection.Close();

        }

        // добавляю інформацію про працівника в бд
        private void button2_Click(object sender, EventArgs e)
        {

            string name = textBox2.Text;
            string surname = textBox3.Text;
            float salary;
            int id = getWorkerID();
            try {
                salary = float.Parse(textBox4.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Зарплата має складатись з цифр");
                textBox4.Text = String.Empty;
                return;
            }

           

            try
            {
                connect();
                if(name != String.Empty && surname != String.Empty)
                {
                    SqlCommand com = connection.CreateCommand();
                    com.CommandText = " Insert into workers (Id,name,surname,salary) " +
                        "VALUES (@id, @name, @surname,@salary)";
                    com.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    com.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                    com.Parameters.Add("@surname", SqlDbType.NVarChar).Value = surname;
                    com.Parameters.Add("@salary", SqlDbType.Float).Value = salary;


                    com.ExecuteNonQuery();
                    connection.Close();

                    MessageBox.Show("Працівника добавленно");
                    //tabControl1.SelectTab(tabProfit);
                    loadWorkers();
                    textBox2.Text = String.Empty;
                    textBox3.Text = String.Empty;
                    textBox4.Text = String.Empty;
                }
                else
                {
                    MessageBox.Show("Заповніть всі поля");
                }

                


            }
            catch (SqlException ex)
            {
                MessageBox.Show("Сталась помилка при додаванні" + ex);
            }
            connection.Close();
        }
        // при вході на вкладку добавлення працівника йде перевірка на адміна
        private void tabAddWorker_Enter(object sender, EventArgs e)
        {
            // створюється форма перевірки в якій верифікується пароль
            Form2 check_form = new Form2();
            check_form.Owner = this;
            check_form.ShowDialog();



            if (controll_pass == false)
            {
                tabControl1.SelectTab(tabProfit);
                MessageBox.Show("Вхід відхилено");
            }
            else
            {

                loadWorkers();
                controll_pass = false;
            }
        }
        public float salary;

        // оновлення інформації про зарплату працівника
        private void button3_Click(object sender, EventArgs e)
        {
            
            try {

               
                try
                {
                    salary = float.Parse(textBox5.Text);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Зарплата має складатися з цифр");
                    textBox5.Text = String.Empty;
                    return;

                }
                try
                {
                    string id = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                }catch(ArgumentOutOfRangeException ex)
                {
                    MessageBox.Show("Виберіть працівника");
                    return;
                }
                if (dataGridView1.SelectedRows[0].Cells[0].Value != null)
                {
                    string id = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                    connect();
                    SqlCommand com = connection.CreateCommand();
                    com.CommandText = "UPDATE workers set salary = @salary where Id=@id";
                    com.Parameters.Add("@salary", SqlDbType.Float).Value = salary;
                    com.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    com.ExecuteNonQuery();
                    MessageBox.Show("Оновлено");
                    loadWorkers();
                    connection.Close();
                }
            }
            catch(SqlException ex)
            {
                MessageBox.Show("Сталась помилка при оновленні заробітньої плати"+ex);
            }
            
        }
        public float total_earn,details_spend;

        // загружаю інформацію в графік для аналізу
        public void loadChartInfo(string year, string mounth)
        {

            DataTable table = new DataTable();
            SqlCommand com = connection.CreateCommand();
            if (year == String.Empty)
            {
                year = "2020";
            }
            if (mounth != String.Empty)
            {
                com.CommandText = "select * from profit where year = @year  and mounth = @mounth";
                com.Parameters.Add("@year", SqlDbType.NVarChar).Value = year;
                com.Parameters.Add("@mounth", SqlDbType.NVarChar).Value = mounth;
                SqlDataAdapter ProfitDA = new SqlDataAdapter();
                ProfitDA.SelectCommand = com;
                ProfitDA.TableMappings.Add("Table", "Profit");
                ProfitDA.Fill(table);
            }
            else
            {
                com.CommandText = "select * from profit where year = @year ";
                com.Parameters.Add("@year", SqlDbType.NVarChar).Value = year;
                SqlDataAdapter ProfitDA = new SqlDataAdapter();
                ProfitDA.SelectCommand = com;
                ProfitDA.TableMappings.Add("Table", "Profit");
                ProfitDA.Fill(table);
            }           
         
            chart1.Titles[0].Text = "Інформація про прибуток за " + year;
            chart1.DataSource = table;
            
            

            chart1.Series["Series1"].XValueMember = "mounth";
            chart1.Series["Series1"].YValueMembers = "total_spend";
            chart1.Series["Series2"].YValueMembers = "total_earn";
            chart1.Series["Series3"].YValueMembers = "salary_spend";
            chart1.Series["Series4"].YValueMembers = "tax";
            chart1.Series["Series5"].YValueMembers = "details_spend";
            chart1.Series["Series6"].YValueMembers = "profit";
           

            chart1.Series[0].IsValueShownAsLabel = true;
            chart1.Series[1].IsValueShownAsLabel = true;
            chart1.Series[2].IsValueShownAsLabel = true;
            chart1.Series[3].IsValueShownAsLabel = true;
            chart1.Series[4].IsValueShownAsLabel = true;
            chart1.Series[5].IsValueShownAsLabel = true;

           chart1.Series[0].LabelForeColor = Color.Red;
           chart1.Series[5].LabelForeColor = Color.Green;
           


            chart1.Series[1].ChartType = SeriesChartType.Bar;
            chart1.Series[2].ChartType = SeriesChartType.Bar;
            chart1.Series[3].ChartType = SeriesChartType.Bar;
            chart1.Series[4].ChartType = SeriesChartType.Bar;
            chart1.Series[5].ChartType = SeriesChartType.Bar;
            chart1.Series[0].ChartType = SeriesChartType.Bar;
            chart1.DataBind();
            connection.Close();
        }
        // загружаю в комбобокс рік 
        private void tabControl1_Enter(object sender, EventArgs e)
        {
            connect();
            string strCmd = "select distinct year from profit";
            SqlCommand cmd = new SqlCommand(strCmd, connection);
            SqlDataAdapter da = new SqlDataAdapter(strCmd, connection);
            DataSet ds = new DataSet();
            da.Fill(ds);
            cmd.ExecuteNonQuery();
            connection.Close();

            comboBox4.DisplayMember = ds.Tables[0].Columns[0].ToString();
            comboBox4.ValueMember = "Тип деталі";
            comboBox4.DataSource = ds.Tables[0];

            comboBox4.Enabled = true;
            comboBox4.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadChartInfo(comboBox4.Text,comboBox2.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            loadChartInfo(comboBox4.Text, comboBox2.Text);
        }

        private void tabProfit_Click(object sender, EventArgs e)
        {

        }
        // загружаю в комбобокс місяць з бд в залежності від року
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Enabled = true;

            connection.Open();
            SqlCommand cmd = new SqlCommand("select mounth from profit where year = @year ", connection);
            cmd.Parameters.Add("@year", SqlDbType.NVarChar).Value = comboBox4.Text;
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();

            da.Fill(ds);
            cmd.ExecuteNonQuery();
            ds.Tables[0].Rows.Add("");
           
            comboBox2.DisplayMember = ds.Tables[0].Columns[0].ToString();
            
            comboBox2.DataSource = ds.Tables[0];

          
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            connection.Close();
          /*  if(comboBox4.Text == String.Empty)
            {
                loadChartInfo(comboBox4.Text,"");

            }*/
            loadChartInfo(comboBox4.Text, comboBox2.Text);
        }

        // видалення працівника з бд
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    string id = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                }catch(ArgumentOutOfRangeException ex)
                {
                    MessageBox.Show("Виберіть будь-ласка працівника");
                    return;
                }
                if (dataGridView1.SelectedRows[0].Cells[0].Value != null)
                {

                    string id = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                    connect();
                    SqlCommand com = connection.CreateCommand();
                    com.CommandText = "DELETE from workers where Id=@id";
                    com.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    com.ExecuteNonQuery();
                    MessageBox.Show("Успішно видалено");
                    loadWorkers();
                    connection.Close();
                    loadWorkers();
                }
               
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Сталась помилка при видаленні працівника з бази" + ex);
            }

        }

        // головний метод обрахунку данних прибутку податку витрат і тд
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {


                // получаю ід 
                int id = getProfitID();
                // получаю місяць з комбобокса
                String mounth = comboBox1.Text;
                //загльна сума яка витрачена на зарплати
                float salary_spend = getTotalSalary();
                // получю рік з комбобокса
                string year = comboBox3.Text;
                total_salary = 0;

                // підключаюсь до бд
                connect();

                // перевіряю чи в базі є облік за данний рік і місяць
                SqlCommand select = new SqlCommand("Select year,mounth from profit where year = @year and mounth = @mounth", connection);
                select.Parameters.Add("@year", SqlDbType.NVarChar).Value = year;
                select.Parameters.Add("@mounth", SqlDbType.NVarChar).Value = mounth;
                SqlDataReader reader = select.ExecuteReader();
                // якщо є то повертаю
                if (reader.HasRows)
                {
                    MessageBox.Show("За цей місяць уже пораховано");

                    reader.Close();
                    return;

                }
                // інакше добавляю 
                else
                {
                    reader.Close();
                    try
                    {
                        total_earn = float.Parse(textBox1.Text);
                        details_spend = float.Parse(textBox6.Text);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Прибуток має складатися з цифр");
                        textBox1.Text = String.Empty;
                        textBox6.Text = String.Empty;
                        return;
                    }
                    // обраховую податок без відсотка
                    float tax = (total_earn - (details_spend + salary_spend));
                    // повний податок який підприємство має заплатити  (податок 18%)
                    float total_tax = tax * 18 / 100;
                    //скільки підприємство витратило
                    float total_spend = details_spend + salary_spend + total_tax;
                    //чистий прибуток підприємства
                    float profit = total_earn - total_spend;





                   
                    SqlCommand com = connection.CreateCommand();
                    com.CommandText = "INSERT into profit (id,mounth,total_earn,salary_spend,tax,details_spend,total_spend,profit,year)" +
                        "VALUES(@id,@mounth,@total_earn,@salary_spend,@tax,@details_spend,@total_spend,@profit,@year)";

                    com.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    com.Parameters.Add("@mounth", SqlDbType.NVarChar).Value = mounth;
                    com.Parameters.Add("@total_earn", SqlDbType.Float).Value = total_earn;
                    com.Parameters.Add("@salary_spend", SqlDbType.Float).Value = salary_spend;
                    com.Parameters.Add("@tax", SqlDbType.Float).Value = total_tax;
                    com.Parameters.Add("@details_spend", SqlDbType.Float).Value = details_spend;
                    com.Parameters.Add("@total_spend", SqlDbType.Float).Value = total_spend;
                    com.Parameters.Add("@profit", SqlDbType.Float).Value = profit;
                    com.Parameters.Add("@year", SqlDbType.NVarChar).Value = year;


                    com.ExecuteNonQuery();
                    MessageBox.Show("Добавлено");

                    connection.Close();
                }

                

            }
            catch (SqlException ex)
            {
                MessageBox.Show("Сталась помилка під час обліку" + ex);
            }
        }
    }
}
